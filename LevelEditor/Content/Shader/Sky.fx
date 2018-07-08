float4x4 MVPMatrix;

float3 sunLocation;
float3 fogColor;

Texture2D tint;
Texture2D sunMorph;
Texture2D moon;

SamplerState Sampler = sampler_state {
    MagFilter = Point;
    MinFilter = Point;
    AddressU = Wrap;
    AddressV = Wrap;
};

struct VertexShaderInput
{
	float3 vertexPosition : POSITION0;
};

struct VertexShaderOutput
{
	float4 vertexPosition : POSITION0;
	float3 pixelPosition : NORMAL0;
};


VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
	
	VertexShaderOutput output;

	output.vertexPosition = mul(float4(input.vertexPosition, 1.0f), MVPMatrix);
	output.pixelPosition = normalize(input.vertexPosition);
	
	return output;
	
}

float Hash( float n ){
        return frac( (1.0 + sin(n)) * 415.92653);
}
float noise( float3 x ){
    float xhash = Hash(round(400*x.x) * 37.0);
    float yhash = Hash(round(400*x.y) * 57.0);
    float zhash = Hash(round(400*x.z) * 67.0);
    return frac(xhash + yhash + zhash);
}

float3 PixelShaderFunction(VertexShaderOutput input) : COLOR
{
	
	float3 outColor = float3(1.0f, 1.0f, 1.0f);
	
	float3 sunNormal = normalize(sunLocation);
	float3 posNormal = normalize(input.pixelPosition);
	
 	float3 normal = posNormal;
	
	float dist = dot(float2(-normal.x, -normal.z), float2(sunNormal.x, sunNormal.z));
	
	float2 texCoords = float2(clamp((sunNormal.y + 1.0f) / 2.0f, 0.0f, 1.0f), clamp(normal.y, 0.01, 1.0f));
	float3 color = tint.Sample(Sampler, texCoords).rgb;
	
	float time = clamp(sunNormal.y, 0.01f, 1.0f);
	float radius = length(posNormal-sunNormal) / 0.1f;
	
	if (radius < 1.0f - 0.001f) {
		float4 sunColor = sunMorph.Sample(Sampler, float2(radius,time));
		color += sunColor.rgb * sunColor.a;
	}
	
	if(sunNormal.y<0.1){//Night or dawn
        float threshold = 0.995;
        //We generate a random value between 0 and 1
        float star_intensity = noise(posNormal);
        //And we apply a threshold to keep only the brightest areas
        if (star_intensity >= threshold){
            //We compute the star intensity
            star_intensity = pow((star_intensity - threshold)/(1.0 - threshold), 6.0)*(-sunNormal.y+0.1);
            color += float3(1.0f, 1.0f, 1.0f) * star_intensity;
        }
    }
	
	radius = length(posNormal + sunNormal);//the moon is at position -sun_pos
    if(radius < 0.03){//We are in the area of the sky which is covered by the moon
        //We define a local plane tangent to the skydome at -sun_norm
        //We work in model space (everything normalized)
        float3 n1 = normalize(cross(-sunNormal,float3(0,1,0)));
        float3 n2 = normalize(cross(-sunNormal,n1));
        //We project pos_norm on this plane
        float x = dot(posNormal,n1);
        float y = dot(posNormal,n2);
        //x,y are two sine, ranging approx from 0 to sqrt(2)*0.03. We scale them to [-1,1], then we will translate to [0,1]
        float scale = 23.57*0.5;
        //we need a compensation term because we made projection on the plane and not on the real sphere + other approximations.
        float compensation = 1.4;
        //And we read in the texture of the moon. The projection we did previously allows us to have an undeformed moon
        //(for the sun we didn't care as there are no details on it)
        color = lerp(color,moon.Sample(Sampler,float2(x,y)*scale*compensation+float2(0.5, 0.5)).rgb, clamp(-sunNormal.y*3,0,1));
    }
	
	float3 forwardNormal = normalize(float3(normal.x, 0.0f, normal.z));	
	
	float fogFactor = clamp((dot(normal, forwardNormal) - 0.9f) / 0.2f, 0.0f, 1.0f);
	
	return lerp(color, fogColor, fogFactor * 0.0001f);
	
}
	
	

technique Main
{
	pass Pass0
	{
		VertexShader = compile vs_5_0 VertexShaderFunction();
		PixelShader = compile ps_5_0 PixelShaderFunction();
	}
}