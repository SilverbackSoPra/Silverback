float bloomThreshold;
float bloomPower;
float bloomIntensity;

float2 blurScale;
static int kernelSize = 6;
static float weights[] = {0.133176, 0.232618, 0.135526, 0.051231, 0.012558, 0.001479};
static float offsets[] = {0.0, 1.458429, 3.403985, 5.351806, 7.302940, 9.0}; 

texture source;
sampler2D textureSampler = sampler_state {
    Texture = (source);
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

float luminance(float3 color) {
	return dot(color, float3(0.299f, 0.587f, 0.114f));
}

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
	
	VertexShaderOutput output;

	output.vertexPosition = float4(input.vertexTexCoord, 0.0f, 1.0f);
	output.pixelTexCoord = (input.vertexTexCoord + 1.0f) / 2.0f;
	output.pixelTexCoord.y = 1.0f - output.pixelTexCoord.y;
	
	return output;
	
}


float3 PixelShaderFunctionMain(VertexShaderOutput input) : COLOR
{
	
	float3 outColor = tex2D(textureSampler, input.pixelTexCoord).rgb;
	
	float pixelLuminance = luminance(outColor);
	float bloomBrightness = pixelLuminance > bloomThreshold ? pixelLuminance : 0.0f;
	
	bloomBrightness = pow(bloomBrightness, bloomPower);
	return outColor * bloomBrightness * bloomIntensity;
	
}

float3 PixelShaderFunctionHorizontalBlur(VertexShaderOutput input) : COLOR
{
	
	float3 outColor = tex2D(textureSampler, input.pixelTexCoord).rgb * weights[0];
	
	for (int i = 1; i < kernelSize; i++) 
	{
		outColor += tex2D(textureSampler, input.pixelTexCoord + float2(offsets[i], 0.0f) * blurScale).rgb * weights[i];
	}
	
	for (int j = 1; j < kernelSize; j++) 
	{
		outColor += tex2D(textureSampler, input.pixelTexCoord - float2(offsets[j], 0.0f) * blurScale).rgb * weights[j];
	}
	
	return outColor;
	
}

float3 PixelShaderFunctionVerticalBlur(VertexShaderOutput input) : COLOR
{
	
	float3 outColor = tex2D(textureSampler, input.pixelTexCoord).rgb * weights[0];
	
	for (int i = 1; i < kernelSize; i++) 
	{
		outColor += tex2D(textureSampler, input.pixelTexCoord + float2(0.0f, offsets[i]) * blurScale).rgb * weights[i];
	}
	
	for (int j = 1; j < kernelSize; j++) 
	{
		outColor += tex2D(textureSampler, input.pixelTexCoord - float2(0.0f, offsets[j]) * blurScale).rgb * weights[j];
	}
	
	return outColor;
	
}

float3 PixelShaderFunctionScale(VertexShaderOutput input) : COLOR
{
	
	return tex2D(textureSampler, input.pixelTexCoord).rgb;
	
}
	
	

technique Main
{
	pass Pass0
	{
		VertexShader = compile vs_5_0 VertexShaderFunction();
		PixelShader = compile ps_5_0 PixelShaderFunctionMain();
	}
}

technique Blur 
{
	pass Horizontal
	{
		VertexShader = compile vs_5_0 VertexShaderFunction();
		PixelShader = compile ps_5_0 PixelShaderFunctionHorizontalBlur();
	}
	
	pass Vertical
	{
		VertexShader = compile vs_5_0 VertexShaderFunction();
		PixelShader = compile ps_5_0 PixelShaderFunctionVerticalBlur();
	}
}

technique Scale
{
	pass Scale
	{
		VertexShader = compile vs_5_0 VertexShaderFunction();
		PixelShader = compile ps_5_0 PixelShaderFunctionScale();
	}
}