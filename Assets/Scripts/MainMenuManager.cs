using UnityEngine;
using UnityEngine.SceneManagement; // Sahne degistirmek icin bu satir SART!

public class MainMainMenuManager : MonoBehaviour
{
    // Inspector'dan AyarlarPanel'ini bu slota surukleyecegiz
    [Tooltip("Ayarlar panelinin GameObject'ini buraya surukleyin.")]
    public GameObject ayarlarPanel;

    // --- BASLA BUTONU ICIN ---
    public void OyunBaslat()
    {
        // ONEMLI: "GameScene" yazan yeri kendi oyun sahnenin ADIYLA degistir.
        // Mesela oyun sahnenin adi "Level_1" ise "Level_1" yaz.
        Debug.Log("Oyun Sahnesi Yukleniyor...");
        SceneManager.LoadScene("GameScene");
    }

    // --- AYARLAR BUTONU ICIN ---
    public void AyarlariAc()
    {
        // Ayarlar panelini gorunur yap
        if (ayarlarPanel != null)
        {
            ayarlarPanel.SetActive(true);
        }
    }

    // --- (Ayarlar Panelindeki) GERI BUTONU ICIN ---
    public void AyarlariKapat()
    {
        // Ayarlar panelini tekrar gorunmez yap
        if (ayarlarPanel != null)
        {
            ayarlarPanel.SetActive(false);
        }
    }

    // --- CIKIS BUTONU ICIN ---
    public void OyundanCik()
    {
        // Editor'de calisip calismadigini anlamak icin Debug.Log ekliyoruz
        Debug.Log("Oyundan Cikiliyor...");

        // Uygulamayi kapatir (Sadece build alinca calisir, Unity editor'unde calismaz)
        Application.Quit();
    }
}