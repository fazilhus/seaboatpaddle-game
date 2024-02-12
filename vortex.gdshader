shader_type spatial;

// Adjust this factor to control the intensity of the whirlpool effect
uniform float whirlpool_strength : hint_range(0.0, 5.0);

// Adjust this factor to control the rotation speed
uniform float rotation_speed : hint_range(-5.0, 5.0);

// Adjust this factor to control the depth of the whirlpool
uniform float whirlpool_depth : hint_range(0.0, 5.0);

// Adjust this factor to control the inner radius of the whirlpool
uniform float inner_radius : hint_range(0.0, 10.0);

uniform sampler2D SCREEN_TEXTURE : hint_screen_texture;
uniform vec3 albedo : source_color;
uniform float metallic : hint_range(0.0, 1.0) = 0;
uniform float roughness : hint_range(0.0, 1.0) = 0.02;
uniform sampler2D textureNormal;
uniform sampler2D TextureNormal2;
uniform vec3 edgeColor : source_color;
uniform float edgeScale = 0.1;

uniform float circle_radius : hint_range(0.0,2.0); // Radius of the circle
uniform float edge_softness : hint_range(0.0, 1.0); // Softness of the edge



void vertex() {
    // Calculate the distance from the center of the object
    float distance = length(VERTEX.xz);
    
    // Calculate the angle based on the position
    float angle = atan(VERTEX.z, VERTEX.x);
    
    // Apply rotation around the y-axis
    float cos_theta = cos(rotation_speed * TIME);
    float sin_theta = sin(rotation_speed * TIME);

    // Apply rotation transformation
    float newX = cos(angle) * distance;
    float newZ = sin(angle) * distance;
    VERTEX.x = newX * cos_theta - newZ * sin_theta;
    VERTEX.z = newX * sin_theta + newZ * cos_theta;

    // Apply the whirlpool effect
    float whirlpool_radius = 2.0; // Adjust the radius of the whirlpool
    float whirlpool_speed = 1.0; // Adjust the speed of the whirlpool
    float whirlpool_factor = min(1.0, distance / whirlpool_radius);
    float whirlpool_angle = TIME * whirlpool_speed * whirlpool_factor * whirlpool_strength;
    float cos_angle = cos(whirlpool_angle);
    float sin_angle = sin(whirlpool_angle);
    VERTEX.x += cos_angle * distance - distance;
    VERTEX.z += sin_angle * distance;

    // Apply depth to the whirlpool
    float depth_factor = clamp((inner_radius - distance) / inner_radius, 0.0, 1.0);
    VERTEX.y -= whirlpool_depth * depth_factor * depth_factor;
	
}

void fragment() //Special function, fragment shader is called for every pixel(fragment) that needs to be rendered on the screen
{
	// Calculate the distance from the center of the circle
    vec2 center = vec2(0.5, 0.8); // Assuming the center of the circle is at (0.5, 0.5) on the screen
    float distance_from_center = length(UV - center);
	
	// Calculate the transparency based on the distance from the center

    float transparency = smoothstep(circle_radius - edge_softness, circle_radius, distance_from_center);
	
	vec3 normalBlend = mix(texture(textureNormal, UV + TIME/10.0).rgb, texture(TextureNormal2, UV + TIME/10.0).rgb, 0.5);
	ALBEDO = textureLod(SCREEN_TEXTURE, SCREEN_UV, 0.01).rgb + albedo;
	ALPHA *= transparency;
    
	METALLIC = metallic;
	ROUGHNESS = roughness;
	NORMAL_MAP = normalBlend;
	
}
