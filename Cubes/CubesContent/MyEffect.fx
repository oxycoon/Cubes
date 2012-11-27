// --------- Variables --------------
float4x4 xViewProjection;
float4x4 xWorld;
float4x4 xView;
float4x4 xProjection;

uniform extern float3 xLightDirection;
uniform extern float3 xDiffuseLight;
uniform extern float3 xDiffuseMaterial;
uniform extern float3 xAmbientLight;
uniform extern float3 xAmbientMaterial;

bool xEnableLighting;
bool xEnableLightingColor;
bool xEnableLightingTexture;
// --------- End variables ----------

//------- Texture Samplers --------

Texture xTexture;
sampler TextureSampler = sampler_state { texture = <xTexture>; magfilter = LINEAR; minfilter = LINEAR; mipfilter=LINEAR; AddressU = mirror; AddressV = mirror;};
//------- End textures ----------

//VS out-data
struct VertexToPixel
{
    float4 Position			: POSITION;
    float4 Color			: COLOR0;
	float Normal			: TEXCOORD0;
	float2 TextureCoords	: TEXCOORD1;
};

//PS out-data
struct PixelToFrame
{
    float4 Color        : COLOR0;
};

// --------- Technique: Simplest ----------
VertexToPixel SimplestVertexShader( float4 inPos : POSITION, float4 inColor : COLOR0)
{
    VertexToPixel Output = (VertexToPixel)0;
     
    Output.Position = mul(inPos, xViewProjection);
    Output.Color = inColor;
 
    return Output;
}
 
 
PixelToFrame MyFirstPixelShader(VertexToPixel PSIn)
{
    PixelToFrame Output = (PixelToFrame)0;    
 
    Output.Color = PSIn.Color;    
 
    return Output;
}
 
technique Simplest
{
    pass Pass0
    {
        VertexShader = compile vs_2_0 SimplestVertexShader();
        PixelShader = compile ps_2_0 MyFirstPixelShader();
    }
}

// --------- Technique: PhongTexturedShader ----------
VertexToPixel PhongTexturedVertexShader( float4 inPos : POSITION, float3 inNormal : NORMAL, float2 inTexCoords : TEXCOORD1 )
{
	VertexToPixel Output = (VertexToPixel)0;

	float4x4 preVP = mul(xView, xProjection);
	float4x4 preWVP = mul(xWorld, preVP);

	Output.Position = mul(inPos, preWVP);

	float3 normal = normalize(inNormal);

	float3x3 rotMat = (float3x3)xWorld;
	float3 rotNor = mul(normal, rotMat);
	Output.Normal = rotNor;

	Output.TextureCoords = inTexCoords;

	return Output;
}

PixelToFrame PhongTexturedPixelShader(VertexToPixel PSIn)
{
	PixelToFrame Output = (PixelToFrame)0;    
 
	float3 normal = PSIn.Normal;

	float s = max(dot(normal, -xLightDirection), 0.0f);
	
	float3 diffuse = s * (xDiffuseMaterial * xDiffuseLight).rgb;
	float3 ambient = xAmbientMaterial * xAmbientLight;
	
	Output.Color = tex2D(TextureSampler, PSIn.TextureCoords);
	//Checks if light is enabled.
	if(xEnableLighting)
	{
		Output.Color.rgb = (ambient + diffuse);
	}
	//checks if light and texture mixing is enabled.
	else if(xEnableLightingTexture)
	{
		Output.Color.rgb *= saturate(diffuse) + ambient;
	}

    return Output;
}

technique PhongTexturedShader
{
	pass Pass0
	{
		VertexShader = compile vs_2_0 PhongTexturedVertexShader();
		PixelShader = compile ps_2_0 PhongTexturedPixelShader();
	}
}

// --------- Technique: PhongShader ----------
VertexToPixel PhongVertexShader( float4 inPos : POSITION, float4 inColor : COLOR0, float3 inNormal : NORMAL)
{
	VertexToPixel Output = (VertexToPixel)0;

	float4x4 preVP = mul(xView, xProjection);
	float4x4 preWVP = mul(xWorld, preVP);

	Output.Position = mul(inPos, preWVP);

	float3 normal = normalize(inNormal);

	float3x3 rotMat = (float3x3)xWorld;
	float3 rotNor = mul(normal, rotMat);
	Output.Normal = rotNor;

	Output.Color = inColor;

	return Output;
}

PixelToFrame PhongPixelShader(VertexToPixel PSIn)
{
	PixelToFrame Output = (PixelToFrame)0;    
 
	float3 normal = PSIn.Normal;

	float s = max(dot(normal, -xLightDirection), 0.0f);
	
	float3 diffuse = s * (xDiffuseMaterial * xDiffuseLight).rgb;
	float3 ambient = xAmbientMaterial * xAmbientLight;
	
	Output.Color = PSIn.Color;
	//Checks if light is enabled.
	if(xEnableLighting)
		Output.Color.rgb = (ambient + diffuse);
	//checks if light and color mixing is enabled.
	if(xEnableLightingColor)
		Output.Color.rgb = PSIn.Color * (ambient + diffuse);

    return Output;
}

technique PhongShader
{
	pass Pass0
	{
		VertexShader = compile vs_2_0 PhongVertexShader();
		PixelShader = compile ps_2_0 PhongPixelShader();
	}
}