using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.SceneManagement; // <-- YENİ EKLENDİ (Sahne değişimi için ŞART)

public class KarakterCanSistemi : MonoBehaviour
{
    [Header("Can Ayarları")]
    public float maxCan = 100f;

    [Header("Ölüm Ayarları")]
    [Tooltip("Karakter ölünce açılacak UI Paneli")]
    public GameObject olumPaneli; // <-- YENİ EKLENDİ (Buraya Panelini sürükle)

    [Header("Mevcut Durum")]
    [SerializeField]
    private float mevcutCan;

    // Düşmanların durması ve input kontrolü için
    public bool hayattaMi { get; private set; } // <-- YENİ EKLENDİ

    private Animator animator;

    [Header("UI Sinyali")]
    public UnityEvent<float> OnCanDegisti;
    public UnityEvent OnOlum;

    [Header("Animasyon Ayarları")]
    [Tooltip("Animator'deki Hasar Alma Trigger'ının tam adı")]
    public string hasarTriggerAdi = "Hit";

    [Tooltip("Hasar aldıktan KAÇ SANİYE SONRA 'Hit' animasyonu başlasın?")]
    public float hasarAnimasyonGecikmesi = 0.2f;

    private Coroutine hasarCoroutine;

    void Start()
    {
        mevcutCan = maxCan;
        hayattaMi = true; // <-- YENİ EKLENDİ (Oyuna hayatta başla)
        animator = GetComponentInChildren<Animator>();

        if (animator == null)
        {
            Debug.LogWarning(gameObject.name + ": KarakterCanSistemi bir Animator bulamadı!");
        }

        OnCanDegisti.Invoke(mevcutCan / maxCan);

        // Panelin başta kapalı olduğundan emin ol
        if (olumPaneli != null)
        {
            olumPaneli.SetActive(false);
        }
    }

    public void HasarAl(float miktar)
    {
        if (!hayattaMi) return; // <-- GÜNCELLENDİ (Ölüysek hasar alma)

        mevcutCan -= miktar;
        mevcutCan = Mathf.Clamp(mevcutCan, 0f, maxCan);

        OnCanDegisti.Invoke(mevcutCan / maxCan);

        if (mevcutCan <= 0)
        {
            OlumFonksiyonu();
        }
        else
        {
            // Can BİTMEDİYSE: Gecikmeli animasyon zamanlayıcısını başlat
            if (hasarCoroutine != null)
            {
                StopCoroutine(hasarCoroutine);
            }
            hasarCoroutine = StartCoroutine(GecikmeliHasarAnimasyonu());
        }
    }

    IEnumerator GecikmeliHasarAnimasyonu()
    {
        yield return new WaitForSeconds(hasarAnimasyonGecikmesi);

        // Ölmediysek animasyonu oynat
        if (hayattaMi && animator != null && !string.IsNullOrEmpty(hasarTriggerAdi))
        {
            // Debug.Log("Gecikme bitti. Animasyon tetikleniyor: " + hasarTriggerAdi); 
            animator.SetTrigger(hasarTriggerAdi);
        }

        hasarCoroutine = null;
    }

    public void Iyiles(float miktar)
    {
        if (!hayattaMi || mevcutCan == maxCan) return; // <-- GÜNCELLENDİ

        mevcutCan += miktar;
        mevcutCan = Mathf.Clamp(mevcutCan, 0f, maxCan);
        OnCanDegisti.Invoke(mevcutCan / maxCan);
    }

    private void OlumFonksiyonu()
    {
        if (!hayattaMi) return; // Sadece bir kere öl

        hayattaMi = false; // Artık ölü
        Debug.Log("Karakter ÖLDÜ!");

        // Varsa çalışan hasar animasyonunu iptal et
        if (hasarCoroutine != null) StopCoroutine(hasarCoroutine);

        OnOlum.Invoke();

        // --- YENİ EKLENDİ (PANEL VE MOUSE) ---
        // 1. Ölüm Panelini aç
        if (olumPaneli != null)
        {
            olumPaneli.SetActive(true);
        }

        // 2. Mouse'u görünür ve serbest yap
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // 3. Hareket ve saldırıyı durdur (Varsa)
        SimpleMove hareketScripti = GetComponent<SimpleMove>();
        if (hareketScripti != null) hareketScripti.enabled = false;

        PlayerAttack attackScripti = GetComponent<PlayerAttack>();
        if (attackScripti != null) attackScripti.enabled = false;
        // -------------------------------------
    }

    void Update()
    {
        // --- YENİ EKLENDİ (ÖLÜM KONTROLÜ) ---

        // Eğer hayattaysak normal test kodları çalışsın
        if (hayattaMi)
        {
            if (Keyboard.current != null && Keyboard.current.kKey.wasPressedThisFrame)
            {
                HasarAl(10f);
            }
        }
        // Eğer öldüysek TUŞ BEKLEME modu çalışsın
        else
        {
            if (Keyboard.current.anyKey.wasPressedThisFrame ||
               (Mouse.current != null && (Mouse.current.leftButton.wasPressedThisFrame || Mouse.current.rightButton.wasPressedThisFrame)))
            {
                Debug.Log("Ana Menüye dönülüyor...");
                SceneManager.LoadScene("MainMenu");
            }
        }
        // ------------------------------------
    }
}