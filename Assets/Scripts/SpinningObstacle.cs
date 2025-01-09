using UnityEngine;

public class PivotRotation : MonoBehaviour
{
    public Transform pivotPoint; // Objenin etraf�nda d�nece�i pivot noktas�
    public float rotationSpeed = 10f; // D�nme h�z�

    public bool rotateClockwise = true; // Saat y�n�nde mi d�necek? (true: saat y�n�nde, false: saat y�n�n�n tersi)

    void Update()
    {
        if (pivotPoint != null)
        {
            // D�n�� y�n�n� belirleme
            float direction = rotateClockwise ? 1f : -1f;

            // Pivot noktas�n�n yerel Y ekseni etraf�nda d�nd�rme
            transform.RotateAround(pivotPoint.position, pivotPoint.up, direction * rotationSpeed * Time.deltaTime);
        }
    }
}
