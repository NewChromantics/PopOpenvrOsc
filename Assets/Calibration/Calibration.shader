Shader "Unlit/Calibration"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		BoundsTopLeft("BoundsTopLeft", VECTOR ) = (-1,0,-1)
		BoundsTopRight("BoundsTopRight", VECTOR ) = ( 1,0,-1)
		BoundsBottomRight("BoundsBottomRight", VECTOR ) = ( 1,0, 1)
		BoundsBottomLeft("BoundsBottomLeft", VECTOR ) = (-1,0, 1)
		BoundsLineWidth("BoundsLineWidth", Range(0.1,1.0) ) = 0.1
		BoundsLineColour("BoundsLineColour", COLOR ) = (1,1,0,1)
		DisplayWidth("DisplayWidth", Range(1,40) ) = 40
		PlayerRadius("PlayerRadius", Range(0.1,10) ) = 2
		PlayerEnabledColour("PlayerEnabledColour", COLOR ) = (0,1,0,1)
		PlayerDisabledColour("PlayerDisableddColour", COLOR ) = (1,0,0,1)
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"
			#include "../PopUnityCommon/PopCommon.cginc"

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

			sampler2D _MainTex;
			float4 _MainTex_ST;


			float3 BoundsTopLeft;
			float3 BoundsTopRight;
			float3 BoundsBottomRight;
			float3 BoundsBottomLeft;
			float BoundsLineWidth;
			float4 BoundsLineColour;

			float DisplayWidth;
			#define DisplayAspectRatio	(2.0f)
			#define DisplayHeight		(DisplayWidth/DisplayAspectRatio)

			#define PLAYER_COUNT	10
			float4 PlayerPositions[PLAYER_COUNT];
			float PlayerRadius;
			float4 PlayerEnabledColour;
			float4 PlayerDisabledColour;


			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}


			void GetArrowLines(out float2 Front,out float2 Left,out float2 Right)
			{
				float Size = 3;
				float2 Middle = lerp( BoundsTopLeft.xz, BoundsTopRight.xz, 0.5f );
				float2 Delta = normalize(BoundsTopRight.xz - BoundsTopLeft.xz);
				float2 Forward = float2( Delta.y, -Delta.x );

				Front = Middle + (Forward*Size);
				Left = Middle - (Delta * Size);
				Right = Middle + (Delta * Size);
			}

			float GetArrowDistance(float2 xy)
			{
				float2 ArrowFront,ArrowLeft,ArrowRight;
				GetArrowLines( ArrowFront, ArrowLeft, ArrowRight );

				float Distance = 999;
				Distance = min( Distance, DistanceToLine2( xy, ArrowLeft, ArrowFront ) );
				Distance = min( Distance, DistanceToLine2( xy, ArrowRight, ArrowFront ) );

				return Distance;						
			}

			float GetCenterDistance(float2 xy)
			{
				return length(xy) / 2.0f;
			}


			float4 GetPlayerPos(int PlayerIndex)
			{
				//	to aid debugging clip to edge
				float4 PlayerPos = PlayerPositions[PlayerIndex];

				PlayerPos.x = clamp( PlayerPos.x, -DisplayWidth, DisplayWidth );
				PlayerPos.z = clamp( PlayerPos.z, -DisplayHeight, DisplayHeight );

				return PlayerPos;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				float2 uv = lerp( float2(-1,1), float2(1,-1), i.uv );
				float4 Colour = float4( uv.x, uv.y, 0, 1 );

				float2 xy = uv * float2( DisplayWidth, DisplayHeight );
				float BoundsDistance = 999;

				BoundsDistance = min( BoundsDistance, DistanceToLine2( xy, BoundsTopLeft.xz, BoundsTopRight.xz ) );
				BoundsDistance = min( BoundsDistance, DistanceToLine2( xy, BoundsTopRight.xz, BoundsBottomRight.xz ) );
				BoundsDistance = min( BoundsDistance, DistanceToLine2( xy, BoundsBottomRight.xz, BoundsBottomLeft.xz ) );
				BoundsDistance = min( BoundsDistance, DistanceToLine2( xy, BoundsBottomLeft.xz, BoundsTopLeft.xz ) );
				BoundsDistance = min( BoundsDistance, GetArrowDistance(xy) );
				BoundsDistance = min( BoundsDistance, GetCenterDistance(xy) );

				if ( BoundsDistance < BoundsLineWidth )
					Colour = BoundsLineColour;

				for ( int p=0;	p<PLAYER_COUNT;	p++ )
				{
					float4 PlayerPos = GetPlayerPos(p);
					float Enabled = PlayerPos.w;

					//	never detected
					if ( Enabled < 0.5f )
						continue;

					if ( length(xy-PlayerPos.xz) < PlayerRadius )
					{
						Colour = Enabled > 0.5f ? PlayerEnabledColour : PlayerDisabledColour;
					}
				}

				return Colour;
			}
			ENDCG
		}
	}
}
