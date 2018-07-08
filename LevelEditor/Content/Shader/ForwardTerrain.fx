float4x4 viewMatrix;
float4x4 projectionMatrix;

float4x4 lightSpaceMatrix;

float3 lightDirection;
float3 lightColor;
float lightAmbient;

float shadowBias;
int shadowNumSamples;
float shadowSampleRange;
float shadowDistance;
float shadowResolution;

float time;

float3 diffuseColor;
bool hasDiffuseMap;
texture diffuseMap;
sampler2D textureSampler = sampler_state {
    Texture = (diffuseMap);
    MagFilter = Linear;
    MinFilter = Linear;
    AddressU = Wrap;
    AddressV = Wrap;
};

Texture2D shadowMap;

float farPlane;
float3 fogColor;
float fogDistance;

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
	float2 pixelTexCoord : TEXCOORD;
};
	

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
	
	VertexShaderOutput output;
	
	output.vertexPosition = mul(float4(input.vertexPosition, 1.0f), viewMatrix);
	output.pixelPosition = output.vertexPosition.xyz;
	output.vertexPosition = mul(output.vertexPosition, projectionMatrix);
	
	output.pixelTexCoord = input.vertexTexCoord;
	
	return output;
	
}

static float2 poissonDisk[16] = {
  float2(0.1159636f, -0.6847993f),
  float2(-0.04063215f, 0.0732412f),
  float2(-0.5671037f, -0.6256651f),
  float2(-0.4569176f, -0.175979f),
  float2(0.517299f, -0.1095803f),
  float2(0.610924f, -0.6100475f),
  float2(-0.9603067f, -0.0002735546f),
  float2(0.9396403f, -0.2230212f),
  float2(-0.4805221f, 0.2525214f),
  float2(-0.7926834f, 0.5755413f),
  float2(-0.2899438f, 0.6315116f),
  float2(0.09232567f, 0.9606919f),
  float2(0.3653378f, 0.6443261f),
  float2(0.8239012f, 0.478811f),
  float2(-0.07792759f, -0.3342873f),
  float2(-0.281228f, -0.9247693f) 
};


float CalculateShadow(float4 shadowCoords, float bias, float pixelDistance) 
{	
	
	if (shadowCoords.w > 0.999f)
		return 1.0f;
	
	shadowCoords.xy = shadowCoords.xy * 0.5f + 0.5f;
	shadowCoords.y = 1.0f - shadowCoords.y;
	shadowCoords.z -= bias;
	
	float visibility = 1.0f;
	float partialSum = 1.0f / shadowNumSamples;
		
	for (int i = 0; i < shadowNumSamples; i++) {
		
		float2 offset = poissonDisk[i] / (shadowResolution * shadowSampleRange);
		
		float factor = shadowMap.SampleCmpLevelZero(cmpSampler, shadowCoords.xy + offset, shadowCoords.z);
		
		visibility -= partialSum * (1.0f - factor);
	}
	
	return clamp(visibility + shadowCoords.w, 0.0f, 1.0f);
	
}

float3 PixelShaderFunction(VertexShaderOutput input) : COLOR
{
	
	float3 color = diffuseColor;
	
	color *= tex2D(textureSampler, input.pixelTexCoord).xyz;
	
	float3 fdx = ddx(input.pixelPosition);
	float3 fdy = ddy(input.pixelPosition);
 	float3 normal = normalize(cross(fdy, fdx));
	
	float pixelDistance = length(input.pixelPosition);
	
	float4 shadowCoords = mul(float4(input.pixelPosition, 1.0f), lightSpaceMatrix);
	shadowCoords.xyz /= shadowCoords.w;	
	shadowCoords.w = clamp((pixelDistance + 5.0f - shadowDistance) * 0.2f, 0.0f, 1.0f);
	
	float shadowFactor = CalculateShadow(shadowCoords, shadowBias, pixelDistance);

	// Specular lighting not necessary for now
	float3 ambient = lightAmbient * color;
	float3 diffuse = max((dot(normal, lightDirection) * lightColor * shadowFactor), lightAmbient) * color;
	
	float3 outColor = diffuse + ambient;
	
	// Calculate fog
	float3 fogFactor = max((pixelDistance - fogDistance) / (farPlane - fogDistance), 0.0f);
	
	return lerp(outColor, fogColor, fogFactor);
	
}
	
	

technique Main
{
	pass Pass0
	{
		VertexShader = compile vs_5_0 VertexShaderFunction();
		PixelShader = compile ps_5_0 PixelShaderFunction();
	}
}