using UnityEngine;

// Bu script, kapýyý KAPATAN ve Uzaylýyý ORTAYA ÇIKARAN tuzak trigger'ýdýr.
[RequireComponent(typeof(BoxCollider))]
public class KapanmaTetikleyicisi1 : MonoBehaviour
{
    [Header("Tuzak Ayarlarý")]
    [Tooltip("Oyuncu bu alana girince KAPANACAK (Aktif olacak) kapý engelleri")]
    public GameObject[] kapanacakKapilar;

    // --- YENÝ EKLENDÝ ---
    [Tooltip("Tuzak çalýþýnca ORTAYA ÇIKACAK (Aktif olacak) Uzaylý/NPC")]
    public GameObject ortayaCikacakUzayli;
    // --------------------

    private bool tetiklendiMi = false;

    void Start()
    {
        // Bu objenin trigger olduðundan emin ol
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Eðer giren "Player" ise VE bu tuzak daha önce çalýþmadýysa
        if (other.CompareTag("Player") && !tetiklendiMi)
        {
            tetiklendiMi = true; // Tuzaðý bir kez çalýþtýr

            Debug.Log("TUZAK ÇALIÞTI! Kapýlar kapanýyor, Uzaylý beliriyor...");

            // 1. Listeye eklenen tüm kapýlarý KAPAT (Aktif et)
            foreach (GameObject kapi in kapanacakKapilar)
            {
                if (kapi != null)
                {
                    kapi.SetActive(true);
                }
            }

            // 2. Uzaylýyý ORTAYA ÇIKAR (Aktif et) --- YENÝ KISIM ---
            if (ortayaCikacakUzayli != null)
            {
                ortayaCikacakUzayli.SetActive(true);
            }
            // -----------------------------------------------------
        }
    }
}