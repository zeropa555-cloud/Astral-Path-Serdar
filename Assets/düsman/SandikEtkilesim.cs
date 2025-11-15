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
        // (Bu fonksiyon aynı, dokunulmadı)
        animator = GetComponent<Animator>();
        if (puzzlePaneli != null)
        {
            puzzlePaneli.SetActive(false);
        }
    }

    // --- BU FONKSİYON GÜNCELLENDİ ---
    void Update()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        bool eTusunaBasildi = keyboard.eKey.wasPressedThisFrame;

        // Eğer oyuncu yakındaysa, E'ye bastıysa ve sandık henüz açılmadıysa
        if (oyuncuYakininda && eTusunaBasildi && !acildi)
        {
            // --- YENİ KONTROL EKLENDİ ---
            // Spawner'a "Tüm round'lar bitti mi?" diye sor
            if (EnemySpawner.tumRoundlarBitti == true)
            {
                // Evet, bitti. Puzzle panelini AÇ.
                if (puzzlePaneli != null)
                {
                    puzzlePaneli.SetActive(true); // Paneli görünür yap
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                }
                else
                {
                    Debug.LogError("Puzzle Paneli kutucuğu boş!");
                }
            }
            else
            {
                // Hayır, bitmedi. Sandığı AÇMA.
                Debug.Log("KİLİTLİ! Sandığı açmak için önce tüm düşmanları yenmelisin!");
                // (Buraya "kilitli" sesi veya mesajı ekleyebilirsin)
            }
            // --- GÜNCELLEME BİTTİ ---
        }
    }
    // --- GÜNCELLEME BİTTİ ---

    // Bu fonksiyonu PuzzleYoneticisi scripti çağıracak
    public void SandigiAc()
    {
        // (Bu fonksiyon aynı, dokunulmadı)
        if (!acildi)
        {
            acildi = true;
            animator.SetTrigger("OpenChest");
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // (Bu fonksiyon aynı, dokunulmadı)
        if (other.CompareTag("Player"))
        {
            oyuncuYakininda = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // (Bu fonksiyon aynı, dokunulmadı)
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