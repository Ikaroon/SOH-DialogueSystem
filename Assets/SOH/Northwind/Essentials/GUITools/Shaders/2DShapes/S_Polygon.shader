Shader "Northwind/S_Polygon"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_EdgeColor("Edge Color", Color) = (1,1,1,0)
		_Radius("Radius", Range(0, 1)) = 1
		_Border("Border", Range(0, 1)) = 0.001
		_CornerCount("Corner Count", Int) = 6
		_EdgeBend("Edge Bend", Range(0, 1)) = 0.5
		_EdgeKnit("Edge Knit", Range(0, 1)) = 0
		_CornerBend("Corner Bend", Range(0, 1)) = 0
	}
	SubShader
	{
		Tags{ "Queue" = "Transparent" }
		Cull Off

		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#define PI 3.14159265359

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			float4 _Color;
			float4 _EdgeColor;
			float _Radius;
			float _Border;
			uint _CornerCount;
			float _EdgeBend;
			float _EdgeKnit;
			float _CornerBend;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			float inverseLerp(float a, float b, float val)
			{
				return (val - a)*(1 / (b - a));
			}

			float4 frag(v2f i) : SV_Target
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

				float edgeDistance = -(length(dir) - _Radius * 0.5);

				//return float4(edgeDistance.xxx, 1);

				edgeDistance = inverseLerp(0, (_Border * _Radius * 0.5), edgeDistance);
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
