//--------------------------------------------------------------------------------------
// 
// WPF ShaderEffect HLSL Template
//
//--------------------------------------------------------------------------------------

//-----------------------------------------------------------------------------------------
// Shader constant register mappings (scalars - float, double, Point, Color, Point3D, etc.)
//-----------------------------------------------------------------------------------------

float opacity : register(C0);
sampler2D implicitInputSampler : register(S0);

float4 main(float2 uv : TEXCOORD) : COLOR
{
	// Get the source color
	float4 color = tex2D(implicitInputSampler, uv);

	// Return new color
	return color * opacity;
}


