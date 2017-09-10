int SetObjectTransform(half4 rotationData, half3 positionData, half3 scaleData)
{
		half4 q 			= rotationData;
		half qr			= q[0];
		half qi			= q[1];
		half qj			= q[2];
		half qk			= q[3];

		float4x4 translation = {
				scaleData.x,0,0,positionData.x,
				0,scaleData.y,0,positionData.y,
				0,0,scaleData.z,positionData.z,
				0,0,0,1
		};

		float4x4 rotation;

		rotation[0][0]			= 1.0f - 2.0f*qj*qj - 2.0f*qk*qk;
		rotation[0][1]			= 2.0f*(qi*qj - qk*qr);
		rotation[0][2]			= 2.0f*(qi*qk + qj*qr);
		rotation[0][3]			= 0.0f;

		rotation[1][0]			= 2.0f*(qi*qj+qk*qr);
		rotation[1][1]			= 1.0f - 2.0f*qi*qi - 2.0f*qk*qk;
		rotation[1][2]			= 2.0f*(qj*qk - qi*qr);
		rotation[1][3]			= 0.0f;

		rotation[2][0]			= 2.0f*(qi*qk - qj*qr);
		rotation[2][1]			= 2.0f*(qj*qk + qi*qr);
		rotation[2][2]			= 1.0f - 2.0f*qi*qi - 2.0f*qj*qj;
		rotation[2][3]			= 0.0f;

		rotation[3][0]			= 0.0f;
		rotation[3][1]			= 0.0f;
		rotation[3][2]			= 0.0f;
		rotation[3][3]			= 1.0f;

		unity_ObjectToWorld = mul(translation, rotation);

		float3x3 w2oRotation;
		w2oRotation[0] = unity_ObjectToWorld[1].yzx * unity_ObjectToWorld[2].zxy - unity_ObjectToWorld[1].zxy * unity_ObjectToWorld[2].yzx;
		w2oRotation[1] = unity_ObjectToWorld[0].zxy * unity_ObjectToWorld[2].yzx - unity_ObjectToWorld[0].yzx * unity_ObjectToWorld[2].zxy;
		w2oRotation[2] = unity_ObjectToWorld[0].yzx * unity_ObjectToWorld[1].zxy - unity_ObjectToWorld[0].zxy * unity_ObjectToWorld[1].yzx;
						
		half det = dot(unity_ObjectToWorld[0], w2oRotation[0]);
		w2oRotation = transpose(w2oRotation);
		w2oRotation *= rcp(det);
		half3 w2oPosition = mul(w2oRotation, -unity_ObjectToWorld._14_24_34);

		unity_WorldToObject._11_21_31_41 = float4(w2oRotation._11_21_31, 0.0f);
		unity_WorldToObject._12_22_32_42 = float4(w2oRotation._12_22_32, 0.0f);
		unity_WorldToObject._13_23_33_43 = float4(w2oRotation._13_23_33, 0.0f);
		unity_WorldToObject._14_24_34_44 = float4(w2oPosition, 1.0f);

		return 0;
}