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
        #pragma surface surf Standard vertex:vert addshadow
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
        
        /*
        Base one unity graphics programming
        https://github.com/IndieVisualLab/UnityGraphicsProgrammingFrontCover
        */
        void vert(inout appdata_full v)
        {
          #ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED

								half3 position 	=  cubeBuffer[unity_InstanceID].position;
								half3 scale = cubeBuffer[unity_InstanceID].scale;
								half4 rotation = cubeBuffer[unity_InstanceID].rotation;

                float4x4 object2world = (float4x4)0;
                object2world._11_22_33_44 = float4(scale.xyz, 1.0);

                float4x4 rotMatrix = eulerAnglesToRotationMatrix(rotation.xyz);

                object2world = mul(rotMatrix, object2world);
                object2world._14_24_34 += position.xyz;

                v.vertex = mul(object2world, v.vertex);
                v.normal = normalize(mul(object2world, v.normal));
						#endif
        }

        void setup()
        {
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
