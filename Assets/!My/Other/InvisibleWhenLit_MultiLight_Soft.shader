Shader "Custom/InvisibleWhenLit_MultiLight_Soft"
{
    Properties
    {
        [HDR] _ShadowColor ("Цвет тени (RGBA)", Color) = (0,0,0,0.8)
        _Softness ("Мягкость перехода", Range(0, 1)) = 0.2
    }
    SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent" "RenderPipeline" = "UniversalPipeline" }
        LOD 100

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForwardOnly" }

            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite On
            Cull Back

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            // Тени главного света
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE
            // Дополнительные источники и их тени
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
            // Мягкие тени
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
            half  _Softness;

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

                float3 positionWS = input.positionWS;
                half3 normalWS = normalize(input.normalWS);

                // --- Вклад главного направленного света ---
                Light mainLight = GetMainLight(TransformWorldToShadowCoord(positionWS));
                half mainContribution = mainLight.shadowAttenuation * saturate(dot(normalWS, mainLight.direction));

                // --- Вклад дополнительных источников ---
                half additionalContribution = 0;
                #ifdef _ADDITIONAL_LIGHTS
                    uint lightsCount = GetAdditionalLightsCount();
                    for (uint i = 0; i < lightsCount; ++i)
                    {
                        Light light = GetAdditionalLight(i, positionWS);
                        // Суммарное затухание: distanceAttenuation * shadowAttenuation
                        half attenuation = light.distanceAttenuation * light.shadowAttenuation;
                        half contribution = attenuation * saturate(dot(normalWS, light.direction));
                        additionalContribution += contribution;
                    }
                #endif

                // Общая освещённость точки
                half totalLight = mainContribution + additionalContribution;

                // Коэффициент видимости тени: 1 - освещённость
                half shadowFactor = 1.0 - totalLight;
                shadowFactor = saturate(shadowFactor);

                // Применяем мягкость перехода
                if (_Softness > 0.0)
                {
                    // Плавный переход: если totalLight от 0 до _Softness, то shadowFactor плавно убывает от 1 до 0
                    shadowFactor = 1.0 - smoothstep(0.0, _Softness, totalLight);
                }
                else
                {
                    // Резкий порог: если totalLight > 0.01, то невидимо
                    shadowFactor = totalLight > 0.01 ? 0.0 : 1.0;
                }

                half alpha = _ShadowColor.a * shadowFactor;
                half3 color = _ShadowColor.rgb;

                return half4(color, alpha);
            }
            ENDHLSL
        }

        // --- Проход ShadowCaster (для отбрасывания теней) ---
        Pass
        {
            Name "ShadowCaster"
            Tags { "LightMode" = "ShadowCaster" }

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
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                return 0;
            }
            ENDHLSL
        }
    }
    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}