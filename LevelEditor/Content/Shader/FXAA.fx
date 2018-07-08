float lumaThreshold;
float lumaThresholdMin;
bool debug;

float2 framebufferResolution;

texture albedoMap;
sampler2D textureSampler = sampler_state {
    Texture = (albedoMap);
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

float3 PixelShaderFunction(VertexShaderOutput input) : COLOR
{
	
	float3 outColor = float3(1.0f, 1.0f, 1.0f);
	
	// Lower to reduce dithering. Notice: Lowering also reduces AA
	const float fxaaSpanMax = 8.0f;
	
	
	const float fxaaReduceMul = 1.0f / 8.0f;
	const float fxaaReduceMin = 1.0f / 128.0f;

    float3 rgbNW = tex2D(textureSampler, input.pixelTexCoord + float2(-1.0f, -1.0f) / framebufferResolution).xyz;
    float3 rgbNE = tex2D(textureSampler, input.pixelTexCoord + float2(1.0f, -1.0f) / framebufferResolution).xyz;
    float3 rgbSW = tex2D(textureSampler, input.pixelTexCoord + float2(-1.0f, 1.0f) / framebufferResolution).xyz;
    float3 rgbSE = tex2D(textureSampler, input.pixelTexCoord + float2(1.0f, 1.0f) / framebufferResolution).xyz;
    float3 rgbM = tex2D(textureSampler, input.pixelTexCoord).xyz;
        
    float3 luma = float3(0.299f, 0.587f, 0.114f);
    float lumaNW = dot(rgbNW, luma);
    float lumaNE = dot(rgbNE, luma);
	float lumaSW = dot(rgbSW, luma);
    float lumaSE = dot(rgbSE, luma);
    float lumaM  = dot(rgbM,  luma);
        
    float lumaMin = min(lumaM, min(min(lumaNW, lumaNE), min(lumaSW, lumaSE)));
    float lumaMax = max(lumaM, max(max(lumaNW, lumaNE), max(lumaSW, lumaSE)));
	
	if (lumaMax - lumaMin < max(lumaThresholdMin, lumaMax * lumaThreshold)) {
		return rgbM;
	}
        
    float2 dir;
    dir.x = -((lumaNW + lumaNE) - (lumaSW + lumaSE));
    dir.y =  ((lumaNW + lumaSW) - (lumaNE + lumaSE));
        
    float dirReduce = max(
            (lumaNW + lumaNE + lumaSW + lumaSE) * (0.25f * fxaaReduceMul),
            fxaaReduceMin);
          
    float rcpDirMin = 1.0f / (min(abs(dir.x), abs(dir.y)) + dirReduce);
        
    dir = min(float2(fxaaSpanMax,  fxaaSpanMax),
            max(float2(-fxaaSpanMax, -fxaaSpanMax),
            dir * rcpDirMin)) / framebufferResolution;
                
    float3 rgbA = (1.0f / 2.0f) * (
            tex2D(textureSampler, input.pixelTexCoord.xy + dir * (1.0f / 3.0f - 0.5f)).xyz +
            tex2D(textureSampler, input.pixelTexCoord.xy + dir * (2.0f / 3.0f - 0.5f)).xyz);
			
    float3 rgbB = rgbA * (1.0f / 2.0f) + (1.0f / 4.0f) * (
            tex2D(textureSampler, input.pixelTexCoord.xy + dir * 0.5f).xyz +
            tex2D(textureSampler, input.pixelTexCoord.xy + dir * -0.5f).xyz);
        
	float lumaB = dot(rgbB, luma);

    if(lumaB < lumaMin || lumaB > lumaMax) {
        outColor.xyz = rgbA;
    }
	else{
        outColor.xyz = rgbB;
    }
	
	if (debug) {
		outColor = float3(1.0f, 0.0f, 0.0f);
	}
	
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