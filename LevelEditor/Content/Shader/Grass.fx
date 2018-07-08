float4x4 viewMatrix;
float4x4 projectionMatrix;

float4x4 lightSpaceMatrix;

float shadowBias;
float shadowDistance;

float3 lightDiffuseColor;
float lightAmbient;

float farPlane;
float3 fogColor;
float fogDistance;

float time;

texture grassTexture;
sampler2D textureSampler = sampler_state {
    Texture = (grassTexture);
    MagFilter = Linear;
    MinFilter = Linear;
    AddressU = Clamp;
    AddressV = Clamp;
};

Texture2D shadowMap;

SamplerComparisonState cmpSampler : register(s1);

struct VertexShaderInput
{
	float3 vertexPosition : POSITION0;
	float2 vertexTexCoord : TEXCOORD0;
};

struct VertexShaderOutput
{
	float4 vertexPosition : POSITION0;
	float3 pixelPosition : POSITION1;
	float2 pixelTexCoord : TEXCOORD0;
	float3 pixelFogFactor : POSITION2;
	float pixelDistance : DEPTH0;
	float4 pixelShadowCoords : COLOR0;
};
	
float len(float3 vec) {
	return dot(vec, vec);
}

VertexShaderOutput VertexShaderFunction(VertexShaderInput input, float3 instancePosition : POSITION1)
{
	
	VertexShaderOutput output;
	
	if (input.vertexPosition.y > 0.1f) {
		input.vertexPosition.x += sin(instancePosition.x + time * 2.0f) * 0.25f;
		input.vertexPosition.z += cos(instancePosition.z + time) * 0.25f;
	}
	
	output.vertexPosition = mul(float4(input.vertexPosition + instancePosition, 1.0f), viewMatrix);
	output.pixelPosition = output.vertexPosition.xyz;
	output.vertexPosition = mul(output.vertexPosition, projectionMatrix);
	
	// Calculate most of the stuff on vertex level, because we're definitly pixel bound when rendering grass
	output.pixelTexCoord = input.vertexTexCoord;
	output.pixelDistance = length(output.pixelPosition);
	output.pixelFogFactor = max((output.pixelDistance - fogDistance) / (farPlane - fogDistance), 0.0f);
	
	output.pixelShadowCoords = mul(float4(output.pixelPosition, 1.0f), lightSpaceMatrix);
	output.pixelShadowCoords.xyz /= output.pixelShadowCoords.w;
	output.pixelShadowCoords.w = clamp((output.pixelDistance + 5.0f - shadowDistance) * 0.2f, 0.0f, 1.0f);
	output.pixelShadowCoords.xy = output.pixelShadowCoords.xy * 0.5f + 0.5f;
	output.pixelShadowCoords.y = 1.0f - output.pixelShadowCoords.y;
	output.pixelShadowCoords.z -= shadowBias;
	
	return output;
	
}

float CalculateShadow(float4 shadowCoords) 
{	
	
	if (shadowCoords.w > 0.999f)
		return 1.0f;
		
	float factor = shadowMap.SampleCmpLevelZero(cmpSampler, shadowCoords.xy, shadowCoords.z);
	
	return clamp(factor + shadowCoords.w, 0.0f, 1.0f);
	
}


float3 PixelShaderFunction(VertexShaderOutput input) : COLOR
{
	
	float4 textureColor = tex2D(textureSampler, input.pixelTexCoord).rgba;
	
	if (textureColor.a < 0.5f) {
		discard;
	}
	
	float3 outColor = textureColor.rgb;
	
	float shadowFactor = CalculateShadow(input.pixelShadowCoords);
	
	float3 ambient = lightAmbient * textureColor.xyz;
	float3 diffuse = max(lightDiffuseColor * shadowFactor, lightAmbient) * textureColor.xyz;
	
	return lerp(ambient + diffuse, fogColor, input.pixelFogFactor);
	
}
	

technique Main
{
	pass Pass0
	{
		VertexShader = compile vs_5_0 VertexShaderFunction();
		PixelShader = compile ps_5_0 PixelShaderFunction();
	}
}