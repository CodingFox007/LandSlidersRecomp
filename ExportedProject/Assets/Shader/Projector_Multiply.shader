Shader "Projector/Multiply"
{
    Properties
    {
        _ShadowTex ("Cookie", 2D) = "gray" {}
        _FalloffTex ("FallOff", 2D) = "white" {}
    }

    SubShader
    {
        Tags { "Queue"="Transparent" }

        Pass
        {
            ZWrite Off
            ColorMask RGB
            Blend DstColor Zero
            Offset -1, -1

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct v2f
            {
                float4 pos : SV_POSITION;
                float4 uvShadow : TEXCOORD0;
                float4 uvFalloff : TEXCOORD1;
            };

            float4x4 unity_Projector;
            float4x4 unity_ProjectorClip;

            sampler2D _ShadowTex;
            sampler2D _FalloffTex;

            v2f vert(float4 vertex : POSITION)
            {
                v2f o;

                o.pos = UnityObjectToClipPos(vertex);
                o.uvShadow = mul(unity_Projector, vertex);
                o.uvFalloff = mul(unity_ProjectorClip, vertex);

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 shadow =
                    tex2Dproj(_ShadowTex, UNITY_PROJ_COORD(i.uvShadow));

                fixed4 falloff =
                    tex2Dproj(_FalloffTex, UNITY_PROJ_COORD(i.uvFalloff));

                shadow.a = 1.0 - shadow.a;

                return lerp(
                    fixed4(1,1,1,1),
                    shadow,
                    falloff.a
                );
            }
            ENDCG
        }
    }
}