Shader "Unlit/Stencil_Unlit_Shader"
{
    Properties
    {
        [MainTexture] _BaseMap("Main Texture", 2D) = "white" {}
        [MainColor] _Color("Color", Color) = (1,1,1,1)
        [IntRange] _StencilID("Stencil ID", Range(0, 255)) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque"
                "Queue" = "Transparent+6"
                "RenderPipeline" = "UniversalPipeline"}
        LOD 100
        Pass
        {
            Stencil {
                Ref [_StencilID]
                Comp Equal
            }

            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float2 uv : TEXCOORD0;
                //UNITY_FOG_COORDS(1)
                float4 positionHCS : SV_POSITION;
            };

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            CBUFFER_START(UnityPerMaterial)
            float4 _Color;
            float4 _BaseMap_ST;
            CBUFFER_END

            Varyings vert (Attributes IN)
            {
                Varyings OUT;

                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = TRANSFORM_TEX(IN.uv, _BaseMap);

                return OUT;
            }

            half4 frag (Varyings IN) : SV_Target
            {
                half4 texel = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv);
                return texel * _Color;
            }
            ENDHLSL
        }
    }
}
