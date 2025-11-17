using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DusmanCanSistemi : MonoBehaviour
{
    public static event System.Action OnDusmanOldu;

    [Header("Can Ayarları")]
    public float maxCan = 50f;
    private float mevcutCan;

    [Header("UI Referansları")]
    public GameObject canBariParent;
    public Slider canBariSlider;
    public float gorunurlukSuresi = 4f;

    [Header("Animasyon Ayarları")]
    public float hasarAnimasyonGecikmesi = 0.1f;

    private DusmanAI dusmanAI;
    private Coroutine yanmaCoroutine;
    private Coroutine uiCoroutine;

    void Start()
    {
        mevcutCan = maxCan;
        if (canBariSlider != null) canBariSlider.value = 1f;
        if (canBariParent != null) canBariParent.SetActive(false);

        dusmanAI = GetComponent<DusmanAI>();
    }

    // 'animasyonOynat' varsayılan true. Yanarken de true yollayacağız ki titresin.
    public void HasarAl(float miktar, bool animasyonOynat = true)
    {
        if (mevcutCan <= 0) return;

        mevcutCan -= miktar;
        if (canBariSlider != null) canBariSlider.value = mevcutCan / maxCan;

        if (mevcutCan <= 0)
        {
            OlumFonksiyonu();
        }
        else
        {
            GosterCanBari();

            // Animasyon isteniyorsa oynat
            if (animasyonOynat && dusmanAI != null)
            {
                StartCoroutine(GecikmeliAnimasyon());
            }
        }
    }

    IEnumerator GecikmeliAnimasyon()
    {
        yield return new WaitForSeconds(hasarAnimasyonGecikmesi);
        if (mevcutCan > 0 && dusmanAI != null)
        {
            dusmanAI.HasarAnimasyonunuBaslat();
        }
    }

    public void YanmaBaslat(float saniyeBasiHasar, int sure)
    {
        if (mevcutCan <= 0) return;
        if (yanmaCoroutine != null) StopCoroutine(yanmaCoroutine);
        yanmaCoroutine = StartCoroutine(YanmaDongusu(saniyeBasiHasar, sure));
    }

    IEnumerator YanmaDongusu(float hasar, int sure)
    {
        for (int i = 0; i < sure; i++)
        {
            yield return new WaitForSeconds(1f); // 1 saniye bekle

            if (mevcutCan > 0)
            {
                // BURASI ÖNEMLİ: 'true' yolluyoruz ki her saniye 'Hit' animasyonu girsin
                HasarAl(hasar, true);
                Debug.Log(gameObject.name + " yanıyor! Can: " + mevcutCan);
            }
            else break;
        }
    }

    void OlumFonksiyonu()
    {
        StopAllCoroutines();
        if (canBariParent != null) canBariParent.SetActive(false);

        if (dusmanAI != null) dusmanAI.OlumAnimasyonunuBaslat();

        OnDusmanOldu?.Invoke();
        Destroy(gameObject, 5f);
    }

    void GosterCanBari()
    {
        if (canBariParent == null) return;
        canBariParent.SetActive(true);
        if (uiCoroutine != null) StopCoroutine(uiCoroutine);
        uiCoroutine = StartCoroutine(GizleUI());
    }

    IEnumerator GizleUI()
    {
        yield return new WaitForSeconds(gorunurlukSuresi);
        if (canBariParent != null) canBariParent.SetActive(false);
    }
}