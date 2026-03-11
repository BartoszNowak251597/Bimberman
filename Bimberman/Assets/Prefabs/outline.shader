Shader "Custom/Outline"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (0.2, 0.2, 0.2, 0.0)
        _EdgeColor ("Edge Color", Color) = (1.0, 1.0, 0.0, 1.0)
        _EdgePower ("Edge Power", Range(0.5, 8.0)) = 3.0
        _EdgeIntensity ("Edge Intensity", Range(0.0, 10.0)) = 2.0
        _Alpha ("Alpha", Range(0.0, 1.0)) = 1.0
    }

    SubShader
    {
        Tags
        {
            "RenderType"="Transparent"
            "Queue"="Transparent"
            "RenderPipeline"="UniversalPipeline"
        }

        Pass
        {
            Name "Forward"
            Tags { "LightMode"="UniversalForward" }

            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Back

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor;
                float4 _EdgeColor;
                float _EdgePower;
                float _EdgeIntensity;
                float _Alpha;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 normalWS    : TEXCOORD0;
                float3 viewDirWS   : TEXCOORD1;
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;

                VertexPositionInputs posInputs = GetVertexPositionInputs(IN.positionOS.xyz);
                VertexNormalInputs normalInputs = GetVertexNormalInputs(IN.normalOS);

                OUT.positionHCS = posInputs.positionCS;
                OUT.normalWS = normalize(normalInputs.normalWS);
                OUT.viewDirWS = normalize(GetCameraPositionWS() - posInputs.positionWS);

                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float ndotv = saturate(dot(normalize(IN.normalWS), normalize(IN.viewDirWS)));
                float fresnel = pow(1.0 - ndotv, _EdgePower);

                float3 finalColor = _BaseColor.rgb + (_EdgeColor.rgb * fresnel * _EdgeIntensity);
                float finalAlpha = saturate((_BaseColor.a + fresnel) * _Alpha);

                return half4(finalColor, finalAlpha);
            }
            ENDHLSL
        }
    }
}