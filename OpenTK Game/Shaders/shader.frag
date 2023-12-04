#version 440 core

out vec4 FragColor;

//in vec4 vertexColor; // Named the same

uniform vec4 uniformColor;

uniform sampler2D tex0;
uniform vec2 flip = vec2(1, 1);
uniform float alpha = 1f;

in vec2 UV0;
in vec3 Normals;
in vec3 FragPos;

void main()
{    
    vec4 colorTex0 = texture(tex0, UV0 * flip);
    colorTex0.a = colorTex0.a * alpha;
    
    FragColor = colorTex0;
}