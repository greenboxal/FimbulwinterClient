float4x4 xViewProjection;


 Texture xTexture;

sampler TextureSampler = sampler_state { texture = <xTexture> ; magfilter = LINEAR; minfilter = LINEAR; mipfilter=LINEAR; AddressU = mirror; AddressV = mirror;};
struct VertexToPixel
{
    float4 Position     : POSITION;    
    float2 TexCoords    : TEXCOORD0;
};


struct PixelToFrame
{
    float4 Color        : COLOR0;
};


 VertexToPixel SimplestVertexShader( float4 inPos : POSITION, float2 inTexCoords : TEXCOORD0)
 {
     VertexToPixel Output = (VertexToPixel)0;
     
     Output.Position =mul(inPos, xViewProjection);
     Output.TexCoords = inTexCoords;
 
     return Output;
 }
 
 PixelToFrame OurFirstPixelShader(VertexToPixel PSIn)
 {
     PixelToFrame Output = (PixelToFrame)0;    
 
     Output.Color = tex2D(TextureSampler, PSIn.TexCoords);


    return Output;
}

technique Simplest
{
    pass Pass0
    {
        VertexShader = compile vs_2_0 SimplestVertexShader();
        PixelShader = compile ps_2_0 OurFirstPixelShader();
    }
}