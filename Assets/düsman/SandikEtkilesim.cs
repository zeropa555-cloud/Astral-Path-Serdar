using UnityEngine;
using UnityEngine.InputSystem;

public class SandikEtkilesim : MonoBehaviour
{
    [Header("Puzzle Ayarları")]
    public GameObject puzzlePaneli; // Unity'den Puzzle Panelini buraya sürükle

    private Animator animator;
    private bool oyuncuYakininda = false;
    private bool acildi = false;

    void Start()
    {
        animator = GetComponent<Animator>();

        // Oyun başlarken puzzle paneli açıksa kapatalım
        if (puzzlePaneli != null)
        {
            puzzlePaneli.SetActive(false);
        }
    }

    void Update()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        bool eTusunaBasildi = keyboard.eKey.wasPressedThisFrame;

        // Eğer oyuncu yakındaysa, E'ye bastıysa ve sandık henüz açılmadıysa
        if (oyuncuYakininda && eTusunaBasildi && !acildi)
        {
            // DİKKAT: Sandığı açma! Sadece Puzzle Panelini aç.
            if (puzzlePaneli != null)
            {
                puzzlePaneli.SetActive(true); // Paneli görünür yap
            }
            else
            {
                Debug.LogError("Kanka 'Puzzle Paneli' kutucuğu boş! Inspector'dan paneli sürükle.");
            }
        }
    }

    // --- BU FONKSİYONU PUZZLE SCRİPTİ ÇAĞIRACAK ---
    public void SandigiAc()
    {
        if (!acildi)
        {
            acildi = true;
            animator.SetTrigger("OpenChest"); // Animasyonu şimdi oynat
        }
    }
    // ----------------------------------------------

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            oyuncuYakininda = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            oyuncuYakininda = false;
            // Oyuncu uzaklaşırsa puzzle paneli kapansın (isteğe bağlı)
            if (puzzlePaneli != null) puzzlePaneli.SetActive(false);
        }
    }
}