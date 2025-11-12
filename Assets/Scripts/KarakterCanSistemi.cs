using UnityEngine;
using UnityEngine.Events; // UI ile konusmak icin bu SART!
using UnityEngine.InputSystem; // <-- HATA GÝDERÝCÝ SATIR BU!

public class KarakterCanSistemi : MonoBehaviour
{
    [Header("Can Ayarlarý")]
    public float maxCan = 100f; // Karakterin maksimum caný

    [Header("Mevcut Durum")]
    [SerializeField] // Inspector'da gorunsun ama diger scriptler degistiremesin
    private float mevcutCan;

    [Header("UI Sinyali")]
    // Bu sinyal, can degistiginde UI'daki slider'a "yeni degeri" gonderecek (0.0 ile 1.0 arasi)
    public UnityEvent<float> OnCanDegisti;

    // Opsiyonel: Karakter oldugunde calisir
    public UnityEvent OnOlum;

    void Start()
    {
        // Oyuna tam canla basla
        mevcutCan = maxCan;

        // UI'in da tam canla baslamasi icin ilk sinyali gonder
        OnCanDegisti.Invoke(mevcutCan / maxCan);
    }

    // DISARIDAN CAGRILACAK FONKSIYONLAR:

    /// <summary>
    /// Karaktere hasar verir.
    /// </summary>
    /// <param name="miktar">Hasar miktari</param>
    public void HasarAl(float miktar)
    {
        if (mevcutCan <= 0) return; // Zaten oluyse tekrar hasar alma

        mevcutCan -= miktar;
        mevcutCan = Mathf.Clamp(mevcutCan, 0f, maxCan); // Canin 0'in altina inmesini engelle

        // UI'a yeni can yuzdesini (%kac kaldi) gonder
        OnCanDegisti.Invoke(mevcutCan / maxCan);

        // Debug.Log("Hasar alindi! Mevcut Can: " + mevcutCan);

        if (mevcutCan <= 0)
        {
            OlumFonksiyonu();
        }
    }

    /// <summary>
    /// Karakteri iyilestirir.
    /// </summary>
    /// <param name="miktar">Iyilestirme miktari</param>
    public void Iyiles(float miktar)
    {
        if (mevcutCan <= 0 || mevcutCan == maxCan) return; // Oluyse veya cani fulse iyilesme

        mevcutCan += miktar;
        mevcutCan = Mathf.Clamp(mevcutCan, 0f, maxCan); // Canin maxCan'i gecmesini engelle

        // UI'a yeni can yuzdesini gonder
        OnCanDegisti.Invoke(mevcutCan / maxCan);

        // Debug.Log("Iyilesildi! Mevcut Can: " + mevcutCan);
    }

    private void OlumFonksiyonu()
    {
        // Burasi karakter olunce calisir
        Debug.Log("Karakter ÖLDÜ!");
        OnOlum.Invoke(); // Olum animasyonu vs. icin sinyal gonder

        // Istersen burada hareketi durdurabilirsin
        // GetComponent<SimpleMove>().enabled = false;
    }

    // --- TEST ICIN ---
    // Oyundayken 'K' tusuna basip calisiyor mu diye bakabilirsin
    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.kKey.wasPressedThisFrame)
        {
            HasarAl(10f); // Test icin 10 hasar ver
        }
    }
}