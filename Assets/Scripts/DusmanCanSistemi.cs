using UnityEngine;
using UnityEngine.UI; // Slider icin SART!
using System.Collections; // Coroutine (zamanlayici) icin SART!

public class DusmanCanSistemi : MonoBehaviour
{
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

    private Coroutine hideTimer; // Zamanlayiciyi tutmak icin

    // Diğer DusmanAI script'ine komut vermek için onu burada tutacağız
    private DusmanAI dusmanAIScripti;

    void Start()
    {
        mevcutCan = maxCan;

        // Slider'i guncelle ama baslangicta gizli tut
        if (canBariSlider != null) canBariSlider.value = 1f;
        if (canBariParent != null) canBariParent.SetActive(false);

        // Başlangıçta, bu objenin üzerindeki DusmanAI script'ini bul ve sakla
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
        mevcutCan = Mathf.Clamp(mevcutCan, 0f, maxCan); // Can 0'ın altına düşmesin

        // UI'i guncelle
        if (canBariSlider != null)
        {
            canBariSlider.value = mevcutCan / maxCan;
        }

        if (mevcutCan <= 0)
        {
            OlumFonksiyonu();
        }
        else
        {
            // Hasar aldi ama olmedi: Can barini goster ve zamanlayiciyi baslat
            GosterCanBari();
        }
    }

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
        StopCoroutine("YanmaDongusu"); // YANMA EFEKTİNİ DE DURDUR (ÖNEMLİ)
        if (canBariParent != null) canBariParent.SetActive(false);

        // Diğer script'i bulup "öl" komutu ver
        if (dusmanAIScripti != null)
        {
            dusmanAIScripti.OlumAnimasyonunuBaslat();
        }

        // Dusman objesini 5 saniye sonra yok et (animasyonun bitmesi icin)
        Destroy(gameObject, 5f);
    }

    // --- ATEŞ TOPU İÇİN İKİNCİ KODDAN GELEN KISIM ---

    // Dışarıdan (Top'tan) çağrılan fonksiyon
    public void YanmaBaslat(float saniyeBasiHasar, int kacSaniyeSurucek)
    {
        if (mevcutCan <= 0) return; // Ölü düşman yanmaz

        // Üst üste yanmasın diye önce eskisi varsa durduruyoruz
        StopCoroutine("YanmaDongusu");
        // Yeni yanmayı başlatıyoruz
        StartCoroutine(YanmaDongusu(saniyeBasiHasar, kacSaniyeSurucek));
    }

    // Saniye saniye can azaltan zamanlayıcı
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
                // Düşman bu 1 saniye içinde öldüyse döngüden çık
                break;
            }
        }
    }
}