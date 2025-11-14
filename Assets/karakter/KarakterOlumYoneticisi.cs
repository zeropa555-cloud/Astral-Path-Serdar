using UnityEngine;

// Gerekli bileşenleri objede zorunlu kılar
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterController))]
public class KarakterOlumYoneticisi : MonoBehaviour
{
    // Script'in ihtiyaç duyduğu diğer bileşenler
    private Animator animator;
    private CharacterController controller;
    private SimpleMove playerMovementScript; // Karakter hareket script'iniz (Adi SimpleMove ise)

    // ----- 1. HATA BURADA: EKSİK OLAN SATIR BU -----
    // Saldırı script'inizin adı "PlayerAttack" ise böyle bırakın.
    // Değilse, "PlayerAttack" kelimesini kendi script'inizin adıyla değiştirin.
    private PlayerAttack playerAttackScript;

    private bool isDead = false;

    void Start()
    {
        // Gerekli bileşenleri bul ve hafızaya al
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        playerMovementScript = GetComponent<SimpleMove>(); // Script'inizin adı farklıysa burada değiştirin

        // ----- 2. HATA BURADA: EKSİK OLAN SATIR BU -----
        // Yukarıda adını ne yazdıysanız, buraya da aynısını yazın
        playerAttackScript = GetComponent<PlayerAttack>();

        if (playerAttackScript == null)
        {
            Debug.LogWarning("KarakterOlumYoneticisi: 'PlayerAttack' adında bir saldırı script'i bulunamadı. (Eğer script adınız farklıysa, Kodu güncelleyin)");
        }
    }

    // --- BU FONKSİYON SİNYAL İLE ÇAĞRILACAK ---
    public void OlumSisteminiTetikle()
    {
        if (isDead) return; // Zaten ölmüşse tekrar çalıştırma
        isDead = true;

        Debug.Log("KarakterOlumYoneticisi: Ölüm tetiklendi!");

        // 1. Karakterin hareket script'ini devre dışı bırak
        if (playerMovementScript != null)
        {
            playerMovementScript.enabled = false;
        }

        // ----- 3. HATA BURADAYDI (Satır 28) -----
        // 2. Saldırı script'ini devre dışı bırak
        // (Yukarıdaki 1. ve 2. satırları eklediğinizde bu hata düzelecektir)
        if (playerAttackScript != null)
        {
            playerAttackScript.enabled = false;
        }
        // ---------------------------------

        // 3. Character Controller'ı devredışı bırak (yerde kalsın, çarpışmasın)
        if (controller != null)
        {
            controller.enabled = false;
        }

        // 4. Ölüm animasyonunu tetikle
        if (animator != null)
        {
            animator.SetTrigger("Die");
        }

        // 5. Karakteri düşman AI'ı için "görünmez" yap
        int ignoreRaycastLayer = LayerMask.NameToLayer("Ignore Raycast");
        TumKatmanlariDegistir(transform, ignoreRaycastLayer);
    }

    /// <summary>
    /// Bu objenin ve tüm alt objelerinin katmanını değiştirir.
    /// </summary>
    void TumKatmanlariDegistir(Transform objTransform, int layer)
    {
        objTransform.gameObject.layer = layer;
        foreach (Transform child in objTransform)
        {
            TumKatmanlariDegistir(child, layer);
        }
    }
}