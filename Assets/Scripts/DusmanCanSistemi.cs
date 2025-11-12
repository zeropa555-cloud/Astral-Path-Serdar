using UnityEngine;
using UnityEngine.UI; // Slider icin SART!
using System.Collections; // Coroutine (zamanlayici) icin SART!

public class DusmanCanSistemi : MonoBehaviour
{
    [Header("Can Ayarlarý")]
    public float maxCan = 50f;
    private float mevcutCan;

    [Header("UI Referanslarý")]
    [Tooltip("DusmanCanBariCanvas objesini buraya surukle")]
    public GameObject canBariParent; // Butun Canvas objesi (gosterip gizlemek icin)

    [Tooltip("DustanSlider objesini buraya surukle")]
    public Slider canBariSlider;

    [Tooltip("Can bari hasar aldiktan kac saniye sonra gizlensin?")]
    public float gorunurlukSuresi = 4f;

    private Coroutine hideTimer; // Zamanlayiciyi tutmak icin

    void Start()
    {
        mevcutCan = maxCan;

        // Slider'i guncelle ama baslangicta gizli tut
        if (canBariSlider != null) canBariSlider.value = 1f;
        if (canBariParent != null) canBariParent.SetActive(false);
    }

    // DISARIDAN CAGRILACAK ANA FONKSIYON
    public void HasarAl(float miktar)
    {
        if (mevcutCan <= 0) return; // Zaten olu

        mevcutCan -= miktar;
        mevcutCan = Mathf.Clamp(mevcutCan, 0f, maxCan);

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

        // Can barini gorunur yap
        canBariParent.SetActive(true);

        // Eger daha onceden calisan bir gizleme zamanlayicisi varsa, onu durdur
        if (hideTimer != null)
        {
            StopCoroutine(hideTimer);
        }

        // Can barini X saniye sonra gizlemek icin yeni bir zamanlayici baslat
        hideTimer = StartCoroutine(CanBariniGizleTimer());
    }

    IEnumerator CanBariniGizleTimer()
    {
        // Belirlenen sure kadar bekle
        yield return new WaitForSeconds(gorunurlukSuresi);

        // Sure dolunca can barini gizle
        canBariParent.SetActive(false);
    }

    void OlumFonksiyonu()
    {
        Debug.Log(gameObject.name + " öldü.");

        // Olunce can barini hemen gizle ve zamanlayiciyi durdur
        if (hideTimer != null) StopCoroutine(hideTimer);
        if (canBariParent != null) canBariParent.SetActive(false);

        // Burada dusmanin olme animasyonunu vs. calistirabilirsin

        // Dusman objesini 2 saniye sonra yok et (animasyonun bitmesi icin)
        Destroy(gameObject, 2f);
    }

    // OnMouseDown() testi buradan silindi.
}