using UnityEngine;
using UnityEngine.InputSystem; // 1. BU SATIRI EKLE (Yeni sistemi dahil et)

public class SandikEtkilesim : MonoBehaviour
{
    private Animator animator;
    private bool oyuncuYakininda = false;
    private bool acildi = false;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // --- YENİ GİRDİ SİSTEMİNE GÖRE GÜNCELLENEN KISIM ---

        // 1. Mevcut klavyeyi kontrol et
        var keyboard = Keyboard.current;
        if (keyboard == null)
        {
            // Klavye bağlı değilse veya sistem hazır değilse bir şey yapma
            return;
        }

        // 2. 'E' tuşuna bu frame (kare) basıldı mı?
        // Input.GetKeyDown(KeyCode.E) yerine bunu kullanıyoruz:
        bool eTusunaBasildi = keyboard.eKey.wasPressedThisFrame;

        // --- GÜNCELLENEN KISMIN SONU ---


        // 1. Koşul: Oyuncu yakınımızda mı?
        // 2. Koşul: Oyuncu 'E' tuşuna bastı mı?
        // 3. Koşul: Sandık daha önce açılmadı mı?
        if (oyuncuYakininda && eTusunaBasildi && !acildi)
        {
            // Eğer tümü doğruysa:
            // 1. Sandık artık "açıldı" olarak işaretle
            acildi = true;
            
            // 2. Animator'e "OpenChest" adındaki tetikleyiciyi gönder
            animator.SetTrigger("OpenChest");
        }
    }

    // Bu fonksiyon, Sandığın "Is Trigger" işaretli Collider'ına BİR ŞEY GİRDİĞİNDE çalışır
    private void OnTriggerEnter(Collider other)
    {
        // Giren objenin etiketi (tag) "Player" mı?
        if (other.CompareTag("Player"))
        {
            oyuncuYakininda = true;
            Debug.Log("Oyuncu sandık alanına girdi.");
        }
    }

    // Bu fonksiyon, o BİR ŞEY alandan ÇIKTIĞINDA çalışır
    private void OnTriggerExit(Collider other)
    {
        // Çıkan obje "Player" mı?
        if (other.CompareTag("Player"))
        {
            oyuncuYakininda = false;
            Debug.Log("Oyuncu sandık alanından çıktı.");
        }
    }
}