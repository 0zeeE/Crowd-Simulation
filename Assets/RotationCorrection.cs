using UnityEngine;

public class FaceMovementDirection : MonoBehaviour
{
    private Vector3 lastPosition;

    void Start()
    {
        lastPosition = transform.position;
    }

    void Update()
    {
        Vector3 currentPosition = transform.position;

        // Hareket vekt�r�n� hesapla (yaln�zca x ve z)
        Vector3 movementDirection = currentPosition - lastPosition;
        movementDirection.y = 0; // Y eksenindeki fark� yok say

        if (movementDirection.sqrMagnitude > 0.01f) // Hareket olup olmad���n� kontrol et
        {
            Quaternion targetRotation = Quaternion.LookRotation(movementDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }

        lastPosition = currentPosition;
    }
}
