using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem; 
using System.Collections; // <-- 1. YENİ EKLENDİ (Zamanlayıcı/Coroutine için ŞART!)

public class KarakterCanSistemi : MonoBehaviour
{
    [Header("Can Ayarları")]
    public float maxCan = 100f;

    [Header("Mevcut Durum")]
    [SerializeField]
    private float mevcutCan;

    private Animator animator;

    [Header("UI Sinyali")]
    public UnityEvent<float> OnCanDegisti;
    public UnityEvent OnOlum;

    [Header("Animasyon Ayarları")]
    [Tooltip("Animator'deki Hasar Alma Trigger'ının tam adı")]
    public string hasarTriggerAdi = "Hit";

    // ----- 2. YENİ EKLENDİ: Gecikme Ayarı -----
    [Tooltip("Hasar aldıktan KAÇ SANİYE SONRA 'Hit' animasyonu başlasın?")]
    public float hasarAnimasyonGecikmesi = 0.2f; // Saniye cinsinden gecikme

    // ----- 3. YENİ EKLENDİ: Zamanlayıcı Referansı -----
    // Aynı anda birden fazla hasar animasyonu başlatmayı engeller
    private Coroutine hasarCoroutine;


    void Start()
    {
        mevcutCan = maxCan;
        animator = GetComponentInChildren<Animator>(); 
        
        if (animator == null)
        {
            Debug.LogWarning(gameObject.name + ": KarakterCanSistemi bir Animator bulamadı! Hasar animasyonu çalışmayacak.");
        }
        
        OnCanDegisti.Invoke(mevcutCan / maxCan);
    }

    public void HasarAl(float miktar)
    {
        if (mevcutCan <= 0) return;

        mevcutCan -= miktar;
        mevcutCan = Mathf.Clamp(mevcutCan, 0f, maxCan);

        OnCanDegisti.Invoke(mevcutCan / maxCan);
        
        // --- 4. GÜNCELLENEN MANTIK ---
        if (mevcutCan <= 0)
        {
            OlumFonksiyonu();
        }
        else
        {
            // Can BİTMEDİYSE: Animasyonu hemen tetikleme
            // animator.SetTrigger(hasarTriggerAdi); // <-- BU SATIR KALDIRILDI

            // YERİNE: Gecikmeli animasyon zamanlayıcısını başlat
            
            // Eğer zaten çalışan bir 'hasar alma' animasyon gecikmesi varsa, onu durdur
            if(hasarCoroutine != null)
            {
                StopCoroutine(hasarCoroutine);
            }
            // Yenisini başlat
            hasarCoroutine = StartCoroutine(GecikmeliHasarAnimasyonu());
        }
    }

    // ----- 5. YENİ EKLENEN ZAMANLAYICI FONKSİYONU -----
    IEnumerator GecikmeliHasarAnimasyonu()
    {
        // 1. Belirlenen süre kadar bekle
        yield return new WaitForSeconds(hasarAnimasyonGecikmesi);

        // 2. Süre doldu, şimdi animasyonu tetikle
        if (animator != null && !string.IsNullOrEmpty(hasarTriggerAdi))
        {
            Debug.Log("Gecikme bitti. Animasyon tetikleniyor: " + hasarTriggerAdi); 
            animator.SetTrigger(hasarTriggerAdi); 
        }

        // 3. Zamanlayıcı işini bitirdi, referansı temizle
        hasarCoroutine = null;
    }


    public void Iyiles(float miktar)
    {
        // ... (Bu fonksiyon aynı kaldı) ...
        if (mevcutCan <= 0 || mevcutCan == maxCan) return;
        mevcutCan += miktar;
        mevcutCan = Mathf.Clamp(mevcutCan, 0f, maxCan);
        OnCanDegisti.Invoke(mevcutCan / maxCan);
    }

    private void OlumFonksiyonu()
    {
        // ... (Bu fonksiyon aynı kaldı) ...
        Debug.Log("Karakter ÖLDÜ!");
        OnOlum.Invoke(); 
    }

    void Update()
    {
        // ... (Test fonksiyonunuz aynı kaldı) ...
        if (Keyboard.current != null && Keyboard.current.kKey.wasPressedThisFrame)
        {
            HasarAl(10f);
        }
    }
}