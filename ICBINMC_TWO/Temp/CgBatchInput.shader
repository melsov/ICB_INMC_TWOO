Shader "Custom/BlockTilingAtlas" {
	Properties {
		_BlockTex ("Base RGB!", 2D) = "white" {}
//		_GameClock ("Clock", Range(0, 1.0)) = 1.0
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
        
        #define TILES_PER_DIM 4.0
        #define TOTAL_TILES TILES_PER_DIM * TILES_PER_DIM
        
        #define NORMAL_THRESHOLD 0.2
        
        uniform sampler2D _BlockTex;
//        uniform fixed _GameClock;

        struct appdata {
            float4 vertex : POSITION;
            float3 normal : NORMAL;
//            float4 color : COLOR;
            half2 texcoord : TEXCOORD0;
        };

        struct v2f {
            half4 pos : SV_POSITION;
            float4 color : COLOR;
            half2 uv : TEXCOORD0;
        };
        

        
        v2f vert(appdata v) 
        {
            v2f o;
            o.pos = mul(UNITY_MATRIX_MVP, v.vertex);

			float vx = v.vertex.x + .5;
			float vy = v.vertex.z + .5;
			
			
			if (v.normal.z > NORMAL_THRESHOLD)
			{
				vy = v.vertex.y + .5;				
			}
			else if ( v.normal.z < -NORMAL_THRESHOLD)
			{
				vy = -(v.vertex.y + .5);
				vx *= -1;
			}
			else if (v.normal.x > NORMAL_THRESHOLD) 
			{
				vx = -(v.vertex.y + .5);
				vy *= -1;
			}
			else if ( v.normal.x < -NORMAL_THRESHOLD)
			{
				vx = (v.vertex.y + .5);
				vy *= -1;
			}
			else if (v.normal.y < -NORMAL_THRESHOLD)
			{
				vx *= -1;
			}
			
			// GET TILE OFFSET
			float index = v.texcoord.x * (TOTAL_TILES);
			float blocky = floor(index/TILES_PER_DIM);
			half2 tile_o = half2(index - blocky * TILES_PER_DIM, blocky) * 0.25;

			// store in color for now
			o.color.xy = tile_o;
			// END OF GET TILE OFFSET
			
			o.uv =  half2(vx, vy);
			return o;
            
        }
        
        fixed4 frag(v2f i) : COLOR 
        {
        	

			half2 fluv = frac(i.uv);
			fluv = fluv * 0.25;
			half2 offset =  i.color.xy;
			fluv = fluv + offset;
	        return tex2D(_BlockTex, fluv);

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
