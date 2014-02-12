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
//   opengl - ALU: 79 to 79
//   d3d9 - ALU: 124 to 124
SubProgram "opengl " {
Keywords { }
Bind "vertex" Vertex
Bind "normal" Normal
Bind "texcoord" TexCoord0
Bind "color" Color
Float 5 [_GameClock]
"!!ARBvp1.0
# 79 ALU
PARAM c[9] = { { 0.34008789, 1.5147929, 0, 0.2 },
		state.matrix.mvp,
		program.local[5],
		{ -0.2, 0.44995117, 0.25, 0.5 },
		{ 1, 3.3327909, 0.75, 16 },
		{ 4, 255 } };
TEMP R0;
TEMP R1;
TEMP R2;
TEMP R3;
ADD R1.x, vertex.position, c[6].w;
SLT R0.x, c[0].w, vertex.normal.z;
ABS R0.y, R0.x;
ADD R2.x, vertex.position.z, c[6].w;
ADD R2.y, vertex.position, -R2.x;
ADD R2.y, R2, c[6].w;
MAD R2.y, R0.x, R2, R2.x;
ADD R2.x, vertex.position.y, -R2.y;
SGE R0.z, c[0], R0.y;
SLT R0.w, vertex.normal.z, c[6].x;
MUL R0.y, R0.z, R0.w;
ADD R2.z, R2.x, c[6].w;
MAD R2.y, R0, R2.z, R2;
ADD R1.y, -R1.x, -R1.x;
MAD R1.y, R0, R1, R1.x;
ADD R1.x, -vertex.position.z, -R1.y;
ADD R1.z, R1.x, -c[6].w;
ABS R0.w, R0;
SGE R0.w, c[0].z, R0;
MUL R0.w, R0.z, R0;
SLT R1.x, c[0].w, vertex.normal;
MUL R0.z, R0.w, R1.x;
MAD R1.z, R0, R1, R1.y;
ADD R1.y, vertex.position.z, -R1.z;
ABS R1.x, R1;
ADD R1.w, R1.y, c[6];
SGE R1.x, c[0].z, R1;
MUL R1.y, R0.w, R1.x;
SLT R1.x, vertex.normal, c[6];
MUL R0.w, R1.y, R1.x;
MAD R1.z, R0.w, R1.w, R1;
ABS R1.x, R1;
SGE R2.x, c[0].z, R1;
MUL R1.y, R1, R2.x;
SLT R1.x, vertex.normal.y, c[6];
ADD R1.w, -R1.z, -R1.z;
MUL R2.x, R1.y, R1;
MAD result.texcoord[0].x, R1.w, R2, R1.z;
ADD R1.w, vertex.position.y, -R2.y;
ADD R1.w, R1, c[6];
MAD R2.x, R0.z, R1.w, R2.y;
MOV R1.z, c[0].x;
ADD R1.z, -R1, c[5].x;
MUL R1.z, R1, c[0].y;
SLT R1.w, R1.z, c[0].z;
ADD R2.w, -R1.z, c[7].x;
ADD R2.y, vertex.position, -R2.x;
ADD R2.y, R2, c[6].w;
MUL R3.x, R1.z, R1.w;
MOV R2.z, c[6].w;
MAD R2.z, R3.x, c[7].y, R2;
MUL R2.w, R2, c[6];
MAD R0.x, R0, R2.w, R2.z;
MUL R2.z, R1, c[6].w;
MAD R0.x, R2.z, R0.y, R0;
MAD R0.y, R2.w, R0.z, R0.x;
MAD R0.z, R2, R0.w, R0.y;
ABS R0.x, R1;
SGE R0.x, c[0].z, R0;
MAD R0.y, R1.z, c[6].z, -R0.z;
MAD result.texcoord[0].y, R0.w, R2, R2.x;
MUL R0.x, R1.y, R0;
ADD R0.w, R0.y, c[7].z;
MAD R0.z, R0.x, R0.w, R0;
MUL R0.y, R0.x, R1.w;
MUL R0.x, R1.z, R0.y;
MUL R1, vertex.color, c[8].y;
MUL R0.w, vertex.texcoord[0].x, c[7];
MAD result.color.z, R0.x, c[6].y, R0;
MUL R0.x, R0.w, c[6].z;
FLR R0.x, R0;
MOV R0.y, R0.x;
MAD R0.x, -R0, c[8], R0.w;
ADD result.texcoord[1], R1, c[6].w;
MUL result.color.xy, R0, c[6].z;
DP4 result.position.w, vertex.position, c[4];
DP4 result.position.z, vertex.position, c[3];
DP4 result.position.y, vertex.position, c[2];
DP4 result.position.x, vertex.position, c[1];
END
# 79 instructions, 4 R-regs
"
}

SubProgram "d3d9 " {
Keywords { }
Bind "vertex" Vertex
Bind "normal" Normal
Bind "texcoord" TexCoord0
Bind "color" Color
Matrix 0 [glstate_matrix_mvp]
Float 4 [_GameClock]
"vs_2_0
; 124 ALU
def c5, 0.20000000, 0.00000000, -0.20000000, 0.50000000
def c6, 1.00000000, -0.34008789, 1.51479292, 0.44995117
def c7, 3.33279085, 0.50000000, 0.25000000, 0.75000000
def c8, 16.00000000, 4.00000000, 255.00000000, 0.50000000
dcl_position0 v0
dcl_normal0 v1
dcl_texcoord0 v2
dcl_color0 v3
mov r0.x, c4
add r0.x, c6.y, r0
mul r1.x, r0, c6.z
slt r1.w, r1.x, c5.y
max r0.x, -r1.w, r1.w
slt r0.x, c5.y, r0
add r0.y, -r0.x, c6.x
mul r0.z, r0.y, c5.w
mad r0.y, r1.x, c7.x, c7
mad r2.z, r0.x, r0.y, r0
slt r0.x, c5, v1.z
add r0.w, -r1.x, c6.x
mad r1.z, r0.w, c5.w, r2
sge r0.z, c5.y, r0.x
sge r0.y, r0.x, c5
mul r0.y, r0, r0.z
max r0.z, -r0.x, r0.x
slt r2.y, c5, r0.z
slt r1.y, v1.z, c5.z
mul r0.z, r0.y, r1.y
max r2.x, -r0.z, r0.z
add r2.w, -r2.y, c6.x
mul r2.z, r2, r2.w
mad r1.z, r2.y, r1, r2
slt r2.x, c5.y, r2
add r2.z, -r2.x, c6.x
max r0.z, -r0, r0
mad r2.y, r1.x, c5.w, r1.z
mul r2.z, r1, r2
mad r2.x, r2, r2.y, r2.z
sge r1.z, c5.y, r1.y
sge r1.y, r1, c5
mul r1.y, r1, r1.z
mul r1.y, r0, r1
slt r1.z, c5.x, v1.x
mul r0.y, r1, r1.z
max r2.z, -r0.y, r0.y
mad r2.y, r0.w, c5.w, r2.x
slt r0.w, c5.y, r2.z
sge r2.z, c5.y, r1
sge r1.z, r1, c5.y
mul r2.z, r1, r2
add r3.x, -r0.w, c6
mul r3.x, r2, r3
mad r0.w, r0, r2.y, r3.x
slt r1.z, v1.x, c5
mul r2.z, r1.y, r2
mul r1.y, r2.z, r1.z
max r2.w, -r1.y, r1.y
slt r2.x, c5.y, r2.w
sge r3.x, c5.y, r1.z
add r2.w, -r2.x, c6.x
sge r1.z, r1, c5.y
mul r1.z, r1, r3.x
max r0.y, -r0, r0
mad r2.y, r1.x, c5.w, r0.w
mul r2.w, r0, r2
slt r0.w, v1.y, c5.z
mul r1.z, r2, r1
mad r2.w, r2.x, r2.y, r2
slt r0.y, c5, r0
sge r3.y, c5, r0.w
sge r3.x, r0.w, c5.y
mul r3.x, r3, r3.y
mul r2.z, r1, r3.x
max r2.x, -r2.z, r2.z
slt r2.x, c5.y, r2
mul r1.w, r2.z, r1
add r2.y, -r2.x, c6.x
mul r2.z, r2.y, r2.w
mad r2.y, r1.x, c7.z, c7.w
mad r2.y, r2.x, r2, r2.z
max r1.w, -r1, r1
slt r1.w, c5.y, r1
add r2.z, -r1.w, c6.x
mad r2.x, r1, c6.w, r2.y
mul r1.x, r2.y, r2.z
max r2.z, -r1.y, r1.y
slt r0.z, c5.y, r0
mad oD0.z, r1.w, r2.x, r1.x
max r1.w, -r0.x, r0.x
mul r0.x, r1.z, r0.w
slt r1.z, c5.y, r1.w
max r0.x, -r0, r0
slt r0.w, c5.y, r0.x
add r0.x, v0.y, c5.w
add r2.y, -r0.z, c6.x
add r1.y, v0.x, c5.w
mul r2.w, r1.y, r2.y
mad r2.w, r0.z, -r1.y, r2
add r1.y, -r0, c6.x
slt r2.z, c5.y, r2
add r3.x, v0.z, c5.w
mul r2.w, r1.y, r2
mad r3.y, r0, -r3.x, r2.w
add r2.w, -r2.z, c6.x
add r1.w, -r1.z, c6.x
mul r3.y, r2.w, r3
mul r1.w, r3.x, r1
mad r1.w, r1.z, r0.x, r1
mul r1.w, r2.y, r1
mad r0.z, r0, r0.x, r1.w
mad r1.x, r2.z, r3, r3.y
add r1.z, -r0.w, c6.x
mul r1.z, r1.x, r1
mad oT0.x, r0.w, -r1, r1.z
mul r0.w, r1.y, r0.z
mad r0.w, r0.y, r0.x, r0
mul r0.z, v2.x, c8.x
mul r1.x, r2.w, r0.w
mul r0.y, r0.z, c7.z
frc r0.w, r0.y
mad oT0.y, r2.z, r0.x, r1.x
add r0.x, r0.y, -r0.w
mov r0.y, r0.x
mad r0.x, -r0, c8.y, r0.z
mad oT1, v3, c8.z, c8.w
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
attribute vec4 _glesMultiTexCoord0;
attribute vec3 _glesNormal;
attribute vec4 _glesColor;
attribute vec4 _glesVertex;
void main ()
{
  vec3 tmpvar_1;
  tmpvar_1 = normalize(_glesNormal);
  mediump vec2 tile_o_2;
  highp float index_3;
  mediump float light_level_4;
  highp float vy_5;
  highp float vx_6;
  highp vec4 tmpvar_7;
  mediump vec2 tmpvar_8;
  highp vec4 tmpvar_9;
  tmpvar_9 = (glstate_matrix_mvp * _glesVertex);
  highp float tmpvar_10;
  tmpvar_10 = (_glesVertex.x + 0.5);
  vx_6 = tmpvar_10;
  vy_5 = (_glesVertex.z + 0.5);
  light_level_4 = 0.5;
  lowp float tmpvar_11;
  tmpvar_11 = ((_GameClock - 0.34) / 0.66);
  lowp float tmpvar_12;
  tmpvar_12 = (tmpvar_11 * 0.5);
  lowp float tmpvar_13;
  tmpvar_13 = ((1.0 - tmpvar_11) * 0.5);
  if ((tmpvar_11 < 0.0)) {
    light_level_4 = (0.5 + (tmpvar_11 / 0.3));
  };
  if ((tmpvar_1.z > 0.2)) {
    vy_5 = (_glesVertex.y + 0.5);
    light_level_4 = (light_level_4 + tmpvar_13);
  } else {
    if ((tmpvar_1.z < -0.2)) {
      vy_5 = (_glesVertex.y + 0.5);
      vx_6 = (tmpvar_10 * -1.0);
      light_level_4 = (light_level_4 + tmpvar_12);
    } else {
      if ((tmpvar_1.x > 0.2)) {
        vx_6 = -((_glesVertex.z + 0.5));
        vy_5 = (_glesVertex.y + 0.5);
        light_level_4 = (light_level_4 + tmpvar_13);
      } else {
        if ((tmpvar_1.x < -0.2)) {
          vx_6 = (_glesVertex.z + 0.5);
          vy_5 = (_glesVertex.y + 0.5);
          light_level_4 = (light_level_4 + tmpvar_12);
        } else {
          if ((tmpvar_1.y < -0.2)) {
            vx_6 = (vx_6 * -1.0);
          } else {
            lowp float tmpvar_14;
            tmpvar_14 = (0.75 + (0.25 * tmpvar_11));
            light_level_4 = tmpvar_14;
            if ((tmpvar_11 < 0.0)) {
              light_level_4 = (light_level_4 + (0.45 * tmpvar_11));
            };
          };
        };
      };
    };
  };
  mediump float tmpvar_15;
  tmpvar_15 = (_glesMultiTexCoord0.x * 16.0);
  index_3 = tmpvar_15;
  highp float tmpvar_16;
  tmpvar_16 = floor((index_3 / 4.0));
  highp vec2 tmpvar_17;
  tmpvar_17.x = (index_3 - (tmpvar_16 * 4.0));
  tmpvar_17.y = tmpvar_16;
  highp vec2 tmpvar_18;
  tmpvar_18 = (tmpvar_17 * 0.25);
  tile_o_2 = tmpvar_18;
  tmpvar_7.xy = tile_o_2;
  tmpvar_7.z = light_level_4;
  highp vec2 tmpvar_19;
  tmpvar_19.x = vx_6;
  tmpvar_19.y = vy_5;
  tmpvar_8 = tmpvar_19;
  gl_Position = tmpvar_9;
  xlv_COLOR = tmpvar_7;
  xlv_TEXCOORD0 = tmpvar_8;
  xlv_ = ((_glesColor * 255.0) + vec4(0.5, 0.5, 0.5, 0.5));
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
  highp float facemodz_4;
  mediump vec2 offset_5;
  mediump vec2 scaled_uv_6;
  highp vec2 tmpvar_7;
  tmpvar_7 = xlv_COLOR.xy;
  offset_5 = tmpvar_7;
  scaled_uv_6 = ((fract(xlv_TEXCOORD0) * 0.25) + offset_5);
  mediump vec2 tmpvar_8;
  tmpvar_8 = floor(abs(xlv_TEXCOORD0));
  highp float xco_9;
  xco_9 = tmpvar_8.x;
  highp float tmpvar_10;
  highp float tmpvar_11;
  tmpvar_11 = (xco_9 / 4.0);
  highp float tmpvar_12;
  tmpvar_12 = (fract(abs(tmpvar_11)) * 4.0);
  highp float tmpvar_13;
  if ((tmpvar_11 >= 0.0)) {
    tmpvar_13 = tmpvar_12;
  } else {
    tmpvar_13 = -(tmpvar_12);
  };
  xco_9 = tmpvar_13;
  if ((tmpvar_13 < 1.0)) {
    tmpvar_10 = xlv_.x;
  } else {
    if ((tmpvar_13 < 2.0)) {
      tmpvar_10 = xlv_.y;
    } else {
      if ((tmpvar_13 < 3.0)) {
        tmpvar_10 = xlv_.z;
      } else {
        tmpvar_10 = xlv_.w;
      };
    };
  };
  mediump float tmpvar_14;
  tmpvar_14 = (floor(tmpvar_8.y) / 4.0);
  mediump float tmpvar_15;
  tmpvar_15 = (fract(abs(tmpvar_14)) * 4.0);
  mediump float tmpvar_16;
  if ((tmpvar_14 >= 0.0)) {
    tmpvar_16 = tmpvar_15;
  } else {
    tmpvar_16 = -(tmpvar_15);
  };
  facemodz_4 = tmpvar_16;
  highp float tmpvar_17;
  tmpvar_17 = (floor((tmpvar_10 / pow (4.0, facemodz_4))) / 4.0);
  highp float tmpvar_18;
  tmpvar_18 = (fract(abs(tmpvar_17)) * 4.0);
  highp float tmpvar_19;
  if ((tmpvar_17 >= 0.0)) {
    tmpvar_19 = tmpvar_18;
  } else {
    tmpvar_19 = -(tmpvar_18);
  };
  light_one_3 = tmpvar_19;
  mediump float tmpvar_20;
  tmpvar_20 = ((light_one_3 + 2.0) / 5.0);
  local_light_2 = tmpvar_20;
  tmpvar_1 = ((texture2D (_BlockTex, scaled_uv_6) * xlv_COLOR.z) * local_light_2);
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
attribute vec4 _glesMultiTexCoord0;
attribute vec3 _glesNormal;
attribute vec4 _glesColor;
attribute vec4 _glesVertex;
void main ()
{
  vec3 tmpvar_1;
  tmpvar_1 = normalize(_glesNormal);
  mediump vec2 tile_o_2;
  highp float index_3;
  mediump float light_level_4;
  highp float vy_5;
  highp float vx_6;
  highp vec4 tmpvar_7;
  mediump vec2 tmpvar_8;
  highp vec4 tmpvar_9;
  tmpvar_9 = (glstate_matrix_mvp * _glesVertex);
  highp float tmpvar_10;
  tmpvar_10 = (_glesVertex.x + 0.5);
  vx_6 = tmpvar_10;
  vy_5 = (_glesVertex.z + 0.5);
  light_level_4 = 0.5;
  lowp float tmpvar_11;
  tmpvar_11 = ((_GameClock - 0.34) / 0.66);
  lowp float tmpvar_12;
  tmpvar_12 = (tmpvar_11 * 0.5);
  lowp float tmpvar_13;
  tmpvar_13 = ((1.0 - tmpvar_11) * 0.5);
  if ((tmpvar_11 < 0.0)) {
    light_level_4 = (0.5 + (tmpvar_11 / 0.3));
  };
  if ((tmpvar_1.z > 0.2)) {
    vy_5 = (_glesVertex.y + 0.5);
    light_level_4 = (light_level_4 + tmpvar_13);
  } else {
    if ((tmpvar_1.z < -0.2)) {
      vy_5 = (_glesVertex.y + 0.5);
      vx_6 = (tmpvar_10 * -1.0);
      light_level_4 = (light_level_4 + tmpvar_12);
    } else {
      if ((tmpvar_1.x > 0.2)) {
        vx_6 = -((_glesVertex.z + 0.5));
        vy_5 = (_glesVertex.y + 0.5);
        light_level_4 = (light_level_4 + tmpvar_13);
      } else {
        if ((tmpvar_1.x < -0.2)) {
          vx_6 = (_glesVertex.z + 0.5);
          vy_5 = (_glesVertex.y + 0.5);
          light_level_4 = (light_level_4 + tmpvar_12);
        } else {
          if ((tmpvar_1.y < -0.2)) {
            vx_6 = (vx_6 * -1.0);
          } else {
            lowp float tmpvar_14;
            tmpvar_14 = (0.75 + (0.25 * tmpvar_11));
            light_level_4 = tmpvar_14;
            if ((tmpvar_11 < 0.0)) {
              light_level_4 = (light_level_4 + (0.45 * tmpvar_11));
            };
          };
        };
      };
    };
  };
  mediump float tmpvar_15;
  tmpvar_15 = (_glesMultiTexCoord0.x * 16.0);
  index_3 = tmpvar_15;
  highp float tmpvar_16;
  tmpvar_16 = floor((index_3 / 4.0));
  highp vec2 tmpvar_17;
  tmpvar_17.x = (index_3 - (tmpvar_16 * 4.0));
  tmpvar_17.y = tmpvar_16;
  highp vec2 tmpvar_18;
  tmpvar_18 = (tmpvar_17 * 0.25);
  tile_o_2 = tmpvar_18;
  tmpvar_7.xy = tile_o_2;
  tmpvar_7.z = light_level_4;
  highp vec2 tmpvar_19;
  tmpvar_19.x = vx_6;
  tmpvar_19.y = vy_5;
  tmpvar_8 = tmpvar_19;
  gl_Position = tmpvar_9;
  xlv_COLOR = tmpvar_7;
  xlv_TEXCOORD0 = tmpvar_8;
  xlv_ = ((_glesColor * 255.0) + vec4(0.5, 0.5, 0.5, 0.5));
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
  highp float facemodz_4;
  mediump vec2 offset_5;
  mediump vec2 scaled_uv_6;
  highp vec2 tmpvar_7;
  tmpvar_7 = xlv_COLOR.xy;
  offset_5 = tmpvar_7;
  scaled_uv_6 = ((fract(xlv_TEXCOORD0) * 0.25) + offset_5);
  mediump vec2 tmpvar_8;
  tmpvar_8 = floor(abs(xlv_TEXCOORD0));
  highp float xco_9;
  xco_9 = tmpvar_8.x;
  highp float tmpvar_10;
  highp float tmpvar_11;
  tmpvar_11 = (xco_9 / 4.0);
  highp float tmpvar_12;
  tmpvar_12 = (fract(abs(tmpvar_11)) * 4.0);
  highp float tmpvar_13;
  if ((tmpvar_11 >= 0.0)) {
    tmpvar_13 = tmpvar_12;
  } else {
    tmpvar_13 = -(tmpvar_12);
  };
  xco_9 = tmpvar_13;
  if ((tmpvar_13 < 1.0)) {
    tmpvar_10 = xlv_.x;
  } else {
    if ((tmpvar_13 < 2.0)) {
      tmpvar_10 = xlv_.y;
    } else {
      if ((tmpvar_13 < 3.0)) {
        tmpvar_10 = xlv_.z;
      } else {
        tmpvar_10 = xlv_.w;
      };
    };
  };
  mediump float tmpvar_14;
  tmpvar_14 = (floor(tmpvar_8.y) / 4.0);
  mediump float tmpvar_15;
  tmpvar_15 = (fract(abs(tmpvar_14)) * 4.0);
  mediump float tmpvar_16;
  if ((tmpvar_14 >= 0.0)) {
    tmpvar_16 = tmpvar_15;
  } else {
    tmpvar_16 = -(tmpvar_15);
  };
  facemodz_4 = tmpvar_16;
  highp float tmpvar_17;
  tmpvar_17 = (floor((tmpvar_10 / pow (4.0, facemodz_4))) / 4.0);
  highp float tmpvar_18;
  tmpvar_18 = (fract(abs(tmpvar_17)) * 4.0);
  highp float tmpvar_19;
  if ((tmpvar_17 >= 0.0)) {
    tmpvar_19 = tmpvar_18;
  } else {
    tmpvar_19 = -(tmpvar_18);
  };
  light_one_3 = tmpvar_19;
  mediump float tmpvar_20;
  tmpvar_20 = ((light_one_3 + 2.0) / 5.0);
  local_light_2 = tmpvar_20;
  tmpvar_1 = ((texture2D (_BlockTex, scaled_uv_6) * xlv_COLOR.z) * local_light_2);
  gl_FragData[0] = tmpvar_1;
}



#endif"
}

SubProgram "flash " {
Keywords { }
Bind "vertex" Vertex
Bind "normal" Normal
Bind "texcoord" TexCoord0
Bind "color" Color
Matrix 0 [glstate_matrix_mvp]
Float 4 [_GameClock]
"agal_vs
c5 0.2 0.0 -0.2 0.5
c6 1.0 -0.340088 1.514793 0.449951
c7 3.332791 0.5 0.25 0.75
c8 16.0 4.0 255.0 0.5
[bc]
aaaaaaaaaaaaabacaeaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov r0.x, c4
abaaaaaaaaaaabacagaaaaffabaaaaaaaaaaaaaaacaaaaaa add r0.x, c6.y, r0.x
adaaaaaaabaaabacaaaaaaaaacaaaaaaagaaaakkabaaaaaa mul r1.x, r0.x, c6.z
ckaaaaaaabaaaiacabaaaaaaacaaaaaaafaaaaffabaaaaaa slt r1.w, r1.x, c5.y
bfaaaaaaacaaaiacabaaaappacaaaaaaaaaaaaaaaaaaaaaa neg r2.w, r1.w
ahaaaaaaaaaaabacacaaaappacaaaaaaabaaaappacaaaaaa max r0.x, r2.w, r1.w
ckaaaaaaaaaaabacafaaaaffabaaaaaaaaaaaaaaacaaaaaa slt r0.x, c5.y, r0.x
bfaaaaaaadaaabacaaaaaaaaacaaaaaaaaaaaaaaaaaaaaaa neg r3.x, r0.x
abaaaaaaaaaaacacadaaaaaaacaaaaaaagaaaaaaabaaaaaa add r0.y, r3.x, c6.x
adaaaaaaaaaaaeacaaaaaaffacaaaaaaafaaaappabaaaaaa mul r0.z, r0.y, c5.w
adaaaaaaaaaaacacabaaaaaaacaaaaaaahaaaaaaabaaaaaa mul r0.y, r1.x, c7.x
abaaaaaaaaaaacacaaaaaaffacaaaaaaahaaaaoeabaaaaaa add r0.y, r0.y, c7
adaaaaaaacaaaeacaaaaaaaaacaaaaaaaaaaaaffacaaaaaa mul r2.z, r0.x, r0.y
abaaaaaaacaaaeacacaaaakkacaaaaaaaaaaaakkacaaaaaa add r2.z, r2.z, r0.z
ckaaaaaaaaaaabacafaaaaoeabaaaaaaabaaaakkaaaaaaaa slt r0.x, c5, a1.z
bfaaaaaaadaaaeacabaaaaaaacaaaaaaaaaaaaaaaaaaaaaa neg r3.z, r1.x
abaaaaaaaaaaaiacadaaaakkacaaaaaaagaaaaaaabaaaaaa add r0.w, r3.z, c6.x
adaaaaaaabaaaeacaaaaaappacaaaaaaafaaaappabaaaaaa mul r1.z, r0.w, c5.w
abaaaaaaabaaaeacabaaaakkacaaaaaaacaaaakkacaaaaaa add r1.z, r1.z, r2.z
cjaaaaaaaaaaaeacafaaaaffabaaaaaaaaaaaaaaacaaaaaa sge r0.z, c5.y, r0.x
cjaaaaaaaaaaacacaaaaaaaaacaaaaaaafaaaaoeabaaaaaa sge r0.y, r0.x, c5
adaaaaaaaaaaacacaaaaaaffacaaaaaaaaaaaakkacaaaaaa mul r0.y, r0.y, r0.z
bfaaaaaaaeaaabacaaaaaaaaacaaaaaaaaaaaaaaaaaaaaaa neg r4.x, r0.x
ahaaaaaaaaaaaeacaeaaaaaaacaaaaaaaaaaaaaaacaaaaaa max r0.z, r4.x, r0.x
ckaaaaaaacaaacacafaaaaoeabaaaaaaaaaaaakkacaaaaaa slt r2.y, c5, r0.z
ckaaaaaaabaaacacabaaaakkaaaaaaaaafaaaakkabaaaaaa slt r1.y, a1.z, c5.z
adaaaaaaaaaaaeacaaaaaaffacaaaaaaabaaaaffacaaaaaa mul r0.z, r0.y, r1.y
bfaaaaaaaeaaaeacaaaaaakkacaaaaaaaaaaaaaaaaaaaaaa neg r4.z, r0.z
ahaaaaaaacaaabacaeaaaakkacaaaaaaaaaaaakkacaaaaaa max r2.x, r4.z, r0.z
bfaaaaaaaeaaacacacaaaaffacaaaaaaaaaaaaaaaaaaaaaa neg r4.y, r2.y
abaaaaaaacaaaiacaeaaaaffacaaaaaaagaaaaaaabaaaaaa add r2.w, r4.y, c6.x
adaaaaaaacaaaeacacaaaakkacaaaaaaacaaaappacaaaaaa mul r2.z, r2.z, r2.w
adaaaaaaabaaaeacacaaaaffacaaaaaaabaaaakkacaaaaaa mul r1.z, r2.y, r1.z
abaaaaaaabaaaeacabaaaakkacaaaaaaacaaaakkacaaaaaa add r1.z, r1.z, r2.z
ckaaaaaaacaaabacafaaaaffabaaaaaaacaaaaaaacaaaaaa slt r2.x, c5.y, r2.x
bfaaaaaaaeaaabacacaaaaaaacaaaaaaaaaaaaaaaaaaaaaa neg r4.x, r2.x
abaaaaaaacaaaeacaeaaaaaaacaaaaaaagaaaaaaabaaaaaa add r2.z, r4.x, c6.x
bfaaaaaaaeaaaeacaaaaaakkacaaaaaaaaaaaaaaaaaaaaaa neg r4.z, r0.z
ahaaaaaaaaaaaeacaeaaaakkacaaaaaaaaaaaakkacaaaaaa max r0.z, r4.z, r0.z
adaaaaaaacaaacacabaaaaaaacaaaaaaafaaaappabaaaaaa mul r2.y, r1.x, c5.w
abaaaaaaacaaacacacaaaaffacaaaaaaabaaaakkacaaaaaa add r2.y, r2.y, r1.z
adaaaaaaacaaaeacabaaaakkacaaaaaaacaaaakkacaaaaaa mul r2.z, r1.z, r2.z
adaaaaaaaeaaabacacaaaaaaacaaaaaaacaaaaffacaaaaaa mul r4.x, r2.x, r2.y
abaaaaaaacaaabacaeaaaaaaacaaaaaaacaaaakkacaaaaaa add r2.x, r4.x, r2.z
cjaaaaaaabaaaeacafaaaaffabaaaaaaabaaaaffacaaaaaa sge r1.z, c5.y, r1.y
cjaaaaaaabaaacacabaaaaffacaaaaaaafaaaaoeabaaaaaa sge r1.y, r1.y, c5
adaaaaaaabaaacacabaaaaffacaaaaaaabaaaakkacaaaaaa mul r1.y, r1.y, r1.z
adaaaaaaabaaacacaaaaaaffacaaaaaaabaaaaffacaaaaaa mul r1.y, r0.y, r1.y
ckaaaaaaabaaaeacafaaaaaaabaaaaaaabaaaaaaaaaaaaaa slt r1.z, c5.x, a1.x
adaaaaaaaaaaacacabaaaaffacaaaaaaabaaaakkacaaaaaa mul r0.y, r1.y, r1.z
bfaaaaaaaeaaacacaaaaaaffacaaaaaaaaaaaaaaaaaaaaaa neg r4.y, r0.y
ahaaaaaaacaaaeacaeaaaaffacaaaaaaaaaaaaffacaaaaaa max r2.z, r4.y, r0.y
adaaaaaaaeaaacacaaaaaappacaaaaaaafaaaappabaaaaaa mul r4.y, r0.w, c5.w
abaaaaaaacaaacacaeaaaaffacaaaaaaacaaaaaaacaaaaaa add r2.y, r4.y, r2.x
ckaaaaaaaaaaaiacafaaaaffabaaaaaaacaaaakkacaaaaaa slt r0.w, c5.y, r2.z
cjaaaaaaacaaaeacafaaaaffabaaaaaaabaaaakkacaaaaaa sge r2.z, c5.y, r1.z
cjaaaaaaabaaaeacabaaaakkacaaaaaaafaaaaffabaaaaaa sge r1.z, r1.z, c5.y
adaaaaaaacaaaeacabaaaakkacaaaaaaacaaaakkacaaaaaa mul r2.z, r1.z, r2.z
bfaaaaaaaeaaaiacaaaaaappacaaaaaaaaaaaaaaaaaaaaaa neg r4.w, r0.w
abaaaaaaadaaabacaeaaaappacaaaaaaagaaaaoeabaaaaaa add r3.x, r4.w, c6
adaaaaaaadaaabacacaaaaaaacaaaaaaadaaaaaaacaaaaaa mul r3.x, r2.x, r3.x
adaaaaaaaaaaaiacaaaaaappacaaaaaaacaaaaffacaaaaaa mul r0.w, r0.w, r2.y
abaaaaaaaaaaaiacaaaaaappacaaaaaaadaaaaaaacaaaaaa add r0.w, r0.w, r3.x
ckaaaaaaabaaaeacabaaaaaaaaaaaaaaafaaaaoeabaaaaaa slt r1.z, a1.x, c5
adaaaaaaacaaaeacabaaaaffacaaaaaaacaaaakkacaaaaaa mul r2.z, r1.y, r2.z
adaaaaaaabaaacacacaaaakkacaaaaaaabaaaakkacaaaaaa mul r1.y, r2.z, r1.z
bfaaaaaaaeaaacacabaaaaffacaaaaaaaaaaaaaaaaaaaaaa neg r4.y, r1.y
ahaaaaaaacaaaiacaeaaaaffacaaaaaaabaaaaffacaaaaaa max r2.w, r4.y, r1.y
ckaaaaaaacaaabacafaaaaffabaaaaaaacaaaappacaaaaaa slt r2.x, c5.y, r2.w
cjaaaaaaadaaabacafaaaaffabaaaaaaabaaaakkacaaaaaa sge r3.x, c5.y, r1.z
bfaaaaaaaeaaabacacaaaaaaacaaaaaaaaaaaaaaaaaaaaaa neg r4.x, r2.x
abaaaaaaacaaaiacaeaaaaaaacaaaaaaagaaaaaaabaaaaaa add r2.w, r4.x, c6.x
cjaaaaaaabaaaeacabaaaakkacaaaaaaafaaaaffabaaaaaa sge r1.z, r1.z, c5.y
adaaaaaaabaaaeacabaaaakkacaaaaaaadaaaaaaacaaaaaa mul r1.z, r1.z, r3.x
bfaaaaaaaeaaacacaaaaaaffacaaaaaaaaaaaaaaaaaaaaaa neg r4.y, r0.y
ahaaaaaaaaaaacacaeaaaaffacaaaaaaaaaaaaffacaaaaaa max r0.y, r4.y, r0.y
adaaaaaaacaaacacabaaaaaaacaaaaaaafaaaappabaaaaaa mul r2.y, r1.x, c5.w
abaaaaaaacaaacacacaaaaffacaaaaaaaaaaaappacaaaaaa add r2.y, r2.y, r0.w
adaaaaaaacaaaiacaaaaaappacaaaaaaacaaaappacaaaaaa mul r2.w, r0.w, r2.w
ckaaaaaaaaaaaiacabaaaaffaaaaaaaaafaaaakkabaaaaaa slt r0.w, a1.y, c5.z
adaaaaaaabaaaeacacaaaakkacaaaaaaabaaaakkacaaaaaa mul r1.z, r2.z, r1.z
adaaaaaaaeaaaiacacaaaaaaacaaaaaaacaaaaffacaaaaaa mul r4.w, r2.x, r2.y
abaaaaaaacaaaiacaeaaaappacaaaaaaacaaaappacaaaaaa add r2.w, r4.w, r2.w
ckaaaaaaaaaaacacafaaaaoeabaaaaaaaaaaaaffacaaaaaa slt r0.y, c5, r0.y
cjaaaaaaadaaacacafaaaaoeabaaaaaaaaaaaappacaaaaaa sge r3.y, c5, r0.w
cjaaaaaaadaaabacaaaaaappacaaaaaaafaaaaffabaaaaaa sge r3.x, r0.w, c5.y
adaaaaaaadaaabacadaaaaaaacaaaaaaadaaaaffacaaaaaa mul r3.x, r3.x, r3.y
adaaaaaaacaaaeacabaaaakkacaaaaaaadaaaaaaacaaaaaa mul r2.z, r1.z, r3.x
bfaaaaaaaeaaaeacacaaaakkacaaaaaaaaaaaaaaaaaaaaaa neg r4.z, r2.z
ahaaaaaaacaaabacaeaaaakkacaaaaaaacaaaakkacaaaaaa max r2.x, r4.z, r2.z
ckaaaaaaacaaabacafaaaaffabaaaaaaacaaaaaaacaaaaaa slt r2.x, c5.y, r2.x
adaaaaaaabaaaiacacaaaakkacaaaaaaabaaaappacaaaaaa mul r1.w, r2.z, r1.w
bfaaaaaaaeaaabacacaaaaaaacaaaaaaaaaaaaaaaaaaaaaa neg r4.x, r2.x
abaaaaaaacaaacacaeaaaaaaacaaaaaaagaaaaaaabaaaaaa add r2.y, r4.x, c6.x
adaaaaaaacaaaeacacaaaaffacaaaaaaacaaaappacaaaaaa mul r2.z, r2.y, r2.w
adaaaaaaacaaacacabaaaaaaacaaaaaaahaaaakkabaaaaaa mul r2.y, r1.x, c7.z
abaaaaaaacaaacacacaaaaffacaaaaaaahaaaappabaaaaaa add r2.y, r2.y, c7.w
adaaaaaaaeaaacacacaaaaaaacaaaaaaacaaaaffacaaaaaa mul r4.y, r2.x, r2.y
abaaaaaaacaaacacaeaaaaffacaaaaaaacaaaakkacaaaaaa add r2.y, r4.y, r2.z
bfaaaaaaaeaaaiacabaaaappacaaaaaaaaaaaaaaaaaaaaaa neg r4.w, r1.w
ahaaaaaaabaaaiacaeaaaappacaaaaaaabaaaappacaaaaaa max r1.w, r4.w, r1.w
ckaaaaaaabaaaiacafaaaaffabaaaaaaabaaaappacaaaaaa slt r1.w, c5.y, r1.w
bfaaaaaaaeaaaiacabaaaappacaaaaaaaaaaaaaaaaaaaaaa neg r4.w, r1.w
abaaaaaaacaaaeacaeaaaappacaaaaaaagaaaaaaabaaaaaa add r2.z, r4.w, c6.x
adaaaaaaaeaaabacabaaaaaaacaaaaaaagaaaappabaaaaaa mul r4.x, r1.x, c6.w
abaaaaaaacaaabacaeaaaaaaacaaaaaaacaaaaffacaaaaaa add r2.x, r4.x, r2.y
adaaaaaaabaaabacacaaaaffacaaaaaaacaaaakkacaaaaaa mul r1.x, r2.y, r2.z
bfaaaaaaaeaaacacabaaaaffacaaaaaaaaaaaaaaaaaaaaaa neg r4.y, r1.y
ahaaaaaaacaaaeacaeaaaaffacaaaaaaabaaaaffacaaaaaa max r2.z, r4.y, r1.y
ckaaaaaaaaaaaeacafaaaaffabaaaaaaaaaaaakkacaaaaaa slt r0.z, c5.y, r0.z
adaaaaaaaeaaaeacabaaaappacaaaaaaacaaaaaaacaaaaaa mul r4.z, r1.w, r2.x
abaaaaaaahaaaeaeaeaaaakkacaaaaaaabaaaaaaacaaaaaa add v7.z, r4.z, r1.x
bfaaaaaaaeaaabacaaaaaaaaacaaaaaaaaaaaaaaaaaaaaaa neg r4.x, r0.x
ahaaaaaaabaaaiacaeaaaaaaacaaaaaaaaaaaaaaacaaaaaa max r1.w, r4.x, r0.x
adaaaaaaaaaaabacabaaaakkacaaaaaaaaaaaappacaaaaaa mul r0.x, r1.z, r0.w
ckaaaaaaabaaaeacafaaaaffabaaaaaaabaaaappacaaaaaa slt r1.z, c5.y, r1.w
bfaaaaaaaeaaabacaaaaaaaaacaaaaaaaaaaaaaaaaaaaaaa neg r4.x, r0.x
ahaaaaaaaaaaabacaeaaaaaaacaaaaaaaaaaaaaaacaaaaaa max r0.x, r4.x, r0.x
ckaaaaaaaaaaaiacafaaaaffabaaaaaaaaaaaaaaacaaaaaa slt r0.w, c5.y, r0.x
abaaaaaaaaaaabacaaaaaaffaaaaaaaaafaaaappabaaaaaa add r0.x, a0.y, c5.w
bfaaaaaaaeaaaeacaaaaaakkacaaaaaaaaaaaaaaaaaaaaaa neg r4.z, r0.z
abaaaaaaacaaacacaeaaaakkacaaaaaaagaaaaaaabaaaaaa add r2.y, r4.z, c6.x
abaaaaaaabaaacacaaaaaaaaaaaaaaaaafaaaappabaaaaaa add r1.y, a0.x, c5.w
adaaaaaaacaaaiacabaaaaffacaaaaaaacaaaaffacaaaaaa mul r2.w, r1.y, r2.y
bfaaaaaaaeaaacacabaaaaffacaaaaaaaaaaaaaaaaaaaaaa neg r4.y, r1.y
adaaaaaaaeaaaiacaaaaaakkacaaaaaaaeaaaaffacaaaaaa mul r4.w, r0.z, r4.y
abaaaaaaacaaaiacaeaaaappacaaaaaaacaaaappacaaaaaa add r2.w, r4.w, r2.w
bfaaaaaaabaaacacaaaaaaffacaaaaaaaaaaaaaaaaaaaaaa neg r1.y, r0.y
abaaaaaaabaaacacabaaaaffacaaaaaaagaaaaaaabaaaaaa add r1.y, r1.y, c6.x
ckaaaaaaacaaaeacafaaaaffabaaaaaaacaaaakkacaaaaaa slt r2.z, c5.y, r2.z
abaaaaaaadaaabacaaaaaakkaaaaaaaaafaaaappabaaaaaa add r3.x, a0.z, c5.w
adaaaaaaacaaaiacabaaaaffacaaaaaaacaaaappacaaaaaa mul r2.w, r1.y, r2.w
bfaaaaaaaeaaabacadaaaaaaacaaaaaaaaaaaaaaaaaaaaaa neg r4.x, r3.x
adaaaaaaadaaacacaaaaaaffacaaaaaaaeaaaaaaacaaaaaa mul r3.y, r0.y, r4.x
abaaaaaaadaaacacadaaaaffacaaaaaaacaaaappacaaaaaa add r3.y, r3.y, r2.w
bfaaaaaaaeaaaeacacaaaakkacaaaaaaaaaaaaaaaaaaaaaa neg r4.z, r2.z
abaaaaaaacaaaiacaeaaaakkacaaaaaaagaaaaaaabaaaaaa add r2.w, r4.z, c6.x
bfaaaaaaaeaaaeacabaaaakkacaaaaaaaaaaaaaaaaaaaaaa neg r4.z, r1.z
abaaaaaaabaaaiacaeaaaakkacaaaaaaagaaaaaaabaaaaaa add r1.w, r4.z, c6.x
adaaaaaaadaaacacacaaaappacaaaaaaadaaaaffacaaaaaa mul r3.y, r2.w, r3.y
adaaaaaaabaaaiacadaaaaaaacaaaaaaabaaaappacaaaaaa mul r1.w, r3.x, r1.w
adaaaaaaaeaaaiacabaaaakkacaaaaaaaaaaaaaaacaaaaaa mul r4.w, r1.z, r0.x
abaaaaaaabaaaiacaeaaaappacaaaaaaabaaaappacaaaaaa add r1.w, r4.w, r1.w
adaaaaaaabaaaiacacaaaaffacaaaaaaabaaaappacaaaaaa mul r1.w, r2.y, r1.w
adaaaaaaaaaaaeacaaaaaakkacaaaaaaaaaaaaaaacaaaaaa mul r0.z, r0.z, r0.x
abaaaaaaaaaaaeacaaaaaakkacaaaaaaabaaaappacaaaaaa add r0.z, r0.z, r1.w
adaaaaaaabaaabacacaaaakkacaaaaaaadaaaaaaacaaaaaa mul r1.x, r2.z, r3.x
abaaaaaaabaaabacabaaaaaaacaaaaaaadaaaaffacaaaaaa add r1.x, r1.x, r3.y
bfaaaaaaaeaaaiacaaaaaappacaaaaaaaaaaaaaaaaaaaaaa neg r4.w, r0.w
abaaaaaaabaaaeacaeaaaappacaaaaaaagaaaaaaabaaaaaa add r1.z, r4.w, c6.x
adaaaaaaabaaaeacabaaaaaaacaaaaaaabaaaakkacaaaaaa mul r1.z, r1.x, r1.z
bfaaaaaaaeaaabacabaaaaaaacaaaaaaaaaaaaaaaaaaaaaa neg r4.x, r1.x
adaaaaaaaeaaabacaaaaaappacaaaaaaaeaaaaaaacaaaaaa mul r4.x, r0.w, r4.x
abaaaaaaaaaaabaeaeaaaaaaacaaaaaaabaaaakkacaaaaaa add v0.x, r4.x, r1.z
adaaaaaaaaaaaiacabaaaaffacaaaaaaaaaaaakkacaaaaaa mul r0.w, r1.y, r0.z
adaaaaaaaeaaaiacaaaaaaffacaaaaaaaaaaaaaaacaaaaaa mul r4.w, r0.y, r0.x
abaaaaaaaaaaaiacaeaaaappacaaaaaaaaaaaappacaaaaaa add r0.w, r4.w, r0.w
adaaaaaaaaaaaeacadaaaaaaaaaaaaaaaiaaaaaaabaaaaaa mul r0.z, a3.x, c8.x
adaaaaaaabaaabacacaaaappacaaaaaaaaaaaappacaaaaaa mul r1.x, r2.w, r0.w
adaaaaaaaaaaacacaaaaaakkacaaaaaaahaaaakkabaaaaaa mul r0.y, r0.z, c7.z
aiaaaaaaaaaaaiacaaaaaaffacaaaaaaaaaaaaaaaaaaaaaa frc r0.w, r0.y
adaaaaaaaeaaacacacaaaakkacaaaaaaaaaaaaaaacaaaaaa mul r4.y, r2.z, r0.x
abaaaaaaaaaaacaeaeaaaaffacaaaaaaabaaaaaaacaaaaaa add v0.y, r4.y, r1.x
acaaaaaaaaaaabacaaaaaaffacaaaaaaaaaaaappacaaaaaa sub r0.x, r0.y, r0.w
aaaaaaaaaaaaacacaaaaaaaaacaaaaaaaaaaaaaaaaaaaaaa mov r0.y, r0.x
bfaaaaaaaeaaabacaaaaaaaaacaaaaaaaaaaaaaaaaaaaaaa neg r4.x, r0.x
adaaaaaaaeaaabacaeaaaaaaacaaaaaaaiaaaaffabaaaaaa mul r4.x, r4.x, c8.y
abaaaaaaaaaaabacaeaaaaaaacaaaaaaaaaaaakkacaaaaaa add r0.x, r4.x, r0.z
adaaaaaaaeaaapacacaaaaoeaaaaaaaaaiaaaakkabaaaaaa mul r4, a2, c8.z
abaaaaaaabaaapaeaeaaaaoeacaaaaaaaiaaaappabaaaaaa add v1, r4, c8.w
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
#line 317
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
#line 325
#line 342
#line 397
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
            vx *= -1.0;
            light_level += neg_shadow_nudge;
        }
        else{
            if ((v.normal.x > 0.2)){
                #line 369
                vx = (-(v.vertex.z + 0.5));
                vy = (v.vertex.y + 0.5);
                light_level += pos_shadow_nudge;
            }
            else{
                if ((v.normal.x < -0.2)){
                    #line 375
                    vx = (v.vertex.z + 0.5);
                    vy = (v.vertex.y + 0.5);
                    light_level += neg_shadow_nudge;
                }
                else{
                    if ((v.normal.y < -0.2)){
                        #line 381
                        vx *= -1.0;
                    }
                    else{
                        #line 385
                        light_level = (0.75 + (0.25 * day_level));
                        if ((day_level < 0.0)){
                            light_level += (0.45 * day_level);
                        }
                    }
                }
            }
        }
    }
    highp float index = (v.texcoord.x * 16.0);
    #line 389
    highp float blocky = floor((index / 4.0));
    mediump vec2 tile_o = (vec2( (index - (blocky * 4.0)), blocky) * 0.25);
    o.color.xy = tile_o;
    o.color.z = light_level;
    #line 393
    o.uv.xy = vec2( vx, vy);
    o.overhangLightLevel = ((v.color32 * 255.0) + vec4( 0.5));
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
#line 317
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
#line 325
#line 342
#line 397
#line 325
highp float getIndex( in highp vec4 overhang, in highp float xco ) {
    xco = xll_mod_f_f( xco, 4.0);
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
#line 397
lowp vec4 frag( in v2f i ) {
    mediump vec2 scaled_uv = (fract(i.uv.xy) * 0.25);
    mediump vec2 offset = i.color.xy;
    #line 401
    scaled_uv = (scaled_uv + offset);
    mediump vec2 model_rel_twoD = floor(abs(i.uv.xy));
    highp float index = getIndex( i.overhangLightLevel, model_rel_twoD.x);
    highp float facemodz = xll_mod_f_f( floor(model_rel_twoD.y), 4.0);
    #line 405
    highp float power_lookup = floor((index / pow( 4.0, facemodz)));
    mediump float light_one = xll_mod_f_f( power_lookup, 4.0);
    lowp float local_light = ((light_one + 2.0) / 5.0);
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
//   opengl - ALU: 44 to 44, TEX: 1 to 1
//   d3d9 - ALU: 51 to 51, TEX: 1 to 1
SubProgram "opengl " {
Keywords { }
SetTexture 0 [_BlockTex] 2D
"!!ARBfp1.0
# 44 ALU, 1 TEX
PARAM c[2] = { { 0.25, 0.2, 4, 1 },
		{ 0, 2, 3 } };
TEMP R0;
TEMP R1;
TEMP R2;
FRC R0.xy, fragment.texcoord[0];
MAD R0.xy, R0, c[0].x, fragment.color.primary;
ABS R1.xy, fragment.texcoord[0];
FLR R1.xy, R1;
MUL R1.w, R1.x, c[0].x;
ABS R1.w, R1;
FRC R1.w, R1;
MUL R1.w, R1, c[0].z;
CMP R2.w, R1.x, -R1, R1;
ADD R1.w, R2, -c[0];
FLR R2.z, R1.y;
MOV R1.x, c[0].w;
CMP R2.x, R1.w, c[1], R1;
SLT R2.y, R2.w, c[1];
MUL R1.x, R2, R2.y;
CMP R2.y, -R1.x, c[1].x, R2.x;
CMP R1.z, R1.w, fragment.texcoord[1].x, R1;
SLT R1.y, R2.w, c[1].z;
MUL R2.x, R2, R2.y;
MUL R1.y, R2.x, R1;
CMP R2.y, -R1, c[1].x, R2;
CMP R1.x, -R1, fragment.texcoord[1].y, R1.z;
MUL R2.w, R2.z, c[0].x;
ABS R2.w, R2;
MUL R2.x, R2, R2.y;
CMP R1.x, -R1.y, fragment.texcoord[1].z, R1;
FRC R2.w, R2;
MUL R2.y, R2.w, c[0].z;
CMP R1.w, R2.z, -R2.y, R2.y;
POW R1.z, c[0].z, R1.w;
RCP R1.y, R1.z;
CMP R1.x, -R2, fragment.texcoord[1].w, R1;
MUL R1.x, R1, R1.y;
FLR R1.x, R1;
MUL R1.y, R1.x, c[0].x;
ABS R1.y, R1;
FRC R1.y, R1;
MUL R1.y, R1, c[0].z;
CMP R1.x, R1, -R1.y, R1.y;
ADD R1.x, R1, c[1].y;
MUL R1.x, R1, c[0].y;
TEX R0, R0, texture[0], 2D;
MUL R0, R0, fragment.color.primary.z;
MUL result.color, R0, R1.x;
END
# 44 instructions, 3 R-regs
"
}

SubProgram "d3d9 " {
Keywords { }
SetTexture 0 [_BlockTex] 2D
"ps_2_0
; 51 ALU, 1 TEX
dcl_2d s0
def c0, 0.25000000, 4.00000000, -1.00000000, -2.00000000
def c1, 1.00000000, 0.00000000, -3.00000000, 2.00000000
def c2, 0.20000000, 0, 0, 0
dcl v0.xyz
dcl t0.xy
dcl t1
frc_pp r1.xy, t0
mad_pp r1.xy, r1, c0.x, v0
texld r7, r1, s0
abs_pp r1.xy, t0
frc_pp r2.xy, r1
add_pp r2.xy, r1, -r2
mul r1.x, r2, c0
abs r1.x, r1
frc r1.x, r1
mul r1.x, r1, c0.y
cmp r3.x, r2, r1, -r1
add r1.x, r3, c0.z
add r2.x, r3, c0.w
frc_pp r6.x, r2.y
add_pp r6.x, r2.y, -r6
mul_pp r8.x, r6, c0
add r3.x, r3, c1.z
abs_pp r8.x, r8
frc_pp r8.x, r8
mul_pp r8.x, r8, c0.y
cmp_pp r5.x, r1, c1, c1.y
cmp r2.x, r2, c1.y, c1
mul_pp r2.x, r5, r2
cmp_pp r4.x, -r2, r5, c1.y
cmp r0.x, r1, r0, t1
mul_pp r5.x, r5, r4
cmp r3.x, r3, c1.y, c1
mul_pp r3.x, r5, r3
cmp_pp r4.x, -r3, r4, c1.y
cmp r0.x, -r2, r0, t1.y
mul_pp r4.x, r5, r4
cmp_pp r6.x, r6, r8, -r8
pow r5.x, c0.y, r6.x
mov r1.x, r5.x
cmp r0.x, -r3, r0, t1.z
rcp r1.x, r1.x
cmp r0.x, -r4, r0, t1.w
mul r0.x, r0, r1
frc r1.x, r0
add r0.x, r0, -r1
mul r1.x, r0, c0
abs r1.x, r1
frc r1.x, r1
mul r1.x, r1, c0.y
cmp r0.x, r0, r1, -r1
add_pp r0.x, r0, c1.w
mul_pp r0.x, r0, c2
mul r1, r7, v0.z
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

SubProgram "gles3 " {
Keywords { }
"!!GLES3"
}

}

#LINE 295

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
