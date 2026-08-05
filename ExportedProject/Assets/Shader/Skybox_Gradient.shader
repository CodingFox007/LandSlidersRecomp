Shader "Skybox/Gradient"
{
    Properties
    {
        _Tint ("Tint Color", Color) = (1,1,1,1)
        _Tex ("Gradient (RGB)", 2D) = "white" {}
		_FogTexture ("Fog Texture", 2D) = "white" {}
		_FogColor ("Fog Color", Color) = (1,1,1,1)
		_FogMinHeight ("Fog Min Height", Float) = -80
		_FogMaxHeight ("Fog Max Height", Float) = -5
		_FogInversion ("Fog Inversion", Float) = 0
		_FogRotation ("Fog Rotation", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue"="Background"
            "RenderType"="Background"
            "PreviewType"="Skybox"
        }

        Cull Off
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

			sampler2D _Tex;
			fixed4 _Tint;
            sampler2D _FogTexture;
			fixed4 _FogColor;
			float _FogMinHeight;
			float _FogMaxHeight;
			float _FogInversion;
			float _FogRotation;
			float4x4 _Rotation;

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD0;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldPos = v.vertex.xyz;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
			{
				
				float3 dir = normalize(mul((float3x3)_Rotation, i.worldPos));
				float y = dir.y;
				y = y * 0.5 + 0.5;

				fixed4 col = tex2D(_Tex, float2(0.5, y));

				// Fog height calculation

				float fogAmount = saturate(
					(i.worldPos.y - _FogMinHeight) /
					(_FogMaxHeight - _FogMinHeight)
				);

				if (_FogInversion > 0.5)
				{
					fogAmount = 1 - fogAmount;
				}

				// Rotate/scroll fog texture
				float2 fogUV = i.worldPos.xz * 0.01;
				fogUV += _FogRotation;

				fixed4 fog = tex2D(_FogTexture, fogUV);

				// Blend fog into sky
				col.rgb = lerp(
					col.rgb,
					fog.rgb * _FogColor.rgb,
					fog.a * fogAmount
				);

				return col * _FogColor;
			}

            ENDCG
        }
    }
}