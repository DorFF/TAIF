Shader "Hidden/CompositeLensFlare" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_DirtTex ("Base (RGB)", 2D) = "grey" {}
	}
	SubShader 
	{
		Pass
		{
		//ZTest Always Cull Off ZWrite Off Lighting Off Fog { Mode off }
		CGPROGRAM
		#pragma vertex vert_img
		#pragma fragment frag
		#pragma fragmentoption ARB_precision_hint_fastest
		#include "UnityCG.cginc"

		uniform sampler2D _MainTex;
		uniform sampler2D _Result;
		uniform sampler2D _DirtTex;
		uniform sampler2D _StarTex;
		uniform float _PowerS;
		uniform float _PowerD;
		uniform float _FlareScale;

		
		float4 frag(v2f_img i) : COLOR
		{
			/*float4 screen = tex2D(_MainTex, i.uv);
			float4 flare = tex2D(_Result, i.uv);
			float4 stars = tex2D(_StarTex, i.uv);
			float4 dirt = tex2D(_DirtTex, i.uv);
			//flare *= (dirt * _PowerD) + flare;
			//flare *= (stars * _PowerS);// + flare;
			flare += screen;
			return flare;*/
			//float2 starcoord = (UNITY_MATRIX_V * float3(i.uv, 1.0)).xy;
			float4 lensMod = tex2D(_StarTex, i.uv);
			//lensMod += tex2D(_StarTex, i.uv);
      		float4 lensFlare = tex2D(_Result, i.uv);
      		//lensFlare = (lensFlare*)*_FlareScale;
      		float4 screen = tex2D(_MainTex, i.uv) + (lensFlare*lensMod*_FlareScale);
      		return screen;
     
		
		}

		ENDCG
		} 
	}
}