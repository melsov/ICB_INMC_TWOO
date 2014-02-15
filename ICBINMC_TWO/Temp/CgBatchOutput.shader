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
        Program "vp" {
// Vertex combos: 1
//   opengl - ALU: 69 to 69
//   d3d9 - ALU: 111 to 111
SubProgram "opengl " {
Keywords { }
Bind "vertex" Vertex
Bind "normal" Normal
Bind "texcoord" TexCoord0
Bind "tangent" ATTR14
Float 5 [_GameClock]
"!!ARBvp1.0
# 69 ALU
PARAM c[9] = { { 0.34008789, 1.5147929, 0, 0.2 },
		state.matrix.mvp,
		program.local[5],
		{ -0.2, 0.44995117, 0.25, 0.5 },
		{ 1, 3.3327909, 0.75, 16 },
		{ 4 } };
TEMP R0;
TEMP R1;
TEMP R2;
ADD R0.x, vertex.position.z, c[6].w;
ADD R0.y, vertex.position, -R0.x;
SLT R1.y, c[0].w, vertex.normal.z;
ADD R0.y, R0, c[6].w;
MAD R0.w, R1.y, R0.y, R0.x;
ADD R0.x, vertex.position.y, -R0.w;
ADD R1.x, R0, c[6].w;
ABS R0.x, R1.y;
SGE R0.y, c[0].z, R0.x;
SLT R0.z, vertex.normal, c[6].x;
MUL R2.x, R0.y, R0.z;
MAD R0.x, R2, R1, R0.w;
ADD R0.w, vertex.position.y, -R0.x;
ABS R0.z, R0;
SGE R0.z, c[0], R0;
MUL R1.z, R0.y, R0;
SLT R1.w, c[0], vertex.normal.x;
MUL R0.z, R1, R1.w;
ADD R0.w, R0, c[6];
MAD R0.w, R0.z, R0, R0.x;
ADD R1.x, vertex.position.y, -R0.w;
MOV R0.y, c[0].x;
ADD R0.y, -R0, c[5].x;
MUL R0.x, R0.y, c[0].y;
SLT R0.y, R0.x, c[0].z;
ADD R2.y, -R0.x, c[7].x;
MUL R2.w, R0.x, R0.y;
MOV R2.z, c[6].w;
MUL R2.y, R2, c[6].w;
MAD R2.z, R2.w, c[7].y, R2;
MAD R2.z, R1.y, R2.y, R2;
MUL R1.y, R0.x, c[6].w;
MAD R2.x, R1.y, R2, R2.z;
MAD R2.x, R2.y, R0.z, R2;
ABS R1.w, R1;
SGE R2.y, c[0].z, R1.w;
MUL R1.z, R1, R2.y;
SLT R1.w, vertex.normal.x, c[6].x;
MUL R2.y, R1.z, R1.w;
ADD R1.x, R1, c[6].w;
MAD result.texcoord[0].y, R2, R1.x, R0.w;
MAD R1.y, R1, R2, R2.x;
MAD R1.x, R0, c[6].z, -R1.y;
ABS R0.w, R1;
ADD R1.w, R1.x, c[7].z;
SGE R1.x, c[0].z, R0.w;
MUL R1.x, R1.z, R1;
ADD R0.w, vertex.position.x, c[6];
ADD R1.z, vertex.position, -R0.w;
MUL R0.y, R1.x, R0;
ADD R1.z, R1, c[6].w;
MAD R0.z, R0, R1, R0.w;
MUL R0.x, R0, R0.y;
ADD R0.y, vertex.position.z, -R0.z;
ADD R0.w, R0.y, c[6];
MAD R1.y, R1.x, R1.w, R1;
MAD result.color.z, R0.x, c[6].y, R1.y;
MUL R0.x, vertex.texcoord[0], c[7].w;
MUL R0.y, R0.x, c[6].z;
MAD result.texcoord[0].x, R2.y, R0.w, R0.z;
FLR R0.z, R0.y;
MOV R0.y, R0.z;
MAD R0.x, -R0.z, c[8], R0;
ADD result.texcoord[1], vertex.attrib[14], c[6].w;
MUL result.color.xy, R0, c[6].z;
DP4 result.position.w, vertex.position, c[4];
DP4 result.position.z, vertex.position, c[3];
DP4 result.position.y, vertex.position, c[2];
DP4 result.position.x, vertex.position, c[1];
END
# 69 instructions, 3 R-regs
"
}

SubProgram "d3d9 " {
Keywords { }
Bind "vertex" Vertex
Bind "normal" Normal
Bind "texcoord" TexCoord0
Bind "tangent" TexCoord2
Matrix 0 [glstate_matrix_mvp]
Float 4 [_GameClock]
"vs_2_0
; 111 ALU
def c5, 0.20000000, 0.00000000, -0.20000000, 0.50000000
def c6, 1.00000000, -0.34008789, 1.51479292, 0.44995117
def c7, 3.33279085, 0.50000000, 0.25000000, 0.75000000
def c8, 16.00000000, 4.00000000, 0, 0
dcl_position0 v0
dcl_normal0 v1
dcl_texcoord0 v2
dcl_tangent0 v3
slt r2.w, c5.x, v1.x
slt r2.x, v1.z, c5.z
mov r0.x, c4
add r0.x, c6.y, r0
mul r0.y, r0.x, c6.z
slt r0.z, r0.y, c5.y
max r0.w, -r0.z, r0.z
slt r1.x, c5.y, r0.w
slt r0.x, c5, v1.z
add r1.y, -r1.x, c6.x
mul r1.z, r1.y, c5.w
mad r1.y, r0, c7.x, c7
mad r1.x, r1, r1.y, r1.z
max r0.w, -r0.x, r0.x
slt r0.w, c5.y, r0
add r1.z, -r0.w, c6.x
add r1.y, -r0, c6.x
mul r1.z, r1.x, r1
mad r1.x, r1.y, c5.w, r1
mad r1.z, r0.w, r1.x, r1
mad r1.w, r0.y, c5, r1.z
sge r1.x, c5.y, r0
sge r0.w, r0.x, c5.y
mul r0.w, r0, r1.x
mul r1.x, r0.w, r2
sge r2.y, c5, r2.x
sge r2.x, r2, c5.y
mul r2.x, r2, r2.y
max r2.y, -r1.x, r1.x
mul r2.x, r0.w, r2
slt r2.y, c5, r2
add r2.z, -r2.y, c6.x
mul r2.z, r1, r2
mad r1.w, r2.y, r1, r2.z
mul r0.w, r2.x, r2
max r1.z, -r0.w, r0.w
mad r2.y, r1, c5.w, r1.w
slt r1.z, c5.y, r1
add r2.z, -r1, c6.x
sge r3.x, c5.y, r2.w
sge r1.y, r2.w, c5
mul r1.y, r1, r3.x
mul r2.z, r1.w, r2
mad r3.x, r1.z, r2.y, r2.z
slt r2.w, v1.x, c5.z
mul r2.x, r2, r1.y
mul r1.y, r2.x, r2.w
max r1.w, -r1.y, r1.y
slt r1.z, c5.y, r1.w
mad r1.w, r0.y, c5, r3.x
add r3.y, -r1.z, c6.x
sge r2.z, c5.y, r2.w
sge r2.y, r2.w, c5
mul r2.y, r2, r2.z
mul r2.x, r2, r2.y
mul r0.z, r2.x, r0
mul r2.z, r3.x, r3.y
mad r2.y, r1.z, r1.w, r2.z
max r1.z, -r2.x, r2.x
slt r1.z, c5.y, r1
add r1.w, -r1.z, c6.x
mul r2.x, r1.w, r2.y
mad r1.w, r0.y, c7.z, c7
mad r1.z, r1, r1.w, r2.x
max r0.z, -r0, r0
slt r0.z, c5.y, r0
mad r0.y, r0, c6.w, r1.z
add r1.w, -r0.z, c6.x
mul r1.z, r1, r1.w
mad oD0.z, r0, r0.y, r1
max r0.w, -r0, r0
slt r0.y, c5, r0.w
add r0.z, -r0.y, c6.x
add r0.w, v0.x, c5
mul r1.z, r0, r0.w
add r0.w, v0.z, c5
mad r1.z, r0.y, r0.w, r1
max r1.w, -r0.x, r0.x
max r1.y, -r1, r1
slt r0.x, c5.y, r1.y
slt r1.y, c5, r1.w
max r1.x, -r1, r1
slt r1.w, c5.y, r1.x
add r2.x, -r1.y, c6
add r1.x, v0.y, c5.w
mul r2.x, r0.w, r2
mad r2.x, r1.y, r1, r2
add r1.y, -r1.w, c6.x
mul r2.x, r1.y, r2
add r1.y, -r0.x, c6.x
mul r1.z, r1.y, r1
mad oT0.x, r0, r0.w, r1.z
mad r1.w, r1, r1.x, r2.x
mul r0.w, r0.z, r1
mad r0.w, r0.y, r1.x, r0
mul r0.z, v2.x, c8.x
mul r1.y, r1, r0.w
mul r0.y, r0.z, c7.z
frc r0.w, r0.y
mad oT0.y, r0.x, r1.x, r1
add r0.x, r0.y, -r0.w
mov r0.y, r0.x
mad r0.x, -r0, c8.y, r0.z
add oT1, v3, c5.w
mul oD0.xy, r0, c7.z
dp4 oPos.w, v0, c3
dp4 oPos.z, v0, c2
dp4 oPos.y, v0, c1
dp4 oPos.x, v0, c0
"
}

SubProgram "gles " {
Keywords { }
"!!GLES


#ifdef VERTEX

varying highp vec4 xlv_;
varying mediump vec2 xlv_TEXCOORD0;
varying highp vec4 xlv_COLOR;
uniform lowp float _GameClock;
uniform highp mat4 glstate_matrix_mvp;
attribute vec4 _glesTANGENT;
attribute vec4 _glesMultiTexCoord0;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  vec3 tmpvar_1;
  tmpvar_1 = normalize(_glesNormal);
  vec4 tmpvar_2;
  tmpvar_2.xyz = normalize(_glesTANGENT.xyz);
  tmpvar_2.w = _glesTANGENT.w;
  mediump vec2 tile_o_3;
  highp float index_4;
  mediump float light_level_5;
  highp float vy_6;
  highp float vx_7;
  highp vec4 tmpvar_8;
  mediump vec2 tmpvar_9;
  highp vec4 tmpvar_10;
  tmpvar_10 = (glstate_matrix_mvp * _glesVertex);
  vx_7 = (_glesVertex.x + 0.5);
  vy_6 = (_glesVertex.z + 0.5);
  light_level_5 = 0.5;
  lowp float tmpvar_11;
  tmpvar_11 = ((_GameClock - 0.34) / 0.66);
  lowp float tmpvar_12;
  tmpvar_12 = (tmpvar_11 * 0.5);
  lowp float tmpvar_13;
  tmpvar_13 = ((1.0 - tmpvar_11) * 0.5);
  if ((tmpvar_11 < 0.0)) {
    light_level_5 = (0.5 + (tmpvar_11 / 0.3));
  };
  if ((tmpvar_1.z > 0.2)) {
    vy_6 = (_glesVertex.y + 0.5);
    light_level_5 = (light_level_5 + tmpvar_13);
  } else {
    if ((tmpvar_1.z < -0.2)) {
      vy_6 = (_glesVertex.y + 0.5);
      light_level_5 = (light_level_5 + tmpvar_12);
    } else {
      if ((tmpvar_1.x > 0.2)) {
        vx_7 = (_glesVertex.z + 0.5);
        vy_6 = (_glesVertex.y + 0.5);
        light_level_5 = (light_level_5 + tmpvar_13);
      } else {
        if ((tmpvar_1.x < -0.2)) {
          vx_7 = (_glesVertex.z + 0.5);
          vy_6 = (_glesVertex.y + 0.5);
          light_level_5 = (light_level_5 + tmpvar_12);
        } else {
          lowp float tmpvar_14;
          tmpvar_14 = (0.75 + (0.25 * tmpvar_11));
          light_level_5 = tmpvar_14;
          if ((tmpvar_11 < 0.0)) {
            light_level_5 = (light_level_5 + (0.45 * tmpvar_11));
          };
        };
      };
    };
  };
  mediump float tmpvar_15;
  tmpvar_15 = (_glesMultiTexCoord0.x * 16.0);
  index_4 = tmpvar_15;
  highp float tmpvar_16;
  tmpvar_16 = floor((index_4 / 4.0));
  highp vec2 tmpvar_17;
  tmpvar_17.x = (index_4 - (tmpvar_16 * 4.0));
  tmpvar_17.y = tmpvar_16;
  highp vec2 tmpvar_18;
  tmpvar_18 = (tmpvar_17 * 0.25);
  tile_o_3 = tmpvar_18;
  tmpvar_8.xy = tile_o_3;
  tmpvar_8.z = light_level_5;
  highp vec2 tmpvar_19;
  tmpvar_19.x = vx_7;
  tmpvar_19.y = vy_6;
  tmpvar_9 = tmpvar_19;
  gl_Position = tmpvar_10;
  xlv_COLOR = tmpvar_8;
  xlv_TEXCOORD0 = tmpvar_9;
  xlv_ = (tmpvar_2 + vec4(0.5, 0.5, 0.5, 0.5));
}



#endif
#ifdef FRAGMENT

varying highp vec4 xlv_;
varying mediump vec2 xlv_TEXCOORD0;
varying highp vec4 xlv_COLOR;
uniform sampler2D _BlockTex;
void main ()
{
  lowp vec4 tmpvar_1;
  lowp float local_light_2;
  mediump float light_one_3;
  mediump vec2 offset_4;
  mediump vec2 scaled_uv_5;
  highp vec2 tmpvar_6;
  tmpvar_6 = xlv_COLOR.xy;
  offset_4 = tmpvar_6;
  scaled_uv_5 = (((xlv_TEXCOORD0 - floor(xlv_TEXCOORD0)) * 0.25) + offset_4);
  mediump vec2 tmpvar_7;
  tmpvar_7 = (floor(xlv_TEXCOORD0) / vec2(4.0, 4.0));
  mediump vec2 tmpvar_8;
  tmpvar_8 = (fract(abs(tmpvar_7)) * vec2(4.0, 4.0));
  mediump float tmpvar_9;
  if ((tmpvar_7.x >= 0.0)) {
    tmpvar_9 = tmpvar_8.x;
  } else {
    tmpvar_9 = -(tmpvar_8.x);
  };
  mediump float tmpvar_10;
  if ((tmpvar_7.y >= 0.0)) {
    tmpvar_10 = tmpvar_8.y;
  } else {
    tmpvar_10 = -(tmpvar_8.y);
  };
  mediump vec2 tmpvar_11;
  tmpvar_11.x = tmpvar_9;
  tmpvar_11.y = tmpvar_10;
  highp float xco_12;
  xco_12 = tmpvar_11.x;
  highp float tmpvar_13;
  if ((xco_12 < 1.0)) {
    tmpvar_13 = xlv_.x;
  } else {
    if ((xco_12 < 2.0)) {
      tmpvar_13 = xlv_.y;
    } else {
      if ((xco_12 < 3.0)) {
        tmpvar_13 = xlv_.z;
      } else {
        tmpvar_13 = xlv_.w;
      };
    };
  };
  mediump float tmpvar_14;
  tmpvar_14 = pow (8.0, tmpvar_10);
  highp float tmpvar_15;
  tmpvar_15 = (floor((tmpvar_13 / tmpvar_14)) / 8.0);
  highp float tmpvar_16;
  tmpvar_16 = (fract(abs(tmpvar_15)) * 8.0);
  highp float tmpvar_17;
  if ((tmpvar_15 >= 0.0)) {
    tmpvar_17 = tmpvar_16;
  } else {
    tmpvar_17 = -(tmpvar_16);
  };
  light_one_3 = tmpvar_17;
  mediump float tmpvar_18;
  tmpvar_18 = ((light_one_3 + 2.0) / 9.0);
  local_light_2 = tmpvar_18;
  tmpvar_1 = ((texture2D (_BlockTex, scaled_uv_5) * xlv_COLOR.z) * local_light_2);
  gl_FragData[0] = tmpvar_1;
}



#endif"
}

SubProgram "glesdesktop " {
Keywords { }
"!!GLES


#ifdef VERTEX

varying highp vec4 xlv_;
varying mediump vec2 xlv_TEXCOORD0;
varying highp vec4 xlv_COLOR;
uniform lowp float _GameClock;
uniform highp mat4 glstate_matrix_mvp;
attribute vec4 _glesTANGENT;
attribute vec4 _glesMultiTexCoord0;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  vec3 tmpvar_1;
  tmpvar_1 = normalize(_glesNormal);
  vec4 tmpvar_2;
  tmpvar_2.xyz = normalize(_glesTANGENT.xyz);
  tmpvar_2.w = _glesTANGENT.w;
  mediump vec2 tile_o_3;
  highp float index_4;
  mediump float light_level_5;
  highp float vy_6;
  highp float vx_7;
  highp vec4 tmpvar_8;
  mediump vec2 tmpvar_9;
  highp vec4 tmpvar_10;
  tmpvar_10 = (glstate_matrix_mvp * _glesVertex);
  vx_7 = (_glesVertex.x + 0.5);
  vy_6 = (_glesVertex.z + 0.5);
  light_level_5 = 0.5;
  lowp float tmpvar_11;
  tmpvar_11 = ((_GameClock - 0.34) / 0.66);
  lowp float tmpvar_12;
  tmpvar_12 = (tmpvar_11 * 0.5);
  lowp float tmpvar_13;
  tmpvar_13 = ((1.0 - tmpvar_11) * 0.5);
  if ((tmpvar_11 < 0.0)) {
    light_level_5 = (0.5 + (tmpvar_11 / 0.3));
  };
  if ((tmpvar_1.z > 0.2)) {
    vy_6 = (_glesVertex.y + 0.5);
    light_level_5 = (light_level_5 + tmpvar_13);
  } else {
    if ((tmpvar_1.z < -0.2)) {
      vy_6 = (_glesVertex.y + 0.5);
      light_level_5 = (light_level_5 + tmpvar_12);
    } else {
      if ((tmpvar_1.x > 0.2)) {
        vx_7 = (_glesVertex.z + 0.5);
        vy_6 = (_glesVertex.y + 0.5);
        light_level_5 = (light_level_5 + tmpvar_13);
      } else {
        if ((tmpvar_1.x < -0.2)) {
          vx_7 = (_glesVertex.z + 0.5);
          vy_6 = (_glesVertex.y + 0.5);
          light_level_5 = (light_level_5 + tmpvar_12);
        } else {
          lowp float tmpvar_14;
          tmpvar_14 = (0.75 + (0.25 * tmpvar_11));
          light_level_5 = tmpvar_14;
          if ((tmpvar_11 < 0.0)) {
            light_level_5 = (light_level_5 + (0.45 * tmpvar_11));
          };
        };
      };
    };
  };
  mediump float tmpvar_15;
  tmpvar_15 = (_glesMultiTexCoord0.x * 16.0);
  index_4 = tmpvar_15;
  highp float tmpvar_16;
  tmpvar_16 = floor((index_4 / 4.0));
  highp vec2 tmpvar_17;
  tmpvar_17.x = (index_4 - (tmpvar_16 * 4.0));
  tmpvar_17.y = tmpvar_16;
  highp vec2 tmpvar_18;
  tmpvar_18 = (tmpvar_17 * 0.25);
  tile_o_3 = tmpvar_18;
  tmpvar_8.xy = tile_o_3;
  tmpvar_8.z = light_level_5;
  highp vec2 tmpvar_19;
  tmpvar_19.x = vx_7;
  tmpvar_19.y = vy_6;
  tmpvar_9 = tmpvar_19;
  gl_Position = tmpvar_10;
  xlv_COLOR = tmpvar_8;
  xlv_TEXCOORD0 = tmpvar_9;
  xlv_ = (tmpvar_2 + vec4(0.5, 0.5, 0.5, 0.5));
}



#endif
#ifdef FRAGMENT

varying highp vec4 xlv_;
varying mediump vec2 xlv_TEXCOORD0;
varying highp vec4 xlv_COLOR;
uniform sampler2D _BlockTex;
void main ()
{
  lowp vec4 tmpvar_1;
  lowp float local_light_2;
  mediump float light_one_3;
  mediump vec2 offset_4;
  mediump vec2 scaled_uv_5;
  highp vec2 tmpvar_6;
  tmpvar_6 = xlv_COLOR.xy;
  offset_4 = tmpvar_6;
  scaled_uv_5 = (((xlv_TEXCOORD0 - floor(xlv_TEXCOORD0)) * 0.25) + offset_4);
  mediump vec2 tmpvar_7;
  tmpvar_7 = (floor(xlv_TEXCOORD0) / vec2(4.0, 4.0));
  mediump vec2 tmpvar_8;
  tmpvar_8 = (fract(abs(tmpvar_7)) * vec2(4.0, 4.0));
  mediump float tmpvar_9;
  if ((tmpvar_7.x >= 0.0)) {
    tmpvar_9 = tmpvar_8.x;
  } else {
    tmpvar_9 = -(tmpvar_8.x);
  };
  mediump float tmpvar_10;
  if ((tmpvar_7.y >= 0.0)) {
    tmpvar_10 = tmpvar_8.y;
  } else {
    tmpvar_10 = -(tmpvar_8.y);
  };
  mediump vec2 tmpvar_11;
  tmpvar_11.x = tmpvar_9;
  tmpvar_11.y = tmpvar_10;
  highp float xco_12;
  xco_12 = tmpvar_11.x;
  highp float tmpvar_13;
  if ((xco_12 < 1.0)) {
    tmpvar_13 = xlv_.x;
  } else {
    if ((xco_12 < 2.0)) {
      tmpvar_13 = xlv_.y;
    } else {
      if ((xco_12 < 3.0)) {
        tmpvar_13 = xlv_.z;
      } else {
        tmpvar_13 = xlv_.w;
      };
    };
  };
  mediump float tmpvar_14;
  tmpvar_14 = pow (8.0, tmpvar_10);
  highp float tmpvar_15;
  tmpvar_15 = (floor((tmpvar_13 / tmpvar_14)) / 8.0);
  highp float tmpvar_16;
  tmpvar_16 = (fract(abs(tmpvar_15)) * 8.0);
  highp float tmpvar_17;
  if ((tmpvar_15 >= 0.0)) {
    tmpvar_17 = tmpvar_16;
  } else {
    tmpvar_17 = -(tmpvar_16);
  };
  light_one_3 = tmpvar_17;
  mediump float tmpvar_18;
  tmpvar_18 = ((light_one_3 + 2.0) / 9.0);
  local_light_2 = tmpvar_18;
  tmpvar_1 = ((texture2D (_BlockTex, scaled_uv_5) * xlv_COLOR.z) * local_light_2);
  gl_FragData[0] = tmpvar_1;
}



#endif"
}

SubProgram "flash " {
Keywords { }
Bind "vertex" Vertex
Bind "normal" Normal
Bind "texcoord" TexCoord0
Bind "tangent" TexCoord2
Matrix 0 [glstate_matrix_mvp]
Float 4 [_GameClock]
"agal_vs
c5 0.2 0.0 -0.2 0.5
c6 1.0 -0.340088 1.514793 0.449951
c7 3.332791 0.5 0.25 0.75
c8 16.0 4.0 0.0 0.0
[bc]
ckaaaaaaacaaaiacafaaaaaaabaaaaaaabaaaaaaaaaaaaaa slt r2.w, c5.x, a1.x
ckaaaaaaacaaabacabaaaakkaaaaaaaaafaaaakkabaaaaaa slt r2.x, a1.z, c5.z
aaaaaaaaaaaaabacaeaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov r0.x, c4
abaaaaaaaaaaabacagaaaaffabaaaaaaaaaaaaaaacaaaaaa add r0.x, c6.y, r0.x
adaaaaaaaaaaacacaaaaaaaaacaaaaaaagaaaakkabaaaaaa mul r0.y, r0.x, c6.z
ckaaaaaaaaaaaeacaaaaaaffacaaaaaaafaaaaffabaaaaaa slt r0.z, r0.y, c5.y
bfaaaaaaabaaaeacaaaaaakkacaaaaaaaaaaaaaaaaaaaaaa neg r1.z, r0.z
ahaaaaaaaaaaaiacabaaaakkacaaaaaaaaaaaakkacaaaaaa max r0.w, r1.z, r0.z
ckaaaaaaabaaabacafaaaaffabaaaaaaaaaaaappacaaaaaa slt r1.x, c5.y, r0.w
ckaaaaaaaaaaabacafaaaaoeabaaaaaaabaaaakkaaaaaaaa slt r0.x, c5, a1.z
bfaaaaaaadaaabacabaaaaaaacaaaaaaaaaaaaaaaaaaaaaa neg r3.x, r1.x
abaaaaaaabaaacacadaaaaaaacaaaaaaagaaaaaaabaaaaaa add r1.y, r3.x, c6.x
adaaaaaaabaaaeacabaaaaffacaaaaaaafaaaappabaaaaaa mul r1.z, r1.y, c5.w
adaaaaaaabaaacacaaaaaaffacaaaaaaahaaaaaaabaaaaaa mul r1.y, r0.y, c7.x
abaaaaaaabaaacacabaaaaffacaaaaaaahaaaaoeabaaaaaa add r1.y, r1.y, c7
adaaaaaaadaaaeacabaaaaaaacaaaaaaabaaaaffacaaaaaa mul r3.z, r1.x, r1.y
abaaaaaaabaaabacadaaaakkacaaaaaaabaaaakkacaaaaaa add r1.x, r3.z, r1.z
bfaaaaaaaeaaabacaaaaaaaaacaaaaaaaaaaaaaaaaaaaaaa neg r4.x, r0.x
ahaaaaaaaaaaaiacaeaaaaaaacaaaaaaaaaaaaaaacaaaaaa max r0.w, r4.x, r0.x
ckaaaaaaaaaaaiacafaaaaffabaaaaaaaaaaaappacaaaaaa slt r0.w, c5.y, r0.w
bfaaaaaaaeaaaiacaaaaaappacaaaaaaaaaaaaaaaaaaaaaa neg r4.w, r0.w
abaaaaaaabaaaeacaeaaaappacaaaaaaagaaaaaaabaaaaaa add r1.z, r4.w, c6.x
bfaaaaaaabaaacacaaaaaaffacaaaaaaaaaaaaaaaaaaaaaa neg r1.y, r0.y
abaaaaaaabaaacacabaaaaffacaaaaaaagaaaaaaabaaaaaa add r1.y, r1.y, c6.x
adaaaaaaabaaaeacabaaaaaaacaaaaaaabaaaakkacaaaaaa mul r1.z, r1.x, r1.z
adaaaaaaaeaaabacabaaaaffacaaaaaaafaaaappabaaaaaa mul r4.x, r1.y, c5.w
abaaaaaaabaaabacaeaaaaaaacaaaaaaabaaaaaaacaaaaaa add r1.x, r4.x, r1.x
adaaaaaaaeaaaeacaaaaaappacaaaaaaabaaaaaaacaaaaaa mul r4.z, r0.w, r1.x
abaaaaaaabaaaeacaeaaaakkacaaaaaaabaaaakkacaaaaaa add r1.z, r4.z, r1.z
adaaaaaaaeaaaiacaaaaaaffacaaaaaaafaaaaoeabaaaaaa mul r4.w, r0.y, c5
abaaaaaaabaaaiacaeaaaappacaaaaaaabaaaakkacaaaaaa add r1.w, r4.w, r1.z
cjaaaaaaabaaabacafaaaaffabaaaaaaaaaaaaaaacaaaaaa sge r1.x, c5.y, r0.x
cjaaaaaaaaaaaiacaaaaaaaaacaaaaaaafaaaaffabaaaaaa sge r0.w, r0.x, c5.y
adaaaaaaaaaaaiacaaaaaappacaaaaaaabaaaaaaacaaaaaa mul r0.w, r0.w, r1.x
adaaaaaaabaaabacaaaaaappacaaaaaaacaaaaaaacaaaaaa mul r1.x, r0.w, r2.x
cjaaaaaaacaaacacafaaaaoeabaaaaaaacaaaaaaacaaaaaa sge r2.y, c5, r2.x
cjaaaaaaacaaabacacaaaaaaacaaaaaaafaaaaffabaaaaaa sge r2.x, r2.x, c5.y
adaaaaaaacaaabacacaaaaaaacaaaaaaacaaaaffacaaaaaa mul r2.x, r2.x, r2.y
bfaaaaaaaeaaabacabaaaaaaacaaaaaaaaaaaaaaaaaaaaaa neg r4.x, r1.x
ahaaaaaaacaaacacaeaaaaaaacaaaaaaabaaaaaaacaaaaaa max r2.y, r4.x, r1.x
adaaaaaaacaaabacaaaaaappacaaaaaaacaaaaaaacaaaaaa mul r2.x, r0.w, r2.x
ckaaaaaaacaaacacafaaaaoeabaaaaaaacaaaaffacaaaaaa slt r2.y, c5, r2.y
bfaaaaaaaeaaacacacaaaaffacaaaaaaaaaaaaaaaaaaaaaa neg r4.y, r2.y
abaaaaaaacaaaeacaeaaaaffacaaaaaaagaaaaaaabaaaaaa add r2.z, r4.y, c6.x
adaaaaaaacaaaeacabaaaakkacaaaaaaacaaaakkacaaaaaa mul r2.z, r1.z, r2.z
adaaaaaaabaaaiacacaaaaffacaaaaaaabaaaappacaaaaaa mul r1.w, r2.y, r1.w
abaaaaaaabaaaiacabaaaappacaaaaaaacaaaakkacaaaaaa add r1.w, r1.w, r2.z
adaaaaaaaaaaaiacacaaaaaaacaaaaaaacaaaappacaaaaaa mul r0.w, r2.x, r2.w
bfaaaaaaaeaaaiacaaaaaappacaaaaaaaaaaaaaaaaaaaaaa neg r4.w, r0.w
ahaaaaaaabaaaeacaeaaaappacaaaaaaaaaaaappacaaaaaa max r1.z, r4.w, r0.w
adaaaaaaacaaacacabaaaaffacaaaaaaafaaaappabaaaaaa mul r2.y, r1.y, c5.w
abaaaaaaacaaacacacaaaaffacaaaaaaabaaaappacaaaaaa add r2.y, r2.y, r1.w
ckaaaaaaabaaaeacafaaaaffabaaaaaaabaaaakkacaaaaaa slt r1.z, c5.y, r1.z
bfaaaaaaacaaaeacabaaaakkacaaaaaaaaaaaaaaaaaaaaaa neg r2.z, r1.z
abaaaaaaacaaaeacacaaaakkacaaaaaaagaaaaaaabaaaaaa add r2.z, r2.z, c6.x
cjaaaaaaadaaabacafaaaaffabaaaaaaacaaaappacaaaaaa sge r3.x, c5.y, r2.w
cjaaaaaaabaaacacacaaaappacaaaaaaafaaaaoeabaaaaaa sge r1.y, r2.w, c5
adaaaaaaabaaacacabaaaaffacaaaaaaadaaaaaaacaaaaaa mul r1.y, r1.y, r3.x
adaaaaaaacaaaeacabaaaappacaaaaaaacaaaakkacaaaaaa mul r2.z, r1.w, r2.z
adaaaaaaadaaabacabaaaakkacaaaaaaacaaaaffacaaaaaa mul r3.x, r1.z, r2.y
abaaaaaaadaaabacadaaaaaaacaaaaaaacaaaakkacaaaaaa add r3.x, r3.x, r2.z
ckaaaaaaacaaaiacabaaaaaaaaaaaaaaafaaaakkabaaaaaa slt r2.w, a1.x, c5.z
adaaaaaaacaaabacacaaaaaaacaaaaaaabaaaaffacaaaaaa mul r2.x, r2.x, r1.y
adaaaaaaabaaacacacaaaaaaacaaaaaaacaaaappacaaaaaa mul r1.y, r2.x, r2.w
bfaaaaaaaeaaacacabaaaaffacaaaaaaaaaaaaaaaaaaaaaa neg r4.y, r1.y
ahaaaaaaabaaaiacaeaaaaffacaaaaaaabaaaaffacaaaaaa max r1.w, r4.y, r1.y
ckaaaaaaabaaaeacafaaaaffabaaaaaaabaaaappacaaaaaa slt r1.z, c5.y, r1.w
adaaaaaaabaaaiacaaaaaaffacaaaaaaafaaaaoeabaaaaaa mul r1.w, r0.y, c5
abaaaaaaabaaaiacabaaaappacaaaaaaadaaaaaaacaaaaaa add r1.w, r1.w, r3.x
bfaaaaaaaeaaaeacabaaaakkacaaaaaaaaaaaaaaaaaaaaaa neg r4.z, r1.z
abaaaaaaadaaacacaeaaaakkacaaaaaaagaaaaaaabaaaaaa add r3.y, r4.z, c6.x
cjaaaaaaacaaaeacafaaaaffabaaaaaaacaaaappacaaaaaa sge r2.z, c5.y, r2.w
cjaaaaaaacaaacacacaaaappacaaaaaaafaaaaoeabaaaaaa sge r2.y, r2.w, c5
adaaaaaaacaaacacacaaaaffacaaaaaaacaaaakkacaaaaaa mul r2.y, r2.y, r2.z
adaaaaaaacaaabacacaaaaaaacaaaaaaacaaaaffacaaaaaa mul r2.x, r2.x, r2.y
adaaaaaaaaaaaeacacaaaaaaacaaaaaaaaaaaakkacaaaaaa mul r0.z, r2.x, r0.z
adaaaaaaacaaaeacadaaaaaaacaaaaaaadaaaaffacaaaaaa mul r2.z, r3.x, r3.y
adaaaaaaaeaaacacabaaaakkacaaaaaaabaaaappacaaaaaa mul r4.y, r1.z, r1.w
abaaaaaaacaaacacaeaaaaffacaaaaaaacaaaakkacaaaaaa add r2.y, r4.y, r2.z
bfaaaaaaaeaaabacacaaaaaaacaaaaaaaaaaaaaaaaaaaaaa neg r4.x, r2.x
ahaaaaaaabaaaeacaeaaaaaaacaaaaaaacaaaaaaacaaaaaa max r1.z, r4.x, r2.x
ckaaaaaaabaaaeacafaaaaffabaaaaaaabaaaakkacaaaaaa slt r1.z, c5.y, r1.z
bfaaaaaaaeaaaeacabaaaakkacaaaaaaaaaaaaaaaaaaaaaa neg r4.z, r1.z
abaaaaaaabaaaiacaeaaaakkacaaaaaaagaaaaaaabaaaaaa add r1.w, r4.z, c6.x
adaaaaaaacaaabacabaaaappacaaaaaaacaaaaffacaaaaaa mul r2.x, r1.w, r2.y
adaaaaaaabaaaiacaaaaaaffacaaaaaaahaaaakkabaaaaaa mul r1.w, r0.y, c7.z
abaaaaaaabaaaiacabaaaappacaaaaaaahaaaaoeabaaaaaa add r1.w, r1.w, c7
adaaaaaaabaaaeacabaaaakkacaaaaaaabaaaappacaaaaaa mul r1.z, r1.z, r1.w
abaaaaaaabaaaeacabaaaakkacaaaaaaacaaaaaaacaaaaaa add r1.z, r1.z, r2.x
bfaaaaaaaeaaaeacaaaaaakkacaaaaaaaaaaaaaaaaaaaaaa neg r4.z, r0.z
ahaaaaaaaaaaaeacaeaaaakkacaaaaaaaaaaaakkacaaaaaa max r0.z, r4.z, r0.z
ckaaaaaaaaaaaeacafaaaaffabaaaaaaaaaaaakkacaaaaaa slt r0.z, c5.y, r0.z
adaaaaaaaaaaacacaaaaaaffacaaaaaaagaaaappabaaaaaa mul r0.y, r0.y, c6.w
abaaaaaaaaaaacacaaaaaaffacaaaaaaabaaaakkacaaaaaa add r0.y, r0.y, r1.z
bfaaaaaaaeaaaeacaaaaaakkacaaaaaaaaaaaaaaaaaaaaaa neg r4.z, r0.z
abaaaaaaabaaaiacaeaaaakkacaaaaaaagaaaaaaabaaaaaa add r1.w, r4.z, c6.x
adaaaaaaabaaaeacabaaaakkacaaaaaaabaaaappacaaaaaa mul r1.z, r1.z, r1.w
adaaaaaaaeaaaeacaaaaaakkacaaaaaaaaaaaaffacaaaaaa mul r4.z, r0.z, r0.y
abaaaaaaahaaaeaeaeaaaakkacaaaaaaabaaaakkacaaaaaa add v7.z, r4.z, r1.z
bfaaaaaaaeaaaiacaaaaaappacaaaaaaaaaaaaaaaaaaaaaa neg r4.w, r0.w
ahaaaaaaaaaaaiacaeaaaappacaaaaaaaaaaaappacaaaaaa max r0.w, r4.w, r0.w
ckaaaaaaaaaaacacafaaaaoeabaaaaaaaaaaaappacaaaaaa slt r0.y, c5, r0.w
bfaaaaaaaeaaacacaaaaaaffacaaaaaaaaaaaaaaaaaaaaaa neg r4.y, r0.y
abaaaaaaaaaaaeacaeaaaaffacaaaaaaagaaaaaaabaaaaaa add r0.z, r4.y, c6.x
abaaaaaaaaaaaiacaaaaaaaaaaaaaaaaafaaaaoeabaaaaaa add r0.w, a0.x, c5
adaaaaaaabaaaeacaaaaaakkacaaaaaaaaaaaappacaaaaaa mul r1.z, r0.z, r0.w
abaaaaaaaaaaaiacaaaaaakkaaaaaaaaafaaaaoeabaaaaaa add r0.w, a0.z, c5
adaaaaaaaeaaaeacaaaaaaffacaaaaaaaaaaaappacaaaaaa mul r4.z, r0.y, r0.w
abaaaaaaabaaaeacaeaaaakkacaaaaaaabaaaakkacaaaaaa add r1.z, r4.z, r1.z
bfaaaaaaaeaaabacaaaaaaaaacaaaaaaaaaaaaaaaaaaaaaa neg r4.x, r0.x
ahaaaaaaabaaaiacaeaaaaaaacaaaaaaaaaaaaaaacaaaaaa max r1.w, r4.x, r0.x
bfaaaaaaaeaaacacabaaaaffacaaaaaaaaaaaaaaaaaaaaaa neg r4.y, r1.y
ahaaaaaaabaaacacaeaaaaffacaaaaaaabaaaaffacaaaaaa max r1.y, r4.y, r1.y
ckaaaaaaaaaaabacafaaaaffabaaaaaaabaaaaffacaaaaaa slt r0.x, c5.y, r1.y
ckaaaaaaabaaacacafaaaaoeabaaaaaaabaaaappacaaaaaa slt r1.y, c5, r1.w
bfaaaaaaaeaaabacabaaaaaaacaaaaaaaaaaaaaaaaaaaaaa neg r4.x, r1.x
ahaaaaaaabaaabacaeaaaaaaacaaaaaaabaaaaaaacaaaaaa max r1.x, r4.x, r1.x
ckaaaaaaabaaaiacafaaaaffabaaaaaaabaaaaaaacaaaaaa slt r1.w, c5.y, r1.x
bfaaaaaaaeaaacacabaaaaffacaaaaaaaaaaaaaaaaaaaaaa neg r4.y, r1.y
abaaaaaaacaaabacaeaaaaffacaaaaaaagaaaaoeabaaaaaa add r2.x, r4.y, c6
abaaaaaaabaaabacaaaaaaffaaaaaaaaafaaaappabaaaaaa add r1.x, a0.y, c5.w
adaaaaaaacaaabacaaaaaappacaaaaaaacaaaaaaacaaaaaa mul r2.x, r0.w, r2.x
adaaaaaaaeaaabacabaaaaffacaaaaaaabaaaaaaacaaaaaa mul r4.x, r1.y, r1.x
abaaaaaaacaaabacaeaaaaaaacaaaaaaacaaaaaaacaaaaaa add r2.x, r4.x, r2.x
bfaaaaaaaeaaaiacabaaaappacaaaaaaaaaaaaaaaaaaaaaa neg r4.w, r1.w
abaaaaaaabaaacacaeaaaappacaaaaaaagaaaaaaabaaaaaa add r1.y, r4.w, c6.x
adaaaaaaacaaabacabaaaaffacaaaaaaacaaaaaaacaaaaaa mul r2.x, r1.y, r2.x
bfaaaaaaaeaaabacaaaaaaaaacaaaaaaaaaaaaaaaaaaaaaa neg r4.x, r0.x
abaaaaaaabaaacacaeaaaaaaacaaaaaaagaaaaaaabaaaaaa add r1.y, r4.x, c6.x
adaaaaaaabaaaeacabaaaaffacaaaaaaabaaaakkacaaaaaa mul r1.z, r1.y, r1.z
adaaaaaaaeaaabacaaaaaaaaacaaaaaaaaaaaappacaaaaaa mul r4.x, r0.x, r0.w
abaaaaaaaaaaabaeaeaaaaaaacaaaaaaabaaaakkacaaaaaa add v0.x, r4.x, r1.z
adaaaaaaabaaaiacabaaaappacaaaaaaabaaaaaaacaaaaaa mul r1.w, r1.w, r1.x
abaaaaaaabaaaiacabaaaappacaaaaaaacaaaaaaacaaaaaa add r1.w, r1.w, r2.x
adaaaaaaaaaaaiacaaaaaakkacaaaaaaabaaaappacaaaaaa mul r0.w, r0.z, r1.w
adaaaaaaaeaaaiacaaaaaaffacaaaaaaabaaaaaaacaaaaaa mul r4.w, r0.y, r1.x
abaaaaaaaaaaaiacaeaaaappacaaaaaaaaaaaappacaaaaaa add r0.w, r4.w, r0.w
adaaaaaaaaaaaeacadaaaaaaaaaaaaaaaiaaaaaaabaaaaaa mul r0.z, a3.x, c8.x
adaaaaaaabaaacacabaaaaffacaaaaaaaaaaaappacaaaaaa mul r1.y, r1.y, r0.w
adaaaaaaaaaaacacaaaaaakkacaaaaaaahaaaakkabaaaaaa mul r0.y, r0.z, c7.z
aiaaaaaaaaaaaiacaaaaaaffacaaaaaaaaaaaaaaaaaaaaaa frc r0.w, r0.y
adaaaaaaaeaaacacaaaaaaaaacaaaaaaabaaaaaaacaaaaaa mul r4.y, r0.x, r1.x
abaaaaaaaaaaacaeaeaaaaffacaaaaaaabaaaaffacaaaaaa add v0.y, r4.y, r1.y
acaaaaaaaaaaabacaaaaaaffacaaaaaaaaaaaappacaaaaaa sub r0.x, r0.y, r0.w
aaaaaaaaaaaaacacaaaaaaaaacaaaaaaaaaaaaaaaaaaaaaa mov r0.y, r0.x
bfaaaaaaaeaaabacaaaaaaaaacaaaaaaaaaaaaaaaaaaaaaa neg r4.x, r0.x
adaaaaaaaeaaabacaeaaaaaaacaaaaaaaiaaaaffabaaaaaa mul r4.x, r4.x, c8.y
abaaaaaaaaaaabacaeaaaaaaacaaaaaaaaaaaakkacaaaaaa add r0.x, r4.x, r0.z
abaaaaaaabaaapaeafaaaaoeaaaaaaaaafaaaappabaaaaaa add v1, a5, c5.w
adaaaaaaahaaadaeaaaaaafeacaaaaaaahaaaakkabaaaaaa mul v7.xy, r0.xyyy, c7.z
bdaaaaaaaaaaaiadaaaaaaoeaaaaaaaaadaaaaoeabaaaaaa dp4 o0.w, a0, c3
bdaaaaaaaaaaaeadaaaaaaoeaaaaaaaaacaaaaoeabaaaaaa dp4 o0.z, a0, c2
bdaaaaaaaaaaacadaaaaaaoeaaaaaaaaabaaaaoeabaaaaaa dp4 o0.y, a0, c1
bdaaaaaaaaaaabadaaaaaaoeaaaaaaaaaaaaaaoeabaaaaaa dp4 o0.x, a0, c0
aaaaaaaaaaaaamaeaaaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov v0.zw, c0
aaaaaaaaahaaaiaeaaaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov v7.w, c0
"
}

SubProgram "gles3 " {
Keywords { }
"!!GLES3#version 300 es


#ifdef VERTEX

#define gl_Vertex _glesVertex
in vec4 _glesVertex;
#define gl_Color _glesColor
in vec4 _glesColor;
#define gl_Normal (normalize(_glesNormal))
in vec3 _glesNormal;
#define gl_MultiTexCoord0 _glesMultiTexCoord0
in vec4 _glesMultiTexCoord0;
#define TANGENT vec4(normalize(_glesTANGENT.xyz), _glesTANGENT.w)
in vec4 _glesTANGENT;

#line 150
struct v2f_vertex_lit {
    highp vec2 uv;
    lowp vec4 diff;
    lowp vec4 spec;
};
#line 186
struct v2f_img {
    highp vec4 pos;
    mediump vec2 uv;
};
#line 180
struct appdata_img {
    highp vec4 vertex;
    mediump vec2 texcoord;
};
#line 318
struct v2f {
    highp vec4 pos;
    highp vec4 color;
    mediump vec2 uv;
    highp vec4 overhangLightLevel;
};
#line 309
struct appdata {
    highp vec4 vertex;
    highp vec3 normal;
    mediump vec2 texcoord;
    highp vec4 color32;
    highp vec4 tangent;
};
uniform highp vec4 _Time;
uniform highp vec4 _SinTime;
#line 3
uniform highp vec4 _CosTime;
uniform highp vec4 unity_DeltaTime;
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _ProjectionParams;
#line 7
uniform highp vec4 _ScreenParams;
uniform highp vec4 _ZBufferParams;
uniform highp vec4 unity_CameraWorldClipPlanes[6];
uniform highp vec4 _WorldSpaceLightPos0;
#line 11
uniform highp vec4 _LightPositionRange;
uniform highp vec4 unity_4LightPosX0;
uniform highp vec4 unity_4LightPosY0;
uniform highp vec4 unity_4LightPosZ0;
#line 15
uniform highp vec4 unity_4LightAtten0;
uniform highp vec4 unity_LightColor[4];
uniform highp vec4 unity_LightPosition[4];
uniform highp vec4 unity_LightAtten[4];
#line 19
uniform highp vec4 unity_SHAr;
uniform highp vec4 unity_SHAg;
uniform highp vec4 unity_SHAb;
uniform highp vec4 unity_SHBr;
#line 23
uniform highp vec4 unity_SHBg;
uniform highp vec4 unity_SHBb;
uniform highp vec4 unity_SHC;
uniform highp vec3 unity_LightColor0;
uniform highp vec3 unity_LightColor1;
uniform highp vec3 unity_LightColor2;
uniform highp vec3 unity_LightColor3;
#line 27
uniform highp vec4 unity_ShadowSplitSpheres[4];
uniform highp vec4 unity_ShadowSplitSqRadii;
uniform highp vec4 unity_LightShadowBias;
uniform highp vec4 _LightSplitsNear;
#line 31
uniform highp vec4 _LightSplitsFar;
uniform highp mat4 unity_World2Shadow[4];
uniform highp vec4 _LightShadowData;
uniform highp vec4 unity_ShadowFadeCenterAndType;
#line 35
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp mat4 glstate_matrix_invtrans_modelview0;
uniform highp mat4 _Object2World;
#line 39
uniform highp mat4 _World2Object;
uniform highp vec4 unity_Scale;
uniform highp mat4 glstate_matrix_transpose_modelview0;
uniform highp mat4 glstate_matrix_texture0;
#line 43
uniform highp mat4 glstate_matrix_texture1;
uniform highp mat4 glstate_matrix_texture2;
uniform highp mat4 glstate_matrix_texture3;
uniform highp mat4 glstate_matrix_projection;
#line 47
uniform highp vec4 glstate_lightmodel_ambient;
uniform highp mat4 unity_MatrixV;
uniform highp mat4 unity_MatrixVP;
uniform lowp vec4 unity_ColorSpaceGrey;
#line 76
#line 81
#line 86
#line 90
#line 95
#line 119
#line 136
#line 157
#line 165
#line 192
#line 205
#line 214
#line 219
#line 228
#line 233
#line 242
#line 259
#line 264
#line 290
#line 298
#line 302
#line 306
uniform sampler2D _BlockTex;
uniform lowp float _GameClock;
uniform highp vec3 _PlayerLoc;
#line 326
#line 342
#line 392
#line 342
v2f vert( in appdata v ) {
    v2f o;
    o.pos = (glstate_matrix_mvp * v.vertex);
    #line 346
    highp float vx = (v.vertex.x + 0.5);
    highp float vy = (v.vertex.z + 0.5);
    mediump float light_level = 0.5;
    lowp float day_level = ((_GameClock - 0.34) / 0.66);
    #line 350
    lowp float neg_shadow_nudge = (day_level * 0.5);
    lowp float pos_shadow_nudge = ((1.0 - day_level) * 0.5);
    if ((day_level < 0.0)){
        #line 354
        light_level += (day_level / 0.3);
    }
    if ((v.normal.z > 0.2)){
        #line 358
        vy = (v.vertex.y + 0.5);
        light_level += pos_shadow_nudge;
    }
    else{
        if ((v.normal.z < -0.2)){
            #line 363
            vy = (v.vertex.y + 0.5);
            light_level += neg_shadow_nudge;
        }
        else{
            if ((v.normal.x > 0.2)){
                #line 368
                vx = (v.vertex.z + 0.5);
                vy = (v.vertex.y + 0.5);
                light_level += pos_shadow_nudge;
            }
            else{
                if ((v.normal.x < -0.2)){
                    #line 374
                    vx = (v.vertex.z + 0.5);
                    vy = (v.vertex.y + 0.5);
                    light_level += neg_shadow_nudge;
                }
                else{
                    #line 380
                    light_level = (0.75 + (0.25 * day_level));
                    if ((day_level < 0.0)){
                        light_level += (0.45 * day_level);
                    }
                }
            }
        }
    }
    highp float index = (v.texcoord.x * 16.0);
    #line 384
    highp float blocky = floor((index / 4.0));
    mediump vec2 tile_o = (vec2( (index - (blocky * 4.0)), blocky) * 0.25);
    o.color.xy = tile_o;
    o.color.z = light_level;
    #line 388
    o.uv.xy = vec2( vx, vy);
    o.overhangLightLevel = ((v.tangent * 1.0) + vec4( 0.5));
    return o;
}

out highp vec4 xlv_COLOR;
out mediump vec2 xlv_TEXCOORD0;
out highp vec4 xlv_;
void main() {
    v2f xl_retval;
    appdata xlt_v;
    xlt_v.vertex = vec4(gl_Vertex);
    xlt_v.normal = vec3(gl_Normal);
    xlt_v.texcoord = vec2(gl_MultiTexCoord0);
    xlt_v.color32 = vec4(gl_Color);
    xlt_v.tangent = vec4(TANGENT);
    xl_retval = vert( xlt_v);
    gl_Position = vec4(xl_retval.pos);
    xlv_COLOR = vec4(xl_retval.color);
    xlv_TEXCOORD0 = vec2(xl_retval.uv);
    xlv_ = vec4(xl_retval.overhangLightLevel);
}


#endif
#ifdef FRAGMENT

#define gl_FragData _glesFragData
layout(location = 0) out mediump vec4 _glesFragData[4];
float xll_mod_f_f( float x, float y ) {
  float d = x / y;
  float f = fract (abs(d)) * y;
  return d >= 0.0 ? f : -f;
}
vec2 xll_mod_vf2_vf2( vec2 x, vec2 y ) {
  vec2 d = x / y;
  vec2 f = fract (abs(d)) * y;
  return vec2 (d.x >= 0.0 ? f.x : -f.x, d.y >= 0.0 ? f.y : -f.y);
}
vec3 xll_mod_vf3_vf3( vec3 x, vec3 y ) {
  vec3 d = x / y;
  vec3 f = fract (abs(d)) * y;
  return vec3 (d.x >= 0.0 ? f.x : -f.x, d.y >= 0.0 ? f.y : -f.y, d.z >= 0.0 ? f.z : -f.z);
}
vec4 xll_mod_vf4_vf4( vec4 x, vec4 y ) {
  vec4 d = x / y;
  vec4 f = fract (abs(d)) * y;
  return vec4 (d.x >= 0.0 ? f.x : -f.x, d.y >= 0.0 ? f.y : -f.y, d.z >= 0.0 ? f.z : -f.z, d.w >= 0.0 ? f.w : -f.w);
}
#line 150
struct v2f_vertex_lit {
    highp vec2 uv;
    lowp vec4 diff;
    lowp vec4 spec;
};
#line 186
struct v2f_img {
    highp vec4 pos;
    mediump vec2 uv;
};
#line 180
struct appdata_img {
    highp vec4 vertex;
    mediump vec2 texcoord;
};
#line 318
struct v2f {
    highp vec4 pos;
    highp vec4 color;
    mediump vec2 uv;
    highp vec4 overhangLightLevel;
};
#line 309
struct appdata {
    highp vec4 vertex;
    highp vec3 normal;
    mediump vec2 texcoord;
    highp vec4 color32;
    highp vec4 tangent;
};
uniform highp vec4 _Time;
uniform highp vec4 _SinTime;
#line 3
uniform highp vec4 _CosTime;
uniform highp vec4 unity_DeltaTime;
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _ProjectionParams;
#line 7
uniform highp vec4 _ScreenParams;
uniform highp vec4 _ZBufferParams;
uniform highp vec4 unity_CameraWorldClipPlanes[6];
uniform highp vec4 _WorldSpaceLightPos0;
#line 11
uniform highp vec4 _LightPositionRange;
uniform highp vec4 unity_4LightPosX0;
uniform highp vec4 unity_4LightPosY0;
uniform highp vec4 unity_4LightPosZ0;
#line 15
uniform highp vec4 unity_4LightAtten0;
uniform highp vec4 unity_LightColor[4];
uniform highp vec4 unity_LightPosition[4];
uniform highp vec4 unity_LightAtten[4];
#line 19
uniform highp vec4 unity_SHAr;
uniform highp vec4 unity_SHAg;
uniform highp vec4 unity_SHAb;
uniform highp vec4 unity_SHBr;
#line 23
uniform highp vec4 unity_SHBg;
uniform highp vec4 unity_SHBb;
uniform highp vec4 unity_SHC;
uniform highp vec3 unity_LightColor0;
uniform highp vec3 unity_LightColor1;
uniform highp vec3 unity_LightColor2;
uniform highp vec3 unity_LightColor3;
#line 27
uniform highp vec4 unity_ShadowSplitSpheres[4];
uniform highp vec4 unity_ShadowSplitSqRadii;
uniform highp vec4 unity_LightShadowBias;
uniform highp vec4 _LightSplitsNear;
#line 31
uniform highp vec4 _LightSplitsFar;
uniform highp mat4 unity_World2Shadow[4];
uniform highp vec4 _LightShadowData;
uniform highp vec4 unity_ShadowFadeCenterAndType;
#line 35
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp mat4 glstate_matrix_invtrans_modelview0;
uniform highp mat4 _Object2World;
#line 39
uniform highp mat4 _World2Object;
uniform highp vec4 unity_Scale;
uniform highp mat4 glstate_matrix_transpose_modelview0;
uniform highp mat4 glstate_matrix_texture0;
#line 43
uniform highp mat4 glstate_matrix_texture1;
uniform highp mat4 glstate_matrix_texture2;
uniform highp mat4 glstate_matrix_texture3;
uniform highp mat4 glstate_matrix_projection;
#line 47
uniform highp vec4 glstate_lightmodel_ambient;
uniform highp mat4 unity_MatrixV;
uniform highp mat4 unity_MatrixVP;
uniform lowp vec4 unity_ColorSpaceGrey;
#line 76
#line 81
#line 86
#line 90
#line 95
#line 119
#line 136
#line 157
#line 165
#line 192
#line 205
#line 214
#line 219
#line 228
#line 233
#line 242
#line 259
#line 264
#line 290
#line 298
#line 302
#line 306
uniform sampler2D _BlockTex;
uniform lowp float _GameClock;
uniform highp vec3 _PlayerLoc;
#line 326
#line 342
#line 392
#line 326
highp float getIndex( in highp vec4 overhang, in highp float xco ) {
    if ((xco < 1.0)){
        #line 330
        return overhang.x;
    }
    if ((xco < 2.0)){
        #line 334
        return overhang.y;
    }
    if ((xco < 3.0)){
        #line 338
        return overhang.z;
    }
    return overhang.w;
}
#line 392
lowp vec4 frag( in v2f i ) {
    mediump vec2 scaled_uv = ((i.uv.xy - floor(i.uv.xy)) * 0.25);
    mediump vec2 offset = i.color.xy;
    #line 396
    scaled_uv = (scaled_uv + offset);
    mediump vec2 model_rel_twoD = xll_mod_vf2_vf2( floor(i.uv.xy), vec2( 4.0));
    highp float index = getIndex( i.overhangLightLevel, model_rel_twoD.x);
    highp float power_lookup = floor((index / pow( 8.0, model_rel_twoD.y)));
    #line 400
    mediump float light_one = xll_mod_f_f( power_lookup, 8.0);
    lowp float local_light = ((light_one + 2.0) / 9.0);
    return ((texture( _BlockTex, scaled_uv) * i.color.z) * local_light);
}
in highp vec4 xlv_COLOR;
in mediump vec2 xlv_TEXCOORD0;
in highp vec4 xlv_;
void main() {
    lowp vec4 xl_retval;
    v2f xlt_i;
    xlt_i.pos = vec4(0.0);
    xlt_i.color = vec4(xlv_COLOR);
    xlt_i.uv = vec2(xlv_TEXCOORD0);
    xlt_i.overhangLightLevel = vec4(xlv_);
    xl_retval = frag( xlt_i);
    gl_FragData[0] = vec4(xl_retval);
}


#endif"
}

}
Program "fp" {
// Fragment combos: 1
//   opengl - ALU: 36 to 36, TEX: 1 to 1
//   d3d9 - ALU: 42 to 42, TEX: 1 to 1
SubProgram "opengl " {
Keywords { }
SetTexture 0 [_BlockTex] 2D
"!!ARBfp1.0
# 36 ALU, 1 TEX
PARAM c[3] = { { 0.25, 0.125, 0.11111111, 4 },
		{ 1, 0, 2, 3 },
		{ 8 } };
TEMP R0;
TEMP R1;
TEMP R2;
FLR R1.xy, fragment.texcoord[0];
ADD R0.xy, fragment.texcoord[0], -R1;
MUL R1.zw, R1.xyxy, c[0].x;
MAD R0.xy, R0, c[0].x, fragment.color.primary;
ABS R1.zw, R1;
FRC R1.zw, R1;
MUL R1.zw, R1, c[0].w;
CMP R1.xy, R1, -R1.zwzw, R1.zwzw;
ADD R2.y, R1.x, -c[1].x;
SLT R1.z, R1.x, c[1];
CMP R2.z, R2.y, c[1].y, c[1].x;
MUL R1.w, R2.z, R1.z;
CMP R1.z, -R1.w, c[1].y, R2;
CMP R2.x, R2.y, fragment.texcoord[1], R2;
POW R1.y, c[2].x, R1.y;
MUL R2.z, R2, R1;
SLT R1.x, R1, c[1].w;
MUL R1.x, R2.z, R1;
CMP R1.z, -R1.x, c[1].y, R1;
CMP R1.w, -R1, fragment.texcoord[1].y, R2.x;
RCP R1.y, R1.y;
MUL R1.z, R2, R1;
CMP R1.x, -R1, fragment.texcoord[1].z, R1.w;
CMP R1.x, -R1.z, fragment.texcoord[1].w, R1;
MUL R1.x, R1, R1.y;
FLR R1.x, R1;
MUL R1.y, R1.x, c[0];
ABS R1.y, R1;
FRC R1.y, R1;
MUL R1.y, R1, c[2].x;
CMP R1.x, R1, -R1.y, R1.y;
ADD R1.x, R1, c[1].z;
MUL R1.x, R1, c[0].z;
TEX R0, R0, texture[0], 2D;
MUL R0, R0, fragment.color.primary.z;
MUL result.color, R0, R1.x;
END
# 36 instructions, 3 R-regs
"
}

SubProgram "d3d9 " {
Keywords { }
SetTexture 0 [_BlockTex] 2D
"ps_2_0
; 42 ALU, 1 TEX
dcl_2d s0
def c0, 0.25000000, 4.00000000, -1.00000000, -2.00000000
def c1, 1.00000000, 0.00000000, -3.00000000, 8.00000000
def c2, 0.12500000, 2.00000000, 0.11111111, 0
dcl v0.xyz
dcl t0.xy
dcl t1
frc_pp r1.xy, t0
mad_pp r2.xy, r1, c0.x, v0
add_pp r1.xy, t0, -r1
mul_pp r3.xy, r1, c0.x
abs_pp r3.xy, r3
frc_pp r3.xy, r3
mul_pp r3.xy, r3, c0.y
cmp_pp r1.xy, r1, r3, -r3
add r4.x, r1, c0.z
add r3.x, r1, c0.w
add r1.x, r1, c1.z
cmp_pp r5.x, r4, c1, c1.y
cmp r3.x, r3, c1.y, c1
mul_pp r3.x, r5, r3
cmp_pp r6.x, -r3, r5, c1.y
cmp r0.x, r4, r0, t1
mul_pp r5.x, r5, r6
cmp r1.x, r1, c1.y, c1
mul_pp r1.x, r5, r1
cmp_pp r6.x, -r1, r6, c1.y
mul_pp r5.x, r5, r6
pow_pp r6.x, c1.w, r1.y
cmp r0.x, -r3, r0, t1.y
cmp r0.x, -r1, r0, t1.z
mov_pp r3.x, r6.x
rcp r1.x, r3.x
cmp r0.x, -r5, r0, t1.w
mul r0.x, r0, r1
frc r1.x, r0
add r0.x, r0, -r1
mul r1.x, r0, c2
abs r1.x, r1
frc r1.x, r1
mul r1.x, r1, c1.w
cmp r0.x, r0, r1, -r1
add_pp r0.x, r0, c2.y
mul_pp r0.x, r0, c2.z
texld r2, r2, s0
mul r1, r2, v0.z
mul r0, r1, r0.x
mov_pp oC0, r0
"
}

SubProgram "gles " {
Keywords { }
"!!GLES"
}

SubProgram "glesdesktop " {
Keywords { }
"!!GLES"
}

SubProgram "flash " {
Keywords { }
SetTexture 0 [_BlockTex] 2D
"agal_ps
c0 0.25 4.0 -1.0 -2.0
c1 1.0 0.0 -3.0 8.0
c2 0.125 2.0 0.111111 0.0
[bc]
aiaaaaaaabaaadacaaaaaaoeaeaaaaaaaaaaaaaaaaaaaaaa frc r1.xy, v0
adaaaaaaacaaadacabaaaafeacaaaaaaaaaaaaaaabaaaaaa mul r2.xy, r1.xyyy, c0.x
abaaaaaaacaaadacacaaaafeacaaaaaaahaaaaoeaeaaaaaa add r2.xy, r2.xyyy, v7
acaaaaaaabaaadacaaaaaaoeaeaaaaaaabaaaafeacaaaaaa sub r1.xy, v0, r1.xyyy
adaaaaaaadaaadacabaaaafeacaaaaaaaaaaaaaaabaaaaaa mul r3.xy, r1.xyyy, c0.x
beaaaaaaadaaadacadaaaafeacaaaaaaaaaaaaaaaaaaaaaa abs r3.xy, r3.xyyy
aiaaaaaaadaaadacadaaaafeacaaaaaaaaaaaaaaaaaaaaaa frc r3.xy, r3.xyyy
adaaaaaaadaaadacadaaaafeacaaaaaaaaaaaaffabaaaaaa mul r3.xy, r3.xyyy, c0.y
bfaaaaaaaaaaadacadaaaafeacaaaaaaaaaaaaaaaaaaaaaa neg r0.xy, r3.xyyy
ckaaaaaaadaaamacabaaaafeacaaaaaaacaaaappabaaaaaa slt r3.zw, r1.xyyy, c2.w
acaaaaaaaeaaadacaaaaaafeacaaaaaaadaaaafeacaaaaaa sub r4.xy, r0.xyyy, r3.xyyy
adaaaaaaabaaadacaeaaaafeacaaaaaaadaaaapoacaaaaaa mul r1.xy, r4.xyyy, r3.zwww
abaaaaaaabaaadacabaaaafeacaaaaaaadaaaafeacaaaaaa add r1.xy, r1.xyyy, r3.xyyy
abaaaaaaaeaaabacabaaaaaaacaaaaaaaaaaaakkabaaaaaa add r4.x, r1.x, c0.z
abaaaaaaadaaabacabaaaaaaacaaaaaaaaaaaappabaaaaaa add r3.x, r1.x, c0.w
abaaaaaaabaaabacabaaaaaaacaaaaaaabaaaakkabaaaaaa add r1.x, r1.x, c1.z
cjaaaaaaaeaaaeacaeaaaaaaacaaaaaaacaaaappabaaaaaa sge r4.z, r4.x, c2.w
adaaaaaaafaaabacaaaaaakkabaaaaaaaeaaaakkacaaaaaa mul r5.x, c0.z, r4.z
abaaaaaaafaaabacafaaaaaaacaaaaaaabaaaaoeabaaaaaa add r5.x, r5.x, c1
ckaaaaaaadaaabacadaaaaaaacaaaaaaacaaaappabaaaaaa slt r3.x, r3.x, c2.w
adaaaaaaadaaabacafaaaaaaacaaaaaaadaaaaaaacaaaaaa mul r3.x, r5.x, r3.x
bfaaaaaaafaaacacadaaaaaaacaaaaaaaaaaaaaaaaaaaaaa neg r5.y, r3.x
ckaaaaaaafaaacacafaaaaffacaaaaaaacaaaappabaaaaaa slt r5.y, r5.y, c2.w
acaaaaaaagaaabacabaaaaffabaaaaaaafaaaaaaacaaaaaa sub r6.x, c1.y, r5.x
adaaaaaaagaaabacagaaaaaaacaaaaaaafaaaaffacaaaaaa mul r6.x, r6.x, r5.y
abaaaaaaagaaabacagaaaaaaacaaaaaaafaaaaaaacaaaaaa add r6.x, r6.x, r5.x
ckaaaaaaaeaaabacaeaaaaaaacaaaaaaacaaaappabaaaaaa slt r4.x, r4.x, c2.w
acaaaaaaagaaacacabaaaaoeaeaaaaaaaaaaaaaaacaaaaaa sub r6.y, v1, r0.x
adaaaaaaagaaacacagaaaaffacaaaaaaaeaaaaaaacaaaaaa mul r6.y, r6.y, r4.x
abaaaaaaaaaaabacagaaaaffacaaaaaaaaaaaaaaacaaaaaa add r0.x, r6.y, r0.x
adaaaaaaafaaabacafaaaaaaacaaaaaaagaaaaaaacaaaaaa mul r5.x, r5.x, r6.x
ckaaaaaaabaaabacabaaaaaaacaaaaaaacaaaappabaaaaaa slt r1.x, r1.x, c2.w
adaaaaaaabaaabacafaaaaaaacaaaaaaabaaaaaaacaaaaaa mul r1.x, r5.x, r1.x
bfaaaaaaaeaaabacabaaaaaaacaaaaaaaaaaaaaaaaaaaaaa neg r4.x, r1.x
ckaaaaaaaeaaabacaeaaaaaaacaaaaaaacaaaappabaaaaaa slt r4.x, r4.x, c2.w
acaaaaaaahaaacacabaaaaffabaaaaaaagaaaaaaacaaaaaa sub r7.y, c1.y, r6.x
adaaaaaaahaaacacahaaaaffacaaaaaaaeaaaaaaacaaaaaa mul r7.y, r7.y, r4.x
abaaaaaaagaaabacahaaaaffacaaaaaaagaaaaaaacaaaaaa add r6.x, r7.y, r6.x
adaaaaaaafaaabacafaaaaaaacaaaaaaagaaaaaaacaaaaaa mul r5.x, r5.x, r6.x
alaaaaaaagaaapacabaaaappabaaaaaaabaaaaffacaaaaaa pow r6, c1.w, r1.y
bfaaaaaaahaaacacadaaaaaaacaaaaaaaaaaaaaaaaaaaaaa neg r7.y, r3.x
ckaaaaaaahaaacacahaaaaffacaaaaaaacaaaappabaaaaaa slt r7.y, r7.y, c2.w
acaaaaaaahaaabacabaaaaffaeaaaaaaaaaaaaaaacaaaaaa sub r7.x, v1.y, r0.x
adaaaaaaahaaabacahaaaaaaacaaaaaaahaaaaffacaaaaaa mul r7.x, r7.x, r7.y
abaaaaaaaaaaabacahaaaaaaacaaaaaaaaaaaaaaacaaaaaa add r0.x, r7.x, r0.x
bfaaaaaaahaaabacabaaaaaaacaaaaaaaaaaaaaaaaaaaaaa neg r7.x, r1.x
ckaaaaaaahaaabacahaaaaaaacaaaaaaacaaaappabaaaaaa slt r7.x, r7.x, c2.w
acaaaaaaaeaaabacabaaaakkaeaaaaaaaaaaaaaaacaaaaaa sub r4.x, v1.z, r0.x
adaaaaaaaeaaabacaeaaaaaaacaaaaaaahaaaaaaacaaaaaa mul r4.x, r4.x, r7.x
abaaaaaaaaaaabacaeaaaaaaacaaaaaaaaaaaaaaacaaaaaa add r0.x, r4.x, r0.x
aaaaaaaaadaaabacagaaaaaaacaaaaaaaaaaaaaaaaaaaaaa mov r3.x, r6.x
afaaaaaaabaaabacadaaaaaaacaaaaaaaaaaaaaaaaaaaaaa rcp r1.x, r3.x
bfaaaaaaahaaabacafaaaaaaacaaaaaaaaaaaaaaaaaaaaaa neg r7.x, r5.x
ckaaaaaaahaaabacahaaaaaaacaaaaaaacaaaappabaaaaaa slt r7.x, r7.x, c2.w
acaaaaaaadaaabacabaaaappaeaaaaaaaaaaaaaaacaaaaaa sub r3.x, v1.w, r0.x
adaaaaaaadaaabacadaaaaaaacaaaaaaahaaaaaaacaaaaaa mul r3.x, r3.x, r7.x
abaaaaaaaaaaabacadaaaaaaacaaaaaaaaaaaaaaacaaaaaa add r0.x, r3.x, r0.x
adaaaaaaaaaaabacaaaaaaaaacaaaaaaabaaaaaaacaaaaaa mul r0.x, r0.x, r1.x
aiaaaaaaabaaabacaaaaaaaaacaaaaaaaaaaaaaaaaaaaaaa frc r1.x, r0.x
acaaaaaaaaaaabacaaaaaaaaacaaaaaaabaaaaaaacaaaaaa sub r0.x, r0.x, r1.x
adaaaaaaabaaabacaaaaaaaaacaaaaaaacaaaaoeabaaaaaa mul r1.x, r0.x, c2
beaaaaaaabaaabacabaaaaaaacaaaaaaaaaaaaaaaaaaaaaa abs r1.x, r1.x
aiaaaaaaabaaabacabaaaaaaacaaaaaaaaaaaaaaaaaaaaaa frc r1.x, r1.x
adaaaaaaabaaabacabaaaaaaacaaaaaaabaaaappabaaaaaa mul r1.x, r1.x, c1.w
bfaaaaaaahaaabacabaaaaaaacaaaaaaaaaaaaaaaaaaaaaa neg r7.x, r1.x
ckaaaaaaadaaabacaaaaaaaaacaaaaaaacaaaappabaaaaaa slt r3.x, r0.x, c2.w
acaaaaaaahaaabacahaaaaaaacaaaaaaabaaaaaaacaaaaaa sub r7.x, r7.x, r1.x
adaaaaaaaaaaabacahaaaaaaacaaaaaaadaaaaaaacaaaaaa mul r0.x, r7.x, r3.x
abaaaaaaaaaaabacaaaaaaaaacaaaaaaabaaaaaaacaaaaaa add r0.x, r0.x, r1.x
abaaaaaaaaaaabacaaaaaaaaacaaaaaaacaaaaffabaaaaaa add r0.x, r0.x, c2.y
adaaaaaaaaaaabacaaaaaaaaacaaaaaaacaaaakkabaaaaaa mul r0.x, r0.x, c2.z
ciaaaaaaacaaapacacaaaafeacaaaaaaaaaaaaaaafaababb tex r2, r2.xyyy, s0 <2d wrap linear point>
adaaaaaaabaaapacacaaaaoeacaaaaaaahaaaakkaeaaaaaa mul r1, r2, v7.z
adaaaaaaaaaaapacabaaaaoeacaaaaaaaaaaaaaaacaaaaaa mul r0, r1, r0.x
aaaaaaaaaaaaapadaaaaaaoeacaaaaaaaaaaaaaaaaaaaaaa mov o0, r0
"
}

SubProgram "gles3 " {
Keywords { }
"!!GLES3"
}

}

#LINE 323

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
