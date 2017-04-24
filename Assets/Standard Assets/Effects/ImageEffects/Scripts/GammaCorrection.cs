using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class GammaCorrection : MonoBehaviour {

    private Shader shader;
    private Material material;
    private bool isSupported;

    void Start()
    {
        isSupported = true;

        if (!shader)
            shader = Shader.Find("Hidden/GammaTextureCorrection");

        if (!material)
            material = new Material(shader);

        if (!SystemInfo.supportsImageEffects || !SystemInfo.supportsRenderTextures || !SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGBHalf))
        {
            isSupported = false;
        }
    }
    void OnDisable()
    {
        if (material)
            DestroyImmediate(material);
    }
    // Update is called once per frame
    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (!isSupported)
        {
            Graphics.Blit(source, destination);
            return;
        }

        if (!material)
            material = new Material(shader);

        material.hideFlags = HideFlags.HideAndDontSave;

        //RenderTexture rt = RenderTexture.GetTemporary(source.width, source.height, 0, source.format);

        Graphics.Blit(source, destination, material);
    }
}
