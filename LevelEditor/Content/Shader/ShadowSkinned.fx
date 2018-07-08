float4x4 modelMatrix;
float4x4 lightSpaceMatrix;

float4x4 bonesTransformation[64];

struct VertexShaderInput
{
	float3 vertexPosition : POSITION0;
	float3 vertexBoneWeights : BLENDWEIGHT0;
	uint3 vertexBoneIDs : BLENDINDICES0;
};

struct VertexShaderOutput
{
	float4 vertexPosition : POSITION0;
	float pixelDepth : TEXCOORD0;	
};
	

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
	
	VertexShaderOutput output;
	
	float4x4 boneTransform = bonesTransformation[input.vertexBoneIDs.x] * input.vertexBoneWeights.x;
	boneTransform += bonesTransformation[input.vertexBoneIDs.y] * input.vertexBoneWeights.y;
	boneTransform += bonesTransformation[input.vertexBoneIDs.z] * input.vertexBoneWeights.z;
	
	output.vertexPosition = mul(float4(input.vertexPosition, 1.0f), mul(mul(boneTransform, modelMatrix), lightSpaceMatrix));
	
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