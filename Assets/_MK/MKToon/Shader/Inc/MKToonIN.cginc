#ifndef MK_TOON_IN
	#define MK_TOON_IN
struct Input 
{
	#if _USE_M_TEX
	float2 uv_MainTex;
	#else
	fixed4 color : COLOR;
	#endif
	#if _USE_NORMAL_MAP
	float2 uv_NormalMap;
	#endif
	#if _USE_NORMAL_MAP
	half3 worldNormal; INTERNAL_DATA
	#endif
	#if _USE_RIM
	half3 viewDir;
	#endif
};
#endif