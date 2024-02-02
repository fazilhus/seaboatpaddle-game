shader_type spatial;

uniform vec3 albedo : source_color;
uniform vec3 albedo2 : source_color;
// hint_range(min, max) = default
uniform float metallic : hint_range(0.0, 1.0) = 0;
uniform float roughness : hint_range(0.0, 1.0) = 0.02;
uniform sampler2D wave;
uniform sampler2D textureNormal;
uniform sampler2D TextureNormal2;
//Vector 2 Direction 2.0x and 1.0y
uniform vec2 waveDirection = vec2(2.0, 0.0);
uniform vec2 waveDirection2 = vec2(0.0, 1.0);
uniform float timeScale : hint_range(0.0, 0.2, 0.005) = 0.025;
uniform float waveTime = 1.0f;
uniform float noiseScale = 10.0f;
uniform float heightScale = 0.15f;
uniform float boatHight = 0.0f;
// depth
uniform sampler2D DEPTH_TEXTURE : hint_depth_texture;
uniform sampler2D SCREEN_TEXTURE : hint_screen_texture;
uniform vec4 colorDeep : source_color;
uniform vec4 ColorShallow : source_color;
uniform float beersLaw = 2.0;
uniform float depthOffset = -0.75;
// edge foam
uniform float edgeScale = 0.1;
uniform float near = 1.0;
uniform float far = 100.0;
uniform vec3 edgeColor : source_color;


//varying variables are used to pass data between vertex shader and the fragment shader.
varying float height;
varying vec3 worldPosition;
// fresnel is physical pehnomenon related to reflaction and refraciton of light.
float fresnel(float amount, vec3 normal, vec3 view) // hints -> static typing feture 
{
	return pow((1.0 - clamp(dot(normalize(normal), normalize(view)), 0.0, 1.0)), amount);
}

float edge(float depth)
{
	depth = 2.0 * depth - 1.0;
	return near * far / (far + depth * (near - far));
}

void vertex() // Called for every vertex the material is visible on.
{
	worldPosition = (MODEL_MATRIX * vec4(VERTEX, 1.0)).xyz;
	height = texture(wave, worldPosition.xz / noiseScale + TIME *timeScale).r;
	VERTEX.y += height * heightScale;
	//boatHight = VERTEX.y;
}

void fragment() //Special function, fragment shader is called for every pixel(fragment) that needs to be rendered on the screen
{
	// 	getting edge depth calc
	float Zdepth = edge(texture(DEPTH_TEXTURE, SCREEN_UV).x);
	float Zpos = edge(FRAGCOORD.z);
	float Zdif = Zdepth - Zpos;
	
	// depth variables and calculations
	float depthTexture = texture(DEPTH_TEXTURE, SCREEN_UV).r * 2.0 -1.0;
	float depth = PROJECTION_MATRIX[3][2] / (depthTexture + PROJECTION_MATRIX[2][2]);
	float depthBlend = exp((depth+VERTEX.z + depthOffset) * -beersLaw);
	float depthBlendPower = clamp(pow(depthBlend, 2.5), 0.0, 1.0);
	vec3 ScreenColor = textureLod(SCREEN_TEXTURE, SCREEN_UV, depthBlendPower * 2.5).rgb;
	vec3 depthColor = mix(ColorShallow.rgb, colorDeep.rgb, depthBlendPower);
	vec3 color = mix(ScreenColor * depthColor, depthColor * 0.25, depthBlendPower * 0.5);
	//Time Calculations for wave (normal map) movements
	vec2 time = (waveTime * waveDirection) * timeScale; // movement rate of the first wave
	vec2 time2 = (waveTime * waveDirection2) * timeScale; // movement rate of the second wave
	//blending normal maps into one
	vec3 normalBlend = mix(texture(textureNormal, worldPosition.xz + time).rgb, texture(TextureNormal2,UV + time2).rgb, 0.5);
	//calculate Fresnel
	float fresnel = fresnel(5.0, NORMAL, VIEW);
	vec3 surfaceColor = mix(albedo, albedo2, fresnel); // interpolate albedo values by frensel
	vec3 depthColorAdj = mix(edgeColor, color, step(edgeScale, Zdif));
	
	ALBEDO = clamp(surfaceColor + depthColorAdj, vec3(0.), vec3(1.0));
	METALLIC = metallic;
	ROUGHNESS = roughness;
	NORMAL_MAP = normalBlend;
}

//void light() {
	// Called for every pixel for every light affecting the material.
	// Uncomment to replace the default light processing function with this one.
//}
