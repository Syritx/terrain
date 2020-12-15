//
//  main.cpp
//  3dGLFW
//
//  Created by Syritx on 2020-12-04.
//

#include "libs.h"
#include "Plain.h"

#include <iostream>

using namespace std;

void keyboard(GLFWwindow* window, Plain plain);
void cursorPosition(GLFWwindow* window, double xpos, double ypos);
void mouseButton(GLFWwindow* window, int b, int a, int m);
float clamp(float v, float min, float max);

vec3 position = vec3(0,0,0),
     eye = vec3(0,0,-1),
     up = vec3(0,1,0);

vec2 lastMousePosition = vec2(0,0);

float xrot, yrot, CAMERA_SPEED = .5f;
bool isMouseDown = false;

int main(int argc, const char * argv[]) {
    
    if (!glfwInit()) glfwTerminate();
        
    glfwWindowHint(GLFW_CONTEXT_VERSION_MAJOR, 3);
    glfwWindowHint(GLFW_CONTEXT_VERSION_MINOR, 2);
    glfwWindowHint(GLFW_RESIZABLE, GL_FALSE);
#ifdef __APPLE__
    glfwWindowHint(GLFW_OPENGL_FORWARD_COMPAT, GL_TRUE);
    glfwWindowHint(GLFW_OPENGL_PROFILE, GLFW_OPENGL_CORE_PROFILE);
#endif
    
    GLFWwindow* window = glfwCreateWindow(1000, 720, "hello", NULL, NULL);
    if (!window) glfwTerminate();
    
    glfwMakeContextCurrent(window);
    glewExperimental = GL_TRUE;
    glewInit();
    
    glfwSetCursorPosCallback(window, cursorPosition);
    glfwSetInputMode(window, GLFW_CURSOR, GLFW_CURSOR_NORMAL);
    glfwSetMouseButtonCallback(window, mouseButton);
        
    Plain plain = Plain();
        
    while (!glfwWindowShouldClose(window)) {
        glClear(GL_COLOR_BUFFER_BIT);
        glClearColor(0,0,0,1);
        keyboard(window, plain);
        
        glfwPollEvents();
        glfwSwapBuffers(window);
    }
    glfwTerminate();
    return 1;
}


// MOUSE

void cursorPosition(GLFWwindow* window, double xpos, double ypos) {
    if (isMouseDown) {
        
        double xdist = lastMousePosition.x - xpos;
        double ydist = lastMousePosition.y - ypos;
        
        xrot += ydist * .5f;
        yrot -= xdist * .5f;
        
        xrot = clamp(xrot, -89.9f, 89.9f);
        
        eye.x = cos(radians(xrot)) * cos(radians(yrot));
        eye.y = sin(radians(xrot));
        eye.z = cos(radians(xrot)) * sin(radians(yrot));
        
        eye = normalize(eye);
    }
    lastMousePosition = vec2(xpos, ypos);
}

void mouseButton(GLFWwindow* window, int b, int a, int m) {
    if (b == GLFW_MOUSE_BUTTON_RIGHT && a == GLFW_PRESS) {
        isMouseDown = true;
    }
    
    if (b == GLFW_MOUSE_BUTTON_RIGHT && a == GLFW_RELEASE) {
        isMouseDown = false;
    }
}


// CAMERA

float clamp(float v, float min, float max) {
    if (v > max) return max;
    if (v < min) return min;
    
    return v;
}

void keyboard(GLFWwindow* window, Plain plain) {
    
    float xDir = 0;
    // vertical
         if (glfwGetKey(window, GLFW_KEY_W) == GLFW_PRESS) position += eye * CAMERA_SPEED;
    else if (glfwGetKey(window, GLFW_KEY_S) == GLFW_PRESS) position -= eye * CAMERA_SPEED;
    
    // horizontal
    vec3 right = normalize(cross(eye, up));
         if (glfwGetKey(window, GLFW_KEY_A) == GLFW_PRESS) position -= right * CAMERA_SPEED;
    else if (glfwGetKey(window, GLFW_KEY_D) == GLFW_PRESS) position += right * CAMERA_SPEED;
    
    plain.render(position, eye, up);
}
