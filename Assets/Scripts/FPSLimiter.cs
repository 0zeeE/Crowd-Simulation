using UnityEngine;

public class FPSLimiter : MonoBehaviour
{
    public int maxFPS = 60;
    void Start()
    {
        Application.targetFrameRate = maxFPS;  // Burada 60 FPS'e kilitleniyor
        QualitySettings.vSyncCount = 0;    // V-Sync kapatýlmalý, yoksa targetFrameRate etkisiz olur
    }
}
