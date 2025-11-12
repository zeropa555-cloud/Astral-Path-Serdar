using UnityEngine;
using System.Collections.Generic; // Sözlük (Dictionary) kullanmak için bu SART!

public class LocalizationManager : MonoBehaviour
{
    // Bu script'i Singleton yapiyoruz (her yerden kolayca erisebilmek icin)
    public static LocalizationManager instance;

    // Tum dillerin tum metinlerini burada saklayacagiz
    //                          <Anahtar,    <Dil Kodu, Metin>>
    private Dictionary<string, Dictionary<string, string>> localizationData;

    private string currentLanguage = "TR"; // Varsayilan dil
    private const string LANGUAGE_KEY = "CurrentLanguage"; // PlayerPrefs icin anahtar

    void Awake()
    {
        // Singleton yapisi
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Bu manager'in diger sahnelere de gecmesini sagla
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Dil veritabanini olustur
        InitializeDictionary();

        // Kayitli dili yukle
        currentLanguage = PlayerPrefs.GetString(LANGUAGE_KEY, "TR"); // Kayit yoksa "TR" basla
    }

    void Start()
    {
        // Oyunu baslatir baslatmaz tum metinleri guncelle
        UpdateAllTexts();
    }

    void InitializeDictionary()
    {
        localizationData = new Dictionary<string, Dictionary<string, string>>();

        // Buraya oyunundaki tum metinleri ekleyeceksin
        // ORNEK:
        AddText("key_basla_buton", "BAÞLA", "START");
        AddText("key_ayarlar_buton", "AYARLAR", "SETTINGS");
        AddText("key_cikis_buton", "ÇIKIÞ", "EXIT");
        AddText("key_geri_buton", "GERÝ", "BACK");
        AddText("key_ses_ayarlari", "SES AYARLARI", "SOUND SETTINGS");
        AddText("key_dil_secimi", "DÝL SEÇÝMÝ", "LANGUAGE");
        AddText("key_ana_ses", "ANA SES", "MASTER");
        AddText("key_muzik", "MÜZÝK", "MUSIC");
        AddText("key_efektler", "SES EFEKTLERÝ", "SFX");
        AddText("key_ayarlar_isim", "Ayarlar", "Settings");

        // Buraya istedigin kadar ekleyebilirsin
        // AddText("key_oyun_adi", "ASTRAL YOL", "ASTRAL PATH");
    }

    // Sozluge ekleme (yardimci fonksiyon)
    void AddText(string key, string tr, string en)
    {
        localizationData[key] = new Dictionary<string, string>
        {
            { "TR", tr },
            { "EN", en }
        };
    }

    // Dili degistiren ana fonksiyon (Butonlar bunu cagiracak)
    public void ChangeLanguage(string langCode) // "TR" veya "EN"
    {
        currentLanguage = langCode;
        PlayerPrefs.SetString(LANGUAGE_KEY, currentLanguage); // Ayari kaydet

        // Tum metinleri guncelle
        UpdateAllTexts();
    }

    // O anki dildeki metni donduren fonksiyon (LocalizedText script'i bunu kullanacak)
    public string GetText(string key)
    {
        if (localizationData.ContainsKey(key) && localizationData[key].ContainsKey(currentLanguage))
        {
            return localizationData[key][currentLanguage];
        }

        Debug.LogWarning("Dil anahtari bulunamadi: " + key + " (Dil: " + currentLanguage + ")");
        return "TEXT_BULUNAMADI"; // Hata olursa bunu goster
    }

    // O an ekranda gorunen tum metinleri guncelle
    public void UpdateAllTexts()
    {
        // Ekranda "LocalizedText" script'ine sahip tum objeleri bul
        LocalizedText[] allTextObjects = FindObjectsOfType<LocalizedText>();
        foreach (LocalizedText textObj in allTextObjects)
        {
            textObj.UpdateText(); // Hepsine "kendini guncelle" de
        }
    }
}