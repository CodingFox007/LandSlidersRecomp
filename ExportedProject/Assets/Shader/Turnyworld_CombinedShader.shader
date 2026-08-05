Shader "Turnyworld/CombinedShader"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}

        _RimColor ("Rim Color", Color) = (1,1,1,1)
        _RimPower ("Rim Power", Range(0.1,8)) = 3

        _Cube ("Reflection Cubemap", Cube) = "" {}
        _ReflectionStrength ("Reflection Strength", Range(0,1)) = 0.1

        _MinLight ("Min Light", Range(0,1)) = 0.2
        _MaxLight ("Max Light", Range(0,1)) = 1.0

        _FogTexture ("Fog Texture", 2D) = "white" {}
        _FogColor ("Fog Color", Color) = (0.7, 0.88, 0.98, 1)
        _FogDensity ("Fog Max Density", Range(0, 1)) = 1.0
        _FogSharpness ("Fog Fade Sharpness", Range(1, 10)) = 8.0 // Higher = faster fade out
        _FogMinHeight ("Fog Start Height", Float) = -2
        _FogMaxHeight ("Fog Full Height (Abyss)", Float) = -60
        _FogInversion ("Fog Inversion", Float) = 0
        _FogRotation ("Fog Rotation", Float) = 0
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 300

        CGPROGRAM
        #pragma surface surf Lambert finalcolor:applyCustomFog
        #pragma multi_compile_fog

        #include "UnityCG.cginc"

        sampler2D _MainTex;
        samplerCUBE _Cube;
        sampler2D _FogTexture;

        fixed4 _RimColor;
        fixed4 _FogColor;

        float _RimPower;
        float _ReflectionStrength;
        float _MinLight;
        float _MaxLight;

        float _FogDensity;
        float _FogSharpness;
        float _FogMinHeight;
        float _FogMaxHeight;
        float _FogInversion;
        float _FogRotation;

        struct Input
        {
            float2 uv_MainTex;
            float3 worldRefl;
            float3 viewDir;
            float3 worldPos;
        };

        void surf (Input IN, inout SurfaceOutput o)
        {
            // 1. Base Texture & Lighting
            fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
            o.Albedo = tex.rgb * _MaxLight;

            // 2. Rim Light & Reflections
            float rim = 1.0 - saturate(dot(normalize(IN.viewDir), o.Normal));
            rim = pow(rim, _RimPower);

            fixed4 refl = texCUBE(_Cube, IN.worldRefl);
            o.Emission = (_RimColor.rgb * rim) + (refl.rgb * _ReflectionStrength);
        }

        void applyCustomFog(Input IN, SurfaceOutput o, inout fixed4 color)
        {
            // 1. Height Linear Ratio (0.0 at top/start height, 1.0 down at full abyss depth)
            float heightRange = max(0.0001, abs(_FogMaxHeight - _FogMinHeight));
            float linearHeight = saturate((_FogMinHeight - IN.worldPos.y) / heightRange);

            if (_FogInversion > 0.5)
            {
                linearHeight = 1.0 - linearHeight;
            }

            // 2. Exponential Falloff (creates the sharp transition + thick bottom base)
            float heightFactor = pow(linearHeight, _FogSharpness);

            // 3. Texture UV offset and sampling
            float2 fogUV = IN.worldPos.xz * 0.01 + float2(_FogRotation, _FogRotation);
            fixed4 fogTex = tex2D(_FogTexture, fogUV);

            // Distance-based fog
			float cameraDistance = distance(_WorldSpaceCameraPos, IN.worldPos);

			// Tune these values to taste
			float distanceFogStart = 10.0;
			float distanceFogEnd = 60.0;

			// 0 = close to camera, 1 = far away
			float distanceFactor =
				saturate((cameraDistance - distanceFogStart) /
						 (distanceFogEnd - distanceFogStart));

			// Combine height fog and distance fog
			float totalFog =
				saturate(heightFactor + distanceFactor * 0.4);

			// Apply texture variation and density
			totalFog *= fogTex.a;
			totalFog *= _FogDensity;

            // 4. Interpolate final color
            fixed3 targetFogColor = fogTex.rgb * _FogColor.rgb;
            color.rgb = lerp(color.rgb, targetFogColor, totalFog);
        }
        ENDCG
    }

    FallBack "Diffuse"
}