using UnityEngine;
using UnityEngine.UI; // Slider icin SART!
using System.Collections; // Coroutine (zamanlayici) icin SART!

public class DusmanCanSistemi : MonoBehaviour
{
    // --- YENİ EKLENDİ (ROUND SİSTEMİ İÇİN) ---
    // Bu sinyal, bir düşman öldüğünde EnemySpawner'a haber verecek
    public static event System.Action OnDusmanOldu;
    // ------------------------------------------

    [Header("Can Ayarları")]
    public float maxCan = 50f;
    private float mevcutCan;

    [Header("UI Referansları")]
    [Tooltip("DusmanCanBariCanvas objesini buraya surukle")]
    public GameObject canBariParent; // Butun Canvas objesi (gosterip gizlemek icin)

    [Tooltip("DustanSlider objesini buraya surukle")]
    public Slider canBariSlider;

    [Tooltip("Can bari hasar aldiktan kac saniye sonra gizlensin?")]
    public float gorunurlukSuresi = 4f;

    // ----- 1. YENİ EKLENEN AYARLAR -----
    [Header("Animasyon Ayarları")]
    [Tooltip("Hasar aldıktan KAÇ SANİYE SONRA 'Hit' animasyonu başlasın?")]
    public float hasarAnimasyonGecikmesi = 0.1f; // Saniye cinsinden gecikme
    private Coroutine hasarCoroutine; // Hasar animasyonu zamanlayıcısı
    // ------------------------------------

    private Coroutine hideTimer; // Zamanlayiciyi tutmak icin
    private DusmanAI dusmanAIScripti;

    void Start()
    {
        mevcutCan = maxCan;
        if (canBariSlider != null) canBariSlider.value = 1f;
        if (canBariParent != null) canBariParent.SetActive(false);
        
        dusmanAIScripti = GetComponent<DusmanAI>();
        if (dusmanAIScripti == null)
        {
            Debug.LogError(gameObject.name + " üzerinde DusmanAI script'i bulunamadı! Ölüm animasyonu çalışmayacak.");
        }
    }

    // DISARIDAN CAGRILACAK ANA FONKSIYON
    public void HasarAl(float miktar)
    {
        if (mevcutCan <= 0) return; // Zaten olu

        mevcutCan -= miktar;
        mevcutCan = Mathf.Clamp(mevcutCan, 0f, maxCan);

        if (canBariSlider != null)
        {
            canBariSlider.value = mevcutCan / maxCan;
        }

        // --- 2. GÜNCELLENEN MANTIK ---
        if (mevcutCan <= 0)
        {
            OlumFonksiyonu();
        }
        else
        {
            // Can BİTMEDİYSE:
            // 1. Can barını göster (Mevcut kodunuz)
            GosterCanBari();

            // 2. Gecikmeli Hasar Animasyonunu başlat (Yeni eklenen)
            if (hasarCoroutine != null) StopCoroutine(hasarCoroutine);
            hasarCoroutine = StartCoroutine(GecikmeliHasarAnimasyonu());
        }
        // -----------------------------
    }

    // ----- 3. YENİ EKLENEN ZAMANLAYICI FONKSİYONU -----
    IEnumerator GecikmeliHasarAnimasyonu()
    {
        // 1. Belirlenen süre kadar bekle
        yield return new WaitForSeconds(hasarAnimasyonGecikmesi);

        // 2. Süre doldu, AI script'ine "animasyonu oynat" de
        if (dusmanAIScripti != null)
        {
            dusmanAIScripti.HasarAnimasyonunuBaslat();
        }

        hasarCoroutine = null;
    }
    // ------------------------------------------------

    void GosterCanBari()
    {
        if (canBariParent == null) return;
        canBariParent.SetActive(true);

        if (hideTimer != null)
        {
            StopCoroutine(hideTimer);
        }
        hideTimer = StartCoroutine(CanBariniGizleTimer());
    }

    IEnumerator CanBariniGizleTimer()
    {
        yield return new WaitForSeconds(gorunurlukSuresi);
        if (canBariParent != null) canBariParent.SetActive(false);
    }

    void OlumFonksiyonu()
    {
        Debug.Log(gameObject.name + " öldü.");

        // Olunce can barini hemen gizle ve zamanlayicilari durdur
        if (hideTimer != null) StopCoroutine(hideTimer);
        
        // --- 4. GÜNCELLENDİ (Yeni Coroutine'i de durdur) ---
        if (hasarCoroutine != null) StopCoroutine(hasarCoroutine);
        // --------------------------------------------------

        StopCoroutine("YanmaDongusu"); // YANMA EFEKTİNİ DE DURDUR (ÖNEMLİ)
        if (canBariParent != null) canBariParent.SetActive(false);

        // Diğer script'i bulup "öl" komutu ver
        if (dusmanAIScripti != null)
        {
            dusmanAIScripti.OlumAnimasyonunuBaslat();
        }

        // "Ben öldüm!" diye spawner'a haber yolla
        OnDusmanOldu?.Invoke();
        
        // Dusman objesini 5 saniye sonra yok et (animasyonun bitmesi icin)
        Destroy(gameObject, 5f);
    }

    // --- ATEŞ TOPU İÇİN OLAN KISIM (DEĞİŞMEDİ) ---

    public void YanmaBaslat(float saniyeBasiHasar, int kacSaniyeSurucek)
    {
        if (mevcutCan <= 0) return; // Ölü düşman yanmaz
        StopCoroutine("YanmaDongusu");
        StartCoroutine(YanmaDongusu(saniyeBasiHasar, kacSaniyeSurucek));
    }

    IEnumerator YanmaDongusu(float hasar, int sure)
    {
        for (int i = 0; i < sure; i++)
        {
            yield return new WaitForSeconds(1f); // 1 saniye bekle

            if (mevcutCan > 0)
            {
                HasarAl(hasar); // Can azalt
                Debug.Log(gameObject.name + " yanıyor! Canı: " + mevcutCan);
            }
            else
            {
                break;
            }
        }
    }
}