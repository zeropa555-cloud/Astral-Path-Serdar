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
            // Sandığı açma! Sadece Puzzle Panelini aç.
            if (puzzlePaneli != null)
            {
                puzzlePaneli.SetActive(true); // Paneli görünür yap

                // Mouse'u görünür yap
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                Debug.LogError("Puzzle Paneli kutucuğu boş!");
            }
        }
    }

    // Bu fonksiyonu PuzzleYoneticisi scripti çağıracak
    public void SandigiAc()
    {
        if (!acildi)
        {
            acildi = true;
            animator.SetTrigger("OpenChest"); // Animasyonu şimdi oynat

            // Mouse'u tekrar gizle
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

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

            // Oyuncu uzaklaşırsa puzzle paneli de kapansın
            if (puzzlePaneli != null && puzzlePaneli.activeSelf)
            {
                puzzlePaneli.SetActive(false);
                // Mouse'u tekrar gizle
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }
}