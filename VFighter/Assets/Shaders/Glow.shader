// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Glow" {
	Properties{
		_MainTex("Texture", 2D) = "white" {} 
        _OutlineTex("Outline Texture", 2D) = "white" {}
		_Color("Color", Color) = (1,1,1,1)
        _OutlineColor("Outline Color", Color) = (1,1,1,1)
		_Glow("Intensity", Range(0, 3)) = 1
        _OutlineWidth("Outline Width", Range(1.0,10.0)) = 1.1
       
	}
		SubShader{
			Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
			LOD 100
			Cull Off
			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha
            Pass{ //only renders once
                Name "OUTLINE"
                ZWrite Off //for transparent objects
                CGPROGRAM // allows communication between tow languages: shader lab and nvidia C for grahics

                //Function Defines = defines the name for the vertex and fragment functions
                #pragma vertex vert//Define for the building fucntion(shape)
                #pragma fragment frag//Define for coloring fucntion(color)

                //Inlcudes
                #include "UnityCG.cginc" //Built in shader functions

                //Structures - Can get data like - vetices, normals, color, uv
                struct appdata //how vertex function gets information
                {
                    float4 vertex : POSITION;
                    float uv : TEXCOORD0; //Textured coordinates
                };

                struct v2f //vertex to fragment(fragment info)
                {
                    float4 pos : SV_POSITION;
                    float2 uv : TEXCOORD0;
                };

                //Imports- Reimport property form shader lab to nvidia cg
                float _OutlineWidth;
                float4 _OutlineColor;
                half _Glow;
                sampler2D _OutlineTex;

                //Vertex Function
                v2f vert(appdata IN)
                {
                    IN.vertex.xyz *= _OutlineWidth;
                    v2f OUT;
                    OUT.pos = UnityObjectToClipPos(IN.vertex); //take object from object space to camera clip space--ie appear on screen
                    OUT.uv = IN.uv;
                    return OUT;
                }

                //Fragment function
                fixed4 frag(v2f IN) : SV_Target //Color of final image
                {
                    float4 texColor = tex2D(_OutlineTex,IN.uv);//gets texture and wraps it aorpudn the object
                    texColor *= _OutlineColor;
                    texColor *= _Glow;
                    return texColor;
                }

                ENDCG
            }
			Pass {
				CGPROGRAM
					#pragma vertex vert
					#pragma fragment frag

					sampler2D _MainTex;
					half4 _MainTex_ST;
					fixed4 _Color;
					half _Glow;

					struct vertIn {
						float4 pos : POSITION;
						half2 tex : TEXCOORD0;
					};

					struct v2f {
						float4 pos : SV_POSITION;
						half2 tex : TEXCOORD0;
					};

					v2f vert(vertIn v) {
						v2f o;
						o.pos = UnityObjectToClipPos(v.pos);
						o.tex = v.tex * _MainTex_ST.xy + _MainTex_ST.zw;
						return o;
					}

					fixed4 frag(v2f f) : SV_Target {
						fixed4 col = tex2D(_MainTex, f.tex);
						col *= _Color;
						col *= _Glow;
						return col;
					}
				ENDCG
			}
            
            
		}
}
