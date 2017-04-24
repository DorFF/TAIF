using UnityEngine;


[ExecuteInEditMode()]

public class RenderCubemapReflection : MonoBehaviour
{
    public enum Cubemapsize
    {
        Low = 1,
        Medium = 2,
        High = 3,
        Ultra = 4,
    }
    public enum Refreshrate
    {
        Low = 0,
        Medium = 1,
        High = 2
    }
    public enum Samplingquality
    {
        NoSampling = 0,
        Low = 1,
        High = 2
    }
    public static RenderCubemapReflection instance;
    public static RenderCubemapReflection GetInstance()
    {
        return instance;
    }
    public static Cubemapsize cubemapSize = Cubemapsize.Medium;
    public Refreshrate RefreshRate = Refreshrate.Medium;
    public static Samplingquality m_SamplingQuality = Samplingquality.NoSampling;
    public RenderingPath path = RenderingPath.DeferredShading;
    public float nearClip = 0.01f;
    public float farClip = 200;
    //[HideInInspector]
    public bool ProxyRendering = true;
    public static bool m_UseExternalCubemap = false;
    public static Cubemap m_ExternalCubemap;
    public static Cubemap m_OldExternalCubemap;
    private static int m_cubemapSize = 128;
    public LayerMask layerMask = ~(7 << 8);
    private static Camera cam;
    private static Camera camProxy;
    private static RenderTexture rtex;
    private static RenderTexture rtexProxy;
    private static Material MatProxy;
    private static Mesh MeshProxy;
    private int faceMask = 0;

    void Awake()
    {
        if (instance == null) instance = this;
    }

    void Start()
    {
        FaceRendering(6);
        //Projector shadow = base.GetComponentInChildren<Projector>();
        //shadow.ignoreLayers = 7 << 8;
    }

    public static void CubemapSize(int value)
    {
        cubemapSize = (Cubemapsize)value;
        switch (cubemapSize)
        {
            case Cubemapsize.Low: m_cubemapSize = 128; break;
            case Cubemapsize.Medium: m_cubemapSize = 256; break;
            case Cubemapsize.High: m_cubemapSize = 512; break;
            case Cubemapsize.Ultra: m_cubemapSize = 1024; break;
        }
    }

    public static void SamplingQuality(int value)
    {
     m_SamplingQuality = (Samplingquality)value;
     switch (m_SamplingQuality)
     {
       case Samplingquality.NoSampling:
       Shader.EnableKeyword("IBL_OFF");
       Shader.DisableKeyword("IBL_LOW");
       Shader.DisableKeyword("IBL_HIGH");
       break;
       case Samplingquality.Low:
       Shader.EnableKeyword("IBL_LOW");
       Shader.DisableKeyword("IBL_OFF");
       Shader.DisableKeyword("IBL_HIGH");
       break;
       case Samplingquality.High:
       Shader.EnableKeyword("IBL_HIGH");
       Shader.DisableKeyword("IBL_OFF");
       Shader.DisableKeyword("IBL_LOW");
       break;
     }
   }

    public static void ExternalCubemap(bool value)
    {
        ExternalCubemap(value, null);
    }

    public static void ExternalCubemap(bool value, Cubemap Cube)
    {
        m_UseExternalCubemap = value;
        if (Cube != null)
        {
            m_ExternalCubemap = Cube;
        }
        if (m_UseExternalCubemap && m_ExternalCubemap )
        {
            Shader.EnableKeyword("IBL_LOW");
            Shader.DisableKeyword("IBL_OFF");
            Shader.DisableKeyword("IBL_HIGH");
            Shader.SetGlobalTexture("_RefCube", m_ExternalCubemap);
            Shader.SetGlobalInt("_RefCubeSize", m_ExternalCubemap.width);
            CleanUp();
        }
        if (m_SamplingQuality == Samplingquality.NoSampling && !m_UseExternalCubemap)
        {
            Shader.EnableKeyword("IBL_OFF");
            Shader.DisableKeyword("IBL_LOW");
            Shader.DisableKeyword("IBL_HIGH");
        }
    }

    void Update()
    {

        if (m_SamplingQuality != Samplingquality.NoSampling && !m_UseExternalCubemap)
        {
            int FacesPerFrame = 1;
            switch (RefreshRate)
            {
                case Refreshrate.Low:
                    FacesPerFrame = 1;
                    break;
                case Refreshrate.Medium:
                    FacesPerFrame = 3;
                    break;
                case Refreshrate.High:
                    FacesPerFrame = 6;
                    break;
            }
            FaceRendering(FacesPerFrame);

        }
        else return;
    }

    void FaceRendering(int Rate)
    {
        for (int i = 0; i < Rate; i++)
        {
            UpdateCubemap(faceMask);
            faceMask++;
            if (faceMask > 5)
            {
                faceMask = 0;
            }
        }
    }

    void UpdateCubemap(int face)
    {
        if (!cam)
        {
            GameObject go = new GameObject("CubemapCamera", typeof(Camera));
            go.hideFlags = HideFlags.HideAndDontSave;
            go.transform.position = transform.position;
            go.transform.rotation = Quaternion.identity;
            //go.AddComponent<CubemapReflectionSettings>();
            cam = go.GetComponent<Camera>();
            cam.renderingPath = path;
            cam.fieldOfView = 90f;
            //cam.GetComponent<Camera>().renderingPath = path;
            cam.allowHDR = true;
            cam.cullingMask = layerMask;
            cam.nearClipPlane = nearClip;
            cam.farClipPlane = farClip;
            cam.enabled = false;
            cam.useOcclusionCulling = true;
            go.AddComponent<CameraQualitySettings>();
        }
        if (rtex && m_cubemapSize != rtex.width)
            DestroyImmediate(rtex);
        if (!rtex)
        {
            rtex = new RenderTexture(m_cubemapSize, m_cubemapSize, 0, RenderTextureFormat.ARGBHalf);
            //rtex.format = RenderTextureFormat.Default;
            //rtex.format = RenderTextureFormat.ARGBHalf;
            rtex.isPowerOfTwo = true;
            rtex.dimension = UnityEngine.Rendering.TextureDimension.Cube;
            rtex.useMipMap = true;
            rtex.hideFlags = HideFlags.HideAndDontSave;
        }
        if (!MeshProxy)
        {
            MeshProxy = new Mesh();
            MeshProxy.vertices = new Vector3[] { new Vector3(-1, -1, 1), new Vector3(-1, 1, 1), new Vector3(1, 1, 1), new Vector3(1, -1, 1) };
            MeshProxy.uv = new Vector2[] { new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 0), };
            MeshProxy.triangles = new int[] { 0, 1, 2, 0, 2, 3 };
        }
        if (ProxyRendering)
        {
            if (rtexProxy && m_cubemapSize != rtexProxy.width)
                DestroyImmediate(rtexProxy);
            if (!rtexProxy)
            {
                rtexProxy = new RenderTexture(m_cubemapSize, m_cubemapSize, 24, RenderTextureFormat.ARGBHalf);
               // rtexProxy.isPowerOfTwo = true;
                rtexProxy.filterMode = FilterMode.Point;
                rtexProxy.hideFlags = HideFlags.HideAndDontSave;
            }
            if (!MatProxy)
            {
            MatProxy = new Material(Shader.Find("Unlit/ZtestUnlit"));
           // MatProxy.SetColor("_Color", Color.red);
            MatProxy.SetTexture("_MainTex", rtexProxy);
            }
            if (!MeshProxy)
            {
                MeshProxy = new Mesh();
                MeshProxy.vertices = new Vector3[] { new Vector3(-1, -1, 1), new Vector3(-1, 1, 1), new Vector3(1, 1, 1), new Vector3(1, -1, 1) };
                MeshProxy.uv = new Vector2[] { new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 0), };
                MeshProxy.triangles = new int[] { 0, 1, 2, 0, 2, 3 };
            }
            if (!camProxy)
            {
                GameObject go1 = new GameObject("CubemapProxyCamera", typeof(Camera));
                go1.hideFlags = HideFlags.HideAndDontSave;
                go1.transform.position = transform.position;
                go1.transform.rotation = Quaternion.identity;
                //go.AddComponent<CubemapReflectionSettings>();
                //  go1.AddComponent
                camProxy = go1.GetComponent<Camera>();
                //camProxy.GetComponent<Camera>().renderingPath = RenderingPath.Forward;
                camProxy.renderingPath = RenderingPath.Forward;
                camProxy.clearFlags = CameraClearFlags.Color;
                camProxy.backgroundColor = Color.black;
                camProxy.allowHDR = true;
                camProxy.cullingMask = 0;
                camProxy.nearClipPlane = 0.1f;
                camProxy.farClipPlane = 100;
                camProxy.enabled = false;
            }
        }
        else
        {
            if (rtexProxy) DestroyImmediate(rtexProxy);
            if (MatProxy)  DestroyImmediate(MatProxy);
            if (camProxy)  DestroyImmediate(camProxy);
            if (MeshProxy) DestroyImmediate(MeshProxy);
            /*  if (rtex)
              {
                  rtex.useMipMap = true;
                  rtex.filterMode = FilterMode.Bilinear;
              }*/
        }
        try
        {
            cam.transform.position = transform.position;
        }
        catch
        {
           cam.transform.position = transform.position;
        }

        Vector3 vector = Vector3.zero;
        if (face == 1)
        {
            vector = new Vector3(0f, 270f, 0f);
        }
        else if (face == 2)
        {
            vector = new Vector3(270f, 0f, 0f);
        }
        else if (face == 3)
        {
            vector = new Vector3(90f, 0f, 0f);
        }
        else if (face == 4)
        {
            vector = new Vector3(0f, 0f, 0f);
        }
        else if (face == 5)
        {
            vector = new Vector3(180f, 0f, 180f);
        }
        else if (face == 0)
        {
            vector = new Vector3(0f, 90f, 0f);
        }

        if (ProxyRendering)
        {
            camProxy.transform.rotation = Quaternion.Euler(vector);
            cam.transform.rotation = Quaternion.Euler(vector);
            cam.targetTexture = rtexProxy;
            cam.Render();
            cam.targetTexture = null;
            camProxy.RenderToCubemap(rtex, ((int)1) << face);
        }
        else
        {
            cam.transform.rotation = Quaternion.Euler(vector);
            cam.RenderToCubemap(rtex, ((int)1) << face);
        }
        AssignMaterial();
    }

    public void AssignMaterial()
    {
        /*foreach (Renderer r in GetComponentsInChildren<Renderer>())//присоединяем к дочерним объектам
		{
			foreach (Material m in r.sharedMaterials)
			{
				if (m.HasProperty("_Cube"))
				{
					m.SetTexture("_Cube",rtex);
				}
				if(m.HasProperty("_CubeSize"))
				{
					m.SetFloat("_CubeSize", cubemapSize);
				}
			}
		}*/
        Shader.SetGlobalTexture("_RefCube", rtex);

        Shader.SetGlobalInt("_RefCubeSize", m_cubemapSize);

    }
    public void OnRenderObject()
    {
        if (ProxyRendering)
        {
            var cam = Camera.current;
            if (!cam || cam.gameObject.name != "CubemapProxyCamera")
            //if (!cam)
            {
                return;
            }
            if (MeshProxy && MatProxy)
            {
                MatProxy.SetPass(0);
                Graphics.DrawMeshNow(MeshProxy, cam.transform.position, cam.transform.rotation);
                // Graphics.DrawMeshNow(MeshProxy, Vector3.zero, Quaternion.identity);
            }
        }
    }

        void OnDisable()
    {
        CleanUp();
    }

    static void CleanUp()
    {
        if (rtexProxy)
            DestroyImmediate(rtexProxy);
        if (MeshProxy)
            DestroyImmediate(MeshProxy);
        if (MatProxy)
            DestroyImmediate(MatProxy);
        if (camProxy)
            DestroyImmediate(camProxy);
        if (cam)
            DestroyImmediate(cam);
        if (rtex)
            DestroyImmediate(rtex);
    }
}