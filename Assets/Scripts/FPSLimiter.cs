using UnityEngine;

public class FPSLimiter : MonoBehaviour
{
    public int maxFPS = 60;
    public int panicModeFps = 30;

    void Start()
    {
        Application.targetFrameRate = maxFPS;  // Burada 60 FPS'e kilitleniyor
        QualitySettings.vSyncCount = 0;    // V-Sync kapatýlmalý, yoksa targetFrameRate etkisiz olur
    }

    [ContextMenu("Update fps")]
    public void UpdateFPS()
    {
        Application.targetFrameRate = maxFPS+panicModeFps;  // Burada guncellenen FPS'e cekilecek.
    }
}
