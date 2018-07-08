float4x4 modelMatrix;
float4x4 lightSpaceMatrix;

struct VertexShaderInput
{
	float3 vertexPosition : POSITION0;
};

struct VertexShaderOutput
{
	float4 vertexPosition : POSITION0;
	float pixelDepth : TEXCOORD0;
	
};
	

VertexShaderOutput VertexShaderFunction(float3 vertexPosition: SV_POSITION)
{
	
	VertexShaderOutput output;
	
	output.vertexPosition = mul(float4(vertexPosition, 1.0f), mul(modelMatrix, lightSpaceMatrix));	
	output.pixelDepth = output.vertexPosition.z / output.vertexPosition.w;
	
	return output;
	
}

float3 PixelShaderFunction(VertexShaderOutput input) : COLOR
{
	
	return float3(input.pixelDepth, 0.0f, 0.0f);
	
}
	
	

technique Main
{
	pass Pass0
	{
		VertexShader = compile vs_5_0 VertexShaderFunction();
		PixelShader = compile ps_5_0 PixelShaderFunction();
	}
}