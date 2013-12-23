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
//   opengl - ALU: 70 to 70
//   d3d9 - ALU: 111 to 111
//   d3d11 - ALU: 12 to 12, TEX: 0 to 0, FLOW: 1 to 1
//   d3d11_9x - ALU: 12 to 12, TEX: 0 to 0, FLOW: 1 to 1
SubProgram "opengl " {
Keywords { }
Bind "vertex" Vertex
Bind "normal" Normal
"!!ARBvp1.0
# 70 ALU
PARAM c[11] = { { 0.69999999, 1, 0, 0.0625 },
		state.matrix.mvp,
		state.matrix.texture[0],
		{ 33.5, 0.5, 0.25, 0.89999998 },
		{ -0.89999998, 16 } };
TEMP R0;
TEMP R1;
TEMP R2;
MUL R0.x, vertex.position.y, c[9];
ADD R0.y, R0.x, c[9];
MUL R1.y, vertex.position.x, c[9].x;
ABS R0.z, R0.y;
FLR R0.z, R0;
SLT R0.w, R0.x, -c[9].y;
ADD R0.x, -R0.z, -R0.z;
MAD R0.x, R0, R0.w, R0.z;
ADD R0.w, R1.y, c[9].y;
ADD R0.x, R0.y, -R0;
MUL R1.x, vertex.position.z, c[9];
ADD R0.y, R1.x, c[9];
MUL R0.x, R0, c[9].z;
ABS R1.z, R0.w;
FLR R1.z, R1;
SLT R1.w, R1.y, -c[9].y;
ADD R1.y, -R1.z, -R1.z;
MAD R1.y, R1, R1.w, R1.z;
ABS R2.x, R0.y;
ADD R0.w, R0, -R1.y;
FLR R1.z, R2.x;
SLT R1.w, R1.x, -c[9].y;
ADD R1.x, -R1.z, -R1.z;
MAD R1.x, R1, R1.w, R1.z;
ADD R0.y, R0, -R1.x;
MUL R1.x, R0.w, c[9].z;
MUL R0.y, R0, c[9].z;
MOV R1.y, R0;
ADD R0.z, R0.x, c[9];
MOV R0.w, R1.x;
ADD R0.zw, R0, -R1.xyxy;
SLT R1.w, vertex.normal.z, c[10].x;
SLT R1.z, c[9].w, vertex.normal;
ADD R1.z, R1, R1.w;
MAD R0.zw, R0, R1.z, R1.xyxy;
ABS R1.x, R1.z;
ADD R0.y, R0, c[9].z;
ADD R0.xy, R0, -R0.zwzw;
SLT R1.z, vertex.normal.x, c[10].x;
SLT R1.y, c[9].w, vertex.normal.x;
ADD R1.y, R1, R1.z;
SGE R1.x, c[0].z, R1;
MUL R1.x, R1, R1.y;
MAD R1.xy, R0, R1.x, R0.zwzw;
MOV R1.zw, c[0].z;
FLR R0.y, vertex.position.x;
FLR R0.x, vertex.position.z;
MUL R0.z, R0.y, c[0].w;
MUL R0.w, R0.x, c[0];
ABS R0.z, R0;
FRC R0.z, R0;
MUL R0.z, R0, c[10].y;
ABS R0.w, R0;
DP4 result.texcoord[0].y, R1, c[6];
DP4 result.texcoord[0].x, R1, c[5];
FRC R1.x, R0.w;
SLT R0.w, R0.y, c[0].z;
ADD R0.y, -R0.z, -R0.z;
MAD R0.y, R0, R0.w, R0.z;
MUL R0.z, R1.x, c[10].y;
SLT R0.w, R0.x, c[0].z;
ADD R0.x, -R0.z, -R0.z;
MAD R0.x, R0, R0.w, R0.z;
MUL result.color.x, R0.y, c[0].w;
MUL result.color.z, R0.x, c[0].w;
MOV result.color.yw, c[0].xxzy;
DP4 result.position.w, vertex.position, c[4];
DP4 result.position.z, vertex.position, c[3];
DP4 result.position.y, vertex.position, c[2];
DP4 result.position.x, vertex.position, c[1];
END
# 70 instructions, 3 R-regs
"
}

SubProgram "d3d9 " {
Keywords { }
Bind "vertex" Vertex
Bind "normal" Normal
Matrix 0 [glstate_matrix_mvp]
Matrix 4 [glstate_matrix_texture0]
"vs_2_0
; 111 ALU
def c8, 33.50000000, 0.50000000, -0.50000000, 0.00000000
def c9, 1.00000000, 0.25000000, 0.89999998, -0.89999998
def c10, 0.06250000, 16.00000000, 0.69999999, 1.00000000
dcl_position0 v0
dcl_normal0 v1
mul r0.y, v0.x, c8.x
add r0.x, r0.y, c8.y
abs r0.z, r0.x
frc r0.w, r0.z
mul r1.x, v0.z, c8
slt r1.y, r1.x, c8.z
add r1.x, r1, c8.y
abs r1.z, r1.x
frc r1.w, r1.z
slt r0.y, r0, c8.z
max r0.y, -r0, r0
max r1.y, -r1, r1
slt r1.y, c8.w, r1
add r1.z, r1, -r1.w
add r2.x, -r1.y, c9
add r0.z, r0, -r0.w
slt r0.y, c8.w, r0
add r0.w, -r0.y, c9.x
mul r0.w, r0.z, r0
mad r0.z, r0.y, -r0, r0.w
add r0.x, r0, -r0.z
mul r1.w, r1.z, r2.x
mad r0.y, r1, -r1.z, r1.w
add r0.y, r1.x, -r0
mul r2.x, r0.y, c9.y
mul r0.z, v0.y, c8.x
add r1.w, r0.z, c8.y
mul r0.x, r0, c9.y
slt r2.y, v1.z, c9.w
slt r2.z, c9, v1
add r2.w, r2.z, r2.y
max r0.w, -r2, r2
slt r3.x, c8.w, r0.w
slt r0.w, r0.z, c8.z
abs r0.z, r1.w
frc r1.x, r0.z
add r1.y, r0.z, -r1.x
max r0.w, -r0, r0
slt r1.z, c8.w, r0.w
add r0.w, -r1.z, c9.x
mul r1.x, r1.y, r0.w
mad r1.x, r1.z, -r1.y, r1
add r1.x, r1.w, -r1
mov r0.y, r2.x
add r0.z, -r3.x, c9.x
mul r0.zw, r0.z, r0.xyxy
sge r1.z, c8.w, r2.w
slt r1.y, v1.x, c9.w
slt r0.y, c9.z, v1.x
add r0.y, r0, r1
sge r1.y, r2.z, -r2
mul r1.y, r1, r1.z
mul r0.y, r1, r0
mul r1.z, r1.x, c9.y
max r0.y, -r0, r0
slt r0.y, c8.w, r0
mov r1.y, r0.x
add r1.x, r1.z, c9.y
mad r0.zw, r3.x, r1.xyxy, r0
add r0.x, -r0.y, c9
mul r0.zw, r0.x, r0
frc r0.x, v0.z
add r1.x, v0.z, -r0
mul r1.x, r1, c10
add r1.w, r2.x, c9.y
mad r2.xy, r0.y, r1.zwzw, r0.zwzw
mov r2.zw, c8.w
frc r0.y, v0.x
add r0.z, v0.x, -r0.y
slt r0.y, v0.x, r0
mul r0.z, r0, c10.x
max r0.y, -r0, r0
slt r0.y, c8.w, r0
abs r0.z, r0
frc r0.z, r0
slt r0.x, v0.z, r0
abs r1.y, r1.x
max r0.x, -r0, r0
slt r1.x, c8.w, r0
frc r0.x, r1.y
mul r1.y, r0.x, c10
add r1.z, -r1.x, c9.x
add r0.w, -r0.y, c9.x
mul r0.z, r0, c10.y
mul r0.w, r0.z, r0
mad r0.x, r0.y, -r0.z, r0.w
mul r1.z, r1.y, r1
mad r0.y, r1.x, -r1, r1.z
dp4 oT0.y, r2, c5
dp4 oT0.x, r2, c4
mul oD0.x, r0, c10
mul oD0.z, r0.y, c10.x
mov oD0.yw, c10.xzzw
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
ConstBuffer "UnityPerDraw" 336 // 64 used size, 6 vars
Matrix 0 [glstate_matrix_mvp] 4
ConstBuffer "UnityPerDrawTexMatrices" 768 // 576 used size, 5 vars
Matrix 512 [glstate_matrix_texture0] 4
BindCB "UnityPerDraw" 0
BindCB "UnityPerDrawTexMatrices" 1
// 24 instructions, 3 temp regs, 0 temp arrays:
// ALU 11 float, 0 int, 1 uint
// TEX 0 (0 load, 0 comp, 0 bias, 0 grad)
// FLOW 1 static, 0 dynamic
"vs_4_0
eefiecedagheakfgcjdcfljmcanadigfmaachbgmabaaaaaaniaeaaaaadaaaaaa
cmaaaaaakaaaaaaabeabaaaaejfdeheogmaaaaaaadaaaaaaaiaaaaaafaaaaaaa
aaaaaaaaaaaaaaaaadaaaaaaaaaaaaaaapapaaaafjaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaahafaaaagaaaaaaaaaaaaaaaaaaaaaaaadaaaaaaacaaaaaa
adaaaaaafaepfdejfeejepeoaaeoepfcenebemaafeeffiedepepfceeaaklklkl
epfdeheogmaaaaaaadaaaaaaaiaaaaaafaaaaaaaaaaaaaaaabaaaaaaadaaaaaa
aaaaaaaaapaaaaaafmaaaaaaaaaaaaaaaaaaaaaaadaaaaaaabaaaaaaapaaaaaa
gcaaaaaaaaaaaaaaaaaaaaaaadaaaaaaacaaaaaaadamaaaafdfgfpfaepfdejfe
ejepeoaaedepemepfcaafeeffiedepepfceeaaklfdeieefclmadaaaaeaaaabaa
opaaaaaafjaaaaaeegiocaaaaaaaaaaaaeaaaaaafjaaaaaeegiocaaaabaaaaaa
ccaaaaaafpaaaaadpcbabaaaaaaaaaaafpaaaaadfcbabaaaabaaaaaaghaaaaae
pccabaaaaaaaaaaaabaaaaaagfaaaaadpccabaaaabaaaaaagfaaaaaddccabaaa
acaaaaaagiaaaaacadaaaaaadiaaaaaipcaabaaaaaaaaaaafgbfbaaaaaaaaaaa
egiocaaaaaaaaaaaabaaaaaadcaaaaakpcaabaaaaaaaaaaaegiocaaaaaaaaaaa
aaaaaaaaagbabaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaakpcaabaaaaaaaaaaa
egiocaaaaaaaaaaaacaaaaaakgbkbaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaak
pccabaaaaaaaaaaaegiocaaaaaaaaaaaadaaaaaapgbpbaaaaaaaaaaaegaobaaa
aaaaaaaaebaaaaafdcaabaaaaaaaaaaaigbabaaaaaaaaaaadiaaaaakdcaabaaa
aaaaaaaaegaabaaaaaaaaaaaaceaaaaaaaaaiadnaaaaiadnaaaaaaaaaaaaaaaa
bnaaaaaimcaabaaaaaaaaaaaagaebaaaaaaaaaaaagaebaiaebaaaaaaaaaaaaaa
bkaaaaagdcaabaaaaaaaaaaaegaabaiaibaaaaaaaaaaaaaadhaaaaakfccabaaa
abaaaaaakgalbaaaaaaaaaaaagabbaaaaaaaaaaaagabbaiaebaaaaaaaaaaaaaa
dgaaaaaikccabaaaabaaaaaaaceaaaaaaaaaaaaadddddddpaaaaaaaaaaaaiadp
dbaaaaakdcaabaaaaaaaaaaaaceaaaaaggggggdpggggggdpaaaaaaaaaaaaaaaa
cgbkbaaaabaaaaaadbaaaaakmcaabaaaaaaaaaaakgbcbaaaabaaaaaaaceaaaaa
aaaaaaaaaaaaaaaagggggglpgggggglpdmaaaaahdcaabaaaaaaaaaaaogakbaaa
aaaaaaaaegaabaaaaaaaaaaadcaaaaaphcaabaaaabaaaaaaigbbbaaaaaaaaaaa
aceaaaaaaaaaagecaaaaagecaaaaagecaaaaaaaaaceaaaaaaaaaaadpaaaaaadp
aaaaaadpaaaaaaaaedaaaaafhcaabaaaacaaaaaaegacbaaaabaaaaaaaaaaaaai
hcaabaaaabaaaaaaegacbaaaabaaaaaaegacbaiaebaaaaaaacaaaaaadiaaaaak
hcaabaaaacaaaaaaegacbaaaabaaaaaaaceaaaaaaaaaiadoaaaaiadoaaaaiado
aaaaaaaadcaaaaajicaabaaaacaaaaaabkaabaaaabaaaaaaabeaaaaaaaaaiado
abeaaaaaaaaaiadodcaaaaapmcaabaaaaaaaaaaakgacbaaaabaaaaaaaceaaaaa
aaaaaaaaaaaaaaaaaaaaiadoaaaaiadoaceaaaaaaaaaaaaaaaaaaaaaaaaaiado
aaaaaaaadhaaaaajdcaabaaaabaaaaaafgafbaaaaaaaaaaaogakbaaaacaaaaaa
egaabaaaacaaaaaadhaaaaajdcaabaaaaaaaaaaaagaabaaaaaaaaaaaogakbaaa
aaaaaaaaegaabaaaabaaaaaadiaaaaaigcaabaaaaaaaaaaafgafbaaaaaaaaaaa
agibcaaaabaaaaaacbaaaaaadcaaaaakdccabaaaacaaaaaaegiacaaaabaaaaaa
caaaaaaaagaabaaaaaaaaaaajgafbaaaaaaaaaaadoaaaaab"
}

SubProgram "gles " {
Keywords { }
"!!GLES


#ifdef VERTEX

varying mediump vec2 xlv_TEXCOORD0;
varying highp vec4 xlv_COLOR;
uniform highp mat4 glstate_matrix_texture0;
uniform highp mat4 glstate_matrix_mvp;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  vec3 tmpvar_1;
  tmpvar_1 = normalize(_glesNormal);
  mediump vec2 texOut_2;
  mediump float texY_3;
  mediump float texZ_4;
  mediump float texX_5;
  mediump vec2 tmpvar_6;
  highp vec4 tmpvar_7;
  tmpvar_7 = (glstate_matrix_mvp * _glesVertex);
  highp float tmpvar_8;
  tmpvar_8 = ((_glesVertex.x * 33.5) + 0.5);
  texX_5 = tmpvar_8;
  mediump float tmpvar_9;
  tmpvar_9 = ((texX_5 - float(int(texX_5))) * 0.25);
  texX_5 = tmpvar_9;
  highp float tmpvar_10;
  tmpvar_10 = ((_glesVertex.z * 33.5) + 0.5);
  texZ_4 = tmpvar_10;
  mediump float tmpvar_11;
  tmpvar_11 = ((texZ_4 - float(int(texZ_4))) * 0.25);
  texZ_4 = tmpvar_11;
  highp float tmpvar_12;
  tmpvar_12 = ((_glesVertex.y * 33.5) + 0.5);
  texY_3 = tmpvar_12;
  mediump float tmpvar_13;
  tmpvar_13 = ((texY_3 - float(int(texY_3))) * 0.25);
  texY_3 = tmpvar_13;
  mediump vec2 tmpvar_14;
  tmpvar_14.x = tmpvar_9;
  tmpvar_14.y = tmpvar_11;
  texOut_2 = tmpvar_14;
  if (((tmpvar_1.z > 0.9) || (tmpvar_1.z < -0.9))) {
    mediump vec2 tmpvar_15;
    tmpvar_15.x = (tmpvar_13 + 0.25);
    tmpvar_15.y = tmpvar_9;
    texOut_2 = tmpvar_15;
  } else {
    if (((tmpvar_1.x > 0.9) || (tmpvar_1.x < -0.9))) {
      mediump vec2 tmpvar_16;
      tmpvar_16.x = tmpvar_13;
      tmpvar_16.y = (tmpvar_11 + 0.25);
      texOut_2 = tmpvar_16;
    };
  };
  highp float tmpvar_17;
  tmpvar_17 = (floor(_glesVertex.x) / 16.0);
  highp float tmpvar_18;
  tmpvar_18 = (fract(abs(tmpvar_17)) * 16.0);
  highp float tmpvar_19;
  if ((tmpvar_17 >= 0.0)) {
    tmpvar_19 = tmpvar_18;
  } else {
    tmpvar_19 = -(tmpvar_18);
  };
  highp float tmpvar_20;
  tmpvar_20 = (tmpvar_19 / 16.0);
  highp float tmpvar_21;
  tmpvar_21 = (floor(_glesVertex.z) / 16.0);
  highp float tmpvar_22;
  tmpvar_22 = (fract(abs(tmpvar_21)) * 16.0);
  highp float tmpvar_23;
  if ((tmpvar_21 >= 0.0)) {
    tmpvar_23 = tmpvar_22;
  } else {
    tmpvar_23 = -(tmpvar_22);
  };
  highp vec4 tmpvar_24;
  tmpvar_24.yw = vec2(0.7, 1.0);
  tmpvar_24.x = tmpvar_20;
  tmpvar_24.z = (tmpvar_23 / 16.0);
  highp vec2 tmpvar_25;
  highp vec2 inUV_26;
  inUV_26 = texOut_2;
  highp vec4 tmpvar_27;
  tmpvar_27.zw = vec2(0.0, 0.0);
  tmpvar_27.x = inUV_26.x;
  tmpvar_27.y = inUV_26.y;
  tmpvar_25 = (glstate_matrix_texture0 * tmpvar_27).xy;
  tmpvar_6 = tmpvar_25;
  gl_Position = tmpvar_7;
  xlv_COLOR = tmpvar_24;
  xlv_TEXCOORD0 = tmpvar_6;
}



#endif
#ifdef FRAGMENT

varying mediump vec2 xlv_TEXCOORD0;
uniform sampler2D _BlockTex;
void main ()
{
  gl_FragData[0] = texture2D (_BlockTex, xlv_TEXCOORD0);
}



#endif"
}

SubProgram "glesdesktop " {
Keywords { }
"!!GLES


#ifdef VERTEX

varying mediump vec2 xlv_TEXCOORD0;
varying highp vec4 xlv_COLOR;
uniform highp mat4 glstate_matrix_texture0;
uniform highp mat4 glstate_matrix_mvp;
attribute vec3 _glesNormal;
attribute vec4 _glesVertex;
void main ()
{
  vec3 tmpvar_1;
  tmpvar_1 = normalize(_glesNormal);
  mediump vec2 texOut_2;
  mediump float texY_3;
  mediump float texZ_4;
  mediump float texX_5;
  mediump vec2 tmpvar_6;
  highp vec4 tmpvar_7;
  tmpvar_7 = (glstate_matrix_mvp * _glesVertex);
  highp float tmpvar_8;
  tmpvar_8 = ((_glesVertex.x * 33.5) + 0.5);
  texX_5 = tmpvar_8;
  mediump float tmpvar_9;
  tmpvar_9 = ((texX_5 - float(int(texX_5))) * 0.25);
  texX_5 = tmpvar_9;
  highp float tmpvar_10;
  tmpvar_10 = ((_glesVertex.z * 33.5) + 0.5);
  texZ_4 = tmpvar_10;
  mediump float tmpvar_11;
  tmpvar_11 = ((texZ_4 - float(int(texZ_4))) * 0.25);
  texZ_4 = tmpvar_11;
  highp float tmpvar_12;
  tmpvar_12 = ((_glesVertex.y * 33.5) + 0.5);
  texY_3 = tmpvar_12;
  mediump float tmpvar_13;
  tmpvar_13 = ((texY_3 - float(int(texY_3))) * 0.25);
  texY_3 = tmpvar_13;
  mediump vec2 tmpvar_14;
  tmpvar_14.x = tmpvar_9;
  tmpvar_14.y = tmpvar_11;
  texOut_2 = tmpvar_14;
  if (((tmpvar_1.z > 0.9) || (tmpvar_1.z < -0.9))) {
    mediump vec2 tmpvar_15;
    tmpvar_15.x = (tmpvar_13 + 0.25);
    tmpvar_15.y = tmpvar_9;
    texOut_2 = tmpvar_15;
  } else {
    if (((tmpvar_1.x > 0.9) || (tmpvar_1.x < -0.9))) {
      mediump vec2 tmpvar_16;
      tmpvar_16.x = tmpvar_13;
      tmpvar_16.y = (tmpvar_11 + 0.25);
      texOut_2 = tmpvar_16;
    };
  };
  highp float tmpvar_17;
  tmpvar_17 = (floor(_glesVertex.x) / 16.0);
  highp float tmpvar_18;
  tmpvar_18 = (fract(abs(tmpvar_17)) * 16.0);
  highp float tmpvar_19;
  if ((tmpvar_17 >= 0.0)) {
    tmpvar_19 = tmpvar_18;
  } else {
    tmpvar_19 = -(tmpvar_18);
  };
  highp float tmpvar_20;
  tmpvar_20 = (tmpvar_19 / 16.0);
  highp float tmpvar_21;
  tmpvar_21 = (floor(_glesVertex.z) / 16.0);
  highp float tmpvar_22;
  tmpvar_22 = (fract(abs(tmpvar_21)) * 16.0);
  highp float tmpvar_23;
  if ((tmpvar_21 >= 0.0)) {
    tmpvar_23 = tmpvar_22;
  } else {
    tmpvar_23 = -(tmpvar_22);
  };
  highp vec4 tmpvar_24;
  tmpvar_24.yw = vec2(0.7, 1.0);
  tmpvar_24.x = tmpvar_20;
  tmpvar_24.z = (tmpvar_23 / 16.0);
  highp vec2 tmpvar_25;
  highp vec2 inUV_26;
  inUV_26 = texOut_2;
  highp vec4 tmpvar_27;
  tmpvar_27.zw = vec2(0.0, 0.0);
  tmpvar_27.x = inUV_26.x;
  tmpvar_27.y = inUV_26.y;
  tmpvar_25 = (glstate_matrix_texture0 * tmpvar_27).xy;
  tmpvar_6 = tmpvar_25;
  gl_Position = tmpvar_7;
  xlv_COLOR = tmpvar_24;
  xlv_TEXCOORD0 = tmpvar_6;
}



#endif
#ifdef FRAGMENT

varying mediump vec2 xlv_TEXCOORD0;
uniform sampler2D _BlockTex;
void main ()
{
  gl_FragData[0] = texture2D (_BlockTex, xlv_TEXCOORD0);
}



#endif"
}

SubProgram "flash " {
Keywords { }
Bind "vertex" Vertex
Bind "normal" Normal
Matrix 0 [glstate_matrix_mvp]
Matrix 4 [glstate_matrix_texture0]
"agal_vs
c8 33.5 0.5 -0.5 0.0
c9 1.0 0.25 0.9 -0.9
c10 0.0625 16.0 0.7 1.0
[bc]
adaaaaaaaaaaacacaaaaaaaaaaaaaaaaaiaaaaaaabaaaaaa mul r0.y, a0.x, c8.x
abaaaaaaaaaaabacaaaaaaffacaaaaaaaiaaaaffabaaaaaa add r0.x, r0.y, c8.y
beaaaaaaaaaaaeacaaaaaaaaacaaaaaaaaaaaaaaaaaaaaaa abs r0.z, r0.x
aiaaaaaaaaaaaiacaaaaaakkacaaaaaaaaaaaaaaaaaaaaaa frc r0.w, r0.z
adaaaaaaabaaabacaaaaaakkaaaaaaaaaiaaaaoeabaaaaaa mul r1.x, a0.z, c8
ckaaaaaaabaaacacabaaaaaaacaaaaaaaiaaaakkabaaaaaa slt r1.y, r1.x, c8.z
abaaaaaaabaaabacabaaaaaaacaaaaaaaiaaaaffabaaaaaa add r1.x, r1.x, c8.y
beaaaaaaabaaaeacabaaaaaaacaaaaaaaaaaaaaaaaaaaaaa abs r1.z, r1.x
aiaaaaaaabaaaiacabaaaakkacaaaaaaaaaaaaaaaaaaaaaa frc r1.w, r1.z
ckaaaaaaaaaaacacaaaaaaffacaaaaaaaiaaaakkabaaaaaa slt r0.y, r0.y, c8.z
bfaaaaaaacaaacacaaaaaaffacaaaaaaaaaaaaaaaaaaaaaa neg r2.y, r0.y
ahaaaaaaaaaaacacacaaaaffacaaaaaaaaaaaaffacaaaaaa max r0.y, r2.y, r0.y
bfaaaaaaadaaacacabaaaaffacaaaaaaaaaaaaaaaaaaaaaa neg r3.y, r1.y
ahaaaaaaabaaacacadaaaaffacaaaaaaabaaaaffacaaaaaa max r1.y, r3.y, r1.y
ckaaaaaaabaaacacaiaaaappabaaaaaaabaaaaffacaaaaaa slt r1.y, c8.w, r1.y
acaaaaaaabaaaeacabaaaakkacaaaaaaabaaaappacaaaaaa sub r1.z, r1.z, r1.w
bfaaaaaaadaaaeacabaaaaffacaaaaaaaaaaaaaaaaaaaaaa neg r3.z, r1.y
abaaaaaaacaaabacadaaaakkacaaaaaaajaaaaoeabaaaaaa add r2.x, r3.z, c9
acaaaaaaaaaaaeacaaaaaakkacaaaaaaaaaaaappacaaaaaa sub r0.z, r0.z, r0.w
ckaaaaaaaaaaacacaiaaaappabaaaaaaaaaaaaffacaaaaaa slt r0.y, c8.w, r0.y
bfaaaaaaaeaaacacaaaaaaffacaaaaaaaaaaaaaaaaaaaaaa neg r4.y, r0.y
abaaaaaaaaaaaiacaeaaaaffacaaaaaaajaaaaaaabaaaaaa add r0.w, r4.y, c9.x
adaaaaaaaaaaaiacaaaaaakkacaaaaaaaaaaaappacaaaaaa mul r0.w, r0.z, r0.w
bfaaaaaaaeaaaeacaaaaaakkacaaaaaaaaaaaaaaaaaaaaaa neg r4.z, r0.z
adaaaaaaaeaaaeacaaaaaaffacaaaaaaaeaaaakkacaaaaaa mul r4.z, r0.y, r4.z
abaaaaaaaaaaaeacaeaaaakkacaaaaaaaaaaaappacaaaaaa add r0.z, r4.z, r0.w
acaaaaaaaaaaabacaaaaaaaaacaaaaaaaaaaaakkacaaaaaa sub r0.x, r0.x, r0.z
adaaaaaaabaaaiacabaaaakkacaaaaaaacaaaaaaacaaaaaa mul r1.w, r1.z, r2.x
bfaaaaaaaeaaaeacabaaaakkacaaaaaaaaaaaaaaaaaaaaaa neg r4.z, r1.z
adaaaaaaaaaaacacabaaaaffacaaaaaaaeaaaakkacaaaaaa mul r0.y, r1.y, r4.z
abaaaaaaaaaaacacaaaaaaffacaaaaaaabaaaappacaaaaaa add r0.y, r0.y, r1.w
acaaaaaaaaaaacacabaaaaaaacaaaaaaaaaaaaffacaaaaaa sub r0.y, r1.x, r0.y
adaaaaaaacaaabacaaaaaaffacaaaaaaajaaaaffabaaaaaa mul r2.x, r0.y, c9.y
adaaaaaaaaaaaeacaaaaaaffaaaaaaaaaiaaaaaaabaaaaaa mul r0.z, a0.y, c8.x
abaaaaaaabaaaiacaaaaaakkacaaaaaaaiaaaaffabaaaaaa add r1.w, r0.z, c8.y
adaaaaaaaaaaabacaaaaaaaaacaaaaaaajaaaaffabaaaaaa mul r0.x, r0.x, c9.y
ckaaaaaaacaaacacabaaaakkaaaaaaaaajaaaappabaaaaaa slt r2.y, a1.z, c9.w
ckaaaaaaacaaaeacajaaaaoeabaaaaaaabaaaaoeaaaaaaaa slt r2.z, c9, a1
abaaaaaaacaaaiacacaaaakkacaaaaaaacaaaaffacaaaaaa add r2.w, r2.z, r2.y
bfaaaaaaaaaaaiacacaaaappacaaaaaaaaaaaaaaaaaaaaaa neg r0.w, r2.w
ahaaaaaaaaaaaiacaaaaaappacaaaaaaacaaaappacaaaaaa max r0.w, r0.w, r2.w
ckaaaaaaadaaabacaiaaaappabaaaaaaaaaaaappacaaaaaa slt r3.x, c8.w, r0.w
ckaaaaaaaaaaaiacaaaaaakkacaaaaaaaiaaaakkabaaaaaa slt r0.w, r0.z, c8.z
beaaaaaaaaaaaeacabaaaappacaaaaaaaaaaaaaaaaaaaaaa abs r0.z, r1.w
aiaaaaaaabaaabacaaaaaakkacaaaaaaaaaaaaaaaaaaaaaa frc r1.x, r0.z
acaaaaaaabaaacacaaaaaakkacaaaaaaabaaaaaaacaaaaaa sub r1.y, r0.z, r1.x
bfaaaaaaaeaaaiacaaaaaappacaaaaaaaaaaaaaaaaaaaaaa neg r4.w, r0.w
ahaaaaaaaaaaaiacaeaaaappacaaaaaaaaaaaappacaaaaaa max r0.w, r4.w, r0.w
ckaaaaaaabaaaeacaiaaaappabaaaaaaaaaaaappacaaaaaa slt r1.z, c8.w, r0.w
bfaaaaaaaeaaaeacabaaaakkacaaaaaaaaaaaaaaaaaaaaaa neg r4.z, r1.z
abaaaaaaaaaaaiacaeaaaakkacaaaaaaajaaaaaaabaaaaaa add r0.w, r4.z, c9.x
adaaaaaaabaaabacabaaaaffacaaaaaaaaaaaappacaaaaaa mul r1.x, r1.y, r0.w
bfaaaaaaaeaaacacabaaaaffacaaaaaaaaaaaaaaaaaaaaaa neg r4.y, r1.y
adaaaaaaaeaaabacabaaaakkacaaaaaaaeaaaaffacaaaaaa mul r4.x, r1.z, r4.y
abaaaaaaabaaabacaeaaaaaaacaaaaaaabaaaaaaacaaaaaa add r1.x, r4.x, r1.x
acaaaaaaabaaabacabaaaappacaaaaaaabaaaaaaacaaaaaa sub r1.x, r1.w, r1.x
aaaaaaaaaaaaacacacaaaaaaacaaaaaaaaaaaaaaaaaaaaaa mov r0.y, r2.x
bfaaaaaaaeaaabacadaaaaaaacaaaaaaaaaaaaaaaaaaaaaa neg r4.x, r3.x
abaaaaaaaaaaaeacaeaaaaaaacaaaaaaajaaaaaaabaaaaaa add r0.z, r4.x, c9.x
adaaaaaaaaaaamacaaaaaakkacaaaaaaaaaaaaefacaaaaaa mul r0.zw, r0.z, r0.yyxy
cjaaaaaaabaaaeacaiaaaappabaaaaaaacaaaappacaaaaaa sge r1.z, c8.w, r2.w
ckaaaaaaabaaacacabaaaaaaaaaaaaaaajaaaappabaaaaaa slt r1.y, a1.x, c9.w
ckaaaaaaaaaaacacajaaaakkabaaaaaaabaaaaaaaaaaaaaa slt r0.y, c9.z, a1.x
abaaaaaaaaaaacacaaaaaaffacaaaaaaabaaaaffacaaaaaa add r0.y, r0.y, r1.y
bfaaaaaaabaaacacacaaaaffacaaaaaaaaaaaaaaaaaaaaaa neg r1.y, r2.y
cjaaaaaaabaaacacacaaaakkacaaaaaaabaaaaffacaaaaaa sge r1.y, r2.z, r1.y
adaaaaaaabaaacacabaaaaffacaaaaaaabaaaakkacaaaaaa mul r1.y, r1.y, r1.z
adaaaaaaaaaaacacabaaaaffacaaaaaaaaaaaaffacaaaaaa mul r0.y, r1.y, r0.y
adaaaaaaabaaaeacabaaaaaaacaaaaaaajaaaaffabaaaaaa mul r1.z, r1.x, c9.y
bfaaaaaaaeaaacacaaaaaaffacaaaaaaaaaaaaaaaaaaaaaa neg r4.y, r0.y
ahaaaaaaaaaaacacaeaaaaffacaaaaaaaaaaaaffacaaaaaa max r0.y, r4.y, r0.y
ckaaaaaaaaaaacacaiaaaappabaaaaaaaaaaaaffacaaaaaa slt r0.y, c8.w, r0.y
aaaaaaaaabaaacacaaaaaaaaacaaaaaaaaaaaaaaaaaaaaaa mov r1.y, r0.x
abaaaaaaabaaabacabaaaakkacaaaaaaajaaaaffabaaaaaa add r1.x, r1.z, c9.y
adaaaaaaaeaaamacadaaaaaaacaaaaaaabaaaaefacaaaaaa mul r4.zw, r3.x, r1.yyxy
abaaaaaaaaaaamacaeaaaaopacaaaaaaaaaaaaopacaaaaaa add r0.zw, r4.wwzw, r0.wwzw
bfaaaaaaaeaaacacaaaaaaffacaaaaaaaaaaaaaaaaaaaaaa neg r4.y, r0.y
abaaaaaaaaaaabacaeaaaaffacaaaaaaajaaaaoeabaaaaaa add r0.x, r4.y, c9
adaaaaaaaaaaamacaaaaaaaaacaaaaaaaaaaaaopacaaaaaa mul r0.zw, r0.x, r0.wwzw
aiaaaaaaaaaaabacaaaaaakkaaaaaaaaaaaaaaaaaaaaaaaa frc r0.x, a0.z
acaaaaaaabaaabacaaaaaakkaaaaaaaaaaaaaaaaacaaaaaa sub r1.x, a0.z, r0.x
adaaaaaaabaaabacabaaaaaaacaaaaaaakaaaaoeabaaaaaa mul r1.x, r1.x, c10
abaaaaaaabaaaiacacaaaaaaacaaaaaaajaaaaffabaaaaaa add r1.w, r2.x, c9.y
adaaaaaaacaaadacaaaaaaffacaaaaaaabaaaapoacaaaaaa mul r2.xy, r0.y, r1.zwww
abaaaaaaacaaadacacaaaafeacaaaaaaaaaaaapoacaaaaaa add r2.xy, r2.xyyy, r0.zwww
aaaaaaaaacaaamacaiaaaappabaaaaaaaaaaaaaaaaaaaaaa mov r2.zw, c8.w
aiaaaaaaaaaaacacaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa frc r0.y, a0.x
acaaaaaaaaaaaeacaaaaaaaaaaaaaaaaaaaaaaffacaaaaaa sub r0.z, a0.x, r0.y
ckaaaaaaaaaaacacaaaaaaaaaaaaaaaaaaaaaaffacaaaaaa slt r0.y, a0.x, r0.y
adaaaaaaaaaaaeacaaaaaakkacaaaaaaakaaaaaaabaaaaaa mul r0.z, r0.z, c10.x
bfaaaaaaaeaaacacaaaaaaffacaaaaaaaaaaaaaaaaaaaaaa neg r4.y, r0.y
ahaaaaaaaaaaacacaeaaaaffacaaaaaaaaaaaaffacaaaaaa max r0.y, r4.y, r0.y
ckaaaaaaaaaaacacaiaaaappabaaaaaaaaaaaaffacaaaaaa slt r0.y, c8.w, r0.y
beaaaaaaaaaaaeacaaaaaakkacaaaaaaaaaaaaaaaaaaaaaa abs r0.z, r0.z
aiaaaaaaaaaaaeacaaaaaakkacaaaaaaaaaaaaaaaaaaaaaa frc r0.z, r0.z
ckaaaaaaaaaaabacaaaaaakkaaaaaaaaaaaaaaaaacaaaaaa slt r0.x, a0.z, r0.x
beaaaaaaabaaacacabaaaaaaacaaaaaaaaaaaaaaaaaaaaaa abs r1.y, r1.x
bfaaaaaaaeaaabacaaaaaaaaacaaaaaaaaaaaaaaaaaaaaaa neg r4.x, r0.x
ahaaaaaaaaaaabacaeaaaaaaacaaaaaaaaaaaaaaacaaaaaa max r0.x, r4.x, r0.x
ckaaaaaaabaaabacaiaaaappabaaaaaaaaaaaaaaacaaaaaa slt r1.x, c8.w, r0.x
aiaaaaaaaaaaabacabaaaaffacaaaaaaaaaaaaaaaaaaaaaa frc r0.x, r1.y
adaaaaaaabaaacacaaaaaaaaacaaaaaaakaaaaoeabaaaaaa mul r1.y, r0.x, c10
bfaaaaaaaeaaabacabaaaaaaacaaaaaaaaaaaaaaaaaaaaaa neg r4.x, r1.x
abaaaaaaabaaaeacaeaaaaaaacaaaaaaajaaaaaaabaaaaaa add r1.z, r4.x, c9.x
bfaaaaaaaeaaacacaaaaaaffacaaaaaaaaaaaaaaaaaaaaaa neg r4.y, r0.y
abaaaaaaaaaaaiacaeaaaaffacaaaaaaajaaaaaaabaaaaaa add r0.w, r4.y, c9.x
adaaaaaaaaaaaeacaaaaaakkacaaaaaaakaaaaffabaaaaaa mul r0.z, r0.z, c10.y
adaaaaaaaaaaaiacaaaaaakkacaaaaaaaaaaaappacaaaaaa mul r0.w, r0.z, r0.w
bfaaaaaaaeaaaeacaaaaaakkacaaaaaaaaaaaaaaaaaaaaaa neg r4.z, r0.z
adaaaaaaaeaaabacaaaaaaffacaaaaaaaeaaaakkacaaaaaa mul r4.x, r0.y, r4.z
abaaaaaaaaaaabacaeaaaaaaacaaaaaaaaaaaappacaaaaaa add r0.x, r4.x, r0.w
adaaaaaaabaaaeacabaaaaffacaaaaaaabaaaakkacaaaaaa mul r1.z, r1.y, r1.z
bfaaaaaaaaaaacacabaaaaffacaaaaaaaaaaaaaaaaaaaaaa neg r0.y, r1.y
adaaaaaaaaaaacacabaaaaaaacaaaaaaaaaaaaffacaaaaaa mul r0.y, r1.x, r0.y
abaaaaaaaaaaacacaaaaaaffacaaaaaaabaaaakkacaaaaaa add r0.y, r0.y, r1.z
bdaaaaaaaaaaacaeacaaaaoeacaaaaaaafaaaaoeabaaaaaa dp4 v0.y, r2, c5
bdaaaaaaaaaaabaeacaaaaoeacaaaaaaaeaaaaoeabaaaaaa dp4 v0.x, r2, c4
adaaaaaaahaaabaeaaaaaaaaacaaaaaaakaaaaoeabaaaaaa mul v7.x, r0.x, c10
adaaaaaaahaaaeaeaaaaaaffacaaaaaaakaaaaaaabaaaaaa mul v7.z, r0.y, c10.x
aaaaaaaaahaaakaeakaaaaoiabaaaaaaaaaaaaaaaaaaaaaa mov v7.yw, c10.xzzw
bdaaaaaaaaaaaiadaaaaaaoeaaaaaaaaadaaaaoeabaaaaaa dp4 o0.w, a0, c3
bdaaaaaaaaaaaeadaaaaaaoeaaaaaaaaacaaaaoeabaaaaaa dp4 o0.z, a0, c2
bdaaaaaaaaaaacadaaaaaaoeaaaaaaaaabaaaaoeabaaaaaa dp4 o0.y, a0, c1
bdaaaaaaaaaaabadaaaaaaoeaaaaaaaaaaaaaaoeabaaaaaa dp4 o0.x, a0, c0
aaaaaaaaaaaaamaeaaaaaaoeabaaaaaaaaaaaaaaaaaaaaaa mov v0.zw, c0
"
}

SubProgram "d3d11_9x " {
Keywords { }
Bind "vertex" Vertex
Bind "normal" Normal
ConstBuffer "UnityPerDraw" 336 // 64 used size, 6 vars
Matrix 0 [glstate_matrix_mvp] 4
ConstBuffer "UnityPerDrawTexMatrices" 768 // 576 used size, 5 vars
Matrix 512 [glstate_matrix_texture0] 4
BindCB "UnityPerDraw" 0
BindCB "UnityPerDrawTexMatrices" 1
// 24 instructions, 3 temp regs, 0 temp arrays:
// ALU 11 float, 0 int, 1 uint
// TEX 0 (0 load, 0 comp, 0 bias, 0 grad)
// FLOW 1 static, 0 dynamic
"vs_4_0_level_9_1
eefiecednmnhlagdidchdkebajcaemgaedhjjeobabaaaaaaleahaaaaaeaaaaaa
daaaaaaaaiadaaaammagaaaaeaahaaaaebgpgodjnaacaaaanaacaaaaaaacpopp
jaacaaaaeaaaaaaaacaaceaaaaaadmaaaaaadmaaaaaaceaaabaadmaaaaaaaaaa
aeaaabaaaaaaaaaaabaacaaaacaaafaaaaaaaaaaaaaaaaaaaaacpoppfbaaaaaf
ahaaapkaaaaaagecaaaaaadpaaaaiadoggggggdpfbaaaaafaiaaapkaaaaaiado
aaaaaaaaaaaaiadnaaaaaaaafbaaaaafajaaapkadddddddpaaaaiadpaaaaaaaa
aaaaaaaabpaaaaacafaaaaiaaaaaapjabpaaaaacafaaabiaabaaapjabdaaaaac
aaaaadiaaaaaoijaacaaaaadaaaaadiaaaaaoeibaaaaoijaafaaaaadaaaaadia
aaaaoeiaaiaakkkaanaaaaadaaaaamiaaaaaeeiaaaaaeeibcdaaaaacaaaaadia
aaaaoeiabdaaaaacaaaaadiaaaaaoeiaacaaaaadabaaadiaaaaaoeiaaaaaoeia
aeaaaaaeaaaaafoaaaaapgiaabaaneiaaaaaneibaeaaaaaeaaaaahiaaaaanija
ahaaaakaahaaffkabdaaaaacabaaahiaaaaaoeiaacaaaaadacaaahiaaaaaoeia
abaaoeibamaaaaadabaaahiaabaaoeibabaaoeiaamaaaaadadaaahiaaaaaoeia
aaaaoeibaeaaaaaeabaaahiaadaaoeiaabaaoeiaacaaoeiaacaaaaadaaaaahia
aaaaoeiaabaaoeibafaaaaadabaaahiaaaaaoeiaahaakkkaaeaaaaaeabaaaiia
aaaaffiaahaakkkaahaakkkaaeaaaaaeaaaaadiaaaaaociaaiaaaakaaiaaoeka
amaaaaadaaaaamiaahaappkaabaacejaamaaaaadacaaadiaabaaocjaahaappkb
acaaaaadaaaaamiaaaaaoeiaacaaeeiaamaaaaadaaaaamiaaaaaoeibaaaaoeia
bcaaaaaeacaaadiaaaaappiaabaaooiaabaaoeiabcaaaaaeabaaadiaaaaakkia
aaaaoeiaacaaoeiaafaaaaadaaaaadiaabaaffiaagaaoekaaeaaaaaeabaaadoa
afaaoekaabaaaaiaaaaaoeiaafaaaaadaaaaapiaaaaaffjaacaaoekaaeaaaaae
aaaaapiaabaaoekaaaaaaajaaaaaoeiaaeaaaaaeaaaaapiaadaaoekaaaaakkja
aaaaoeiaaeaaaaaeaaaaapiaaeaaoekaaaaappjaaaaaoeiaaeaaaaaeaaaaadma
aaaappiaaaaaoekaaaaaoeiaabaaaaacaaaaammaaaaaoeiaabaaaaacaaaaakoa
ajaagakappppaaaafdeieefclmadaaaaeaaaabaaopaaaaaafjaaaaaeegiocaaa
aaaaaaaaaeaaaaaafjaaaaaeegiocaaaabaaaaaaccaaaaaafpaaaaadpcbabaaa
aaaaaaaafpaaaaadfcbabaaaabaaaaaaghaaaaaepccabaaaaaaaaaaaabaaaaaa
gfaaaaadpccabaaaabaaaaaagfaaaaaddccabaaaacaaaaaagiaaaaacadaaaaaa
diaaaaaipcaabaaaaaaaaaaafgbfbaaaaaaaaaaaegiocaaaaaaaaaaaabaaaaaa
dcaaaaakpcaabaaaaaaaaaaaegiocaaaaaaaaaaaaaaaaaaaagbabaaaaaaaaaaa
egaobaaaaaaaaaaadcaaaaakpcaabaaaaaaaaaaaegiocaaaaaaaaaaaacaaaaaa
kgbkbaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaakpccabaaaaaaaaaaaegiocaaa
aaaaaaaaadaaaaaapgbpbaaaaaaaaaaaegaobaaaaaaaaaaaebaaaaafdcaabaaa
aaaaaaaaigbabaaaaaaaaaaadiaaaaakdcaabaaaaaaaaaaaegaabaaaaaaaaaaa
aceaaaaaaaaaiadnaaaaiadnaaaaaaaaaaaaaaaabnaaaaaimcaabaaaaaaaaaaa
agaebaaaaaaaaaaaagaebaiaebaaaaaaaaaaaaaabkaaaaagdcaabaaaaaaaaaaa
egaabaiaibaaaaaaaaaaaaaadhaaaaakfccabaaaabaaaaaakgalbaaaaaaaaaaa
agabbaaaaaaaaaaaagabbaiaebaaaaaaaaaaaaaadgaaaaaikccabaaaabaaaaaa
aceaaaaaaaaaaaaadddddddpaaaaaaaaaaaaiadpdbaaaaakdcaabaaaaaaaaaaa
aceaaaaaggggggdpggggggdpaaaaaaaaaaaaaaaacgbkbaaaabaaaaaadbaaaaak
mcaabaaaaaaaaaaakgbcbaaaabaaaaaaaceaaaaaaaaaaaaaaaaaaaaagggggglp
gggggglpdmaaaaahdcaabaaaaaaaaaaaogakbaaaaaaaaaaaegaabaaaaaaaaaaa
dcaaaaaphcaabaaaabaaaaaaigbbbaaaaaaaaaaaaceaaaaaaaaaagecaaaaagec
aaaaagecaaaaaaaaaceaaaaaaaaaaadpaaaaaadpaaaaaadpaaaaaaaaedaaaaaf
hcaabaaaacaaaaaaegacbaaaabaaaaaaaaaaaaaihcaabaaaabaaaaaaegacbaaa
abaaaaaaegacbaiaebaaaaaaacaaaaaadiaaaaakhcaabaaaacaaaaaaegacbaaa
abaaaaaaaceaaaaaaaaaiadoaaaaiadoaaaaiadoaaaaaaaadcaaaaajicaabaaa
acaaaaaabkaabaaaabaaaaaaabeaaaaaaaaaiadoabeaaaaaaaaaiadodcaaaaap
mcaabaaaaaaaaaaakgacbaaaabaaaaaaaceaaaaaaaaaaaaaaaaaaaaaaaaaiado
aaaaiadoaceaaaaaaaaaaaaaaaaaaaaaaaaaiadoaaaaaaaadhaaaaajdcaabaaa
abaaaaaafgafbaaaaaaaaaaaogakbaaaacaaaaaaegaabaaaacaaaaaadhaaaaaj
dcaabaaaaaaaaaaaagaabaaaaaaaaaaaogakbaaaaaaaaaaaegaabaaaabaaaaaa
diaaaaaigcaabaaaaaaaaaaafgafbaaaaaaaaaaaagibcaaaabaaaaaacbaaaaaa
dcaaaaakdccabaaaacaaaaaaegiacaaaabaaaaaacaaaaaaaagaabaaaaaaaaaaa
jgafbaaaaaaaaaaadoaaaaabejfdeheogmaaaaaaadaaaaaaaiaaaaaafaaaaaaa
aaaaaaaaaaaaaaaaadaaaaaaaaaaaaaaapapaaaafjaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaahafaaaagaaaaaaaaaaaaaaaaaaaaaaaadaaaaaaacaaaaaa
adaaaaaafaepfdejfeejepeoaaeoepfcenebemaafeeffiedepepfceeaaklklkl
epfdeheogmaaaaaaadaaaaaaaiaaaaaafaaaaaaaaaaaaaaaabaaaaaaadaaaaaa
aaaaaaaaapaaaaaafmaaaaaaaaaaaaaaaaaaaaaaadaaaaaaabaaaaaaapaaaaaa
gcaaaaaaaaaaaaaaaaaaaaaaadaaaaaaacaaaaaaadamaaaafdfgfpfaepfdejfe
ejepeoaaedepemepfcaafeeffiedepepfceeaakl"
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
#line 346
#line 192
highp vec2 MultiplyUV( in highp mat4 mat, in highp vec2 inUV ) {
    highp vec4 temp = vec4( inUV.x, inUV.y, 0.0, 0.0);
    temp = (mat * temp);
    #line 196
    return temp.xy;
}
#line 321
v2f vert( in appdata v ) {
    v2f o;
    o.pos = (glstate_matrix_mvp * v.vertex);
    #line 325
    o.color.xyz = ((v.normal * 0.5) + 0.5);
    o.color.w = 1.0;
    mediump float scale = 0.25;
    mediump float fudgeS = 0.5;
    #line 329
    mediump float fudgeAlso = 33.5;
    mediump float texX = ((v.vertex.x * fudgeAlso) + fudgeS);
    texX = ((texX - float(int(texX))) * scale);
    mediump float texZ = ((v.vertex.z * fudgeAlso) + fudgeS);
    #line 333
    texZ = ((texZ - float(int(texZ))) * scale);
    mediump float texY = ((v.vertex.y * fudgeAlso) + fudgeS);
    texY = ((texY - float(int(texY))) * scale);
    o.color = vec4( texX, texZ, texY, 1.0);
    #line 337
    mediump vec2 texOut = vec2( texX, texZ);
    if (((v.normal.z > 0.9) || (v.normal.z < -0.9))){
        texOut = vec2( (texY + scale), texX);
    }
    else{
        if (((v.normal.x > 0.9) || (v.normal.x < -0.9))){
            texOut = vec2( texY, (texZ + scale));
        }
    }
    highp float rem = (xll_mod_f_f( floor(v.vertex.x), 16.0) / 16.0);
    #line 341
    highp float zrem = (xll_mod_f_f( floor(v.vertex.z), 16.0) / 16.0);
    o.color = vec4( rem, 0.7, zrem, 1.0);
    o.uv = MultiplyUV( glstate_matrix_texture0, texOut);
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
#line 346
#line 346
lowp vec4 frag( in v2f i ) {
    return texture( _BlockTex, i.uv);
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
//   opengl - ALU: 1 to 1, TEX: 1 to 1
//   d3d9 - ALU: 1 to 1, TEX: 1 to 1
//   d3d11 - ALU: 0 to 0, TEX: 1 to 1, FLOW: 1 to 1
//   d3d11_9x - ALU: 0 to 0, TEX: 1 to 1, FLOW: 1 to 1
SubProgram "opengl " {
Keywords { }
SetTexture 0 [_BlockTex] 2D
"!!ARBfp1.0
# 1 ALU, 1 TEX
TEX result.color, fragment.texcoord[0], texture[0], 2D;
END
# 1 instructions, 0 R-regs
"
}

SubProgram "d3d9 " {
Keywords { }
SetTexture 0 [_BlockTex] 2D
"ps_2_0
; 1 ALU, 1 TEX
dcl_2d s0
dcl t0.xy
texld r0, t0, s0
mov_pp oC0, r0
"
}

SubProgram "d3d11 " {
Keywords { }
SetTexture 0 [_BlockTex] 2D 0
// 2 instructions, 0 temp regs, 0 temp arrays:
// ALU 0 float, 0 int, 0 uint
// TEX 1 (0 load, 0 comp, 0 bias, 0 grad)
// FLOW 1 static, 0 dynamic
"ps_4_0
eefiecedakjplffcpldbahdacbaljbljjkedcmceabaaaaaaeaabaaaaadaaaaaa
cmaaaaaakaaaaaaaneaaaaaaejfdeheogmaaaaaaadaaaaaaaiaaaaaafaaaaaaa
aaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaafmaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaapaaaaaagcaaaaaaaaaaaaaaaaaaaaaaadaaaaaaacaaaaaa
adadaaaafdfgfpfaepfdejfeejepeoaaedepemepfcaafeeffiedepepfceeaakl
epfdeheocmaaaaaaabaaaaaaaiaaaaaacaaaaaaaaaaaaaaaaaaaaaaaadaaaaaa
aaaaaaaaapaaaaaafdfgfpfegbhcghgfheaaklklfdeieefcgeaaaaaaeaaaaaaa
bjaaaaaafkaaaaadaagabaaaaaaaaaaafibiaaaeaahabaaaaaaaaaaaffffaaaa
gcbaaaaddcbabaaaacaaaaaagfaaaaadpccabaaaaaaaaaaaefaaaaajpccabaaa
aaaaaaaaegbabaaaacaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaadoaaaaab
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
[bc]
ciaaaaaaaaaaapacaaaaaaoeaeaaaaaaaaaaaaaaafaababb tex r0, v0, s0 <2d wrap linear point>
aaaaaaaaaaaaapadaaaaaaoeacaaaaaaaaaaaaaaaaaaaaaa mov o0, r0
"
}

SubProgram "d3d11_9x " {
Keywords { }
SetTexture 0 [_BlockTex] 2D 0
// 2 instructions, 0 temp regs, 0 temp arrays:
// ALU 0 float, 0 int, 0 uint
// TEX 1 (0 load, 0 comp, 0 bias, 0 grad)
// FLOW 1 static, 0 dynamic
"ps_4_0_level_9_1
eefiecedhfdklepjboecglemgiljbacdmhfniniiabaaaaaalaabaaaaaeaaaaaa
daaaaaaajmaaaaaaaiabaaaahmabaaaaebgpgodjgeaaaaaageaaaaaaaaacpppp
dmaaaaaaciaaaaaaaaaaciaaaaaaciaaaaaaciaaabaaceaaaaaaciaaaaaaaaaa
aaacppppbpaaaaacaaaaaaiaabaacdlabpaaaaacaaaaaajaaaaiapkaecaaaaad
aaaacpiaabaaoelaaaaioekaabaaaaacaaaicpiaaaaaoeiappppaaaafdeieefc
geaaaaaaeaaaaaaabjaaaaaafkaaaaadaagabaaaaaaaaaaafibiaaaeaahabaaa
aaaaaaaaffffaaaagcbaaaaddcbabaaaacaaaaaagfaaaaadpccabaaaaaaaaaaa
efaaaaajpccabaaaaaaaaaaaegbabaaaacaaaaaaeghobaaaaaaaaaaaaagabaaa
aaaaaaaadoaaaaabejfdeheogmaaaaaaadaaaaaaaiaaaaaafaaaaaaaaaaaaaaa
abaaaaaaadaaaaaaaaaaaaaaapaaaaaafmaaaaaaaaaaaaaaaaaaaaaaadaaaaaa
abaaaaaaapaaaaaagcaaaaaaaaaaaaaaaaaaaaaaadaaaaaaacaaaaaaadadaaaa
fdfgfpfaepfdejfeejepeoaaedepemepfcaafeeffiedepepfceeaaklepfdeheo
cmaaaaaaabaaaaaaaiaaaaaacaaaaaaaaaaaaaaaaaaaaaaaadaaaaaaaaaaaaaa
apaaaaaafdfgfpfegbhcghgfheaaklkl"
}

SubProgram "gles3 " {
Keywords { }
"!!GLES3"
}

}

#LINE 115

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
