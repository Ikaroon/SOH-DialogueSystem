Shader "Northwind/S_Disc"
{
	Properties
	{
		_Color ("Color", Color) = (1,1,1,1)
		_EdgeColor("Edge Color", Color) = (1,1,1,0)
		_Radius("Radius", Range(0, 1)) = 1
		_Border("Border", Range(0, 1)) = 0.001
	}
	SubShader
	{
		Tags { "Queue"="Transparent" }
		Cull Off

		Pass
		{ 
			Blend SrcAlpha OneMinusSrcAlpha
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

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
			
			v2f vert (appdata v)
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
			
			float4 frag (v2f i) : SV_Target
			{
				float edgeDistance = -(distance(i.uv,float2(0.5, 0.5)) - _Radius * 0.5);
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
