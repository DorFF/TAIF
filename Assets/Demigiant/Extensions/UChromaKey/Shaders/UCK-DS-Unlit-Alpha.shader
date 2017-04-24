// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "UChromaKey/Direct source/Unlit transparent" {
Properties {
	_MainTex ("Video source (RGB)", 2D) = "white" {} 
	_Color ("Main Color", Color) = (1,1,1,1)	
	_CKCol ("ChromaKey Color", Color) = (1,1,1,1)	
	_Range ("Range", Range (0.0, 2.83)) = 0.01
	_HueRange ("Hue Range", Range (0.0, 5.0)) = 0.1
	_EdgeSharp ("Edge sharpness", Range (1.0, 20.0)) = 20.0
	_Opacity ("Opacity", Range (0.0,1.0)) = 1.0
	_uvShiftMulti ("Screen shift(XY) and multiplier(ZW)",Vector) = (0,0,1,1)		
	_flipHorizontal ("Horizontal flip",Int) = 0.0 
	_flipVertical ("Vertical flip",Int) = 0.0
	_Crop ("Crop", Vector) = (0,0,0,0)
	_uvDefX ("",Float) = 0.0
	_uvCoefX ("",Float) = 0.0
	_uvDefY ("",Float) = 0.0
	_uvCoefY ("",Float) = 0.0	
}

SubShader {
	Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
	LOD 100
	
	ZWrite Off
	Blend SrcAlpha OneMinusSrcAlpha 
	
	Pass {  
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			
			#include "UnityCG.cginc"

			struct appdata_t {
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				half2 texcoord : TEXCOORD0;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			fixed4 _Color;
			fixed4 _CKCol;
			fixed4 _NewColor;
			half _Range;
			half _HueRange;
			half _EdgeSharp;
			half _uvDefX;		
			half _uvCoefX;		
			half _uvDefY;		
			half _uvCoefY;
			half _Opacity;
			half4 _Crop;
			
			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				half2 nuv;
				nuv.x = (_uvDefX + _uvCoefX * i.texcoord.x);
				nuv.y = (_uvDefY + _uvCoefY * i.texcoord.y);
				
				if (!(nuv.x > (1 - _Crop.y) || nuv.x < _Crop.x || nuv.y > (1 - _Crop.z) || nuv.y < _Crop.w))
				{
					fixed4 c = tex2D(_MainTex, nuv) * _Color;
					half hueDiff = abs(atan2(1.73205 * (c.g - c.b), 2 * c.r - c.g - c.b + 0.001) - atan2(1.73205 * (_CKCol.g - _CKCol.b), 2 * _CKCol.r - _CKCol.g - _CKCol.b + 0.001));
					c.a = (1 - saturate((1 - ((c.r - _CKCol.r)*(c.r - _CKCol.r) + (c.g - _CKCol.g)*(c.g - _CKCol.g) + (c.b - _CKCol.b)*(c.b - _CKCol.b)) / (_Range * _Range)) * _EdgeSharp)
									* saturate(1.0 - min(hueDiff,6.28319 - hueDiff)/(_HueRange * _HueRange)) * _EdgeSharp) * _Opacity;
					return c;
				}
				else
				{
					return 0;
				}
			}
		ENDCG
	}
}
CustomEditor "UChromaKey_DS_unlit_editor" 
}
