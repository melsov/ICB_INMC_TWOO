Shader "Custom/BlockTilingAtlas" {
	Properties {
		_BlockTex ("Base RGB!", 2D) = "white" {}
		_GameClock ("Clock", Range(0, 1.0)) = 1.0
		_PlayerLoc ("PlayerLocation", Vector) = (0,0,0)
		
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
// Upgrade NOTE: excluded shader from DX11 and Xbox360; has structs without semantics (struct v2f members overhangLightLevel)
#pragma exclude_renderers d3d11 xbox360
        
        #pragma vertex vert
        #pragma fragment frag

        // /Applications/Unity/Unity.app/Contents/CGIncludes/UnityCG.cginc
        #include "UnityCG.cginc" 
        
//        #define TEST_COLOR  
//		#define LIGHT_BY_RANGE
        
        #define TILES_PER_DIM 4.0
        #define TOTAL_TILES TILES_PER_DIM * TILES_PER_DIM
        #define NORMAL_THRESHOLD 0.2
        #define SHADE_LEVEL 0.5
        #define NIGHT_LENGTH 0.34
        #define DAY_LENGTH 0.66
        #define FACE_SET_MAX_LENGTH 4.0
        #define HALF_FACE_SET_MAX_LENGTH 2.0
        #define BITS_PER_LIGHT_LEVEL 2.0
        
        #define NUM_LIGHT_LEVELS 8.0
        #define NUM_LIGHT_LEVELS_PLUS_ONE 9.0
        
        #define LIGHT_LEVEL_VERTEX_MULTIPLIER 1.0
        
        #define CORNER_LIGHT_MULTIPLIER (255.0/16.0)
        
        uniform sampler2D _BlockTex;
        uniform fixed _GameClock;
        uniform float3 _PlayerLoc;

        struct appdata {
            float4 vertex : POSITION;
            float3 normal : NORMAL;
            half2 texcoord : TEXCOORD0;
			float4 color32 : COLOR;
			float4 tangent : TANGENT;
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

			half light_level = .5;
			fixed day_level = (_GameClock - NIGHT_LENGTH) / DAY_LENGTH;
//			fixed day_level = (_SinTime[0] - NIGHT_LENGTH) / DAY_LENGTH; //nice to use but more difficult to sync with world events ?
			// GAME CLOCK could just set a constant days_per_tick (or delta tick?) then use the _Time[0] thing (sin of)
			// thus avoid duplications of a constant update.
			fixed neg_shadow_nudge = day_level * SHADE_LEVEL;
			fixed pos_shadow_nudge = (1.0 - day_level) * SHADE_LEVEL;
			
			if (day_level < 0.0) 
			{
				light_level += day_level/.3;
			}
			
			if (v.normal.z > NORMAL_THRESHOLD)
			{
				vy = v.vertex.y + .5;
				light_level += pos_shadow_nudge;				
			}
			else if ( v.normal.z < -NORMAL_THRESHOLD)
			{
				vy = (v.vertex.y + .5);
				light_level += neg_shadow_nudge;
			}
			else if (v.normal.x > NORMAL_THRESHOLD) 
			{
				vx  = (v.vertex.z + .5); // TEST NO FLIP TO NEG!!
				vy = (v.vertex.y + .5);
				light_level += pos_shadow_nudge;

			}
			else if ( v.normal.x < -NORMAL_THRESHOLD)
			{
				vx = (v.vertex.z + .5);
				vy = (v.vertex.y + .5);
				light_level += neg_shadow_nudge;
			}
//			else if (v.normal.y < -NORMAL_THRESHOLD)
//			{
//				vx *= -1; 
//			} 
			else // ypositive
			{
				light_level = .75 + .25 * day_level;
				if (day_level < 0.0)
					light_level += .45 * day_level;
			} 
			
//			// player flashlight
//			float4 worldpos = floor( mul(_Object2World, v.vertex) / 2.0 ) * 2.0;
//			float dist = distance(_PlayerLoc.xz, worldpos.xz);
//			if (dist < 15.0)
//			{
//				light_level += 1.5 * (1.0/dist);
//			}

			o.color.z = light_level;
			o.uv.xy =  half2(vx, vy);

			return o;
            
        }
        
        fixed4 frag(v2f i) : COLOR 
        { 
        	return tex2D(_BlockTex, i.uv) * i.color.z;
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
