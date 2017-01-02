#ifndef MK_TOON_INC
	#define MK_TOON_INC

inline half3 CalculateNormals(sampler2D normalMap, float2 uv, half bumpiness)
{
	half3 unpackedNormals = half3(tex2D(normalMap, uv).rg * 2 - 1, 2-bumpiness);
	return normalize(unpackedNormals);
}

inline half3 LightDirection(half3 worldPosition, half3 lightPosition, half3 lT)
{
	return normalize(lerp(lightPosition, lightPosition - worldPosition, lT));
}

inline half GetSpecular(half ndl, half3 normals, half3 halfV, half shine)
{
	return pow(max(0.0,dot( normals, halfV)), shine);
}

inline half LevelLighting(half brightness, half levels, half smoothness, half v)
{
	half levelC = floor(brightness * levels);
	return lerp(levelC / levels, 1, v*(smoothness));
}

inline half TreshHoldLighting(half lThreshold, half smoothness, half v)
{
	return smoothstep(lThreshold-smoothness, lThreshold+smoothness, v);
}

inline fixed3 RimColor(half rimSize, half3 vDn, fixed4 col)
{
	return col * pow ((1.0 - saturate(vDn)), rimSize) * (col.a*5);
}
#endif