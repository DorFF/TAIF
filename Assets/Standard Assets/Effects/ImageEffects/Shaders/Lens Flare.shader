Shader "Hidden/Per-Pixel Lens Flare" {
	Properties 
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_FlareTex ("Base (RGB)", 2D) = "white" {}
		//_uLensColor ("Base (RGB)", 2D) = "white" {}
	}
	SubShader 
	{
		Pass
		{
		//ZTest Off Cull Off ZWrite Off Lighting Off Fog { Mode off }
		//Blend DstColor Zero  
		CGPROGRAM
		#pragma vertex vert_img
		#pragma fragment frag
		#pragma fragmentoption ARB_precision_hint_fastest
		#pragma target 3.0
		#include "UnityCG.cginc"
		

		uniform sampler2D _MainTex;
		uniform sampler2D _FlareTex;
		uniform sampler2D _DirtTex;
		uniform sampler2D _uLensColor;
		uniform float _uDispersal;
		uniform float _uHaloWidth;
		uniform float _uDistortion;
		uniform float _HaloIntenisty;
		uniform float _Threshold;
		uniform float _BlurSize;
		uniform float _Power;
		uniform int _uSamples;
		uniform float _HorizontalSize;
		uniform float4 _Result;
		uniform float _FlareBias;

//--------------Distortion Function---------------------------------------*/
		float4 texDistorted(in sampler2D tex, in float2 texcoord, in float2 direction, in float3 distortion)
		 {
			return float4(tex2D(tex, texcoord + direction * (distortion.r / 1)).r,
						  tex2D(tex, texcoord + direction * (distortion.g / 6)).g,
						  tex2D(tex, texcoord + direction * (distortion.b / 1)).b, 1.0);
		}

//------------------------------------------------------------------------*/

		
		float4 frag(v2f_img i) : COLOR
		{
			float2 mytexcoord = i.uv;
			float2 texcoord = -i.uv + float2(1.0, 1.0); //Flip the texcoordinates
			float2 texelSize = 1.0 / _HorizontalSize;
			
			float2 ghostVec = (float2(0.5, 0.5) - texcoord) * _uDispersal;
			
			float3 distortion = float3(-texelSize.x * _uDistortion, 0.0, texelSize.x * _uDistortion);
			
			//sample ghost
			//unroll [(8)]
			float4 _Result = float4(0, 0, 0, 0);
			for (int i = 0; i < 16; i++) 
			{
				float2 offset = frac(texcoord + ghostVec * float(i));
				
				float weight = length(float2(0.5, 0.5) - offset) / length(float2(0.5, 0.5));
				weight = pow(1.0 - weight, 10.0);
				
				_Result += texDistorted(_FlareTex, offset, normalize(ghostVec), distortion) * weight;
				//_Result+= tex2D(_FlareTex, offset) * weight;
			}
				float2 thistex = length(float2(0.5, 0.5) - texcoord) / length(float2(0.5, 0.5));
				//_Result /=16;
				_Result *= tex2D(_uLensColor, thistex);
				//float4 lens = tex2D(_DirtTex, texcoord);
				//lens = (lens * _Power);
//				_Result *= lens;
				
			//sample halo
			float2 haloVec = normalize(ghostVec) * _uHaloWidth;
			half thislength = length(float2(0.5, 0.5) - frac(texcoord + haloVec));
			float weight = thislength / length(float2(0.5, 0.5));
			weight = pow(1.0 - weight, _HaloIntenisty);
			_Result += texDistorted(_FlareTex, frac(texcoord + haloVec), normalize(ghostVec), distortion * 4.0f) * weight;
			_Result = max(0,(_Result-_FlareBias));
			
			//Add textures to _Result
			//float4 base = tex2D(_MainTex, texcoord);
			//lensMod += tex2D(_StarTex, texcoord);
			//_Result *= lensMod;
			_Result = saturate((_Result - _Threshold) / (1 - _Threshold));
			
			//float4 base = tex2D(_MainTex, texcoord);
			//_Result += base;
			
			return _Result;
			
			}
		

		ENDCG
		
	 }
  }
}