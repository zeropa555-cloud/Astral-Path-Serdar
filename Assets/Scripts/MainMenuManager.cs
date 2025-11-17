using UnityEngine;
using UnityEngine.SceneManagement;
// Artik Slider, Audio veya PlayerPrefs kodlarina ihtiyaci kalmadi
using System.Collections;

public class MainMenuManager : MonoBehaviour
{
    // --- SES AYARLARI BURADAN CIKARILDI ---

    [Header("Ana Paneller")]
    public GameObject ayarlarPanel;

    [Header("Ayar Paneli Ogeleri")]
    public GameObject sesButon;
    public GameObject dilButon;
    public GameObject anaGeriButon; // Ayarlar panelinden Ana Menu'ye donen buton

    [Header("Ayar Alt Panelleri")]
    public GameObject sesAyarlariPanel;
    public GameObject dilSecimiPanel;

    // --- Start() icindeki ses kodlari cikarildi ---

    // --- (Arka Plan Fade kodlarin varsa burada olmali) ---
    // ...

    // --- Ana Menü Fonksiyonlarý ---

    public void OyunBaslat()
    {
        Debug.Log("Oyun Sahnesi Yukleniyor...");
        SceneManager.LoadScene("IntroScene");
    }

    public void OyundanCik()
    {
        Debug.Log("Oyundan Cikiliyor...");
        Application.Quit();
    }

    // --- Ayarlar Menüsü Panel Fonksiyonlarý ---

    public void AyarlariAc()
    {
        if (ayarlarPanel != null)
        {
            ayarlarPanel.SetActive(true);
            if (sesButon != null) sesButon.SetActive(true);
            if (dilButon != null) dilButon.SetActive(true);
            if (anaGeriButon != null) anaGeriButon.SetActive(true);
            if (sesAyarlariPanel != null) sesAyarlariPanel.SetActive(false);
            if (dilSecimiPanel != null) dilSecimiPanel.SetActive(false);
        }
    }

    public void AyarlariKapat()
    {
        if (ayarlarPanel != null)
        {
            ayarlarPanel.SetActive(false);
        }
    }

    public void SesPaneliniAc()
    {
        if (sesButon != null) sesButon.SetActive(false);
        if (dilButon != null) dilButon.SetActive(false);
        if (anaGeriButon != null) anaGeriButon.SetActive(false);
        if (sesAyarlariPanel != null) sesAyarlariPanel.SetActive(true);
    }

    public void DilPaneliniAc()
    {
        if (sesButon != null) sesButon.SetActive(false);
        if (dilButon != null) dilButon.SetActive(false);
        if (anaGeriButon != null) anaGeriButon.SetActive(false);
        if (dilSecimiPanel != null) dilSecimiPanel.SetActive(true);
    }

    public void AyarAltPanelindenGeriDon()
    {
        if (sesAyarlariPanel != null) sesAyarlariPanel.SetActive(false);
        if (dilSecimiPanel != null) dilSecimiPanel.SetActive(false);
        if (sesButon != null) sesButon.SetActive(true);
        if (dilButon != null) dilButon.SetActive(true);
        if (anaGeriButon != null) anaGeriButon.SetActive(true);
    }

    // --- TUM SES FONKSIYONLARI BURADAN CIKARILDI ---
}