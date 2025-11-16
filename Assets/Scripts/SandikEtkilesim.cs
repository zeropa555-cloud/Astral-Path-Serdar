using UnityEngine;
using UnityEngine.InputSystem;

public class SandikEtkilesim : MonoBehaviour
{
    // --- KAPI AÇMA İÇİN EKLENDİ ---
    [Header("Açılacak Kapılar")]
    [Tooltip("Sandık açılınca AÇILACAK (gizlenecek) kapı/bariyerler")]
    public GameObject[] acilacakKapilar;
    // -------------------------------

    // --- KAPSÜL (SENİN KODUNDA ZATEN VARDI) ---
    [Header("Sandık Ödülü")]
    [Tooltip("Sandık açılınca görünecek 'Zaman Kapsülü' UI objesi")]
    public GameObject zamanKapsuluUI;
    // ---------------------------------------

    [Header("Puzzle Ayarları")]
    public GameObject puzzlePaneli;

    private Animator animator;
    private bool oyuncuYakininda = false;
    private bool acildi = false;

    // --- BU FONKSİYON GÜNCELLENDİ (İKİSİNİ DE GİZLEMEK İÇİN) ---
    void Start()
    {
        animator = GetComponent<Animator>();

        if (puzzlePaneli != null)
        {
            puzzlePaneli.SetActive(false);
        }

        // Kapsülü gizle
        if (zamanKapsuluUI != null)
        {
            zamanKapsuluUI.SetActive(false);
        }

        // Kapıların da oyun başında AÇIK (gizli) olduğundan emin ol
        foreach (GameObject kapi in acilacakKapilar)
        {
            if (kapi != null) kapi.SetActive(false);
        }
    }
    // --- GÜNCELLEME BİTTİ ---

    void Update()
    {
        // (Bu fonksiyon aynı kaldı, dokunulmadı)
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        bool eTusunaBasildi = keyboard.eKey.wasPressedThisFrame;

        if (oyuncuYakininda && eTusunaBasildi && !acildi)
        {
            if (EnemySpawner.tumRoundlarBitti == true)
            {
                if (puzzlePaneli != null)
                {
                    puzzlePaneli.SetActive(true);
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                }
            }
            else
            {
                Debug.Log("KİLİTLİ! Önce arenadaki düşmanları yenmelisin!");
            }
        }
    }

    // --- BU FONKSİYON GÜNCELLENDİ (İKİSİNİ DE AKTİF ETMEK İÇİN) ---
    public void SandigiAc()
    {
        if (!acildi)
        {
            acildi = true;
            animator.SetTrigger("OpenChest"); // Animasyonu oynat
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            // 1. Zaman Kapsülünü göster
            if (zamanKapsuluUI != null)
            {
                zamanKapsuluUI.SetActive(true);
                Debug.Log("Zaman Kapsülü UI'ı gösteriliyor!");
            }

            // 2. KAPILARI AÇ!
            Debug.Log("Sandık açıldı, TUZAK KAPILARI AÇILIYOR!");
            foreach (GameObject kapi in acilacakKapilar)
            {
                if (kapi != null) kapi.SetActive(false); // Kapıları aç
            }
        }
    }
    // --- GÜNCELLEME BİTTİ ---

    private void OnTriggerEnter(Collider other)
    {
        // (Aynı kaldı)
        if (other.CompareTag("Player"))
        {
            oyuncuYakininda = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // (Aynı kaldı)
        if (other.CompareTag("Player"))
        {
            oyuncuYakininda = false;
            if (puzzlePaneli != null && puzzlePaneli.activeSelf)
            {
                puzzlePaneli.SetActive(false);
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }
}