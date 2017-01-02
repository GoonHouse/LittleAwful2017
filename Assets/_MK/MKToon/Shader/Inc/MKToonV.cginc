#ifndef MK_TOON_V
	#define MK_TOON_V

#define T_V 0.5

uniform half _ToonyFy;
#if _USE_CUSTOM_SHADOW
uniform fixed4 _ShadowColor;
#endif
uniform fixed4 _Color;
uniform half _Brightness;
#if _USE_M_TEX
uniform sampler2D _MainTex;
#endif
#if _USE_NORMAL_MAP
uniform sampler2D _NormalMap;
uniform half _Bumpiness;
#endif
#if _LIGHTMODE_LEVELS
uniform half _LightLevelsDiffuse;
#if _USE_SPECULAR
uniform half _LightLevelsSpecular;
#endif
#elif _LIGHTMODE_TRESHOLD
uniform half _LThreshold;
#endif
#if _USE_SPECULAR
uniform half _Shininess;
uniform fixed4 _SpecularColor;
#endif
#if _USE_RIM
uniform fixed4 _RimColor;
uniform half _RimSize;
#endif
#if _LIGHTMODE_LEVELS || _LIGHTMODE_TRESHOLD
uniform half _Smoothness;
#endif
#endif