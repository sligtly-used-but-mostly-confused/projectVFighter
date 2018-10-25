﻿Shader "Custom/celshadeTest" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
        
        _MainTex ("Texture", 2D) = "white" {}
        _Treshold ("Cel treshold", Range(1., 20.)) = 5.
        _Ambient ("Ambient intensity", Range(0., 0.5)) = 0.1
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
            float3 worldNormal;
		};

		fixed4 _Color;
        
        float LightToonShading(float3 normal, float3 lightDir, float threshold)
            {
                float NdotL = max(0.0, dot(normalize(normal), normalize(lightDir)));
                return float(floor(float(NdotL * threshold)) / (threshold - 0.5));
            }
		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex);
            c.rgb *= saturate(LightToonShading(IN.worldNormal, _WorldSpaceLightPos0.xyz, _Treshold) + _Ambient) * _LightColor0.rgb;
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
        
		ENDCG
	}
	FallBack "Diffuse"
}
