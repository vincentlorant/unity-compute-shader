Shader "ComputeShader/Cubes"
{
    Properties 
		{
				_Color ("Color", Color) = (1,1,1,1)
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Alpha ("Alpha", Range(0,1)) = 0.5
    }
    SubShader {
        Tags { "RenderType"="Transparent" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model
        #pragma surface surf Standard addshadow fullforwardshadows alpha
        #pragma multi_compile_instancing
        #pragma instancing_options procedural:setup

				#include "TransformMatrix.cginc"

        struct Input 
				{
            half4 col;
        };

				struct Cube
				{
						half3 position;
						half3 velocity;
						half3 scale;
						half4 rotation;
				};


				#ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
						StructuredBuffer<Cube> cubeBuffer;
				#endif

        void setup()
        {
						#ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED

								half3 position 	=  cubeBuffer[unity_InstanceID].position;
								half3 scale = cubeBuffer[unity_InstanceID].scale;
								half4 rotation = cubeBuffer[unity_InstanceID].rotation;

								//apply transformation
								SetObjectTransform(rotation,position,scale);

						#endif
        }

        half _Glossiness;
        half _Alpha;
				half4 _Color;

        void surf (Input IN, inout SurfaceOutputStandard o) 
				{
            o.Albedo = _Color.rgb;
            o.Smoothness = _Glossiness;
            o.Alpha = _Alpha;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
