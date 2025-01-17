﻿Shader "Custom/NormalExtractor"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
		_BumpMap("Bumpmap",2D) = "bump" {}

		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
			_Point("Point",float) = (0,0,0)
			_StoredNormal("StoredNormal",float) = (0,0,0)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;
		sampler2D _BumpMap;

        struct Input
        {
            float2 uv_MainTex;
			float2 uv_BumpMap;
			float3 worldPos;
        };

        half _Glossiness;
        half _Metallic;
		float3 _Point;
        fixed4 _Color;
		float3 _StoredNormal;
		


        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;

			o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));


			float dist = sqrt(pow(_Point.x - IN.worldPos.x, 2) + pow(_Point.y - IN.worldPos.y, 2) + pow(_Point.z - IN.worldPos.z, 2));
			if (dist < 0.1) {
				o.Albedo = float4(1, 0, 0, 1);
				_StoredNormal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
				
			}
			else {
				o.Albedo = c.rgb;
			}
        }
        ENDCG
    }
    FallBack "Diffuse"
}
