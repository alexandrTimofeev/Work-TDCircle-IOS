Shader "Custom/InvisibleWhenLit_FinalFixed"
{
    Properties
    {
        [HDR] _ShadowTint ("Оттенок тени (RGB)", Color) = (1,1,1,1)
        _Softness ("Мягкость: свет→тень", Range(0,5)) = 1.0
        _ColorSoftness ("Мягкость: чёрный→цвет", Range(0,5)) = 0.5
        _WhiteThreshold ("Порог белого (разность компонент)", Range(0,1)) = 0.05
        [Toggle] _ForceMainLightWhite ("Считать главный свет белым", Float) = 1
        [Toggle] _DebugMode ("Отладка", Float) = 0
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
            half _Softness;
            half _ColorSoftness;
            half _WhiteThreshold;
            float _ForceMainLightWhite;
            float _DebugMode;

            Varyings vert(Attributes input)
            {
                Varyings output;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                output.positionCS = vertexInput.positionCS;
                output.positionWS = vertexInput.positionWS;
                output.normalWS = TransformObjectToWorldNormal(input.normalOS);
                return output;
            }

            // Функция определения белого света (ахроматический)
            bool IsWhite(half3 color, bool isMainLight)
            {
                if (isMainLight && _ForceMainLightWhite > 0.5)
                    return true;

                // Находим максимальную и минимальную компоненты
                half maxComp = max(max(color.r, color.g), color.b);
                half minComp = min(min(color.r, color.g), color.b);
                // Если разница меньше порога, считаем свет белым (серым/ахроматическим)
                return (maxComp - minComp) < _WhiteThreshold;
            }

            half4 frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                float3 positionWS = input.positionWS;
                half3 normalWS = normalize(input.normalWS);

                half totalIntensity = 0;        // сумма вкладов всех источников (для прозрачности)
                half3 colorAccum = 0;            // сумма цветов цветных источников, умноженных на вклад
                half colorWeight = 0;             // сумма вкладов цветных источников (вес)

                // --- Главный свет ---
                Light mainLight = GetMainLight(TransformWorldToShadowCoord(positionWS));
                half NdotL_main = saturate(dot(normalWS, mainLight.direction));
                half mainContrib = mainLight.shadowAttenuation * NdotL_main;
                totalIntensity += mainContrib;

                if (!IsWhite(mainLight.color, true))
                {
                    colorAccum += mainLight.color * mainContrib;
                    colorWeight += mainContrib;
                }

                // --- Дополнительные источники ---
                #ifdef _ADDITIONAL_LIGHTS
                    uint lightCount = GetAdditionalLightsCount();
                    half4 shadowMask = half4(1,1,1,1);
                    for (uint i = 0; i < lightCount; ++i)
                    {
                        Light light = GetAdditionalLight(i, positionWS, shadowMask);
                        half NdotL = saturate(dot(normalWS, light.direction));
                        half contrib = light.distanceAttenuation * light.shadowAttenuation * NdotL;
                        totalIntensity += contrib;

                        if (!IsWhite(light.color, false))
                        {
                            colorAccum += light.color * contrib;
                            colorWeight += contrib;
                        }
                    }
                #endif

                // --- Отладка ---
                if (_DebugMode > 0.5)
                {
                    // Красный = есть цветной вклад (colorWeight > 0)
                    // Зелёный = есть общий вклад (totalIntensity > 0)
                    half red = colorWeight > 0 ? 1.0 : 0.0;
                    half green = totalIntensity > 0 ? 1.0 : 0.0;
                    return half4(red, green, 0, 1);
                }

                // --- Прозрачность (переход свет/тень) ---
                half shadowFactor = 1.0 - smoothstep(0, _Softness, totalIntensity);
                shadowFactor = saturate(shadowFactor);
                half alpha = _ShadowTint.a * shadowFactor;

                // --- Цвет (появление из чёрного) ---
                half3 finalColor = 0;
                if (colorWeight > 0)
                {
                    half3 mixedColor = colorAccum / colorWeight;
                    half appearance = smoothstep(0, _ColorSoftness, colorWeight);
                    appearance = saturate(appearance);
                    finalColor = mixedColor * appearance;
                }
                finalColor *= _ShadowTint.rgb;

                return half4(finalColor, alpha);
            }
            ENDHLSL
        }

        // --- ShadowCaster Pass ---
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