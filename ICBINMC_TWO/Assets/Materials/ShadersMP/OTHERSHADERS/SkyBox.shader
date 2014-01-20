Shader "GLSL shader for skybox" 
{
   Properties 
   {
//      _Cube ("Environment Map", Cube) = "" {}
   }
   SubShader 
   {
      Tags { "Queue" = "Background" }
 
      Pass {   
         ZWrite Off
         Cull Front
 
         GLSLPROGRAM
 
         // User-specified uniform
         uniform samplerCube _Cube;   
 
         // The following built-in uniforms 
         // are also defined in "UnityCG.glslinc", 
         // i.e. one could #include "UnityCG.glslinc" 
         //uniform vec3 _WorldSpaceCameraPos; 
            // camera position in world space
         //uniform mat4 _Object2World; // model matrix
 
         // Varying
         //varying vec3 viewDirection;
         
         // Varying
         //varying vec3 texCoords;
 
         #ifdef VERTEX
 
         void main()
         {            
            //mat4 modelMatrix = _Object2World;
 
            //viewDirection = vec3(modelMatrix * gl_Vertex 
             //  - vec4(_WorldSpaceCameraPos, 1.0));
             
             //texCoords = (vec3)gl_MultiTexCoord0;
             
 			//texCoords = (vec3)gl_MultiTexCoord0;
            gl_Position = gl_ModelViewProjectionMatrix * gl_Vertex;
         }
 
         #endif
 
 
         #ifdef FRAGMENT
 
         void main()
         {
            gl_FragColor = vec4(0,1,1,1); // textureCube(_Cube, viewDirection);
         }
 
         #endif
 
         ENDGLSL
      }
   }
}

