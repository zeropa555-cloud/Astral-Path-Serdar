using UnityEngine;
using UnityEngine.Events; // UI ile konusmak icin bu SART!

public class KarakterCanSistemi : MonoBehaviour
{
    [Header("Can Ayarlarý")]
    public float maxCan = 100f; // Karakterin maksimum caný

    [Header("Mevcut Durum")]
    [SerializeField] // Inspector'da gorunsun ama diger scriptler degistiremesin
    private float mevcutCan;

    [Header("UI Sinyali")]
    // Bu sinyal, can degistiginde UI'daki slider'a "yeni degeri" gonderecek
    public UnityEvent<float> OnCanDegisti;

    // Opsiyonel: Karakter oldugunde calisir
    public UnityEvent OnOlum;

    void Start()
    {
        mevcutCan = maxCan;
        OnCanDegisti.Invoke(mevcutCan / maxCan);
    }

    // DISARIDAN CAGRILACAK ANA FONKSIYON:
    // Düþmanýn hasar vermesi için DüþmanAI scripti bu fonksiyonu çaðýracak!
    public void HasarAl(float miktar)
    {
        if (mevcutCan <= 0) return; // Zaten olu

        mevcutCan -= miktar;
        mevcutCan = Mathf.Clamp(mevcutCan, 0f, maxCan); // Can 0'ýn altýna inmesin

        // UI'a yeni can yuzdesini (%kac kaldi) gonder
        OnCanDegisti.Invoke(mevcutCan / maxCan);

        if (mevcutCan <= 0)
        {
            OlumFonksiyonu();
        }
    }

    public void Iyiles(float miktar)
    {
        if (mevcutCan <= 0 || mevcutCan == maxCan) return;

        mevcutCan += miktar;
        mevcutCan = Mathf.Clamp(mevcutCan, 0f, maxCan);

        OnCanDegisti.Invoke(mevcutCan / maxCan);
    }

    private void OlumFonksiyonu()
    {
        Debug.Log("Karakter ÖLDÜ!");
        OnOlum.Invoke(); // Olum animasyonu vs. icin sinyal gonder

        // Hareketi durdur
        SimpleMove hareketScripti = GetComponent<SimpleMove>();
        if (hareketScripti != null)
        {
            hareketScripti.enabled = false;
        }

        // Saldýrýyý durdur
        PlayerAttack attackScripti = GetComponent<PlayerAttack>();
        if (attackScripti != null)
        {
            attackScripti.enabled = false;
        }
    }

    // --- 'K' TUÞU TEST KISMI KALDIRILDI ---
    // void Update()
    // {
    // }
}