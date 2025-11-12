using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Transform camTransform;

    void Start()
    {
        // Ana kameranin transformunu bul ve sakla
        camTransform = Camera.main.transform;
    }

    // LateUpdate, tum hareketler ve kamera guncellemeleri bittikten sonra calisir
    void LateUpdate()
    {
        if (camTransform == null) return;

        // Bu objenin (Canvas'in) kameraya bakmasini sagla
        // Kendi pozisyonuna kameranin "ileri" yonunu ekleyerek bakar
        transform.LookAt(transform.position + camTransform.forward);
    }
}