Shader "Custom/BendTransparent" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB) Alpha (A)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
	}
	SubShader {
		Tags { "Queue"="Transparent" "RenderType"="Transparent" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard alpha:blend vertex:vert

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		uniform half3 _CurveOrigin;
		uniform fixed3 _ReferenceDirection;
		uniform half _Curvature;
		uniform fixed3 _Scale;
		uniform half _FlatMargin;
		uniform half _HorizonWaveFrequency;

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
		};

		half4 Bend(half4 v)
		{
			half4 wpos = mul (unity_ObjectToWorld, v);

			half2 xzDist = (wpos.xz - _CurveOrigin.xz) / _Scale.xz;
			half dist = length(xzDist);

			half2 direction = lerp(_ReferenceDirection.xz, xzDist, min(dist, 1));

			half theta = acos(clamp(dot(normalize(direction), _ReferenceDirection.xz), -1, 1));

			dist = max(0, dist - _FlatMargin);

			wpos.y -= dist * dist * _Curvature * cos(theta * _HorizonWaveFrequency);

			wpos = mul (unity_WorldToObject, wpos);

			return wpos;
		}

		void vert (inout appdata_full v)
		{
			half4 vpos = Bend(v.vertex);

			v.vertex = vpos;
		}

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb * 10;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
