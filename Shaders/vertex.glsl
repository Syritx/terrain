#version 100

precision mediump float;
varying vec3 fragColor;
attribute vec3 vertexPosition;
attribute vec3 vertexColor;
uniform mat4 mWorld;
uniform mat4 mView;
uniform mat4 mProj;

void main() {
    fragColor = vertexColor;
    gl_Position = mProj * mView * mWorld * vec4(vertexPosition, 1.0);
}