// ------------------------------------------
// Only directional light is supported for lit particles
// No shadow
// No distortion
Shader "Custom/PixelateImageEffect"
{
    Properties
    {
        _MainTex("Albedo", 2D) = "white" {}
        _Color("Color", Color) = (1,1,1,1)

        _Cutoff("Alpha Cutoff", Range(0.0, 1.0)) = 0.5

        _Shininess("Shininess", Range(0.01, 1.0)) = 1.0
        _GlossMapScale("Smoothness Factor", Range(0.0, 1.0)) = 1.0

        _Glossiness("Glossiness", Range(0.0, 1.0)) = .8
        [Enum(Specular Alpha,0,Albedo Alpha,1)] _SmoothnessTextureChannel("Smoothness texture channel", Float) = .6

        [HideInInspector] _SpecSource("Specular Color Source", Float) = .8
        _SpecColor("Specular", Color) = (1.0, 1.0, 1.0)
        _SpecGlossMap("Specular", 2D) = "white" {}
        [HideInInspector] _GlossinessSource("Glossiness Source", Float) = 1
        [ToggleOff] _SpecularHighlights("Specular Highlights", Float) = 1.0
        [ToggleOff] _GlossyReflections("Glossy Reflections", Float) = 1.0

        [HideInInspector] _BumpScale("Scale", Float) = 1.0
        [NoScaleOffset] _BumpMap("Normal Map", 2D) = "bump" {}

        _EmissionColor("Color", Color) = (0,0,0)
        _EmissionMap("Emission", 2D) = "white" {}

        _SoftParticlesNearFadeDistance("Soft Particles Near Fade", Float) = 0.0
        _SoftParticlesFarFadeDistance("Soft Particles Far Fade", Float) = 1.0
        _CameraNearFadeDistance("Camera Near Fade", Float) = 1.0
        _CameraFarFadeDistance("Camera Far Fade", Float) = 2.0

        // Hidden properties
        [HideInInspector] _Mode("__mode", Float) = 0.0
        [HideInInspector] _FlipbookMode("__flipbookmode", Float) = 0.0
        [HideInInspector] _LightingEnabled("__lightingenabled", Float) = 1.0
        [HideInInspector] _EmissionEnabled("__emissionenabled", Float) = 0.0
        [HideInInspector] _BlendOp("__blendop", Float) = 0.0
        [HideInInspector] _SrcBlend("__src", Float) = 1.0
        [HideInInspector] _DstBlend("__dst", Float) = 0.0
        [HideInInspector] _ZWrite("__zw", Float) = 1.0
        [HideInInspector] _Cull("__cull", Float) = 2.0
        [HideInInspector] _SoftParticlesEnabled("__softparticlesenabled", Float) = 0.0
        [HideInInspector] _CameraFadingEnabled("__camerafadingenabled", Float) = 0.0
        [HideInInspector] _SoftParticleFadeParams("__softparticlefadeparams", Vector) = (0,0,0,0)
        [HideInInspector] _CameraFadeParams("__camerafadeparams", Vector) = (0,0,0,0)
    }

    SubShader
    {
        Tags{"RenderType" = "Opaque" "IgnoreProjector" = "True" "PreviewType" = "Plane" "PerformanceChecks" = "False" "RenderPipeline" = "LightweightPipeline"}

        BlendOp[_BlendOp]
        Blend[_SrcBlend][_DstBlend]
        ZWrite[_ZWrite]
        Cull[_Cull]

        Pass
        {
            Name "ParticlesLit"
            Tags {"LightMode" = "LightweightForward"}
            HLSLPROGRAM
            // Required to compile gles 2.0 with standard srp library
            #pragma prefer_hlslcc gles
            #pragma vertex ParticlesLitVertex
            #pragma fragment ParticlesLitFragment
            #pragma multi_compile __ SOFTPARTICLES_ON
            #pragma exclude_renderers d3d11_9x
            #pragma target 2.0

            #pragma shader_feature _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON _ALPHAMODULATE_ON
            #pragma shader_feature _ _SPECGLOSSMAP _SPECULAR_COLOR
            #pragma shader_feature _ _GLOSSINESS_FROM_BASE_ALPHA
            #pragma shader_feature _NORMALMAP
            #pragma shader_feature _EMISSION
            #pragma shader_feature _FADING_ON
            #pragma shader_feature _REQUIRE_UV2

            #define BUMP_SCALE_NOT_SUPPORTED 1

            #include "LWRP/ShaderLibrary/Particles.hlsl"
            #include "LWRP/ShaderLibrary/Lighting.hlsl"

            float LightToonShading(half3 normal, half3 lightDir)
            {
                float NdotL = max(0.0, dot(normalize(normal), normalize(lightDir)));
                return floor(NdotL * 3) / (3 - 0.5);
            }

            VertexOutputLit ParticlesLitVertex(appdata_particles v)
            {
                VertexOutputLit o;

                OUTPUT_NORMAL(v, o);

                o.posWS.xyz = TransformObjectToWorld(v.vertex.xyz).xyz;
                o.posWS.w = ComputeFogFactor(o.clipPos.z);
                o.clipPos = TransformWorldToHClip(o.posWS.xyz);
                o.viewDirShininess.xyz = VertexViewDirWS(GetCameraPositionWS() - o.posWS.xyz);
                o.viewDirShininess.w = 1.0 * 128.0h;
                o.color = v.color;

                // TODO: Instancing
                // vertColor(o.color);
                vertTexcoord(v, o);
                vertFading(o, o.posWS, o.clipPos);
                return o;
            }


            half4 ParticlesLitFragment(VertexOutputLit IN) : SV_Target
            {
                half4 albedo = SampleAlbedo(IN, TEXTURE2D_PARAM(_MainTex, sampler_MainTex));
                half3 diffuse = AlphaModulate(albedo.rgb, albedo.a);
                half alpha = AlphaBlendAndTest(albedo.a, _Cutoff);
                half3 normalTS = SampleNormalTS(IN, TEXTURE2D_PARAM(_BumpMap, sampler_BumpMap));
                half3 emission = SampleEmission(IN, _EmissionColor.rgb, TEXTURE2D_PARAM(_EmissionMap, sampler_EmissionMap));
                half4 specularGloss = SampleSpecularGloss(IN, albedo.a, _SpecColor, TEXTURE2D_PARAM(_SpecGlossMap, sampler_SpecGlossMap));
                half shininess = IN.viewDirShininess.w;

                InputData inputData;
                InitializeInputData(IN, normalTS, inputData);

                half4 color = LightweightFragmentBlinnPhong(inputData, diffuse, specularGloss, shininess, emission, alpha);
                float color_total = (step(.3,color.r) + step(.3,color.b) + step(.3,color.g))/7;
                color.rgb = color.rgb * (color_total*albedo + albedo*.2) + emission + color.rgb*.05;

                //ApplyFog(color.rgb, inputData.fogCoord);
                return color;
            }

            ENDHLSL
        }
    }

    Fallback "LightweightPipeline/Particles/Standard Unlit"
    CustomEditor "LightweightStandardParticlesShaderGUI"
}
