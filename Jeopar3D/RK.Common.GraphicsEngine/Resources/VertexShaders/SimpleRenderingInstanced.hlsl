//-----------------------------------------------------------------------------
//Constant buffers
cbuffer arguments                  : register(c0)
{
    float4x4  WorldViewProj;
    float4x4  World;
    float3    LightPosition;
    float     LightPower;
    float     StrongLightFactor;
    float     Ambient;
    float     Opacity;
    float     Dummy2;
}

//-----------------------------------------------------------------------------
//VertexShader input
struct VS_IN
{
    //Object data
    float3 pos        : POSITION0;
    float3 normal     : NORMAL0;
    float4 col        : COLOR0;
    float2 tex        : TEXCOORD0;

    //Per instance data
    float4 transform1 : POSITION1;
    float4 transform2 : POSITION2;
    float4 transform3 : POSITION3;
    float4 transform4 : POSITION4;
};

//PixelShader input
struct PS_IN
{
    float4 pos    : SV_POSITION;
    float4 col    : COLOR0;
    float2 tex    : TEXCOORD0;
    float3 normal : TEXCOORD1;
    float3 pos3D  : TEXCOORD2;
};

//-----------------------------------------------------------------------------
//VertexShader implementation
PS_IN main(VS_IN input) 
{
    PS_IN output = (PS_IN)0;
    
    //Build per instance transform matrix
    float4x4 perInstanceTransform = float4x4(
    	input.transform1.r, input.transform1.g, input.transform1.b, input.transform1.a,
    	input.transform2.r, input.transform2.g, input.transform2.b, input.transform2.a,
    	input.transform3.r, input.transform3.g, input.transform3.b, input.transform3.a,
    	input.transform4.r, input.transform4.g, input.transform4.b, input.transform4.a);

    //Calculate standard values
    float4 instanceRelatedPosition = mul(float4(input.pos.xyz, 1.0), perInstanceTransform);
    output.pos = mul(float4(instanceRelatedPosition.xyz, 1.0), WorldViewProj);
    output.col = input.col;
    output.tex = input.tex;
    
    //Calculate values needed for per-pixel lighting
    output.normal = normalize(mul(input.normal, (float3x3)World));
    output.pos3D = mul(float4(instanceRelatedPosition.xyz, 1.0), World);
     
    return output;
}
