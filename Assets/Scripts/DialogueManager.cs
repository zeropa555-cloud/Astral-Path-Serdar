using UnityEngine;
using TMPro; // TextMeshPro icin SART!
using System.Collections.Generic; // Queue (Kuyruk) kullanmak icin SART!
using UnityEngine.InputSystem;
using UnityEngine.UI; // Tiklama algilamak icin

public class DialogueManager : MonoBehaviour
{
    // Singleton (Bu script'e her yerden kolayca ulasmak icin)
    public static DialogueManager instance;

    [Header("UI Referanslarý")]
    public GameObject diyalogPanel; // Hierarchy'den surukle
    public TextMeshProUGUI isimText; // Hierarchy'den surukle
    public TextMeshProUGUI metinAlaniText; // Hierarchy'den surukle
    public Image portreImage; // PORTRE ÝÇÝN EKLENEN YENÝ REFERANS

    // Konusma cumlelerini tutacagimiz kuyruk
    private Queue<DiyalogSatiri> cumleler;

    void Awake()
    {
        // Singleton kurulumu
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        cumleler = new Queue<DiyalogSatiri>();
        if (diyalogPanel != null) diyalogPanel.SetActive(false); // Baslangicta kapali
    }

    void Update()
    {
        // Eger diyalog paneli aciksa VE fareye tiklandiysa
        // (Not: Bu kod diyaloðu "ilerletmek" içindir, "E" tuþu sadece baslatir)
        if (diyalogPanel.activeSelf && Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            // UI'a tiklamadigimizdan emin ol (guvenlik amacli)
            if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
                return;

            SiradakiCumleyiGoster(); // Bir sonraki cumleye gec
        }
    }

    // DISARIDAN CAGRILACAK ANA FONKSIYON
    public void KonusmayiBaslat(Konusma konusma)
    {
        diyalogPanel.SetActive(true); // Paneli gorunur yap
        cumleler.Clear(); // Eski konusmayi temizle

        // Yeni konusmanin tum cumlelerini kuyruga ekle
        foreach (DiyalogSatiri satir in konusma.satirlar)
        {
            cumleler.Enqueue(satir);
        }

        SiradakiCumleyiGoster(); // Ilk cumleyi goster
    }

    public void SiradakiCumleyiGoster()
    {
        // Eger kuyrukta cumle kalmadiysa, konusmayi bitir
        if (cumleler.Count == 0)
        {
            KonusmayiBitir();
            return;
        }

        // Kuyruktaki ilk cumleyi cek
        DiyalogSatiri satir = cumleler.Dequeue();

        // UI elemanlarini guncelle
        isimText.text = satir.konusmaciAdi;
        metinAlaniText.text = satir.metin;

        // --- GÜNCELLENEN PORTRE KISMI ---
        if (satir.konusmaciPortresi != null)
        {
            // Eger bu satir icin bir portre resmi atanmissa
            portreImage.sprite = satir.konusmaciPortresi;
            portreImage.enabled = true; // Resim kutusunu gorunur yap
        }
        else
        {
            // Portre atanmamissa (mesela oyuncu konusuyorsa)
            portreImage.enabled = false; // Resim kutusunu gizle
        }
        // --- GÜNCELLEME BÝTTÝ ---
    }

    public void KonusmayiBitir()
    {
        diyalogPanel.SetActive(false); // Paneli gizle
        Debug.Log("Konuþma bitti.");
    }
}