Shader "Custom/test" {
	Properties {
		_MainTex( "Albedo (RGB)", 2D ) = "white" { }
	}
	SubShader {
	    Pass {
			Blend SrcAlpha OneMinusSrcAlpha
			Cull Back
			Tags { "RenderType"="Opaque" }
			LOD 200
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			// Use shader model 3.0 target, to get nicer looking lighting
			#pragma target 3.0

			#include "UnityCG.cginc"
			sampler2D _MainTex;

			struct INPUT {
				float4 pos : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			v2f vert( INPUT input ) {
				v2f output;

				output.uv = input.uv;
				input.pos = mul( UNITY_MATRIX_MVP, input.pos );
				output.pos = input.pos;

				return output;
			}

			float4 frag ( v2f input ) : SV_Target {
				// 色を出力
				float4 color = float4( 1, 1, 1, 0 );

				color = tex2D ( _MainTex, input.uv );

				return color;
			}
			ENDCG
		}
	}
	FallBack "Diffuse"
}
