Shader "Transparent/PlanetAtmosphere" {
Properties {
	_Atmosphere ("Atmosphere", Range (0.0,5.0)) = 1.0
	_Intensity ("Intensity", Range (0.0,15.0)) = 1.0
	_AtmosphereShift ("Atmosphere Shift", Range (0.0,5.0)) = 1.0
	_AtmospherePow ("Atmosphere Pow", Range (0.0,5.0)) = 1.0
	_MainTex ("Atmosphere Gradient", 2D) = "white" {}
}

SubShader {
	Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
	LOD 200

	Lighting Off
	
CGPROGRAM
#pragma surface surf Lambert alpha

sampler2D _MainTex;
half _Atmosphere;
half _Intensity;
half _AtmosphereShift;
half _AtmospherePow;

struct Input {
	float2 uv_MainTex;
};

void surf (Input IN, inout SurfaceOutput o) {
	half3 diffuse = half3( 0.0, 0.0, 0.0);
}
ENDCG
}

Fallback "Transparent/VertexLit"
}
