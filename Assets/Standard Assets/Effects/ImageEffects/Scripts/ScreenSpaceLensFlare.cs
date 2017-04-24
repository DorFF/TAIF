using UnityEngine;
using System.Collections;
namespace UnityStandardAssets.ImageEffects
{
[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
	[AddComponentMenu("Image Effects/Camera/Screen Space Lens Flare")]
	public class ScreenSpaceLensFlare : PostEffectsBase {
		public float StarPower;
		public float DirtPower;
		[Range(0f, 1f)]
		public float FlareScale = 1f;

		[Range(0f, 10f)]
		public float FlareBias = 0.8f;

		[Range(0f, 1f)]
		public float Threshold = 0.0f;
		[Range(1, 8)]
		public int Downsampling = 2;

		public Texture2D LensColor;
		public float HaloWidth = 0.6f;
		public float HaloIntenisty = 5.0f;
		public float Dispersal = 0.3f;
		public float BlurDistance = 1.5f;
		public float Distortion = 2f;

		public Texture2D DirtTex;
		public Texture2D StarTex;

        private Shader LensFlareShader;
        private Shader CompositeShader;
        private Shader SeparableBlurShader;

		private Material LensFlareMaterial;
		private Material SeparableBlurMaterial;
		private Material CompositeMaterial;

	
		public override bool CheckResources ()
		{
			CheckSupport (false);

            if (!LensFlareShader)
                LensFlareShader = Shader.Find("Hidden/Per-Pixel Lens Flare");
            if (!CompositeShader)
                CompositeShader = Shader.Find("Hidden/CompositeLensFlare");
            if(!SeparableBlurShader)
                SeparableBlurShader = Shader.Find("Hidden/SeparableBlur");

            LensFlareMaterial = CheckShaderAndCreateMaterial (LensFlareShader, LensFlareMaterial);
			SeparableBlurMaterial = CheckShaderAndCreateMaterial(SeparableBlurShader,SeparableBlurMaterial);
			CompositeMaterial = CheckShaderAndCreateMaterial (CompositeShader, CompositeMaterial);
			
			if (!isSupported)
				ReportAutoDisable ();
			return isSupported;
		}
	void OnRenderImage(RenderTexture source, RenderTexture destination)
	{

			if (CheckResources()==false)
			{
				Graphics.Blit (source, destination);
				return;
			}
			var rtFormat= RenderTextureFormat.ARGBFloat;
			int width = source.width/Downsampling;
			int height = source.height/Downsampling;
			RenderTexture RT1 = RenderTexture.GetTemporary(width, height, 0, rtFormat);
			RenderTexture RT2 = RenderTexture.GetTemporary(width, height, 0, rtFormat);
			//Step #1: Generate Lens flare on Screen and threshold ADD downsampling later
			LensFlareMaterial.SetFloat("_HorizontalSize", 512.0f);
			//material.SetTexture("_DirtTex", DirtTex);
			//material.SetTexture("_StarTex", StarTex);
			LensFlareMaterial.SetFloat("_uHaloWidth", HaloWidth);
			LensFlareMaterial.SetFloat("_HaloIntenisty", HaloIntenisty);
			LensFlareMaterial.SetFloat("_uDistortion", Distortion);
			LensFlareMaterial.SetFloat("_uDispersal", Dispersal);
			LensFlareMaterial.SetTexture("_uLensColor", LensColor);
			LensFlareMaterial.SetFloat("_Threshold", Threshold);
			LensFlareMaterial.SetTexture("_FlareTex", source);
			LensFlareMaterial.SetFloat("_BlurSize", BlurDistance);
			LensFlareMaterial.SetFloat("_Power", DirtPower);
			LensFlareMaterial.SetFloat("_FlareBias", FlareBias);
			Graphics.Blit(source, RT1, LensFlareMaterial);

			float WidthOverHeight = width / height;
			float OneOverBase = 1.0f / 512.0f;
			//Seperable Blur (Unitys approach)
			for(int i = 0; i < 4; i++) 
			{
				SeparableBlurMaterial.SetVector ("offsets", new Vector4 (0.0f, BlurDistance * OneOverBase, 0.0f, 0.0f));
				Graphics.Blit (RT1, RT2, SeparableBlurMaterial); 
				SeparableBlurMaterial.SetVector ("offsets", new Vector4 (BlurDistance * OneOverBase / WidthOverHeight, 0.0f, 0.0f, 0.0f));
				Graphics.Blit (RT2, RT1, SeparableBlurMaterial);
			}
			RenderTexture.ReleaseTemporary (RT2);
			CompositeMaterial.SetTexture("_Result", RT1);
			CompositeMaterial.SetTexture("_StarTex", StarTex);
			CompositeMaterial.SetTexture("_DirtTex", DirtTex);
			CompositeMaterial.SetFloat("_PowerS", StarPower);
			CompositeMaterial.SetFloat("_PowerD", DirtPower);
			CompositeMaterial.SetFloat("_FlareScale", FlareScale);
			Graphics.Blit(source, destination, CompositeMaterial);
			//Graphics.Blit(source, destination);


			RenderTexture.ReleaseTemporary (RT1);

	}
	
	/*void OnDisable ()
	{
		if(CompositeMaterial)
			DestroyImmediate(CompositeMaterial);
	}*/
	
	
	}}