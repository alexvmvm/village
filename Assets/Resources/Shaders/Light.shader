Shader "Custom/Lights/Light"
{
	Properties
	{
		_Color("Bottom Color", Color) = (1,1,1,1)
		_Radius("Radius", Float) = 1
	}

	SubShader
	{
		Tags
		{ 
			"RenderType" = "Transparent" 
			"IgnoreProjector" = "True"
			"Queue" = "Transparent+100" 
		}

		Pass
		{
			Cull Back
			Blend DstAlpha One
			ZTest LEqual
			ZWrite Off

			//Stencil
			//{
			//	Ref 1 
			//	Comp equal
			//}

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				fixed4 color : COLOR;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				fixed4 color : COLOR;
				float distance : float;
			};

			fixed4 _Color;
			float _Radius;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.distance = distance(float3(0, 0, 0), v.vertex);
				o.color = v.color;
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{				
				float distance = (i.distance / _Radius); // 0 -> 1 as progressing to outside
				float normalizedDistance = 1 - distance; // 1 -> 0 as progressing to outside 
				
				// 0 -> 0.43
				fixed colorOffset = lerp(_Color.a, 0, distance);

				return _Color * colorOffset;

				// return alpha 
				return fixed4(colorOffset, colorOffset, colorOffset, colorOffset);
			}

			ENDCG
		}

		//Pass 
		//{
		//	Cull Back
		//	Blend One OneMinusSrcAlpha

		//	Stencil
		//	{
		//		Ref 1
		//		Comp notequal
		//		Pass replace
		//	}

		//	CGPROGRAM
		//	#pragma vertex vert
		//	#pragma fragment frag

		//	#include "UnityCG.cginc"

		//		struct appdata
		//	{
		//		float4 vertex : POSITION;
		//		fixed4 color : COLOR;
		//	};

		//	struct v2f
		//	{
		//		float4 vertex : SV_POSITION;
		//		float distance : float2;
		//		fixed4 color : COLOR;
		//	};

		//	fixed4 _Color;
		//	float _Radius;

		//	v2f vert(appdata v)
		//	{
		//		v2f o;
		//		o.vertex = UnityObjectToClipPos(v.vertex);
		//		o.distance = distance(float3(0, 0, 0), v.vertex);
		//		o.color = v.color;
		//		return o;
		//	}

		//	fixed4 frag(v2f i) : SV_Target
		//	{
		//		
		//		return fixed4(0,0,0,0);

		//		float distance = (i.distance / _Radius); // 0 -> 1 as progressing to outside
		//		float normalizedDistance = 1 - distance; // 1 -> 0 as progressing to outside 

		//												 // 0 -> 0.43
		//		fixed colorOffset = lerp(0, _Color.a, distance);

		//		// return alpha 
		//		return fixed4(0, 0, 0, colorOffset);
		//	}

		//	ENDCG
		//}
	}
}





