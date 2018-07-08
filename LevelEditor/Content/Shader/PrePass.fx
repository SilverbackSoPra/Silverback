float4x4 modelMatrix;
float4x4 viewMatrix;
float4x4 projectionMatrix;


struct VertexShaderInput
{
	float3 vertexPosition : POSITION0;
};

struct VertexShaderOutput
{
	float4 vertexPosition : POSITION0;
};
	

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
	
	VertexShaderOutput output;
	
	output.vertexPosition = mul(mul(float4(input.vertexPosition, 1.0f), modelMatrix), viewMatrix);
	output.vertexPosition = mul(output.vertexPosition, projectionMatrix);
	
	return output;
	
}
	

technique Main
{
	pass Pass0
	{
		VertexShader = compile vs_5_0 VertexShaderFunction();
	}
}