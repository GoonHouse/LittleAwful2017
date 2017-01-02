#ifndef MK_TOON_SURF
	#define MK_TOON_SURF
void surf (Input IN, inout SurfaceOutput o) 
{
	fixed4 c;
	#if _USE_M_TEX
	c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
	#else
	c = IN.color * _Color;
	#endif
	c.rgb = lerp(c.rgb * _Brightness, pow(c.rgb, 1.75) * (_Brightness*1.2), _ToonyFy);
	o.Albedo = c.rgb;
	#if _USE_NORMAL_MAP
	o.Normal = CalculateNormals(_NormalMap, IN.uv_NormalMap, _Bumpiness);
	#endif
	#if _USE_SPECULAR
	o.Specular = _Shininess;
	o.Gloss = _SpecularColor.a;
	#endif
	#if _USE_RIM
	half vDn = dot(IN.viewDir, o.Normal);
	fixed3 rimColor  = RimColor(_RimSize, vDn, _RimColor);
	o.Emission = rimColor;
	#endif
	o.Alpha = c.a;
}
#endif