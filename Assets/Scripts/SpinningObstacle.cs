using UnityEngine;

public class PivotRotation : MonoBehaviour
{
    public Transform pivotPoint; // Objenin etrafýnda döneceði pivot noktasý
    public float rotationSpeed = 10f; // Dönme hýzý

    public bool rotateClockwise = true; // Saat yönünde mi dönecek? (true: saat yönünde, false: saat yönünün tersi)

    void Update()
    {
        if (pivotPoint != null)
        {
            // Dönüþ yönünü belirleme
            float direction = rotateClockwise ? 1f : -1f;

            // Pivot noktasýnýn yerel Y ekseni etrafýnda döndürme
            transform.RotateAround(pivotPoint.position, pivotPoint.up, direction * rotationSpeed * Time.deltaTime);
        }
    }
}
