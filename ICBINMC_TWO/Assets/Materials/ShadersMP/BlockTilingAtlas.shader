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

#ifdef LIGHT_BY_RANGE
			float overhangLightLevel;
#else
            float4 overhangLightLevel;
#endif
        };
        
                
        // GET A COMPONENT IN FLOAT4 BASED ON THE XCOORD % MAX_LENGTH
        // I.E. EACH COMPONENT APPLIES TO ONE OF THE FOUR ROWS
        inline float getIndex(float4 overhang, float xco) 
        {
//        	xco = fmod(xco, FACE_SET_MAX_LENGTH);  // assume already modded
        	if (xco < 1.0) { 
				return overhang.x;
			} 
			if (xco < 2.0) {
				return overhang.y;
			} 
			if (xco < 3.0) {
				return overhang.z;
			}
			return overhang.w;
        }
        
        v2f vert(appdata v) 
        {
            v2f o;
            o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
            
            //courtesy: https://bitbucket.org/volumesoffun/cubiquity-for-unity3d/src/91c73281414fd4bee8449154c56f3ca4f6cfd150/Assets/Cubiquity/Resources/ColoredCubesVolume.shader?at=master
            // Unity can't cope with the idea that we're peforming lighting without having per-vertex
      	// normals. We specify dummy ones here to avoid having to use up vertex buffer space for them.
//      	v.normal = float3 (0.0f, 0.0f, 1.0f);
//      	v.tangent = float4 (1.0f, 0.0f, 0.0f, 1.0f);   
            

			float vx = v.vertex.x + .5;
			float vy = v.vertex.z + .5;
			//wait just use floor?
//			float4 worldpos = floor( mul(_Object2World, v.vertex));
//			sneak the world pos into the "uv" pos
//			 NOTE: theoretically using half type for uv limits world lighting system to (generally 16 bits) 60,000 +/- position in any direction! (if worlds get bigger than that switch v2f.uv to float)
//			half worldx = worldpos.x;
//			half worldy = worldpos.z;
			
//			half pposx = _PlayerLoc.x;
//			half pposy = _PlayerLoc.z; 

			
			
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
//				worldy = worldpos.y;
//				pposy = _PlayerLoc.y;
			}
			else if ( v.normal.z < -NORMAL_THRESHOLD)
			{
				vy = (v.vertex.y + .5);
				
				//TEST NO FLIP TO NEG
//				vx *= -1;

				light_level += neg_shadow_nudge;
//				worldy = worldpos.y;
//				pposy = _PlayerLoc.y;
			}
			else if (v.normal.x > NORMAL_THRESHOLD) 
			{


//				vx  = -(v.vertex.z + .5);
				vx  = (v.vertex.z + .5); // TEST NO FLIP TO NEG!!
				vy = (v.vertex.y + .5);
				light_level += pos_shadow_nudge;
//				worldx = worldpos.y;
//				pposx = _PlayerLoc.y;
				
			}
			else if ( v.normal.x < -NORMAL_THRESHOLD)
			{
				vx = (v.vertex.z + .5);
				vy = (v.vertex.y + .5);
				light_level += neg_shadow_nudge;
//				worldx = worldpos.y;
//				pposx = _PlayerLoc.y;
			}
//			else if (v.normal.y < -NORMAL_THRESHOLD)
//			{
			// TEST. NO FLIP TO NEG
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
			
			
			// maybe each block can hvae light level data plus light direction data: directions are pizza directions?
			
			// GET TILE OFFSET

			float index = v.texcoord.x * (TOTAL_TILES);
			float blocky = floor(index/TILES_PER_DIM);
			half2 tile_o = half2(index - blocky * TILES_PER_DIM , blocky) * 0.25;
			o.color.xy = tile_o; // store in color for now
			
//			//			TEST //TEST
//			o.color.xy = half2(.01,.01);
			// END OF GET TILE OFFSET
			
//			o.color.zw = float2(pposx, pposy); //_PlayerLoc.xz;  // light_level;
			o.color.z = light_level;
//			o.color.w = half(local_light);

			
			o.uv.xy =  half2(vx, vy);
			
			//TESTTESTTEST
//			half2 fracxy = fmod(o.uv.xy, half2(4.0,4.0));
//			if (fracxy.x < .01 && fracxy.x > -.01)
//				o.uv.x *= .1;
//			o.uv.xy =  half2(vx, vy) * .2 + .3;// + half2(.03,.02);
			
			
			
			//LIGHT LEVEL INFLUENCED BY THE OVERHANGS / CAVES ETC.
#ifdef LIGHT_BY_RANGE
			o.overhangLightLevel = v.color32.r > 0.0 ? v.color32.r * CORNER_LIGHT_MULTIPLIER + 0.1 : 0.0;
#else			
//			o.overhangLightLevel = v.color32 * 255.0 + float4(.5); // float2(1.0 * 65535.0 + .5, v.texcoord1.y * 65535.0 + .5); // v.texcoord1.xy * 4.5 * 65535.0 + .5; // float2( 65535.5, 65535.5); //
// TEST TANGENT
			o.overhangLightLevel = v.tangent * LIGHT_LEVEL_VERTEX_MULTIPLIER + float4(.5); 
#endif
			//END CAVE/OVERHANG LIGHT
			
//			o.uv.zw = half2(worldx, worldy);

#ifdef TEST_COLOR
			float c_index = v.texcoord1.y; // v.texcoord.y * 16.0; // 8 == total colors
			fixed red = c_index; // > 0.5 ? 1.0 : 0.0; //  fmod(c_index, 2.0) > 0.0 ? 1.0 : 0.0; //fmod(1.5, 2.0) > 1.499 ? 1.0 : 0.0; //
			fixed green = fmod(floor(c_index / 2.0), 2.0) > 0.0 ? 1.0 : 0.0;
			fixed blue = fmod(floor(c_index / 4.0), 2.0) > 0.0 ? 1.0 : 0.0;
			fixed darken = fmod(floor(c_index/ 8.0), 2.0) > 0.0 ? .4 : 1.0;
			o.color.xyz = float3(red, red, red) ;
			
			o.color.xyz = float3(red, green, blue) * darken + v.normal * .1; //  fmod(float3(v.vertex * v.vertex.zxy), 7.0) / 3.0 ;	
//			o.color.xyz = v.color32.xyz;
//			o.color.w = 1.0;
#endif
			
			return o;
            
        }
        
        fixed4 frag(v2f i) : COLOR 
        { 
#ifdef TEST_COLOR
			fixed4 foolcompiler = tex2D(_BlockTex, i.uv);
        	return fixed4(i.color); // * tex2D(_BlockTex, i.uv);
#endif
			
//			half2 scaled_uv = frac(i.uv.xy) * .25;
			half2 chunkCoord = floor(i.uv.xy); 
			half2 scaled_uv = (i.uv.xy - chunkCoord) * .25; 
			half2 offset =  i.color.xy; // + half2(.03);
			
			//TEST
//			if ((scaled_uv.x < .01 ) || (scaled_uv.x > .24))
//				scaled_uv.x = offset.x + 0.125; 
//				return fixed4(0.0,1.0,0.0,1.0); 

//				scaled_uv.x = abs(scaled_uv.x) * .01;
			//ENDTEST
			
			scaled_uv = scaled_uv + offset; 
			
#ifdef LIGHT_BY_RANGE
//			return fixed4(.3,1.0, 0.0,1.0) * i.color.z;
//			return tex2D(_BlockTex, scaled_uv) * i.overhangLightLevel; // TEST
			return tex2D(_BlockTex, scaled_uv) * i.color.z * i.overhangLightLevel; // color z == (day) light level
#endif			

			//4294967296 = 2 ^ 32 and also 4 ^ 16
			//65536; // = 2 ^ 16
			// GENERAL NOTE: 4967295.0 % 2 > 0.
			// 4294967295.0 % 2 is not > 0!!

			// CONSIDER: (adopted) DO WE NEED ABS()?
			// IS THERE ANOTHER WAY TO GET THE TEXTURES ALIGNED WITHOUT
			// FLIPPING THE NORMALS IN THE VERT SHADER?
			
			half2 model_rel_twoD = fmod(chunkCoord, FACE_SET_MAX_LENGTH);
			float index = getIndex(i.overhangLightLevel, model_rel_twoD.x); // 0; // (fmod(floor(model_rel_twoD.x), FACE_SET_MAX_LENGTH) > 1.0) ? ll_two : local_light_index; 
			
//			float facemodx = fmod(floor(model_rel_twoD.x), HALF_FACE_SET_MAX_LENGTH);
//			float facemodz = fmod(floor(model_rel_twoD.y), FACE_SET_MAX_LENGTH);
//			float facemodxz = fmod(floor(model_rel_twoD.x), HALF_FACE_SET_MAX_LENGTH) * FACE_SET_MAX_LENGTH + fmod(floor(model_rel_twoD.y), FACE_SET_MAX_LENGTH);
//			float facemodxz = fmod(model_rel_twoD.x, HALF_FACE_SET_MAX_LENGTH) * FACE_SET_MAX_LENGTH + fmod(model_rel_twoD.y, FACE_SET_MAX_LENGTH);
//			float power_lookup = floor(index / pow(2.0, (facemodxz) * BITS_PER_LIGHT_LEVEL));
//			float power_lookup2 =  floor(index  / pow(2.0, (facemodxz) * BITS_PER_LIGHT_LEVEL + 1));
//			fixed light_one = fmod( power_lookup , 2.0  ); // > 0.0 ? 0.5 : 0.0; // pretend on off
//			fixed light_two = fmod( power_lookup2 , 2.0  ); //  > 0.0 ? 1.0 : 0.0; // pretend on off //WANT TEST
//			fixed local_light =   (light_one + light_two) / 1.5;

			//ALT POW 4 version
//			float facemodz = fmod(floor(model_rel_twoD.y), FACE_SET_MAX_LENGTH); 
			float power_lookup = floor(index / pow(NUM_LIGHT_LEVELS, model_rel_twoD.y)); 
			half light_one = fmod( power_lookup , NUM_LIGHT_LEVELS  );
			

//			if (light_one < 1.0) 
//				return fixed4(1.0, 1.0,0.0,1.0) * i.color.z; //test
			if (light_one < 1.0) 
				return fixed4(1.0, 1.0 * model_rel_twoD.y/FACE_SET_MAX_LENGTH,0.0,1.0) * i.color.z; //test	
//			if (light_one < 6.0) 
//				return fixed4(0.0, 1.0 * model_rel_twoD.y/FACE_SET_MAX_LENGTH,1.0,1.0) * i.color.z; //test		
			
			fixed local_light = (light_one + 2.0) / NUM_LIGHT_LEVELS_PLUS_ONE; // LOWEST IS NOT ZERO //   light_one / 3.0;
			
			
			
	        return tex2D(_BlockTex, scaled_uv) * i.color.z * local_light; // color z == light level

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
