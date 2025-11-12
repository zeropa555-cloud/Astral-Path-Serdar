using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenuManager : MonoBehaviour
{
    [Header("Ana Paneller")]
    [Tooltip("Ayarlar panelinin GameObject'ini buraya surukleyin.")]
    public GameObject ayarlarPanel;

    [Header("Ayar Paneli Ogeleri")]
    // ButonGrubu yerine butonlari tek tek tanimlayalim
    public GameObject sesButon;
    public GameObject dilButon;
    public GameObject anaGeriButon; // Ayarlar panelinden Ana Menu'ye donen buton

    [Header("Ayar Alt Panelleri")]
    public GameObject sesAyarlariPanel;
    public GameObject dilSecimiPanel;

    // --- Arka Plan Fade Kodlarin ---
    // (Arka plan fade ile ilgili kodlarin buradaysa,
    //  onlara dokunma, onlar calismaya devam etmeli)
    // ... [backgroundCanvasGroup, fadeSuresi, hoverSayaci vs.] ...
    // ... [OnButtonHoverEnter, OnButtonHoverExit, FadeBackground] ...


    // --- Ana Menü Fonksiyonlarý ---

    public void OyunBaslat()
    {
        Debug.Log("Oyun Sahnesi Yukleniyor...");
        SceneManager.LoadScene("GameScene");
    }

    public void OyundanCik()
    {
        Debug.Log("Oyundan Cikiliyor...");
        Application.Quit();
    }

    // --- Ayarlar Menüsü Fonksiyonlarý ---

    // ANA MENÜ'deki "AYARLAR" butonu bunu cagirir
    public void AyarlariAc()
    {
        if (ayarlarPanel != null)
        {
            ayarlarPanel.SetActive(true);

            // Ayarlar acildiginda her zaman ana butonlar gorunsun
            if (sesButon != null) sesButon.SetActive(true);
            if (dilButon != null) dilButon.SetActive(true);
            if (anaGeriButon != null) anaGeriButon.SetActive(true);

            // Diger paneller kapansin
            if (sesAyarlariPanel != null) sesAyarlariPanel.SetActive(false);
            if (dilSecimiPanel != null) dilSecimiPanel.SetActive(false);
        }
    }

    // AYARLAR PANELÝ'ndeki ana "GERI" butonu bunu cagirir
    public void AyarlariKapat()
    {
        if (ayarlarPanel != null)
        {
            ayarlarPanel.SetActive(false);
        }
    }

    // AYARLAR PANELÝ'ndeki "SES" butonu bunu cagirir
    public void SesPaneliniAc()
    {
        // Ana butonlari gizle
        if (sesButon != null) sesButon.SetActive(false);
        if (dilButon != null) dilButon.SetActive(false);
        if (anaGeriButon != null) anaGeriButon.SetActive(false);

        // Ses panelini ac
        if (sesAyarlariPanel != null) sesAyarlariPanel.SetActive(true);
    }

    // AYARLAR PANELÝ'ndeki "DIL" butonu bunu cagirir
    public void DilPaneliniAc()
    {
        // Ana butonlari gizle
        if (sesButon != null) sesButon.SetActive(false);
        if (dilButon != null) dilButon.SetActive(false);
        if (anaGeriButon != null) anaGeriButon.SetActive(false);

        // Dil panelini ac
        if (dilSecimiPanel != null) dilSecimiPanel.SetActive(true);
    }

    // SES veya DIL panelindeki "GERI" butonu bunu cagirir
    public void AyarAltPanelindenGeriDon()
    {
        // Alt panelleri kapat
        if (sesAyarlariPanel != null) sesAyarlariPanel.SetActive(false);
        if (dilSecimiPanel != null) dilSecimiPanel.SetActive(false);

        // Ana butonlari goster
        if (sesButon != null) sesButon.SetActive(true);
        if (dilButon != null) dilButon.SetActive(true);
        if (anaGeriButon != null) anaGeriButon.SetActive(true);
    }
}