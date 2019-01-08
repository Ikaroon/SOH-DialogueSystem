Shader "Hidden/Northwind/SP_Polygon"
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

			#define PI 3.14159265359

			// --------------------------------------------<>

			uint _CornerCount;
			float _EdgeBend;
			float _EdgeKnit;
			float _CornerBend;
	
			float inverseLerp(float a, float b, float val)
			{
				return (val - a)*(1 / (b - a));
			}

			float4 shape(input i)
			{
				float2 dir = i.uv - float2(0.5, 0.5);

				float desiredAngle = 360 / _CornerCount;
				float angle = abs(atan2(dir.y, dir.x) * 180 / PI);

				float angleVal = (angle % desiredAngle) / desiredAngle;
				float changeValSquared = ((2 * angleVal - 1) * (2 * angleVal - 1) - 1) * _EdgeBend; //(2*x-1)^2-1
				float changeValKnit = (sqrt((2 * angleVal - 1) * (2 * angleVal - 1)) - 1) * _EdgeBend;

				float cornerAngleVal = sqrt((angleVal * 2 - 1) * (angleVal * 2 - 1));
				float changeCornerVal = ((2 * angleVal) * (2 * angleVal) - 1) * _CornerBend;

				float changeVal = lerp(changeValSquared, changeValKnit, _EdgeKnit);// *changeCornerVal;

				dir = lerp(i.uv, float2(0.5, 0.5), changeVal) - float2(0.5, 0.5);

				float edgeDistance = -(length(dir) - 0.5);

				//return float4(edgeDistance.xxx, 1);

				edgeDistance = inverseLerp(0, (_Border * 0.5 / _Radius), edgeDistance);
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
