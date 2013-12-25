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
        Program "vp" {
// Vertex combos: 1
//   opengl - ALU: 52 to 52
//   d3d9 - ALU: 71 to 71
//   d3d11 - ALU: 8 to 8, TEX: 0 to 0, FLOW: 1 to 1
//   d3d11_9x - ALU: 8 to 8, TEX: 0 to 0, FLOW: 1 to 1
SubProgram "opengl " {
Keywords { }
Bind "vertex" Vertex
Bind "normal" Normal
Bind "texcoord" TexCoord0
"!!ARBvp1.0
# 52 ALU
PARAM c[6] = { { 16, 0.25, 4, 0.5 },
		state.matrix.mvp,
		{ 0, 0.2, -0.2 } };
TEMP R0;
TEMP R1;
TEMP R2;
SLT R0.w, c[5].y, vertex.normal.z;
ADD R0.x, vertex.position, c[0].w;
ABS R0.y, R0.w;
ADD R2.x, vertex.position.z, c[0].w;
ADD R2.y, vertex.position, -R2.x;
SGE R1.x, c[5], R0.y;
SLT R1.y, vertex.normal.z, c[5].z;
MUL R0.y, R1.x, R1;
ADD R0.z, -R0.x, -R0.x;
MAD R1.z, R0, R0.y, R0.x;
ABS R0.x, R1.y;
SGE R0.x, c[5], R0;
MUL R0.z, R1.x, R0.x;
ADD R1.y, -vertex.position, -R1.z;
SLT R1.w, c[5].y, vertex.normal.x;
MUL R0.x, R0.z, R1.w;
ADD R1.x, R1.y, -c[0].w;
MAD R1.x, R1, R0, R1.z;
ABS R1.z, R1.w;
ADD R1.y, vertex.position, -R1.x;
SGE R1.w, c[5].x, R1.z;
ADD R1.z, R1.y, c[0].w;
MUL R1.y, R0.z, R1.w;
SLT R1.w, vertex.normal.x, c[5].z;
MUL R0.z, R1.y, R1.w;
MAD R1.x, R1.z, R0.z, R1;
ADD R2.y, R2, c[0].w;
MAD R1.z, R0.w, R2.y, R2.x;
ABS R2.x, R1.w;
ADD R2.y, -vertex.position, -R1.z;
ADD R1.w, R2.y, -c[0];
SGE R2.x, c[5], R2;
ADD R0.w, -R1.x, -R1.x;
MAD R0.y, R0, R1.w, R1.z;
SLT R2.y, vertex.normal, c[5].z;
MUL R1.y, R1, R2.x;
MUL R1.y, R1, R2;
MAD result.texcoord[0].x, R0.w, R1.y, R1;
ADD R1.x, -R0.y, -R0.y;
MAD R0.y, R0.x, R1.x, R0;
MUL R0.w, vertex.texcoord[0].x, c[0].x;
ADD R1.x, -R0.y, -R0.y;
MUL R0.x, R0.w, c[0].y;
FLR R0.x, R0;
MAD result.texcoord[0].y, R0.z, R1.x, R0;
MOV R0.y, R0.x;
MAD R0.x, -R0, c[0].z, R0.w;
MUL result.color.xy, R0, c[0].y;
DP4 result.position.w, vertex.position, c[4];
DP4 result.position.z, vertex.position, c[3];
DP4 result.position.y, vertex.position, c[2];
DP4 result.position.x, vertex.position, c[1];
END
# 52 instructions, 3 R-regs
"
}

SubProgram "d3d9 " {
Keywords { }
Bind "vertex" Vertex
Bind "normal" Normal
Bind "texcoord" TexCoord0
Matrix 0 [glstate_matrix_mvp]
"vs_2_0
; 71 ALU
def c4, 0.20000000, 0.00000000, -0.20000000, 0.50000000
def c5, 1.00000000, 16.00000000, 0.25000000, 4.00000000
dcl_position0 v0
dcl_normal0 v1
dcl_texcoord0 v2
slt r0.y, c4.x, v1.z
slt r1.x, v1.z, c4.z
sge r0.z, c4.y, r0.y
sge r0.x, r0.y, c4.y
mul r0.w, r0.x, r0.z
sge r0.z, c4.y, r1.x
sge r0.x, r1, c4.y
mul r0.x, r0, r0.z
slt r0.z, c4.x, v1.x
mul r0.x, r0.w, r0
mul r1.x, r0.w, r1
mul r1.y, r0.x, r0.z
sge r1.w, c4.y, r0.z
max r0.w, -r1.y, r1.y
max r1.x, -r1, r1
sge r0.z, r0, c4.y
mul r0.z, r0, r1.w
slt r1.y, c4, r1.x
max r0.y, -r0, r0
slt r1.x, c4.y, r0.y
mul r0.x, r0, r0.z
add r0.y, -r1.x, c5.x
add r1.z, v0, c4.w
mul r1.z, r0.y, r1
add r0.y, v0, c4.w
mad r1.x, r0.y, r1, r1.z
add r1.z, -r1.y, c5.x
mul r1.x, r1.z, r1
mad r2.x, -r0.y, r1.y, r1
slt r0.w, c4.y, r0
add r1.x, -r0.w, c5
mul r2.y, r1.x, r2.x
mad r1.w, r0, -r2.x, r2.y
slt r2.x, v1, c4.z
add r2.y, v0.x, c4.w
mul r0.z, r0.x, r2.x
mul r1.z, r2.y, r1
mad r1.y, r1, -r2, r1.z
mul r1.y, r1.x, r1
max r0.z, -r0, r0
slt r0.z, c4.y, r0
add r1.x, -r0.z, c5
mad r0.w, r0, -r0.y, r1.y
mul r1.y, r1.x, r0.w
mul r1.z, r1.x, r1.w
sge r1.x, c4.y, r2
sge r0.w, r2.x, c4.y
mul r0.w, r0, r1.x
mul r0.x, r0, r0.w
slt r1.x, v1.y, c4.z
mul r0.x, r0, r1
mad r1.x, r0.z, r0.y, r1.y
max r0.y, -r0.x, r0.x
slt r0.w, c4.y, r0.y
mul r0.x, v2, c5.y
add r1.y, -r0.w, c5.x
mul r1.y, r1.x, r1
mul r0.y, r0.x, c5.z
mad oT0.y, r0.z, -r1.w, r1.z
frc r0.z, r0.y
add r0.z, r0.y, -r0
mov r0.y, r0.z
mad r0.x, -r0.z, c5.w, r0
mad oT0.x, r0.w, -r1, r1.y
mul oD0.xy, r0, c5.z
dp4 oPos.w, v0, c3
dp4 oPos.z, v0, c2
dp4 oPos.y, v0, c1
dp4 oPos.x, v0, c0
"
}

SubProgram "d3d11 " {
Keywords { }
Bind "vertex" Vertex
Bind "normal" Normal
Bind "texcoord" TexCoord0
ConstBuffer "UnityPerDraw" 336 // 64 used size, 6 vars
Matrix 0 [glstate_matrix_mvp] 4
BindCB "UnityPerDraw" 0
// 19 instructions, 4 temp regs, 0 temp arrays:
// ALU 8 float, 0 int, 0 uint
// TEX 0 (0 load, 0 comp, 0 bias, 0 grad)
// FLOW 1 static, 0 dynamic
"vs_4_0
eefiecedggcioocokhmpagmbbhhonhjlinmoencdabaaaaaaeaaeaaaaadaaaaaa
cmaaaaaakaaaaaaabeabaaaaejfdeheogmaaaaaaadaaaaaaaiaaaaaafaaaaaaa
aaaaaaaaaaaaaaaaadaaaaaaaaaaaaaaapapaaaafjaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaahahaaaagaaaaaaaaaaaaaaaaaaaaaaaadaaaaaaacaaaaaa
adabaaaafaepfdejfeejepeoaaeoepfcenebemaafeeffiedepepfceeaaklklkl
epfdeheogmaaaaaaadaaaaaaaiaaaaaafaaaaaaaaaaaaaaaabaaaaaaadaaaaaa
aaaaaaaaapaaaaaafmaaaaaaaaaaaaaaaaaaaaaaadaaaaaaabaaaaaaapamaaaa
gcaaaaaaaaaaaaaaaaaaaaaaadaaaaaaacaaaaaaadamaaaafdfgfpfaepfdejfe
ejepeoaaedepemepfcaafeeffiedepepfceeaaklfdeieefcceadaaaaeaaaabaa
mjaaaaaafjaaaaaeegiocaaaaaaaaaaaaeaaaaaafpaaaaadpcbabaaaaaaaaaaa
fpaaaaadhcbabaaaabaaaaaafpaaaaadbcbabaaaacaaaaaaghaaaaaepccabaaa
aaaaaaaaabaaaaaagfaaaaaddccabaaaabaaaaaagfaaaaaddccabaaaacaaaaaa
giaaaaacaeaaaaaadiaaaaaipcaabaaaaaaaaaaafgbfbaaaaaaaaaaaegiocaaa
aaaaaaaaabaaaaaadcaaaaakpcaabaaaaaaaaaaaegiocaaaaaaaaaaaaaaaaaaa
agbabaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaakpcaabaaaaaaaaaaaegiocaaa
aaaaaaaaacaaaaaakgbkbaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaakpccabaaa
aaaaaaaaegiocaaaaaaaaaaaadaaaaaapgbpbaaaaaaaaaaaegaobaaaaaaaaaaa
diaaaaakdcaabaaaaaaaaaaaagbabaaaacaaaaaaaceaaaaaaaaaiaebaaaaiaea
aaaaaaaaaaaaaaaaebaaaaafccaabaaaabaaaaaabkaabaaaaaaaaaaadcaaaaak
bcaabaaaabaaaaaabkaabaiaebaaaaaaabaaaaaaabeaaaaaaaaaiaeaakaabaaa
aaaaaaaadiaaaaakdccabaaaabaaaaaaegaabaaaabaaaaaaaceaaaaaaaaaiado
aaaaiadoaaaaaaaaaaaaaaaadcaaaaapdcaabaaaaaaaaaaajgbfbaaaaaaaaaaa
aceaaaaaaaaaiadpaaaaialpaaaaaaaaaaaaaaaaaceaaaaaaaaaaadpaaaaaalp
aaaaaaaaaaaaaaaaaaaaaaahccaabaaaabaaaaaackbabaaaaaaaaaaaabeaaaaa
aaaaaadpdbaaaaakhcaabaaaacaaaaaacgbjbaaaabaaaaaaaceaaaaamnmmemlo
mnmmemlomnmmemloaaaaaaaaaaaaaaakhcaabaaaadaaaaaabgbgbaaaaaaaaaaa
aceaaaaaaaaaaadpaaaaaadpaaaaaadpaaaaaaaadhaaaaakbcaabaaaabaaaaaa
ckaabaaaacaaaaaabkaabaiaebaaaaaaadaaaaaabkaabaaaadaaaaaadhaaaaaj
dcaabaaaaaaaaaaafgafbaaaacaaaaaaegaabaaaaaaaaaaaegaabaaaabaaaaaa
dbaaaaakmcaabaaaaaaaaaaaaceaaaaaaaaaaaaaaaaaaaaamnmmemdomnmmemdo
kgbcbaaaabaaaaaadhaaaaakdcaabaaaaaaaaaaapgapbaaaaaaaaaaaigaabaia
ebaaaaaaadaaaaaaegaabaaaaaaaaaaadhaaaaakdcaabaaaaaaaaaaaagaabaaa
acaaaaaabgafbaiaebaaaaaaadaaaaaaegaabaaaaaaaaaaadhaaaaajdccabaaa
acaaaaaakgakbaaaaaaaaaaabgafbaaaadaaaaaaegaabaaaaaaaaaaadoaaaaab
"
}

SubProgram "gles " {
Keywords { }
"!!GLES


#ifdef VERTEX

varying mediump vec2 xlv_TEXCOORD0;
varying highp vec4 xlv_COLOR;
uniform highp mat4 glstate_matrix_mvp;
attribute vec4 _glesMultiTexCoord0;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  vec3 tmpvar_1;
  tmpvar_1 = normalize(_glesNormal);
  mediump vec2 tile_o_2;
  highp float index_3;
  highp float vy_4;
  highp float vx_5;
  highp vec4 tmpvar_6;
  mediump vec2 tmpvar_7;
  highp vec4 tmpvar_8;
  tmpvar_8 = (glstate_matrix_mvp * _glesVertex);
  highp float tmpvar_9;
  tmpvar_9 = (_glesVertex.x + 0.5);
  vx_5 = tmpvar_9;
  vy_4 = (_glesVertex.z + 0.5);
  if ((tmpvar_1.z > 0.2)) {
    vy_4 = (_glesVertex.y + 0.5);
  } else {
    if ((tmpvar_1.z < -0.2)) {
      vy_4 = -((_glesVertex.y + 0.5));
      vx_5 = (tmpvar_9 * -1.0);
    } else {
      if ((tmpvar_1.x > 0.2)) {
        vx_5 = -((_glesVertex.y + 0.5));
        vy_4 = (vy_4 * -1.0);
      } else {
        if ((tmpvar_1.x < -0.2)) {
          vx_5 = (_glesVertex.y + 0.5);
          vy_4 = (vy_4 * -1.0);
        } else {
          if ((tmpvar_1.y < -0.2)) {
            vx_5 = (vx_5 * -1.0);
          };
        };
      };
    };
  };
  mediump float tmpvar_10;
  tmpvar_10 = (_glesMultiTexCoord0.x * 16.0);
  index_3 = tmpvar_10;
  highp float tmpvar_11;
  tmpvar_11 = floor((index_3 / 4.0));
  highp vec2 tmpvar_12;
  tmpvar_12.x = (index_3 - (tmpvar_11 * 4.0));
  tmpvar_12.y = tmpvar_11;
  highp vec2 tmpvar_13;
  tmpvar_13 = (tmpvar_12 * 0.25);
  tile_o_2 = tmpvar_13;
  tmpvar_6.xy = tile_o_2;
  highp vec2 tmpvar_14;
  tmpvar_14.x = vx_5;
  tmpvar_14.y = vy_4;
  tmpvar_7 = tmpvar_14;
  gl_Position = tmpvar_8;
  xlv_COLOR = tmpvar_6;
  xlv_TEXCOORD0 = tmpvar_7;
}



#endif
#ifdef FRAGMENT

varying mediump vec2 xlv_TEXCOORD0;
varying highp vec4 xlv_COLOR;
uniform sampler2D _BlockTex;
void main ()
{
  mediump vec2 offset_1;
  highp vec2 tmpvar_2;
  tmpvar_2 = xlv_COLOR.xy;
  offset_1 = tmpvar_2;
  mediump vec2 tmpvar_3;
  tmpvar_3 = ((fract(xlv_TEXCOORD0) * 0.25) + offset_1);
  gl_FragData[0] = texture2D (_BlockTex, tmpvar_3);
}



#endif"
}

SubProgram "glesdesktop " {
Keywords { }
"!!GLES


#ifdef VERTEX

varying mediump vec2 xlv_TEXCOORD0;
varying highp vec4 xlv_COLOR;
uniform highp mat4 glstate_matrix_mvp;
attribute vec4 _glesMultiTexCoord0;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  vec3 tmpvar_1;
  tmpvar_1 = normalize(_glesNormal);
  mediump vec2 tile_o_2;
  highp float index_3;
  highp float vy_4;
  highp float vx_5;
  highp vec4 tmpvar_6;
  mediump vec2 tmpvar_7;
  highp vec4 tmpvar_8;
  tmpvar_8 = (glstate_matrix_mvp * _glesVertex);
  highp float tmpvar_9;
  tmpvar_9 = (_glesVertex.x + 0.5);
  vx_5 = tmpvar_9;
  vy_4 = (_glesVertex.z + 0.5);
  if ((tmpvar_1.z > 0.2)) {
    vy_4 = (_glesVertex.y + 0.5);
  } else {
    if ((tmpvar_1.z < -0.2)) {
      vy_4 = -((_glesVertex.y + 0.5));
      vx_5 = (tmpvar_9 * -1.0);
    } else {
      if ((tmpvar_1.x > 0.2)) {
        vx_5 = -((_glesVertex.y + 0.5));
        vy_4 = (vy_4 * -1.0);
      } else {
        if ((tmpvar_1.x < -0.2)) {
          vx_5 = (_glesVertex.y + 0.5);
          vy_4 = (vy_4 * -1.0);
        } else {
          if ((tmpvar_1.y < -0.2)) {
            vx_5 = (vx_5 * -1.0);
          };
        };
      };
    };
  };
  mediump float tmpvar_10;
  tmpvar_10 = (_glesMultiTexCoord0.x * 16.0);
  index_3 = tmpvar_10;
  highp float tmpvar_11;
  tmpvar_11 = floor((index_3 / 4.0));
  highp vec2 tmpvar_12;
  tmpvar_12.x = (index_3 - (tmpvar_11 * 4.0));
  tmpvar_12.y = tmpvar_11;
  highp vec2 tmpvar_13;
  tmpvar_13 = (tmpvar_12 * 0.25);
  tile_o_2 = tmpvar_13;
  tmpvar_6.xy = tile_o_2;
  highp vec2 tmpvar_14;
  tmpvar_14.x = vx_5;
  tmpvar_14.y = vy_4;
  tmpvar_7 = tmpvar_14;
  gl_Position = tmpvar_8;
  xlv_COLOR = tmpvar_6;
  xlv_TEXCOORD0 = tmpvar_7;
}



#endif
#ifdef FRAGMENT

varying mediump vec2 xlv_TEXCOORD0;
varying highp vec4 xlv_COLOR;
uniform sampler2D _BlockTex;
void main ()
{
  mediump vec2 offset_1;
  highp vec2 tmpvar_2;
  tmpvar_2 = xlv_COLOR.xy;
  offset_1 = tmpvar_2;
  mediump vec2 tmpvar_3;
  tmpvar_3 = ((fract(xlv_TEXCOORD0) * 0.25) + offset_1);
  gl_FragData[0] = texture2D (_BlockTex, tmpvar_3);
}



#endif"
}

SubProgram "flash " {
Keywords { }
Bind "vertex" Vertex
Bind "normal" Normal
Bind "texcoord" TexCoord0
Matrix 0 [glstate_matrix_mvp]
"agal_vs
c4 0.2 0.0 -0.2 0.5
c5 1.0 16.0 0.25 4.0
[bc]
ckaaaaaaaaaaacacaeaaaaaaabaaaaaaabaaaakkaaaaaaaa slt r0.y, c4.x, a1.z
ckaaaaaaabaaabacabaaaakkaaaaaaaaaeaaaakkabaaaaaa slt r1.x, a1.z, c4.z
cjaaaaaaaaaaaeacaeaaaaffabaaaaaaaaaaaaffacaaaaaa sge r0.z, c4.y, r0.y
cjaaaaaaaaaaabacaaaaaaffacaaaaaaaeaaaaffabaaaaaa sge r0.x, r0.y, c4.y
adaaaaaaaaaaaiacaaaaaaaaacaaaaaaaaaaaakkacaaaaaa mul r0.w, r0.x, r0.z
cjaaaaaaaaaaaeacaeaaaaffabaaaaaaabaaaaaaacaaaaaa sge r0.z, c4.y, r1.x
cjaaaaaaaaaaabacabaaaaaaacaaaaaaaeaaaaffabaaaaaa sge r0.x, r1.x, c4.y
adaaaaaaaaaaabacaaaaaaaaacaaaaaaaaaaaakkacaaaaaa mul r0.x, r0.x, r0.z
ckaaaaaaaaaaaeacaeaaaaaaabaaaaaaabaaaaaaaaaaaaaa slt r0.z, c4.x, a1.x
adaaaaaaaaaaabacaaaaaappacaaaaaaaaaaaaaaacaaaaaa mul r0.x, r0.w, r0.x
adaaaaaaabaaabacaaaaaappacaaaaaaabaaaaaaacaaaaaa mul r1.x, r0.w, r1.x
adaaaaaaabaaacacaaaaaaaaacaaaaaaaaaaaakkacaaaaaa mul r1.y, r0.x, r0.z
cjaaaaaaabaaaiacaeaaaaffabaaaaaaaaaaaakkacaaaaaa sge r1.w, c4.y, r0.z
bfaaaaaaacaaacacabaaaaffacaaaaaaaaaaaaaaaaaaaaaa neg r2.y, r1.y
ahaaaaaaaaaaaiacacaaaaffacaaaaaaabaaaaffacaaaaaa max r0.w, r2.y, r1.y
bfaaaaaaacaaaeacabaaaaaaacaaaaaaaaaaaaaaaaaaaaaa neg r2.z, r1.x
ahaaaaaaabaaabacacaaaakkacaaaaaaabaaaaaaacaaaaaa max r1.x, r2.z, r1.x
cjaaaaaaaaaaaeacaaaaaakkacaaaaaaaeaaaaffabaaaaaa sge r0.z, r0.z, c4.y
adaaaaaaaaaaaeacaaaaaakkacaaaaaaabaaaappacaaaaaa mul r0.z, r0.z, r1.w
ckaaaaaaabaaacacaeaaaaoeabaaaaaaabaaaaaaacaaaaaa slt r1.y, c4, r1.x
bfaaaaaaadaaacacaaaaaaffacaaaaaaaaaaaaaaaaaaaaaa neg r3.y, r0.y
ahaaaaaaaaaaacacadaaaaffacaaaaaaaaaaaaffacaaaaaa max r0.y, r3.y, r0.y
ckaaaaaaabaaabacaeaaaaffabaaaaaaaaaaaaffacaaaaaa slt r1.x, c4.y, r0.y
adaaaaaaaaaaabacaaaaaaaaacaaaaaaaaaaaakkacaaaaaa mul r0.x, r0.x, r0.z
bfaaaaaaadaaabacabaaaaaaacaaaaaaaaaaaaaaaaaaaaaa neg r3.x, r1.x
abaaaaaaaaaaacacadaaaaaaacaaaaaaafaaaaaaabaaaaaa add r0.y, r3.x, c5.x
abaaaaaaabaaaeacaaaaaaoeaaaaaaaaaeaaaappabaaaaaa add r1.z, a0, c4.w
adaaaaaaabaaaeacaaaaaaffacaaaaaaabaaaakkacaaaaaa mul r1.z, r0.y, r1.z
abaaaaaaaaaaacacaaaaaaoeaaaaaaaaaeaaaappabaaaaaa add r0.y, a0, c4.w
adaaaaaaadaaabacaaaaaaffacaaaaaaabaaaaaaacaaaaaa mul r3.x, r0.y, r1.x
abaaaaaaabaaabacadaaaaaaacaaaaaaabaaaakkacaaaaaa add r1.x, r3.x, r1.z
bfaaaaaaadaaacacabaaaaffacaaaaaaaaaaaaaaaaaaaaaa neg r3.y, r1.y
abaaaaaaabaaaeacadaaaaffacaaaaaaafaaaaaaabaaaaaa add r1.z, r3.y, c5.x
adaaaaaaabaaabacabaaaakkacaaaaaaabaaaaaaacaaaaaa mul r1.x, r1.z, r1.x
bfaaaaaaadaaacacaaaaaaffacaaaaaaaaaaaaaaaaaaaaaa neg r3.y, r0.y
adaaaaaaacaaabacadaaaaffacaaaaaaabaaaaffacaaaaaa mul r2.x, r3.y, r1.y
abaaaaaaacaaabacacaaaaaaacaaaaaaabaaaaaaacaaaaaa add r2.x, r2.x, r1.x
ckaaaaaaaaaaaiacaeaaaaffabaaaaaaaaaaaappacaaaaaa slt r0.w, c4.y, r0.w
bfaaaaaaadaaaiacaaaaaappacaaaaaaaaaaaaaaaaaaaaaa neg r3.w, r0.w
abaaaaaaabaaabacadaaaappacaaaaaaafaaaaoeabaaaaaa add r1.x, r3.w, c5
adaaaaaaacaaacacabaaaaaaacaaaaaaacaaaaaaacaaaaaa mul r2.y, r1.x, r2.x
bfaaaaaaadaaabacacaaaaaaacaaaaaaaaaaaaaaaaaaaaaa neg r3.x, r2.x
adaaaaaaabaaaiacaaaaaappacaaaaaaadaaaaaaacaaaaaa mul r1.w, r0.w, r3.x
abaaaaaaabaaaiacabaaaappacaaaaaaacaaaaffacaaaaaa add r1.w, r1.w, r2.y
ckaaaaaaacaaabacabaaaaoeaaaaaaaaaeaaaakkabaaaaaa slt r2.x, a1, c4.z
abaaaaaaacaaacacaaaaaaaaaaaaaaaaaeaaaappabaaaaaa add r2.y, a0.x, c4.w
adaaaaaaaaaaaeacaaaaaaaaacaaaaaaacaaaaaaacaaaaaa mul r0.z, r0.x, r2.x
adaaaaaaabaaaeacacaaaaffacaaaaaaabaaaakkacaaaaaa mul r1.z, r2.y, r1.z
bfaaaaaaadaaacacacaaaaffacaaaaaaaaaaaaaaaaaaaaaa neg r3.y, r2.y
adaaaaaaadaaacacabaaaaffacaaaaaaadaaaaffacaaaaaa mul r3.y, r1.y, r3.y
abaaaaaaabaaacacadaaaaffacaaaaaaabaaaakkacaaaaaa add r1.y, r3.y, r1.z
adaaaaaaabaaacacabaaaaaaacaaaaaaabaaaaffacaaaaaa mul r1.y, r1.x, r1.y
bfaaaaaaadaaaeacaaaaaakkacaaaaaaaaaaaaaaaaaaaaaa neg r3.z, r0.z
ahaaaaaaaaaaaeacadaaaakkacaaaaaaaaaaaakkacaaaaaa max r0.z, r3.z, r0.z
ckaaaaaaaaaaaeacaeaaaaffabaaaaaaaaaaaakkacaaaaaa slt r0.z, c4.y, r0.z
bfaaaaaaadaaaeacaaaaaakkacaaaaaaaaaaaaaaaaaaaaaa neg r3.z, r0.z
abaaaaaaabaaabacadaaaakkacaaaaaaafaaaaoeabaaaaaa add r1.x, r3.z, c5
bfaaaaaaadaaacacaaaaaaffacaaaaaaaaaaaaaaaaaaaaaa neg r3.y, r0.y
adaaaaaaaaaaaiacaaaaaappacaaaaaaadaaaaffacaaaaaa mul r0.w, r0.w, r3.y
abaaaaaaaaaaaiacaaaaaappacaaaaaaabaaaaffacaaaaaa add r0.w, r0.w, r1.y
adaaaaaaabaaacacabaaaaaaacaaaaaaaaaaaappacaaaaaa mul r1.y, r1.x, r0.w
adaaaaaaabaaaeacabaaaaaaacaaaaaaabaaaappacaaaaaa mul r1.z, r1.x, r1.w
cjaaaaaaabaaabacaeaaaaffabaaaaaaacaaaaaaacaaaaaa sge r1.x, c4.y, r2.x
cjaaaaaaaaaaaiacacaaaaaaacaaaaaaaeaaaaffabaaaaaa sge r0.w, r2.x, c4.y
adaaaaaaaaaaaiacaaaaaappacaaaaaaabaaaaaaacaaaaaa mul r0.w, r0.w, r1.x
adaaaaaaaaaaabacaaaaaaaaacaaaaaaaaaaaappacaaaaaa mul r0.x, r0.x, r0.w
ckaaaaaaabaaabacabaaaaffaaaaaaaaaeaaaakkabaaaaaa slt r1.x, a1.y, c4.z
adaaaaaaaaaaabacaaaaaaaaacaaaaaaabaaaaaaacaaaaaa mul r0.x, r0.x, r1.x
adaaaaaaadaaabacaaaaaakkacaaaaaaaaaaaaffacaaaaaa mul r3.x, r0.z, r0.y
abaaaaaaabaaabacadaaaaaaacaaaaaaabaaaaffacaaaaaa add r1.x, r3.x, r1.y
bfaaaaaaadaaabacaaaaaaaaacaaaaaaaaaaaaaaaaaaaaaa neg r3.x, r0.x
ahaaaaaaaaaaacacadaaaaaaacaaaaaaaaaaaaaaacaaaaaa max r0.y, r3.x, r0.x
ckaaaaaaaaaaaiacaeaaaaffabaaaaaaaaaaaaffacaaaaaa slt r0.w, c4.y, r0.y
adaaaaaaaaaaabacadaaaaoeaaaaaaaaafaaaaffabaaaaaa mul r0.x, a3, c5.y
bfaaaaaaadaaaiacaaaaaappacaaaaaaaaaaaaaaaaaaaaaa neg r3.w, r0.w
abaaaaaaabaaacacadaaaappacaaaaaaafaaaaaaabaaaaaa add r1.y, r3.w, c5.x
adaaaaaaabaaacacabaaaaaaacaaaaaaabaaaaffacaaaaaa mul r1.y, r1.x, r1.y
adaaaaaaaaaaacacaaaaaaaaacaaaaaaafaaaakkabaaaaaa mul r0.y, r0.x, c5.z
bfaaaaaaadaaaiacabaaaappacaaaaaaaaaaaaaaaaaaaaaa neg r3.w, r1.w
adaaaaaaadaaacacaaaaaakkacaaaaaaadaaaappacaaaaaa mul r3.y, r0.z, r3.w
abaaaaaaaaaaacaeadaaaaffacaaaaaaabaaaakkacaaaaaa add v0.y, r3.y, r1.z
aiaaaaaaaaaaaeacaaaaaaffacaaaaaaaaaaaaaaaaaaaaaa frc r0.z, r0.y
acaaaaaaaaaaaeacaaaaaaffacaaaaaaaaaaaakkacaaaaaa sub r0.z, r0.y, r0.z
aaaaaaaaaaaaacacaaaaaakkacaaaaaaaaaaaaaaaaaaaaaa mov r0.y, r0.z
bfaaaaaaadaaaeacaaaaaakkacaaaaaaaaaaaaaaaaaaaaaa neg r3.z, r0.z
adaaaaaaadaaabacadaaaakkacaaaaaaafaaaappabaaaaaa mul r3.x, r3.z, c5.w
abaaaaaaaaaaabacadaaaaaaacaaaaaaaaaaaaaaacaaaaaa add r0.x, r3.x, r0.x
bfaaaaaaadaaabacabaaaaaaacaaaaaaaaaaaaaaaaaaaaaa neg r3.x, r1.x
adaaaaaaadaaabacaaaaaappacaaaaaaadaaaaaaacaaaaaa mul r3.x, r0.w, r3.x
abaaaaaaaaaaabaeadaaaaaaacaaaaaaabaaaaffacaaaaaa add v0.x, r3.x, r1.y
adaaaaaaahaaadaeaaaaaafeacaaaaaaafaaaakkabaaaaaa mul v7.xy, r0.xyyy, c5.z
bdaaaaaaaaaaaiadaaaaaaoeaaaaaaaaadaaaaoeabaaaaaa dp4 o0.w, a0, c3
bdaaaaaaaaaaaeadaaaaaaoeaaaaaaaaacaaaaoeabaaaaaa dp4 o0.z, a0, c2
bdaaaaaaaaaaacadaaaaaaoeaaaaaaaaabaaaaoeabaaaaaa dp4 o0.y, a0, c1
bdaaaaaaaaaaabadaaaaaaoeaaaaaaaaaaaaaaoeabaaaaaa dp4 o0.x, a0, c0
aaaaaaaaaaaaamaeaaaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov v0.zw, c0
aaaaaaaaahaaamaeaaaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov v7.zw, c0
"
}

SubProgram "d3d11_9x " {
Keywords { }
Bind "vertex" Vertex
Bind "normal" Normal
Bind "texcoord" TexCoord0
ConstBuffer "UnityPerDraw" 336 // 64 used size, 6 vars
Matrix 0 [glstate_matrix_mvp] 4
BindCB "UnityPerDraw" 0
// 19 instructions, 4 temp regs, 0 temp arrays:
// ALU 8 float, 0 int, 0 uint
// TEX 0 (0 load, 0 comp, 0 bias, 0 grad)
// FLOW 1 static, 0 dynamic
"vs_4_0_level_9_1
eefiecedbfajmdgkojphhkgbepcniimaaafedpngabaaaaaaiiagaaaaaeaaaaaa
daaaaaaaheacaaaakaafaaaabeagaaaaebgpgodjdmacaaaadmacaaaaaaacpopp
aiacaaaadeaaaaaaabaaceaaaaaadaaaaaaadaaaaaaaceaaabaadaaaaaaaaaaa
aeaaabaaaaaaaaaaaaaaaaaaaaacpoppfbaaaaafafaaapkaaaaaaadpmnmmemdo
mnmmemloaaaaaamafbaaaaafagaaapkaaaaaiaebaaaaiaeaaaaaiadoaaaaaaaa
fbaaaaafahaaapkaaaaaiadpaaaaialpaaaaaadpaaaaaalpbpaaaaacafaaaaia
aaaaapjabpaaaaacafaaabiaabaaapjabpaaaaacafaaaciaacaaapjaacaaaaad
aaaaadiaaaaaojjbafaaaakbaeaaaaaeaaaaamiaaaaajejaahaaeekaahaaoeka
amaaaaadabaaahiaabaancjaafaakkkaacaaaaadacaaaliaaaaakejaafaaaaka
afaaaaadabaaaeiaabaakkiaacaaaaiaaeaaaaaeacaaaeiaabaakkiaafaappka
acaaaaiabcaaaaaeadaaadiaabaaffiaaaaaooiaacaaooiaamaaaaadaaaaamia
afaaffkaabaacejabcaaaaaeabaaagiaaaaappiaaaaanaiaadaanaiabcaaaaae
aaaaadiaabaaaaiaacaaoeibabaaojiaacaaaaadabaaadiaaaaaoeibacaaoeia
aeaaaaaeabaaadoaaaaakkiaabaaoeiaaaaaoeiaafaaaaadaaaaadiaacaaaaja
agaaoekabdaaaaacaaaaaeiaaaaaffiaacaaaaadabaaaciaaaaakkibaaaaffia
aeaaaaaeabaaabiaabaaffiaagaaffkbaaaaaaiaafaaaaadaaaaadoaabaaoeia
agaakkkaafaaaaadaaaaapiaaaaaffjaacaaoekaaeaaaaaeaaaaapiaabaaoeka
aaaaaajaaaaaoeiaaeaaaaaeaaaaapiaadaaoekaaaaakkjaaaaaoeiaaeaaaaae
aaaaapiaaeaaoekaaaaappjaaaaaoeiaaeaaaaaeaaaaadmaaaaappiaaaaaoeka
aaaaoeiaabaaaaacaaaaammaaaaaoeiappppaaaafdeieefcceadaaaaeaaaabaa
mjaaaaaafjaaaaaeegiocaaaaaaaaaaaaeaaaaaafpaaaaadpcbabaaaaaaaaaaa
fpaaaaadhcbabaaaabaaaaaafpaaaaadbcbabaaaacaaaaaaghaaaaaepccabaaa
aaaaaaaaabaaaaaagfaaaaaddccabaaaabaaaaaagfaaaaaddccabaaaacaaaaaa
giaaaaacaeaaaaaadiaaaaaipcaabaaaaaaaaaaafgbfbaaaaaaaaaaaegiocaaa
aaaaaaaaabaaaaaadcaaaaakpcaabaaaaaaaaaaaegiocaaaaaaaaaaaaaaaaaaa
agbabaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaakpcaabaaaaaaaaaaaegiocaaa
aaaaaaaaacaaaaaakgbkbaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaakpccabaaa
aaaaaaaaegiocaaaaaaaaaaaadaaaaaapgbpbaaaaaaaaaaaegaobaaaaaaaaaaa
diaaaaakdcaabaaaaaaaaaaaagbabaaaacaaaaaaaceaaaaaaaaaiaebaaaaiaea
aaaaaaaaaaaaaaaaebaaaaafccaabaaaabaaaaaabkaabaaaaaaaaaaadcaaaaak
bcaabaaaabaaaaaabkaabaiaebaaaaaaabaaaaaaabeaaaaaaaaaiaeaakaabaaa
aaaaaaaadiaaaaakdccabaaaabaaaaaaegaabaaaabaaaaaaaceaaaaaaaaaiado
aaaaiadoaaaaaaaaaaaaaaaadcaaaaapdcaabaaaaaaaaaaajgbfbaaaaaaaaaaa
aceaaaaaaaaaiadpaaaaialpaaaaaaaaaaaaaaaaaceaaaaaaaaaaadpaaaaaalp
aaaaaaaaaaaaaaaaaaaaaaahccaabaaaabaaaaaackbabaaaaaaaaaaaabeaaaaa
aaaaaadpdbaaaaakhcaabaaaacaaaaaacgbjbaaaabaaaaaaaceaaaaamnmmemlo
mnmmemlomnmmemloaaaaaaaaaaaaaaakhcaabaaaadaaaaaabgbgbaaaaaaaaaaa
aceaaaaaaaaaaadpaaaaaadpaaaaaadpaaaaaaaadhaaaaakbcaabaaaabaaaaaa
ckaabaaaacaaaaaabkaabaiaebaaaaaaadaaaaaabkaabaaaadaaaaaadhaaaaaj
dcaabaaaaaaaaaaafgafbaaaacaaaaaaegaabaaaaaaaaaaaegaabaaaabaaaaaa
dbaaaaakmcaabaaaaaaaaaaaaceaaaaaaaaaaaaaaaaaaaaamnmmemdomnmmemdo
kgbcbaaaabaaaaaadhaaaaakdcaabaaaaaaaaaaapgapbaaaaaaaaaaaigaabaia
ebaaaaaaadaaaaaaegaabaaaaaaaaaaadhaaaaakdcaabaaaaaaaaaaaagaabaaa
acaaaaaabgafbaiaebaaaaaaadaaaaaaegaabaaaaaaaaaaadhaaaaajdccabaaa
acaaaaaakgakbaaaaaaaaaaabgafbaaaadaaaaaaegaabaaaaaaaaaaadoaaaaab
ejfdeheogmaaaaaaadaaaaaaaiaaaaaafaaaaaaaaaaaaaaaaaaaaaaaadaaaaaa
aaaaaaaaapapaaaafjaaaaaaaaaaaaaaaaaaaaaaadaaaaaaabaaaaaaahahaaaa
gaaaaaaaaaaaaaaaaaaaaaaaadaaaaaaacaaaaaaadabaaaafaepfdejfeejepeo
aaeoepfcenebemaafeeffiedepepfceeaaklklklepfdeheogmaaaaaaadaaaaaa
aiaaaaaafaaaaaaaaaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaafmaaaaaa
aaaaaaaaaaaaaaaaadaaaaaaabaaaaaaapamaaaagcaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaacaaaaaaadamaaaafdfgfpfaepfdejfeejepeoaaedepemepfcaafeef
fiedepepfceeaakl"
}

SubProgram "gles3 " {
Keywords { }
"!!GLES3#version 300 es


#ifdef VERTEX

#define gl_Vertex _glesVertex
in vec4 _glesVertex;
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
#line 314
struct v2f {
    highp vec4 pos;
    highp vec4 color;
    mediump vec2 uv;
};
#line 307
struct appdata {
    highp vec4 vertex;
    highp vec3 normal;
    mediump vec2 texcoord;
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
#line 321
#line 357
#line 321
v2f vert( in appdata v ) {
    v2f o;
    o.pos = (glstate_matrix_mvp * v.vertex);
    #line 325
    highp float vx = (v.vertex.x + 0.5);
    highp float vy = (v.vertex.z + 0.5);
    if ((v.normal.z > 0.2)){
        #line 329
        vy = (v.vertex.y + 0.5);
    }
    else{
        if ((v.normal.z < -0.2)){
            #line 333
            vy = (-(v.vertex.y + 0.5));
            vx *= -1.0;
        }
        else{
            if ((v.normal.x > 0.2)){
                #line 338
                vx = (-(v.vertex.y + 0.5));
                vy *= -1.0;
            }
            else{
                if ((v.normal.x < -0.2)){
                    #line 343
                    vx = (v.vertex.y + 0.5);
                    vy *= -1.0;
                }
                else{
                    if ((v.normal.y < -0.2)){
                        #line 348
                        vx *= -1.0;
                    }
                }
            }
        }
    }
    highp float index = (v.texcoord.x * 16.0);
    highp float blocky = floor((index / 4.0));
    #line 352
    mediump vec2 tile_o = (vec2( (index - (blocky * 4.0)), blocky) * 0.25);
    o.color.xy = tile_o;
    o.uv = vec2( vx, vy);
    return o;
}
out highp vec4 xlv_COLOR;
out mediump vec2 xlv_TEXCOORD0;
void main() {
    v2f xl_retval;
    appdata xlt_v;
    xlt_v.vertex = vec4(gl_Vertex);
    xlt_v.normal = vec3(gl_Normal);
    xlt_v.texcoord = vec2(gl_MultiTexCoord0);
    xl_retval = vert( xlt_v);
    gl_Position = vec4(xl_retval.pos);
    xlv_COLOR = vec4(xl_retval.color);
    xlv_TEXCOORD0 = vec2(xl_retval.uv);
}


#endif
#ifdef FRAGMENT

#define gl_FragData _glesFragData
layout(location = 0) out mediump vec4 _glesFragData[4];

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
#line 314
struct v2f {
    highp vec4 pos;
    highp vec4 color;
    mediump vec2 uv;
};
#line 307
struct appdata {
    highp vec4 vertex;
    highp vec3 normal;
    mediump vec2 texcoord;
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
#line 321
#line 357
#line 357
lowp vec4 frag( in v2f i ) {
    mediump vec2 fluv = fract(i.uv);
    fluv = (fluv * 0.25);
    #line 361
    mediump vec2 offset = i.color.xy;
    fluv = (fluv + offset);
    return texture( _BlockTex, fluv);
}
in highp vec4 xlv_COLOR;
in mediump vec2 xlv_TEXCOORD0;
void main() {
    lowp vec4 xl_retval;
    v2f xlt_i;
    xlt_i.pos = vec4(0.0);
    xlt_i.color = vec4(xlv_COLOR);
    xlt_i.uv = vec2(xlv_TEXCOORD0);
    xl_retval = frag( xlt_i);
    gl_FragData[0] = vec4(xl_retval);
}


#endif"
}

}
Program "fp" {
// Fragment combos: 1
//   opengl - ALU: 3 to 3, TEX: 1 to 1
//   d3d9 - ALU: 3 to 3, TEX: 1 to 1
//   d3d11 - ALU: 1 to 1, TEX: 1 to 1, FLOW: 1 to 1
//   d3d11_9x - ALU: 1 to 1, TEX: 1 to 1, FLOW: 1 to 1
SubProgram "opengl " {
Keywords { }
SetTexture 0 [_BlockTex] 2D
"!!ARBfp1.0
# 3 ALU, 1 TEX
PARAM c[1] = { { 0.25 } };
TEMP R0;
FRC R0.xy, fragment.texcoord[0];
MAD R0.xy, R0, c[0].x, fragment.color.primary;
TEX result.color, R0, texture[0], 2D;
END
# 3 instructions, 1 R-regs
"
}

SubProgram "d3d9 " {
Keywords { }
SetTexture 0 [_BlockTex] 2D
"ps_2_0
; 3 ALU, 1 TEX
dcl_2d s0
def c0, 0.25000000, 0, 0, 0
dcl v0.xy
dcl t0.xy
frc_pp r0.xy, t0
mad_pp r0.xy, r0, c0.x, v0
texld r0, r0, s0
mov_pp oC0, r0
"
}

SubProgram "d3d11 " {
Keywords { }
SetTexture 0 [_BlockTex] 2D 0
// 4 instructions, 1 temp regs, 0 temp arrays:
// ALU 1 float, 0 int, 0 uint
// TEX 1 (0 load, 0 comp, 0 bias, 0 grad)
// FLOW 1 static, 0 dynamic
"ps_4_0
eefiecediblkbnljjdhdgkpkehpdfnibclphjnjjabaaaaaajiabaaaaadaaaaaa
cmaaaaaakaaaaaaaneaaaaaaejfdeheogmaaaaaaadaaaaaaaiaaaaaafaaaaaaa
aaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaafmaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaapadaaaagcaaaaaaaaaaaaaaaaaaaaaaadaaaaaaacaaaaaa
adadaaaafdfgfpfaepfdejfeejepeoaaedepemepfcaafeeffiedepepfceeaakl
epfdeheocmaaaaaaabaaaaaaaiaaaaaacaaaaaaaaaaaaaaaaaaaaaaaadaaaaaa
aaaaaaaaapaaaaaafdfgfpfegbhcghgfheaaklklfdeieefclmaaaaaaeaaaaaaa
cpaaaaaafkaaaaadaagabaaaaaaaaaaafibiaaaeaahabaaaaaaaaaaaffffaaaa
gcbaaaaddcbabaaaabaaaaaagcbaaaaddcbabaaaacaaaaaagfaaaaadpccabaaa
aaaaaaaagiaaaaacabaaaaaabkaaaaafdcaabaaaaaaaaaaaegbabaaaacaaaaaa
dcaaaaamdcaabaaaaaaaaaaaegaabaaaaaaaaaaaaceaaaaaaaaaiadoaaaaiado
aaaaaaaaaaaaaaaaegbabaaaabaaaaaaefaaaaajpccabaaaaaaaaaaaegaabaaa
aaaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaadoaaaaab"
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
c0 0.25 0.0 0.0 0.0
[bc]
aiaaaaaaaaaaadacaaaaaaoeaeaaaaaaaaaaaaaaaaaaaaaa frc r0.xy, v0
adaaaaaaaaaaadacaaaaaafeacaaaaaaaaaaaaaaabaaaaaa mul r0.xy, r0.xyyy, c0.x
abaaaaaaaaaaadacaaaaaafeacaaaaaaahaaaaoeaeaaaaaa add r0.xy, r0.xyyy, v7
ciaaaaaaaaaaapacaaaaaafeacaaaaaaaaaaaaaaafaababb tex r0, r0.xyyy, s0 <2d wrap linear point>
aaaaaaaaaaaaapadaaaaaaoeacaaaaaaaaaaaaaaaaaaaaaa mov o0, r0
"
}

SubProgram "d3d11_9x " {
Keywords { }
SetTexture 0 [_BlockTex] 2D 0
// 4 instructions, 1 temp regs, 0 temp arrays:
// ALU 1 float, 0 int, 0 uint
// TEX 1 (0 load, 0 comp, 0 bias, 0 grad)
// FLOW 1 static, 0 dynamic
"ps_4_0_level_9_1
eefiecedbcehmmpndekkanbhkaedkdefdaogebbeabaaaaaaemacaaaaaeaaaaaa
daaaaaaaoaaaaaaakeabaaaabiacaaaaebgpgodjkiaaaaaakiaaaaaaaaacpppp
iaaaaaaaciaaaaaaaaaaciaaaaaaciaaaaaaciaaabaaceaaaaaaciaaaaaaaaaa
aaacppppfbaaaaafaaaaapkaaaaaiadoaaaaaaaaaaaaaaaaaaaaaaaabpaaaaac
aaaaaaiaaaaacplabpaaaaacaaaaaaiaabaacdlabpaaaaacaaaaaajaaaaiapka
bdaaaaacaaaacdiaabaaoelaaeaaaaaeaaaacdiaaaaaoeiaaaaaaakaaaaaoela
ecaaaaadaaaacpiaaaaaoeiaaaaioekaabaaaaacaaaicpiaaaaaoeiappppaaaa
fdeieefclmaaaaaaeaaaaaaacpaaaaaafkaaaaadaagabaaaaaaaaaaafibiaaae
aahabaaaaaaaaaaaffffaaaagcbaaaaddcbabaaaabaaaaaagcbaaaaddcbabaaa
acaaaaaagfaaaaadpccabaaaaaaaaaaagiaaaaacabaaaaaabkaaaaafdcaabaaa
aaaaaaaaegbabaaaacaaaaaadcaaaaamdcaabaaaaaaaaaaaegaabaaaaaaaaaaa
aceaaaaaaaaaiadoaaaaiadoaaaaaaaaaaaaaaaaegbabaaaabaaaaaaefaaaaaj
pccabaaaaaaaaaaaegaabaaaaaaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaa
doaaaaabejfdeheogmaaaaaaadaaaaaaaiaaaaaafaaaaaaaaaaaaaaaabaaaaaa
adaaaaaaaaaaaaaaapaaaaaafmaaaaaaaaaaaaaaaaaaaaaaadaaaaaaabaaaaaa
apadaaaagcaaaaaaaaaaaaaaaaaaaaaaadaaaaaaacaaaaaaadadaaaafdfgfpfa
epfdejfeejepeoaaedepemepfcaafeeffiedepepfceeaaklepfdeheocmaaaaaa
abaaaaaaaiaaaaaacaaaaaaaaaaaaaaaaaaaaaaaadaaaaaaaaaaaaaaapaaaaaa
fdfgfpfegbhcghgfheaaklkl"
}

SubProgram "gles3 " {
Keywords { }
"!!GLES3"
}

}

#LINE 110

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
