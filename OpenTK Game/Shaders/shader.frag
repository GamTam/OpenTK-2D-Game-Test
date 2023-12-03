#version 440 core

out vec4 FragColor;

//in vec4 vertexColor; // Named the same

uniform vec4 uniformColor;

uniform sampler2D tex0;
uniform sampler2D tex1;

in vec2 UV0;
in vec3 Normals;
in vec3 FragPos;

void main()
{
    vec4 colorTex0 = texture(tex0, UV0);
    vec4 colorTex1 = texture(tex1, UV0);
    
    float threshhold = 0.9f;
    float average = (colorTex1.r + colorTex1.g + colorTex1.b)/3;
    
    if(average > threshhold)
    {
        colorTex1 = uniformColor;
    }
    
    FragColor = mix(colorTex0, colorTex1, colorTex1.a);
}