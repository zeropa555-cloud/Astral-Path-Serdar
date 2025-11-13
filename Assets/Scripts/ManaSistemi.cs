using UnityEngine;
using UnityEngine.UI; // Slider kullanmak için gerekli

public class ManaSistemi : MonoBehaviour
{
    [Header("Mana Ayarlarý")]
    public float maxMana = 100f;
    public float dolumHizi = 15f;    // Saniyede kaç mana dolsun
    public float beklemeSuresi = 5f; // Harcadýktan sonra kaç sn beklesin

    [Header("UI Baðlantýsý")]
    public Slider manaSlider; // Unity'deki Slider'ý buraya sürükle

    private float mevcutMana;
    private float sonHarcamaZamani; // En son ne zaman büyü attýk?

    void Start()
    {
        mevcutMana = maxMana;

        // Slider ayarlarýný yap
        if (manaSlider != null)
        {
            manaSlider.maxValue = maxMana;
            manaSlider.value = mevcutMana;
        }
    }

    void Update()
    {
        // Eðer son harcamadan 'beklemeSuresi' (5 sn) kadar zaman geçtiyse
        // VE mana eksikse -> Doldurmaya baþla
        if (Time.time - sonHarcamaZamani > beklemeSuresi && mevcutMana < maxMana)
        {
            mevcutMana += dolumHizi * Time.deltaTime;

            // Manayý maxMana'da sabitle (taþmasýn)
            if (mevcutMana > maxMana) mevcutMana = maxMana;

            UIGuncelle();
        }
    }

    // Baþka scriptler (PlayerAttack) manayý buradan isteyecek
    public bool ManaHarca(float miktar)
    {
        if (mevcutMana >= miktar)
        {
            mevcutMana -= miktar;
            sonHarcamaZamani = Time.time; // Zamanlayýcýyý sýfýrla (tekrar 5 sn bekleyecek)
            UIGuncelle();
            return true; // Ýþlem baþarýlý, ateþ edebilirsin
        }
        else
        {
            Debug.Log("Yetersiz Mana!");
            return false; // Mana yok, ateþ edemezsin
        }
    }

    void UIGuncelle()
    {
        if (manaSlider != null)
        {
            manaSlider.value = mevcutMana;
        }
    }
}