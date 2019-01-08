struct v2g
{
	float4 pos : SV_POSITION;
};

struct input
{
	float2 uv : TEXCOORD0;
	float4 vertex : SV_POSITION;
};

float4 _Color;
float4 _EdgeColor;

float _Border;
float _Radius;

float4 _WorldPos;

float4x4 _MAT_V;
float4x4 _MAT_P;

v2g vert_shape()
{
	v2g o;
	o.pos = _WorldPos;
	return o;
}

[maxvertexcount(4)]
void geom_shape(point v2g p[1], inout TriangleStream<input> triStream) {
	float3 pos = p[0].pos;
	pos = mul(_MAT_V, float4(pos.xyz, 1)).xyz;

	input v[4];

	v[0].vertex = float4(pos.xyz + float3(_Radius, _Radius, 0), 1);	//Top		Left
	v[0].vertex = mul(_MAT_P, v[0].vertex);
	v[0].uv = float2(0, 1);
	v[1].vertex = float4(pos.xyz + float3(_Radius, -_Radius, 0), 1);	//Bottom	Left
	v[1].vertex = mul(_MAT_P, v[1].vertex);
	v[1].uv = float2(0, 0);
	v[2].vertex = float4(pos.xyz + float3(-_Radius, _Radius, 0), 1);		//Top		Right
	v[2].vertex = mul(_MAT_P, v[2].vertex);
	v[2].uv = float2(1, 1);
	v[3].vertex = float4(pos.xyz + float3(-_Radius, -_Radius, 0), 1);	//Bottom	Right
	v[3].vertex = mul(_MAT_P, v[3].vertex);
	v[3].uv = float2(1, 0);

	triStream.Append(v[0]);
	triStream.Append(v[1]);
	triStream.Append(v[2]);
	triStream.Append(v[3]);

}

float4 ShapeFunc(input i);

float4 frag_shape(input i) : SV_Target
{
	return ShapeFunc(i);
}