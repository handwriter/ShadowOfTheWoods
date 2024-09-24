#if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
StructuredBuffer<float3> PositionsBuffer;
#endif

float3 position;

void ConfigureProcedural () {
	#if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
	position = PositionsBuffer[unity_InstanceID];
	#endif
}

void ShaderGraphFunction_float (out float3 PositionOut) {
	PositionOut = position;
}

void ShaderGraphFunction_half (out half3 PositionOut) {
	PositionOut = position;
}