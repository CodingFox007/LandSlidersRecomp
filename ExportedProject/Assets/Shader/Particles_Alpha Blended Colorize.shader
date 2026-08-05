Shader "Particles/Alpha Blended Colorize" {
	Properties {
		_TintColor ("Tint Color", Vector) = (0.5,0.5,0.5,0.5)
		_MainTex ("Particle Texture", 2D) = "white" {}
		_InvFade ("Soft Particles Factor", Range(0.01, 3)) = 1
		_Colorize ("Colorize", Vector) = (1,1,1,1)
	}
	SubShader{
		Tags
		{
			"Queue"="Transparent"
			"RenderType"="Transparent"
		}

		Blend SrcAlpha OneMinusSrcAlpha
		ZWrite Off
		Cull Off
		LOD 200

		Pass
		{
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			float4x4 unity_ObjectToWorld;
			float4x4 unity_MatrixVP;
			float4 _MainTex_ST;

			struct Vertex_Stage_Input
			{
				float4 pos : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct Vertex_Stage_Output
			{
				float2 uv : TEXCOORD0;
				float4 pos : SV_POSITION;
			};

			Vertex_Stage_Output vert(Vertex_Stage_Input input)
			{
				Vertex_Stage_Output output;
				output.uv = (input.uv.xy * _MainTex_ST.xy) + _MainTex_ST.zw;
				output.pos = mul(unity_MatrixVP, mul(unity_ObjectToWorld, input.pos));
				return output;
			}

			sampler2D _MainTex;

			float4 _TintColor;
			float4 _Colorize;

			struct Fragment_Stage_Input
			{
				float2 uv : TEXCOORD0;
			};

			float4 frag(Fragment_Stage_Input input) : SV_TARGET
			{
				float4 tex = tex2D(_MainTex, input.uv);

				tex *= _TintColor;
				tex.rgb *= _Colorize.rgb;

				return tex;
			}

			ENDHLSL
		}
	}
}