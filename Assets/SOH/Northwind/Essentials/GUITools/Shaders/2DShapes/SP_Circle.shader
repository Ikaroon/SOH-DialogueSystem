Shader "Hidden/Northwind/SP_Circle"
{
	SubShader
	{
		Tags{ "Queue" = "Transparent" }
		Cull Off

		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma fragment frag_shape
			#pragma vertex vert_shape
			#pragma geometry geom_shape

			#define ShapeFunc shape

			#include "ShapeProcedural.cginc"

			// --------------------------------------------<>

			float _Width;

			float inverseLerp(float a, float b, float val)
			{
				return (val - a)*(1 / (b - a));
			}

			float4 shape(input i)
			{
				float edgeDistance = abs(distance(i.uv, float2(0.5, 0.5)) - (1 - _Width * 2) * 0.5);
				edgeDistance = 1 - saturate(inverseLerp(0, _Width, edgeDistance));
				edgeDistance = inverseLerp(0, (_Border * 0.5 / (_Radius - _Width)), edgeDistance);
				edgeDistance = saturate(edgeDistance);


				float4 col = _Color;

				col = saturate(col * edgeDistance);
				col += saturate(_EdgeColor * 1 - edgeDistance);

				return col;
			}

			ENDCG
		}
	}
}
