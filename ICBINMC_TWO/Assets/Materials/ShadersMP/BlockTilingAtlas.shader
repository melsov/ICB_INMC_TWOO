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
        
        uniform sampler2D _BlockTex;
//        uniform fixed _GameClock;

        struct appdata {
            float4 vertex : POSITION;
            float3 normal : NORMAL;
            half2 texcoord : TEXCOORD0;
        };

        struct v2f {
            half4 pos : SV_POSITION;
            float4 color : COLOR;
            half2 uv : TEXCOORD0;
        };
        
        
        
        v2f vert(appdata v) {
            v2f o;
            o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
            o.color.xyz = v.normal * 0.5 + 0.5;
            o.color.w = 1.0;
			
			half scale = 0.25;
			half fudgeS = 0.5;
			half fudgeAlso = 33.5;
			half texX =  v.vertex.x * fudgeAlso + fudgeS; 
			texX = half(texX - int(texX)) * scale;

			half texZ =  v.vertex.z * fudgeAlso + fudgeS;
			texZ = half(texZ - int(texZ)) * scale;
			
			half texY = v.vertex.y * fudgeAlso + fudgeS;
			texY = half(texY - int(texY)) * scale;
			
			o.color = float4(texX, texZ, texY, 1.0);


			half2 texOut = half2(texX, texZ);
			
			if (v.normal.z > .9 || v.normal.z < -.9)
				texOut = half2(texY + scale, texX);
			else if (v.normal.x > .9 || v.normal.x < -.9)
				texOut = half2(texY, texZ + scale);
			
			float rem = fmod(floor(v.vertex.x), 16.0)/16.0;
			float zrem = fmod(floor(v.vertex.z), 16.0)/16.0;
			
//			rem = rem > .5 ? rem - 0.5 : rem;
			o.color = float4(rem, .7, zrem, 1.0);
			
//			if (texX > .9 || texZ > .9)
//			float xf = float(v.vertex.x - int(v.vertex.x));
//			if (xf < 0.04 )
//				o.color.xyz += -1.0;
//            o.uv = MultiplyUV( UNITY_MATRIX_TEXTURE0, v.texcoord ); 
			o.uv = MultiplyUV( UNITY_MATRIX_TEXTURE0, texOut ); 

            return o;
        }
        
        fixed4 frag(v2f i) : COLOR {
        	
        	
			
        
	        return tex2D(_BlockTex, i.uv);
//			fixed4 texcolor = tex2D(_BlockTex, i.uv);
//			return fixed4(i.color);
//			
//			float2 wcoord = float2(i.uv);
//			fixed4 color;
//			
//			//				if (fmod(1.0*wcoord.x,0.25)<1.0 || fmod(20.0*wcoord.y,15.0)<1.0 ) {
//			if (wcoord.x < 0.1 ) {//|| wcoord.y < 0.9) {
//			//					color = fixed4(wcoord.xy,0.0,1.0);
//				color = fixed4(1.0, 0.8,0.0,1.0);
//			} else {
//				color = fixed4(0.3,0.3,0.3,1.0);
//			//					color =   fixed4(i.color) + tex2D(_BlockTex, i.uv);
//			}
//			return color + texcolor;
        
//        	return   fixed4(i.color); // + tex2D(_BlockTex, i.uv);  //fixed4(i.uv.x, i.uv.y, .7, 1.0);
//            return  tex2D(_BlockTex, i.uv);// TIME OFF FOR THE MOMNT * _GameClock; // fixed4(1.0, .6, 1.0, 1.0);
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
