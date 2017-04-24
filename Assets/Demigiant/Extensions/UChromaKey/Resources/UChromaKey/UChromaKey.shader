Shader "Hidden/UChromaKey" 
{
	Properties
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}		
	}

	SubShader
	{
		Pass
		{
		
		CGPROGRAM
		#pragma vertex vert_img
		#pragma fragment frag		
		#pragma target 3.0
		
		#include "UnityCG.cginc"

		uniform sampler2D _MainTex;
		uniform sampler2D _UChromaKeyTex;
		fixed4 _PatCol;		
		half _Range;
		half _HueRange;
		half _uvDefX;		
		half _uvCoefX;		
		half _uvDefY;		
		half _uvCoefY;
		half _opacity;
		half _smoothing;
		half4 _Crop;
		
		
		float4 frag(v2f_img i) : COLOR
		{
			half2 nuv = i.uv;
			nuv.x = nuv.x * _uvCoefX;
			nuv.y = nuv.y * _uvCoefY;
			half2 shiftUV = (_uvDefX,_uvDefY);
			nuv = nuv + shiftUV;
			float4 mc = tex2D(_MainTex, i.uv);
			float4 c = tex2D(_UChromaKeyTex,nuv);
			
			if (!(nuv.x > (1 - _Crop.y) || nuv.x < _Crop.x || nuv.y > (1 - _Crop.z) || nuv.y < _Crop.w))
			{										
				half hueDiff = abs(atan2(1.73205 * (c.g - c.b), 2 * c.r - c.g - c.b + 0.001) - atan2(1.73205 * (_PatCol.g - _PatCol.b), 2 * _PatCol.r - _PatCol.g - _PatCol.b + 0.001));
			
				mc.rgb = lerp(lerp(mc.rgb, c.rgb,_opacity),mc.rgb,
								saturate((1.0 - ((c.r - _PatCol.r)*(c.r - _PatCol.r) + (c.g - _PatCol.g)*(c.g - _PatCol.g) + (c.b - _PatCol.b)*(c.b - _PatCol.b)) / (_Range * _Range))*_smoothing)
								* saturate((1.0 - min(hueDiff,6.28319 - hueDiff)/(_HueRange * _HueRange))*_smoothing));			
			}
			
			return mc;
		}

		ENDCG
		} 
	}
}