using UnityEngine;

// Bu scripti B�y� Topu Prefab'�na ekle.
// Prefab'da ayr�ca Rigidbody ve SphereCollider (Is Trigger = true) olmal�.
public class BuyuTopu : MonoBehaviour
{
    [Header("B�y� Ayarlar�")]
    public float hiz = 25f;
    public float hasarMiktari = 10f; // Hasar� buradan alacak
    public float yasamSuresi = 4f; // 4 saniye sonra yok olur (menzil)

    void Start()
    {
        // B�y� topuna bir Rigidbody ver ve yer�ekimini kapat (Inspector'dan)
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            // Topu ileri do�ru f�rlat
            rb.linearVelocity = transform.forward * hiz;
        }
        else
        {
            Debug.LogError("BuyuTopu'nda Rigidbody bulunamad�!");
        }

        // B�y�y� 'yasamSuresi' saniye sonra yok et (kimseye �arpmazsa)
        Destroy(gameObject, yasamSuresi);
    }

    // Bir �eye �arpt���nda (Collider'� 'Is Trigger' olmal�)
    void OnTriggerEnter(Collider other)
    {
        // �arpt���m�z �eyde DusmanCanSistemi var m�?
        DusmanCanSistemi dusman = other.GetComponent<DusmanCanSistemi>();

        if (dusman != null)
        {
            // EVET! Senin DusmanCanSistemi script'ini bulduk.
            // Hasar ver.
            dusman.HasarAl(hasarMiktari);

            // B�y� topunu hemen yok et
            Destroy(gameObject);
        }
        else if (other.CompareTag("Player"))
        {
            // B�y� topu oyuncuya �arparsa (��kar ��kmaz) bir �ey yapma, i�inden ge�sin
            // Bu sat�r� bo� b�rakabilirsin veya bir ses efekti eklersin
        }
        else
        {
            // D��man de�il ama "Player" da de�ilse (yani Duvar, Zemin vs.)
            // B�y� topunu yok et
            Destroy(gameObject);
        }
    }
}