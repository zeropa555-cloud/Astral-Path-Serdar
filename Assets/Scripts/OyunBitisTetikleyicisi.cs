using UnityEngine;
using UnityEngine.SceneManagement; // Sahne deðiþimi için ÞART
using System.Collections;

[RequireComponent(typeof(BoxCollider))]
public class OyunBitisTetikleyicisi : MonoBehaviour
{
    [Header("Ayarlar")]
    [Tooltip("Gidilecek sahnenin adý (Credits sahnesi)")]
    public string bitisSahneAdi = "Credits";

    [Tooltip("Oyuncu alana girdikten kaç saniye sonra sahne deðiþsin?")]
    public float beklemeSuresi = 1.0f; // 1 saniye bekleme iyidir

    private bool bittiMi = false;

    void Start()
    {
        // Tetikleyici olduðundan emin olalým
        GetComponent<BoxCollider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Çarpan þey Oyuncu mu ve oyun daha bitmedi mi?
        if (other.CompareTag("Player") && !bittiMi)
        {
            bittiMi = true;
            Debug.Log("OYUN BÝTTÝ! Credits sahnesine geçiliyor...");

            // Bitiþ sürecini baþlat
            StartCoroutine(SahneDegistir());
        }
    }

    IEnumerator SahneDegistir()
    {
        // Biraz bekle (Oyuncu kapýdan içeri adýmýný atsýn)
        yield return new WaitForSeconds(beklemeSuresi);

        // 1. Mouse'u serbest býrak (Çok Önemli! Yoksa menüde mouse görünmez)
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // 2. Credits sahnesini yükle
        SceneManager.LoadScene(bitisSahneAdi);
    }
}