// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/PixelShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Treshold ("Cel treshold", Range(1., 20.)) = 2.5
        _Ambient ("Ambient intensity", Range(0., 0.5)) = 0.1
        _Columns("Pixel Columns", int) = 64
        _Rows("Pixel Rows", int) = 64
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "LightMode"="ForwardBase" }
 
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
           
            #include "UnityCG.cginc"


            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldNormal : NORMAL;
            };
 
            float _Treshold;
 
            float LightToonShading(float3 normal, float3 lightDir)
            {
                float NdotL = max(0.0, dot(normalize(normal), normalize(lightDir)));
                return floor(NdotL * _Treshold) / (_Treshold - 0.5);
            }
 
            sampler2D _MainTex;
            float4 _MainTex_ST;
            int _Columns;
            int _Rows;
 
            v2f vert (appdata_full v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                //o.pos = o.pos - _Columns;
                float2 offset = v.texcoord;
                offset.x *= _Columns;
                offset.y *= _Rows;
                offset.x = round(offset.x);
                offset.y = round(offset.y);
                offset.x /= _Columns;
                offset.y /= _Rows;
                o.uv = TRANSFORM_TEX(offset, _MainTex);
                o.worldNormal = mul(v.normal.xyz, (float3x3) unity_WorldToObject);

                return o;
            }
 
            fixed4 _LightColor0;
            half _Ambient;
 
            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;
                fixed4 col = tex2D(_MainTex, uv);
                col.rgb *= saturate(LightToonShading(i.worldNormal, _WorldSpaceLightPos0.xyz) + _Ambient) * _LightColor0.rgb;
                return col;
            }
            ENDCG
        }
    }
}