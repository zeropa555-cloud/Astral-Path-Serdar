using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class TeleportKapisi : MonoBehaviour
{
    [Header("Ayarlar")]
    [Tooltip("Oyuncunun ýþýnlanacaðý nokta (Boþ bir obje)")]
    public Transform gidilecekYer;

    void Start()
    {
        // Tetikleyici olduðundan emin olalým
        GetComponent<BoxCollider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Çarpan þey Oyuncu mu?
        if (other.CompareTag("Player"))
        {
            Debug.Log("Oyuncu ýþýnlanýyor...");
            Isinla(other.transform);
        }
    }

    void Isinla(Transform oyuncu)
    {
        // 1. Eðer karakterde 'CharacterController' varsa önce onu KAPATMALIYIZ.
        // (Yoksa Unity pozisyon deðiþtirmeye izin vermez)
        CharacterController cc = oyuncu.GetComponent<CharacterController>();
        if (cc != null)
        {
            cc.enabled = false;
        }

        // 2. Pozisyonu ve Yönü deðiþtir (IÞINLA!)
        oyuncu.position = gidilecekYer.position;
        oyuncu.rotation = gidilecekYer.rotation; // Gittiði yerdeki okun yönüne baksýn

        // 3. CharacterController'ý geri AÇ.
        if (cc != null)
        {
            cc.enabled = true;
        }
    }
}