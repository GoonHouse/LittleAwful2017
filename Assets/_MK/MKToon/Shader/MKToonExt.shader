Shader "Hidden/MKToon/Ext" 
{
	SubShader 
	{ 
		Pass
		{
			Tags { "RenderType"="Opaque" }
			LOD 200
			Name "OUTLINE"
			Cull Front
			Lighting Off
			Blend One Zero
			CGPROGRAM 
			#pragma vertex vert 
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest


			fixed4 _OutlineColor;
			float _OutlineSize;
			struct Input
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};

			struct Output
			{
				float4 pos : SV_POSITION;
				fixed4 color : TEXCOORD0;
			};

			Output vert(Input i)
			{
				Output o;

				i.vertex.xyz += normalize(i.normal) * _OutlineSize;
				o.pos = mul(UNITY_MATRIX_MVP, i.vertex);
				o.color = _OutlineColor;
				return o;
			}

			fixed4 frag(Output o) : SV_Target
			{
				return o.color;
			}
			
			ENDCG 
		}
		Pass
		{
			Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
			LOD 200
			Name "T_OUTLINE"
			Cull Front
			Lighting Off
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM 
			#pragma vertex vert 
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest

			fixed4 _OutlineColor;
			float _OutlineSize;
			struct Input
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};

			struct Output
			{
				float4 pos : SV_POSITION;
				fixed4 color : TEXCOORD0;
			};

			Output vert(Input i)
			{
				Output o;

				i.vertex.xyz += normalize(i.normal) * _OutlineSize;
				o.pos = mul(UNITY_MATRIX_MVP, i.vertex);
				o.color = _OutlineColor;
				return o;
			}

			fixed4 frag(Output o) : SV_Target
			{
				return o.color;
			}
			
			ENDCG 
		}
	}
	FallBack "Diffuse"
}