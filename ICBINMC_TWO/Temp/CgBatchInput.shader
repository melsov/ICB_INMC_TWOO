Shader "Custom/CursorBlockShader" {
	Properties {
		_BlockTex ("Base RGB!", 2D) = "greeny" {}
		_BaseColor ("Base Color", Color) = (1.0,0.19,0.16,0.0)
//		_GameClock ("Clock", Range(0, 1.0)) = 1.0
//		_PlayerLoc ("PlayerLocation", Vector) = (0,0,0)
		
	}
//    SubShader {
//    Tags {"Queue" = "Background" }
//    Pass
//    {
//      TODO: research drawing to background--have to make a sep material/shader for this.
//    }
//
//    }

	SubShader {
    Tags {"Queue" = "Geometry" }
    Pass 
    {
        LOD 100
        CGPROGRAM
        
        #pragma vertex vert
        #pragma fragment frag

        // /Applications/Unity/Unity.app/Contents/CGIncludes/UnityCG.cginc
        #include "UnityCG.cginc" 
        
        uniform sampler2D _BlockTex;
        float4 _BaseColor;

        struct appdata {
            float4 vertex : POSITION;
//            float3 normal : NORMAL;
            half2 texcoord : TEXCOORD0;
//			float4 color32 : COLOR;
        };

        struct v2f {
            half4 pos : SV_POSITION;
//            float4 color : COLOR;
            half2 uv : TEXCOORD0;
        };
        
        v2f vert(appdata v) 
        {
            v2f o;
            o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
            o.uv = v.texcoord;
            return o;
        }
        
        fixed4 frag(v2f i) : COLOR 
        { 
//        	return tex2D(_BlockTex, i.uv);
        	fixed4 tex_color = tex2D(_BlockTex, i.uv);
        	return _BaseColor * tex_color.a;
//        	return fixed4(tex_color.r,tex_color.r,tex_color.r, 0.0 ); // if white, opaque
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
