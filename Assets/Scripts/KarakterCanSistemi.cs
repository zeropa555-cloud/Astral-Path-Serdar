using UnityEngine;
using UnityEngine.Events; // UI ile konusmak icin bu SART!

public class KarakterCanSistemi : MonoBehaviour
{
    [Header("Can Ayarlar ")]
    public float maxCan = 100f; // Karakterin maksimum can 

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
    // D  man n hasar vermesi i in D  manAI scripti bu fonksiyonu  a  racak!
    public void HasarAl(float miktar)
    {
        if (mevcutCan <= 0) return; // Zaten olu

        mevcutCan -= miktar;
        mevcutCan = Mathf.Clamp(mevcutCan, 0f, maxCan); // Can 0' n alt na inmesin

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
        Debug.Log("Karakter  LD !");
        OnOlum.Invoke(); // Olum animasyonu vs. icin sinyal gonder

        // Hareketi durdur
        SimpleMove hareketScripti = GetComponent<SimpleMove>();
        if (hareketScripti != null)
        {
            hareketScripti.enabled = false;
        }

        // Sald r y  durdur
        PlayerAttack attackScripti = GetComponent<PlayerAttack>();
        if (attackScripti != null)
        {
            attackScripti.enabled = false;
        }
    }


}