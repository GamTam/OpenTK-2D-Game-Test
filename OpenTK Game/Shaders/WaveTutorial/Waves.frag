#version 440 core

out vec4 FragColor;

uniform float time;

in vec2 UV0;
in vec3 Normals;
in vec3 FragPos;

void main()
{
    const float m = 10;
    const float fuzziness = 0.75;
    
    float r = fuzziness * sin(UV0.x * m + sin(UV0.y * m + time*10) + time);
    
    FragColor = vec4(r, 0.5, 0.7, 1);
}