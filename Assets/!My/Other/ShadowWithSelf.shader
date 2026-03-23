Shader "Custom/InvisibleWhenLit_SoftShadow"
{
    Properties
    {
        [HDR] _ShadowColor ("Цвет тени (с альфой)", Color) = (0,0,0,0.8)
        _ShadowSoftness ("Мягкость перехода", Range(0, 1)) = 0.2
    }
    SubShader
    {
        Tags{ "RenderType" = "Transparent" "Queue" = "Transparent" "RenderPipeline" = "UniversalPipeline" }
        LOD 100

        // --- Основной проход ---
        Pass
        {
            Name "ForwardUnlit"
            Tags{ "LightMode" = "UniversalForwardOnly" }

            Blend SrcAlpha OneMinusSrcAlpha   // Обычный альфа-блендинг
            ZWrite On
            Cull Back

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _SHADOWS_SOFT

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
                float3 normalWS   : TEXCOORD1;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            half4 _ShadowColor;
            half  _ShadowSoftness;

            Varyings vert(Attributes input)
            {
                Varyings output = (Varyings)0;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                output.positionCS = vertexInput.positionCS;
                output.positionWS = vertexInput.positionWS;
                output.normalWS = TransformObjectToWorldNormal(input.normalOS);

                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                // Теневые координаты для главного света
                float4 shadowCoord = TransformWorldToShadowCoord(input.positionWS);
                Light mainLight = GetMainLight(shadowCoord);

                // Нормаль
                half3 normalWS = normalize(input.normalWS);

                // Коэффициент освещённости от главного света:
                // lit = затенённость (attenuation) * косинус угла (NdotL)
                half NdotL = saturate(dot(normalWS, mainLight.direction));
                half lit = mainLight.shadowAttenuation * NdotL; // 0 = полная тень, 1 = полный свет

                // Коэффициент видимости тени: чем меньше lit, тем больше видимость
                half shadowFactor = 1.0 - lit;

                // Мягкий переход на границе свет/тень
                if (_ShadowSoftness > 0.0)
                {
                    // Используем smoothstep для плавного перехода в диапазоне [0, _ShadowSoftness]
                    shadowFactor = smoothstep(0.0, _ShadowSoftness, shadowFactor);
                }
                else
                {
                    // Резкий переход: если shadowFactor < 0.5? Нет, проще:
                    shadowFactor = shadowFactor > 0.01 ? 1.0 : 0.0;
                }

                // Итоговая альфа = альфа цвета тени * коэффициент видимости
                half alpha = _ShadowColor.a * shadowFactor;

                // Цвет тени (можно модулировать с чем-то ещё, если нужно)
                half3 color = _ShadowColor.rgb;

                return half4(color, alpha);
            }
            ENDHLSL
        }

        // --- Проход ShadowCaster (обязателен для отбрасывания тени) ---
        Pass
        {
            Name "ShadowCaster"
            Tags{ "LightMode" = "ShadowCaster" }

            ZWrite On
            ZTest LEqual
            Cull Back

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
            };

            Varyings vert(Attributes input)
            {
                Varyings output;
                UNITY_SETUP_INSTANCE_ID(input);
                // Для теней достаточно просто позиции
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                return 0; // цвет не важен
            }
            ENDHLSL
        }
    }
    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}