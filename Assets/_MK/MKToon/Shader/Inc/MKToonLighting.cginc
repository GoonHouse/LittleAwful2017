#ifndef MK_TOON_LIGHTING
	#define MK_TOON_LIGHTING

fixed4 LightingMKToon (SurfaceOutput s, half3 lightDir, half3 viewDir, half atten)
{
	half ndl = max(0.0, dot (s.Normal, lightDir));
	ndl = lerp(ndl, ndl*T_V + T_V, _ToonyFy);
	half diffuseI = ndl;	
	#if !(POINT) && !(SPOT)
	diffuseI *= atten;
	#endif		
	#if _LIGHTMODE_LEVELS
	diffuseI *= LevelLighting(diffuseI, _LightLevelsDiffuse, _Smoothness, ndl);
	#elif _LIGHTMODE_TRESHOLD
	diffuseI *= TreshHoldLighting(_LThreshold, _Smoothness, ndl);
	#endif


	#if _USE_SPECULAR
	fixed3 specular;
	half specI;
	specular = atten * _LightColor0.rgb * _SpecularColor;
	#if _LIGHTMODE_LEVELS
	specI =  GetSpecular(ndl ,s.Normal, normalize(lightDir + viewDir), s.Specular * (_LightLevelsSpecular / 6.0));
	specular *= LevelLighting(specI, _LightLevelsSpecular, _Smoothness, specI);
	#elif _LIGHTMODE_TRESHOLD
	specI =  GetSpecular(ndl ,s.Normal, normalize(lightDir + viewDir), s.Specular);
	specular *= TreshHoldLighting(_LThreshold, _Smoothness, specI);
	#else
	specI =  GetSpecular(ndl ,s.Normal, normalize(lightDir + viewDir), s.Specular);
	specular *= specI;
	#endif
	#endif

	fixed4 c;

	#if _USE_CUSTOM_SHADOW
	half3 diffuse = half3(diffuseI,diffuseI,diffuseI);
	#if _LIGHTMODE_LEVELS
	_ShadowColor = lerp(_ShadowColor, 1, _ShadowColor.a-T_V);
	#else
	_ShadowColor = lerp(_ShadowColor, 1, _ShadowColor.a);
	#endif
	diffuse = lerp(_ShadowColor.rgb,1,diffuseI);
	c.rgb = s.Albedo * _LightColor0.rgb * diffuse * _Color.rgb;
	#else
	c.rgb = s.Albedo * _LightColor0.rgb * diffuseI * _Color.rgb;
	#endif

	#if _USE_SPECULAR
	c.rgb += specular * s.Gloss;
	#endif

	#if (POINT || SPOT)
	c.rgb *= atten;
	#endif

	c.a = s.Alpha;
	return c;
}
#endif