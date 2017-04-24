using UnityEngine;
using System.Collections;

public class CameraQualitySettings : MonoBehaviour
{

    private float QualShadow;
    private int PixelLight;
    // private int TextureLimit;
    void OnPreRender()
    {
        QualShadow = QualitySettings.shadowDistance;
        PixelLight = QualitySettings.pixelLightCount;
        //TextureLimit = QualitySettings.masterTextureLimit;
        QualitySettings.shadowDistance = 50;
        QualitySettings.pixelLightCount = 2;
       // QualitySettings.masterTextureLimit = 5;
    }
    void OnPostRender()
    {
        QualitySettings.shadowDistance = QualShadow;
        QualitySettings.pixelLightCount = PixelLight;
        //  QualitySettings.masterTextureLimit = TextureLimit;
    }
}