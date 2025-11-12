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

    private bool isDead = false;

    void Start()
    {
        // Gerekli bileşenleri bul ve hafızaya al
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        playerMovementScript = GetComponent<SimpleMove>(); // Script'inizin adı farklıysa burada değiştirin
    }

    // --- BU FONKSİYON SİNYAL İLE ÇAĞRILACAK ---
    // Bu fonksiyonun public (genel) olması SART!
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

        // 2. Character Controller'ı devredışı bırak (yerde kalsın, çarpışmasın)
        if (controller != null)
        {
            controller.enabled = false;
        }

        // 3. Ölüm animasyonunu tetikle
        // (Karakterinizin Animator'unde "Die" adında bir Trigger olmalı)
        if (animator != null)
        {
            animator.SetTrigger("Die");
        }
    }
}