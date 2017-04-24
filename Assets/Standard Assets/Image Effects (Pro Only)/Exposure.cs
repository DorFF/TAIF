using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Image Effects/Color Adjustments/Exposure")]
public class Exposure : ImageEffectBase {
	public float Expose=1;
	public float gamma=1;

	// Called by camera to apply image effect
	void OnRenderImage (RenderTexture source, RenderTexture destination) {
		material.SetFloat("_EV", Expose);
		material.SetFloat("_Gamma", gamma);
		Graphics.Blit (source, destination, material);
	}
}