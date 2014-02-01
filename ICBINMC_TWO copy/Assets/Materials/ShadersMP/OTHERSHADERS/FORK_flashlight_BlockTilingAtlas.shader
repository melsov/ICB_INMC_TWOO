Shader "Custom/FLASHLIGHT_FORK_BlockTilingAtlas" {
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
        
        #pragma vertex vert
        #pragma fragment frag

        // /Applications/Unity/Unity.app/Contents/CGIncludes/UnityCG.cginc
        #include "UnityCG.cginc"
        
        #define TILES_PER_DIM 4.0
        #define TOTAL_TILES TILES_PER_DIM * TILES_PER_DIM
        
        #define NORMAL_THRESHOLD 0.2
        #define SHADE_LEVEL 0.5
        #define NIGHT_LENGTH 0.34
        #define DAY_LENGTH 0.66
        
        uniform sampler2D _BlockTex;
        uniform fixed _GameClock;
        uniform float3 _PlayerLoc;

        struct appdata {
            float4 vertex : POSITION;
            float3 normal : NORMAL;
//            float4 color : COLOR;
            half2 texcoord : TEXCOORD0;
        };

        struct v2f {
            half4 pos : SV_POSITION;
            float4 color : COLOR;
            half4 uv : TEXCOORD0;
        };
        
        v2f vert(appdata v) 
        {
            v2f o;
            o.pos = mul(UNITY_MATRIX_MVP, v.vertex);

			float vx = v.vertex.x + .5;
			float vy = v.vertex.z + .5;
			//wait just use floor?
			float4 worldpos = floor( mul(_Object2World, v.vertex));
//			sneak the world pos into the "uv" pos
//			 NOTE: theoretically using half type for uv limits world lighting system to (generally 16 bits) 60,000 +/- position in any direction! (if worlds get bigger than that switch v2f.uv to float)
			half worldx = worldpos.x;
			half worldy = worldpos.z;
			
			half pposx = _PlayerLoc.x;
			half pposy = _PlayerLoc.z; 
			
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
				worldy = worldpos.y;
				pposy = _PlayerLoc.y;
			}
			else if ( v.normal.z < -NORMAL_THRESHOLD)
			{
				vy = -(v.vertex.y + .5);
				vx *= -1;
				light_level += neg_shadow_nudge;
				worldy = worldpos.y;
				pposy = _PlayerLoc.y;
			}
			else if (v.normal.x > NORMAL_THRESHOLD) 
			{
				vx = -(v.vertex.y + .5);
				vy *= -1;
				light_level += pos_shadow_nudge;
				worldx = worldpos.y;
				pposx = _PlayerLoc.y;
				
			}
			else if ( v.normal.x < -NORMAL_THRESHOLD)
			{
				vx = (v.vertex.y + .5);
				vy *= -1;
				light_level += neg_shadow_nudge;
				worldx = worldpos.y;
				pposx = _PlayerLoc.y;
			}
			else if (v.normal.y < -NORMAL_THRESHOLD)
			{
				vx *= -1;
				
			} 
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
			
			
			// maybe each block can hvae light level data plus light direction data: directions are pizza directions?
			
			// GET TILE OFFSET
			float index = v.texcoord.x * (TOTAL_TILES);
			float blocky = floor(index/TILES_PER_DIM);
			half2 tile_o = half2(index - blocky * TILES_PER_DIM, blocky) * 0.25;

			// store in color for now
			o.color.xy = tile_o;
			// END OF GET TILE OFFSET
			
			o.color.zw = float2(pposx, pposy); //_PlayerLoc.xz;  // light_level;
			
			o.uv.xy =  half2(vx, vy);
			
			
			o.uv.zw = half2(worldx, worldy);
			
			return o;
            
        }
        
        fixed4 frag(v2f i) : COLOR 
        {
        	

			half2 fluv = frac(i.uv.xy);
			fluv = fluv * 0.25;
			half2 offset =  i.color.xy;
			fluv = fluv + offset;
			
			//light level staircase
			float dist = ceil( distance(i.color.zw, ceil( i.uv.zw)));
			
			half light_level = 1.0 * 1.0/dist;  // floor( i.color.z * 10.0) / 10.0; // don't do this here? (it gets weird)
			
			light_level = min(light_level, 1.0);
//			// player flashlight
//			float4 worldpos = mul(_Object2World, i.pos );
//			float dist = distance(_PlayerLoc.xz, worldpos.xz);
//			if (dist < 15.0)
//			{
//				light_level += 4.5 * (1.0/dist);
//			}
			
			
	        return tex2D(_BlockTex, fluv) * light_level; // color z == light level

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
