using UnityEngine;

namespace UnityStandardAssets.ImageEffects
{
[ExecuteInEditMode]
[RequireComponent (typeof(Camera))]
	[AddComponentMenu ("Image Effects/Rendering/Atmospheric Fog")]
	public class AtmosphericFog : PostEffectsBase
{
	public Shader fogShader = null;
	public Material m_atmosphereImageEffect = null;
	public Shader CombineFogShader;
	private Material m_CombineMaterial;
	
	new void Start()
	{
		Camera.main.depthTextureMode |= DepthTextureMode.Depth;
	}

	public override bool CheckResources ()
	{
		CheckSupport (true);
		
			m_atmosphereImageEffect = CheckShaderAndCreateMaterial (fogShader, m_atmosphereImageEffect);
			m_CombineMaterial =  CheckShaderAndCreateMaterial (CombineFogShader, m_CombineMaterial);
		
		if (!isSupported)
			ReportAutoDisable ();
		return isSupported;
	}

	[ImageEffectOpaque]
	void OnRenderImage(RenderTexture source, RenderTexture destination) 
	{	

		if (CheckResources()==false)
		{
			Graphics.Blit (source, destination);
			return;
		}
		//Graphics.Blit(source, destination);
		//return;
		
		//This will apply the atmospheric scattering to all objects in the scenne that have written to the depth buffer
		//The world pos is reconstructed from the depth values. To do the some information about the frustum must be passed
		//in to the shader. The code below calculates the position of the frustum corners
		//This method has been adapted from the global fog image effect
		
		float CAMERA_NEAR = Camera.main.nearClipPlane;
		float CAMERA_FAR = Camera.main.farClipPlane;
		float CAMERA_FOV = Camera.main.fieldOfView;
		float CAMERA_ASPECT_RATIO = Camera.main.aspect;
		
		Matrix4x4 frustumCorners = Matrix4x4.identity;		
		
		float fovWHalf = CAMERA_FOV * 0.5f;
		
		Vector3 toRight = Camera.main.transform.right * CAMERA_NEAR * Mathf.Tan (fovWHalf * Mathf.Deg2Rad) * CAMERA_ASPECT_RATIO;
		Vector3 toTop = Camera.main.transform.up * CAMERA_NEAR * Mathf.Tan (fovWHalf * Mathf.Deg2Rad);
		
		Vector3 topLeft = (Camera.main.transform.forward * CAMERA_NEAR - toRight + toTop);
		float CAMERA_SCALE = topLeft.magnitude * CAMERA_FAR/CAMERA_NEAR;	
		
		topLeft.Normalize();
		topLeft *= CAMERA_SCALE;
		
		Vector3 topRight = (Camera.main.transform.forward * CAMERA_NEAR + toRight + toTop);
		topRight.Normalize();
		topRight *= CAMERA_SCALE;
		
		Vector3 bottomRight = (Camera.main.transform.forward * CAMERA_NEAR + toRight - toTop);
		bottomRight.Normalize();
		bottomRight *= CAMERA_SCALE;
		
		Vector3 bottomLeft = (Camera.main.transform.forward * CAMERA_NEAR - toRight - toTop);
		bottomLeft.Normalize();
		bottomLeft *= CAMERA_SCALE;
		
		frustumCorners.SetRow (0, topLeft); 
		frustumCorners.SetRow (1, topRight);		
		frustumCorners.SetRow (2, bottomRight);
		frustumCorners.SetRow (3, bottomLeft);		
		
		m_atmosphereImageEffect.SetMatrix ("_FrustumCorners", frustumCorners);

		RenderTexture rt = RenderTexture.GetTemporary(source.width/2, source.height/2, 0, source.format);
		
			//CustomGraphicsBlit(source, destination, m_atmosphereImageEffect, 0);
			CustomGraphicsBlit(source, rt, m_atmosphereImageEffect, 0);
		
			m_CombineMaterial.SetTexture ("_FogTex", rt);
		
		Graphics.Blit (source, destination,m_CombineMaterial);
		RenderTexture.ReleaseTemporary(rt);
	}
	
	static void CustomGraphicsBlit(RenderTexture source, RenderTexture dest, Material fxMaterial, int passNr) 
	{
		RenderTexture.active = dest;
		
		fxMaterial.SetTexture ("_MainTex", source);	        
		
		GL.PushMatrix ();
		GL.LoadOrtho ();
		
		fxMaterial.SetPass (passNr);	
		
		GL.Begin (GL.QUADS);
		
		//This custom blit is needed as infomation about what corner verts relate to what frustum corners is needed
		//A index to the frustum corner is store in the z pos of vert
		
		GL.MultiTexCoord2 (0, 0.0f, 0.0f); 
		GL.Vertex3 (0.0f, 0.0f, 3.0f); // BL
		
		GL.MultiTexCoord2 (0, 1.0f, 0.0f); 
		GL.Vertex3 (1.0f, 0.0f, 2.0f); // BR
		
		GL.MultiTexCoord2 (0, 1.0f, 1.0f); 
		GL.Vertex3 (1.0f, 1.0f, 1.0f); // TR
		
		GL.MultiTexCoord2 (0, 0.0f, 1.0f); 
		GL.Vertex3 (0.0f, 1.0f, 0.0f); // TL
		
		GL.End ();
		GL.PopMatrix ();
		
	}	
}
}
