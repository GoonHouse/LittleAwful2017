Shader "MK/MKToon/Transparent" 
{
	Properties 
	{
		_Color ("Color", Color) = (1,1,1,1)
		_Brightness("Brightness", Range (0.5, 2)) = 1
		_MainTex ("Color (RGB) Alpha (A)", 2D) = "white"
		_NormalMap ("Normalmap", 2D) = "bump" {}
		_Bumpiness("Bumpiness", Range (0.4, 1.6)) = 1
		_LightLevelsDiffuse("LLevelsDiffuse", Range (1.1, 6)) = 3
		_LightLevelsSpecular("LLevelsSpecular", Range (1.1, 6)) = 3
		_LThreshold("LTreshhold", Range (0.5, 1)) = 0.6
		_Shininess ("Shininess",  Range (5.0, 128)) = 20
		_SpecularColor ("SpecColor", Color) = (1,1,1,1)
		_ToonyFy("ToonyFy",  Range (0, 1)) = 1

		_RimColor ("Rim Color", Color) = (0.26,0.19,0.16,1.0)
		_RimSize ("Rim Size", Range(0.5,8.0)) = 3.0

		_ShadowColor ("Shadow Color", Color) = (0.0,0.0,0.0,0.37)

		_Smoothness ("Smoothness", Range(0.01,1)) = 0.01

		[Enum(UnityEngine.Rendering.CullMode)] _CullMode ("Culling", Float) = 2

		[Toggle(_USE_NORMAL_MAP)] _UseNormalMap ("Normal Map", Float) = 0
		[Toggle(_USE_RIM)] _UseRim ("Rim", Float) = 0
		[Toggle(_USE_SPECULAR)] _UseSpecular ("Specular", Float) = 0
		[Toggle(_USE_CUSTOM_SHADOW)] _UseCustomShadow ("Custom Shadow", Float) = 0
		[Toggle(_USE_M_TEX)] _UseMTex ("Main Texture", Float) = 1
		[KeywordEnum(Default, Treshold, Levels)] _LightMode ("Lightmode", Float) = 0
	}
	SubShader 
	{
		LOD 200

		Cull [_CullMode]
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
		Blend SrcAlpha OneMinusSrcAlpha
		ZWrite On

		CGPROGRAM
		#pragma shader_feature _USE_NORMAL_MAP
		#pragma shader_feature _USE_RIM
		#pragma shader_feature _LIGHTMODE_DEFAULT _LIGHTMODE_TRESHOLD _LIGHTMODE_LEVELS
		#pragma shader_feature _USE_SPECULAR
		#pragma shader_feature _USE_CUSTOM_SHADOW
		#pragma shader_feature _USE_M_TEX
		#pragma fragmentoption ARB_precision_hint_fastest
		#pragma surface surf MKToon interpolateview alpha
		#pragma target 2.0

		#include "/Inc/MKToonV.cginc"
		#include "/Inc/MKToonInc.cginc"
		#include "/Inc/MKToonIN.cginc"

		#include "/Inc/MKToonSurf.cginc"

		#include "/Inc/MKToonLighting.cginc"

		ENDCG
	}
	FallBack "Legacy Shaders/Transparent/Diffuse"
	CustomEditor "MKToon.MKToonEditor"
}