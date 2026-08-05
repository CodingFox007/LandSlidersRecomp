Shader "Turnyworld/Diffuse+Rimlight+Transparency"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _RimColor ("Rim Color", Color) = (1,1,1,1)
        _RimPower ("Rim Power", Range(0.5,8)) = 3
        _AlbedoIntense ("Texture Strength", Range(0,1)) = 1
        _Transparency ("Opacity", Range(0,1)) = 0.8
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }

        LOD 200
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        CGPROGRAM
        #pragma surface surf Lambert alpha:fade

        sampler2D _MainTex;

        fixed4 _RimColor;
        float _RimPower;
        float _AlbedoIntense;
        float _Transparency;

        struct Input
        {
            float2 uv_MainTex;
            float3 viewDir;
        };

        void surf(Input IN, inout SurfaceOutput o)
        {
            fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);

            o.Albedo = tex.rgb * _AlbedoIntense;

            float rim = 1.0 - saturate(
                dot(normalize(IN.viewDir), o.Normal)
            );

            rim = pow(rim, _RimPower);

            o.Emission = _RimColor.rgb * rim * 0.35;

            o.Alpha = tex.a * _Transparency;
        }
        ENDCG
    }

    Fallback "Transparent/Diffuse"
}