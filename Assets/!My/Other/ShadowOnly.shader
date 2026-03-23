Shader "Custom/ShadowOnly"
{
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }
        ColorMask 0
        Pass
        {
            Name "ShadowCaster"
            Tags { "LightMode"="ShadowCaster" }
        }
    }
}