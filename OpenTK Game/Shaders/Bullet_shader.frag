#version 440 core

out vec4 FragColor;

//in vec4 vertexColor; // Named the same

uniform vec4 uniformColor;

uniform sampler2D tex;

struct PointLight {
    vec3 lightColor;
    vec3 lightPosition;
    float lightIntensity;
    vec3 viewPos;
};

uniform PointLight pointLights[100];
uniform int pointLightCount;

in vec2 UV0;
in vec3 Normals;
in vec3 FragPos;

vec3 HandleLighting() {
    vec3 outCol;
    
    for (int i=0; i<pointLightCount; i++) {
        
        const PointLight currentLight = pointLights[i];
        
        float ambientStength = 0.2f;
        vec3 ambient = ambientStength * currentLight.lightColor;

        vec3 normals = normalize(Normals);
        vec3 lightDir = normalize(currentLight.lightPosition - FragPos);

        float diff = max(dot(normals, lightDir), 0);
        vec3 diffuse = diff * currentLight.lightColor;

        diffuse = vec3(min(diffuse.x, 1), min(diffuse.y,1), min(diffuse.z,1));

        const float shininess = 128;
        float specularStrength = 0.5f;
        vec3 viewDir = normalize(currentLight.viewPos - FragPos);
        vec3 reflectoinDir = reflect(-lightDir, normals);
        float spec = pow(max(dot(viewDir, reflectoinDir), 0), shininess);
        vec3 specularColor = specularStrength * spec * currentLight.lightColor;
        
        outCol += (ambient + specularColor + diffuse) * currentLight.lightIntensity;
    }
    
    return outCol;
}

void main()
{
    vec4 colorTex0 = texture(tex, UV0);
    
    float threshhold = 0.9f;
    
    FragColor = colorTex0;
    
    //FragColor = vec4(vec3(FragColor) * (HandleLighting() * 0.5), FragColor.a);
    FragColor = vec4(vec3(FragColor), FragColor.a);
    //FragColor = vec4(vec3(FragColor) * (ambient + diffuse), FragColor.a);
}