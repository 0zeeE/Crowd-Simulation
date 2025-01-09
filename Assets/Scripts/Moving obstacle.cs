using UnityEngine;

public class PingPongMovement : MonoBehaviour
{
    public Vector3 movementAxis = Vector3.right; // Hareket ekseni (default: X ekseni)
    public float movementDistance = 5f; // Ýleri geri hareket mesafesi
    public float movementSpeed = 2f; // Hareket hýzý

    private Vector3 startPosition; // Baþlangýç pozisyonu

    void Start()
    {
        // Objenin baþlangýç pozisyonunu kaydet
        startPosition = transform.position;
    }

    void Update()
    {
        // PingPong fonksiyonu ile ileri geri hareket
        float pingPong = Mathf.PingPong(Time.time * movementSpeed, movementDistance);
        transform.position = startPosition + movementAxis.normalized * pingPong;
    }
}
