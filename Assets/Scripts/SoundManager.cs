using UnityEngine;
using UnityEngine.Audio; // Audio Mixer'ý kontrol etmek için bu kütüphane ÞART!
using UnityEngine.UI;    // Slider'larý kontrol etmek için bu kütüphane ÞART!

// Koddaki class adinin (SesYoneticisi) dosya adiyla (SesYoneticisi.cs) ayni olmasi SART!
public class SesYoneticisi : MonoBehaviour
{
    [Header("Ses Kontrol Merkezi")]
    public AudioMixer anaMixer; // Project panelindeki AnaMixer'ý buraya sürükleyeceðiz.

    [Header("Arayüz Elemanlarý")]
    public Slider masterSlider;
    public Slider muzikSlider;
    public Slider sfxSlider;
    // public Button geriButonu; // Buna ihtiyacýmýz kalmadý
    // using UnityEngine.SceneManagement; // Buna ihtiyacýmýz kalmadý

    // PlayerPrefs'te ayarlarý kaydetmek için kullanacaðýmýz anahtarlar
    private const string MASTER_KEY = "MasterVolume";
    private const string MUZIK_KEY = "MuzikVolume";
    private const string SFX_KEY = "SFXVolume";

    void Start()
    {
        // Kayýtlý ayarlarý yükle ve slider'lara ata
        // Kayýt yoksa %75 (0.75) baþla
        masterSlider.value = PlayerPrefs.GetFloat(MASTER_KEY, 0.75f);
        muzikSlider.value = PlayerPrefs.GetFloat(MUZIK_KEY, 0.75f);
        sfxSlider.value = PlayerPrefs.GetFloat(SFX_KEY, 0.75f);

        // Slider'lar her hareket ettirildiðinde ilgili fonksiyonu çaðýrmalarý için "dinleyici" ekle
        // BU KISIM SAYESINDE INSPECTOR'DAN SURUKLEMEYE GEREK KALMAZ!
        masterSlider.onValueChanged.AddListener(SetMasterVolume);
        muzikSlider.onValueChanged.AddListener(SetMuzikVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);

        // Geri butonu listener'ý çýkarýldý, onu MainMenuManager yönetecek

        // Slider'larýn mevcut deðerlerini Audio Mixer'a uygula (Oyun baþlarken sesin doðru ayarlanmasý için)
        SetMasterVolume(masterSlider.value);
        SetMuzikVolume(muzikSlider.value);
        SetSFXVolume(sfxSlider.value);
    }

    // --- SES AYARLAMA FONKSÝYONLARI ---

    public void SetMasterVolume(float sliderValue)
    {
        float volumeInDb = Mathf.Log10(sliderValue) * 20;
        if (sliderValue == 0)
        {
            volumeInDb = -80f;
        }

        // DIKKAT: Mixer'daki parametre adinin "MasterVolume" oldugundan emin ol
        anaMixer.SetFloat("MasterVolume", volumeInDb);
        PlayerPrefs.SetFloat(MASTER_KEY, sliderValue); // Ayarý kaydet
    }

    public void SetMuzikVolume(float sliderValue)
    {
        float volumeInDb = Mathf.Log10(sliderValue) * 20;
        if (sliderValue == 0) volumeInDb = -80f;

        // DIKKAT: Mixer'daki parametre adinin "MusicVolume" oldugundan emin ol
        anaMixer.SetFloat("MusicVolume", volumeInDb);
        PlayerPrefs.SetFloat(MUZIK_KEY, sliderValue); // Ayarý kaydet
    }

    public void SetSFXVolume(float sliderValue)
    {
        float volumeInDb = Mathf.Log10(sliderValue) * 20;
        if (sliderValue == 0) volumeInDb = -80f;

        // DIKKAT: Mixer'daki parametre adinin "SFXVolume" oldugundan emin ol
        anaMixer.SetFloat("SFXVolume", volumeInDb);
        PlayerPrefs.SetFloat(SFX_KEY, sliderValue); // Ayarý kaydet
    }

    // --- GERÝ DÖNME FONKSÝYONU ---
    // Bu fonksiyona gerek kalmadý, çünkü panel sisteminde 
    // "Geri" butonunu MainMenuManager yönetiyor.
    /*
    public void GeriDon()
    {
        PlayerPrefs.Save();
        SceneManager.LoadScene("MainMenu");
    }
    */
}