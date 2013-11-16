Shader "Custom/BlockShader" {
	Properties {
		_BlockTex ("Base RGB!", 2D) = "white" {}
	}

	SubShader {
    
    Pass 
    {
        LOD 100
        CGPROGRAM
        
        #pragma vertex vert
        #pragma fragment frag
        
        #include "UnityCG.cginc"
        
        uniform sampler2D _BlockTex;

        
        struct appdata {
//            float4 vertex : POSITION;
            half4 vertex : POSITION;
//            float3 normal : NORMAL;
            half2 texcoord : TEXCOORD0;
//            fixed2 texcoord : TEXCOORD0;
        };

        struct v2f {
//            float4 pos : SV_POSITION;
            half4 pos : SV_POSITION;
//            float4 color : COLOR;
            half2 uv : TEXCOORD0;
//            half2 uv : TEXCOORD0;
        };
        
        
        
        v2f vert(appdata v) {
            v2f o;
            o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
//            o.color.xyz = v.normal * 0.5 + 0.5;
//            o.color.w = 1.0;

            o.uv = MultiplyUV( UNITY_MATRIX_TEXTURE0, v.texcoord ); // v.texcoord.xy; //float4(v.texcoord.xy, 0,0);

            return o;
        }
        
        fixed4 frag(v2f i) : COLOR {
            return  tex2D(_BlockTex, i.uv);
        }

        ENDCG
    }
    

	}
	FallBack "Diffuse"
}


//		Tags { "RenderType"="Opaque" }
//		LOD 200
//		
//		CGPROGRAM
//		#pragma surface surf Lambert
//
//		sampler2D _MainTex;
//
//		struct Input {
//			float2 uv_MainTex;
//		};
//
//		void surf (Input IN, inout SurfaceOutput o) {
//			half4 c = tex2D (_MainTex, IN.uv_MainTex);
//			o.Albedo = c.rgb;
//			o.Alpha = c.a;
//		}
//		ENDCG
//
//
//	}
//	FallBack "Diffuse"
//}

//	SubShader {
//		Pass {
//            
//            Fog { Mode Off } // don't know if this helps
//
//			CGPROGRAM
//			#pragma vertex vert_img
//			#pragma fragment frag
//
//			#include "UnityCG.cginc"
//
//			uniform sampler2D _MainTex;
//
//			fixed4 frag(v2f_img i) : COLOR {
//				return tex2D(_MainTex, i.uv);
//			}
//			ENDCG
//		}
//	}
//}

        
//        fixed4 frag(v2f i) : COLOR {
//            fixed4 retColor;
//            retColor = tex2D(_BlockTex, i.uv);
//            retColor.xyz = -1. * fixed3( i.color.x) * .2 + tex2D(_BlockTex, i.uv).xyz;
//            // + i.color.xyz;
//            return  tex2D(_BlockTex, i.uv);
//        }
