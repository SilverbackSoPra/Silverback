const float gamma = 1.0f / 2.2f;

float saturation;
float bloomPasses;

texture albedoMap;
sampler2D textureSampler = sampler_state {
    Texture = (albedoMap);
    MagFilter = Linear;
    MinFilter = Linear;
    AddressU = Clamp;
    AddressV = Clamp;
};

Texture2D bloom1;
Texture2D bloom2;
Texture2D bloom3;
Texture2D bloom4;
Texture2D bloom5;

SamplerState Sampler = sampler_state {
    MagFilter = Linear;
    MinFilter = Linear;
    AddressU = Clamp;
    AddressV = Clamp;
};


struct VertexShaderInput
{
	float2 vertexTexCoord : TEXCOORD0;
};

struct VertexShaderOutput
{
	float4 vertexPosition : POSITION0;
	float2 pixelTexCoord : TEXCOORD;
};


VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
	
	VertexShaderOutput output;

	output.vertexPosition = float4(input.vertexTexCoord, 0.0f, 1.0f);
	output.pixelTexCoord = (input.vertexTexCoord + 1.0f) / 2.0f;
	output.pixelTexCoord.y = 1.0f - output.pixelTexCoord.y;
	
	return output;
	
}


float3 ACESToneMap(float3 hdrColor) {
    float a = 2.51f;
    float b = 0.03f;
    float c = 2.43f;
    float d = 0.59f;
    float e = 0.14f;
    return clamp((hdrColor*(a*hdrColor+b))/
		(hdrColor*(c*hdrColor+d)+e), 0.0f, 1.0f);
}

float3 ToneMap(float3 hdrColor) {
	
	return float3(1.0f, 1.0f, 1.0f) - exp(-hdrColor);
	
}

float3 saturate(float3 color, float factor) {
	const float3 luma = float3(0.299f, 0.587f, 0.114f);
	float luminance = dot(color, luma);
    float3 pixelLuminance = float3(luminance, luminance, luminance);
	return lerp(pixelLuminance, color, factor);
}

float3 PixelShaderFunction(VertexShaderOutput input) : COLOR
{
	
	float3 outColor = float3(1.0f, 1.0f, 1.0f);
	
	float3 textureColor = tex2D(textureSampler, input.pixelTexCoord).rgb;
	
	if (bloomPasses > 0) {
		textureColor += bloom1.Sample(Sampler, input.pixelTexCoord).rgb;
	}
	if (bloomPasses > 1) {
		textureColor += bloom2.Sample(Sampler, input.pixelTexCoord).rgb;
	}
	if (bloomPasses > 2) {
		textureColor += bloom3.Sample(Sampler, input.pixelTexCoord).rgb;
	}
	if (bloomPasses > 3) {
		textureColor += bloom4.Sample(Sampler, input.pixelTexCoord).rgb;
	}
	if (bloomPasses > 4) {
		textureColor += bloom5.Sample(Sampler, input.pixelTexCoord).rgb;
	}
	
	outColor = ACESToneMap(textureColor);
	
	outColor = saturate(outColor, saturation);
	
	return outColor;
	
}
	
	

technique Main
{
	pass Pass0
	{
		VertexShader = compile vs_5_0 VertexShaderFunction();
		PixelShader = compile ps_5_0 PixelShaderFunction();
	}
}