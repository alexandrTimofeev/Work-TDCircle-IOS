Shader "Custom/InvisibleWhenLit_ColoredShadows"
{
    Properties
    {
        [HDR] _ShadowTint ("Доп. оттенок тени (RGBA)", Color) = (1,1,1,0.8)
        _Softness ("Мягкость перехода", Range(0, 1)) = 0.2
        _Threshold ("Порог отсечения", Range(0, 0.5)) = 0.01
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

            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
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

            half4 _ShadowTint;
            half  _Softness;
            half  _Threshold;

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

                // --- Аккумуляторы цвета и веса ---
                half3 accumulatedLightColor = 0;  // Суммарный цвет света
                half totalWeight = 0;              // Суммарный вес для нормализации

                // --- Главный направленный свет ---
                Light mainLight = GetMainLight(TransformWorldToShadowCoord(positionWS));
                half NdotL_main = saturate(dot(normalWS, mainLight.direction));
                half mainAtten = mainLight.shadowAttenuation * NdotL_main;
                
                // Добавляем цвет главного света, умноженный на его вклад
                accumulatedLightColor += mainLight.color * mainAtten;
                totalWeight += mainAtten;

                // --- Дополнительные источники ---
                #ifdef _ADDITIONAL_LIGHTS
                    uint lightsCount = GetAdditionalLightsCount();
                    for (uint i = 0; i < lightsCount; ++i)
                    {
                        Light light = GetAdditionalLight(i, positionWS);
                        half NdotL = saturate(dot(normalWS, light.direction));
                        half attenuation = light.distanceAttenuation * light.shadowAttenuation;
                        half contribution = attenuation * NdotL;
                        
                        // Добавляем цвет дополнительного света, умноженный на его вклад
                        accumulatedLightColor += light.color * contribution;
                        totalWeight += contribution;
                    }
                #endif

                // --- Определяем видимость и цвет ---
                half alpha = 0;
                half3 finalColor = 0;

                if (totalWeight > _Threshold)
                {
                    // Точка освещена - полностью прозрачна
                    alpha = 0;
                    finalColor = 0;
                }
                else
                {
                    // Точка в тени - видима с цветом от источников
                    // Нормализуем цвет, чтобы избежать пересветов при нескольких источниках
                    half3 shadowColor = totalWeight > 0 ? accumulatedLightColor / totalWeight : 0;
                    
                    // Применяем оттенок (позволяет тонировать тени)
                    shadowColor *= _ShadowTint.rgb;
                    
                    // Коэффициент видимости с мягкостью
                    half shadowFactor = 1.0 - saturate(totalWeight / _Softness);
                    
                    alpha = _ShadowTint.a * shadowFactor;
                    finalColor = shadowColor;
                }

                return half4(finalColor, alpha);
            }
            ENDHLSL
        }

        // --- Проход ShadowCaster (без изменений) ---
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