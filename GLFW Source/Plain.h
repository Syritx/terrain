//
//  Plain.h
//  3dGLFW
//
//  Created by Syritx on 2020-12-04.
//

#ifndef Plain_h
#define Plain_h

#include "libs.h"
#include "Shader.h"
#include "noise.h"

#include <iostream>
#include <glm/gtc/noise.hpp>
using namespace std;

const char* vertexShaderSource =
"#version 100 \n"
"precision mediump float;"
"varying vec3 fragColor;"
"attribute vec3 vertexPosition;"
"attribute vec3 vertexColor;"
"uniform mat4 mWorld;"
"uniform mat4 mView;"
"uniform mat4 mProj;"
"\n"
"void main() {"
"    fragColor = vertexColor;"
"    gl_Position = mProj * mView * mWorld * vec4(vertexPosition, 1.0);"
"}";

const char* fragmentShaderSource =
"#version 100\n"
"precision mediump float;"
"varying vec3 fragColor;"
"\n"
"void main() {"
"    gl_FragColor = vec4(fragColor, 1.0);"
"}";

#define BUFFER_OFFSET(i) ((char*)NULL + (i))

int mapSize = 10;

float size = 1.5f;
int indicies_array_size;


class Plain {
private:
    GLuint ibo, vbo, vao;
    
    float vertices[120*120*24] = {};

    unsigned int indices[120*120*6] = {};
    
    
    
    float createNoiseLayer(int o, float l, float p, float x, float y, float seed) {
        
        float freq = 2,
        ampl = 10;
        
        float noise = 0;
        
        for (int i = 0; i < o; i++) {
            noise += perlin_noise().noise(x*freq, y*freq, seed)*ampl;
            freq *= l;
            ampl *= p;
        }
        return noise;
    }
    
public:
    Shader shader = Shader(vertexShaderSource, fragmentShaderSource);
    
    void set_vertices(int resolution) {
        int _id = 0;
        
        int seed = (int)random() % 1000;
        cout << seed;
                
        for (int x = 0; x < resolution; x++) {
            for (int z = 0; z < resolution; z++) {
                
                vertices[_id*24] = x;
                vertices[_id*24+1] = createNoiseLayer(55, 2.f, .5f, (float)x/resolution, (float)z/resolution, seed);
                vertices[_id*24+2] = z;
                
                vertices[_id*24+3] = (float)x/resolution + (float)z/resolution;
                vertices[_id*24+4] = 0;
                vertices[_id*24+5] = 1-(float)x/resolution + (float)z/resolution;
                
                vertices[_id*24+6] = x;
                vertices[_id*24+7] = createNoiseLayer(55, 2.f, .5f, (float)x/resolution, (float)(z+1)/resolution, seed);
                vertices[_id*24+8] = z+1;
                
                vertices[_id*24+9] =  (float)x/resolution + (float)(z+1)/resolution;
                vertices[_id*24+10] = 0;
                vertices[_id*24+11] = 1-(float)x/resolution + (float)(z+1)/resolution;
                
                vertices[_id*24+12] = x+1;
                vertices[_id*24+13] = createNoiseLayer(55, 2.f, .5f, (float)(x+1)/resolution, (float)(z+1)/resolution, seed);
                vertices[_id*24+14] = z+1;
                
                vertices[_id*24+15] = (float)(x+1)/resolution + (float)(z+1)/resolution;
                vertices[_id*24+16] = 0;
                vertices[_id*24+17] = 1-(float)(x+1)/resolution + (float)(z+1)/resolution;
                
                vertices[_id*24+18] = x+1;
                vertices[_id*24+19] = createNoiseLayer(55, 2.f, .5f, (float)(x+1)/resolution, (float)z/resolution, seed);
                vertices[_id*24+20] = z;
                
                vertices[_id*24+21] = (float)(x+1)/resolution + (float)z/resolution;
                vertices[_id*24+22] = 0;
                vertices[_id*24+23] = 1-(float)(x+1)/resolution + (float)z/resolution;
                
                
                indices[_id*6] =   _id*4;
                indices[_id*6+1] = _id*4+1;
                indices[_id*6+2] = _id*4+2;
                indices[_id*6+3] = _id*4;
                indices[_id*6+4] = _id*4+2;
                indices[_id*6+5] = _id*4+3;
                
                _id++;
            }
        }
    }
    
    
    
    Plain() {
                
        set_vertices(120);
        
        indicies_array_size = sizeof(indices)/sizeof(indices[0]);
        glGenBuffers(1, &vbo);
        glBindBuffer(GL_ARRAY_BUFFER, vbo);
        
        try {
            glBufferData(GL_ARRAY_BUFFER, sizeof(vertices)/sizeof(vertices[0]) * sizeof(float), vertices, GL_STATIC_DRAW);
        }
        catch (const exception& e ){
            cout << e.what();
        }
        
        glGenBuffers(1, &ibo);
        glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, ibo);
        glBufferData(GL_ELEMENT_ARRAY_BUFFER, sizeof(indices)/sizeof(indices[0]) * sizeof(unsigned int), indices, GL_STATIC_DRAW);
        
        glGenVertexArrays(1, &vao);
        glBindVertexArray(vao);
        glBindBuffer(GL_ARRAY_BUFFER, vbo);
        glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, ibo);
        
        int positionAttrib = glGetAttribLocation(shader.program, "vertexPosition");
        int colorAttrib = glGetAttribLocation(shader.program, "vertexColor");
        
        glVertexAttribPointer(
        positionAttrib,
        3,
        GL_FLOAT,
        GL_FALSE,
        6 * sizeof(float), nullptr);
        
        glVertexAttribPointer(
        colorAttrib,
        3,
        GL_FLOAT,
        GL_FALSE,
        6 * sizeof(float),
        BUFFER_OFFSET(3 * sizeof(float)));
        
        glEnableVertexAttribArray(positionAttrib);
        glEnableVertexAttribArray(colorAttrib);
        
        shader.use_shader();
        
        int worldUniformLocation = glGetUniformLocation(shader.program, "mWorld"),
            viewUniformLocation = glGetUniformLocation(shader.program, "mView"),
            projectionUniformLocation = glGetUniformLocation(shader.program, "mProj");
        
        mat4 worldMatrix = mat4(1.0f);
        mat4 viewMatrix = lookAt(vec3(0,0,0), vec3(0,0,0), vec3(0,1,0));
        mat4 projection = perspective(radians(90.f), 1000.f/720.f, .1f, 2000.f);
        
        shader.set_mat4(worldUniformLocation, worldMatrix);
        shader.set_mat4(viewUniformLocation, viewMatrix);
        shader.set_mat4(projectionUniformLocation, projection);
    }
    
    void render(vec3 position, vec3 eye, vec3 up) {
        glEnable(GL_DEPTH_TEST);
        glClearColor(0.f, 0.f, 0.f, 1.f);
        glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);
        
        shader.use_shader();
        
        int worldUniformLocation = glGetUniformLocation(shader.program, "mWorld"),
            viewUniformLocation = glGetUniformLocation(shader.program, "mView"),
            projectionUniformLocation = glGetUniformLocation(shader.program, "mProj");
        
        mat4 worldMatrix = mat4(1.0f);
        mat4 viewMatrix = lookAt(position, position+eye, up);
        mat4 projection = perspective(radians(90.f), 1000.f/720.f, .1f, 2000.f);
        
        shader.set_mat4(worldUniformLocation, worldMatrix);
        shader.set_mat4(viewUniformLocation, viewMatrix);
        shader.set_mat4(projectionUniformLocation, projection);
        
        glBindVertexArray(vao);
        glDrawElements(GL_LINES, indicies_array_size, GL_UNSIGNED_INT, 0);
    }
};

#endif /* Plain_h */
