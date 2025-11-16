using UnityEngine;

// Bu script, kapýyý KAPATAN tuzak trigger'ýdýr.
[RequireComponent(typeof(BoxCollider))]
public class KapanmaTetikleyicisi : MonoBehaviour
{
    [Header("Kapanacak Kapýlar")]
    [Tooltip("Oyuncu bu alana girince KAPANACAK (Aktif olacak) kapý engelleri")]
    public GameObject[] kapanacakKapilar;

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

            Debug.Log("TUZAK ÇALIÞTI! Kapýlar kapanýyor.");

            // Listeye eklenen tüm kapýlarý KAPAT (Aktif et)
            foreach (GameObject kapi in kapanacakKapilar)
            {
                if (kapi != null)
                {
                    kapi.SetActive(true);
                }
            }

            // (Ýsteðe baðlý) Bu tetikleyiciyle iþimiz bitti, kendini de kapatabilir
            // gameObject.SetActive(false);
        }
    }
}