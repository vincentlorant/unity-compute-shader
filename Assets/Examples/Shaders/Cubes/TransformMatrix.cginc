/*
Base one unity graphics programming
https://github.com/IndieVisualLab/UnityGraphicsProgrammingFrontCover
*/

float4x4 eulerAnglesToRotationMatrix(float3 angles)
{
	float ch = cos(angles.y); float sh = sin(angles.y); // heading
	float ca = cos(angles.z); float sa = sin(angles.z); // attitude
	float cb = cos(angles.x); float sb = sin(angles.x); // bank
	// RyRxRz (Heading Bank Attitude)
	return float4x4(
	ch * ca + sh * sb * sa, -ch * sa + sh * sb * ca, sh * cb, 0,
	cb * sa, cb * ca, -sb, 0,
	-sh * ca + ch * sb * sa, sh * sa + ch * sb * ca, ch * cb, 0,
	0, 0, 0, 1
	);
}