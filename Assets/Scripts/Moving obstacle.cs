using UnityEngine;

public class PingPongMovement : MonoBehaviour
{
    public Vector3 movementAxis = Vector3.right; // Hareket ekseni (default: X ekseni)
    public float movementDistance = 5f; // �leri geri hareket mesafesi
    public float movementSpeed = 2f; // Hareket h�z�

    private Vector3 startPosition; // Ba�lang�� pozisyonu

    void Start()
    {
        // Objenin ba�lang�� pozisyonunu kaydet
        startPosition = transform.position;
    }

    void Update()
    {
        // PingPong fonksiyonu ile ileri geri hareket
        float pingPong = Mathf.PingPong(Time.time * movementSpeed, movementDistance);
        transform.position = startPosition + movementAxis.normalized * pingPong;
    }
}
