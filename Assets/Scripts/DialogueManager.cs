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
        // (Burasý ayný kaldý)
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        // (Burasý ayný kaldý)
        cumleler = new Queue<DiyalogSatiri>();
        if (diyalogPanel != null) diyalogPanel.SetActive(false);
    }

    void Update()
    {
        // (Burasý ayný kaldý)
        if (diyalogPanel.activeSelf && Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
                return;

            SiradakiCumleyiGoster(); // Bir sonraki cumleye gec
        }
    }

    // DISARIDAN CAGRILACAK ANA FONKSIYON
    public void KonusmayiBaslat(Konusma konusma)
    {
        // (Burasý ayný kaldý)
        diyalogPanel.SetActive(true);
        cumleler.Clear();

        foreach (DiyalogSatiri satir in konusma.satirlar)
        {
            cumleler.Enqueue(satir);
        }

        SiradakiCumleyiGoster();
    }

    public void SiradakiCumleyiGoster()
    {
        // (Burasý ayný kaldý)
        if (cumleler.Count == 0)
        {
            KonusmayiBitir();
            return;
        }

        DiyalogSatiri satir = cumleler.Dequeue();
        isimText.text = satir.konusmaciAdi;
        metinAlaniText.text = satir.metin;

        if (satir.konusmaciPortresi != null)
        {
            portreImage.sprite = satir.konusmaciPortresi;
            portreImage.enabled = true;
        }
        else
        {
            portreImage.enabled = false;
        }
    }

    // --- BU FONKSÝYON GÜNCELLENDÝ ---
    public void KonusmayiBitir()
    {
        diyalogPanel.SetActive(false); // Paneli gizle
        Debug.Log("Konuþma bitti.");

        // --- YENÝ EKLENDÝ (KAPIYI AÇMAK ÝÇÝN) ---
        // Konuþan NPC'ye "Ben bittim, kapýný açabilirsin" sinyalini yolla
        NPCDialogue.KonusmaBittiSinyali();
        // ---------------------------------------
    }
    // --- GÜNCELLEME BÝTTÝ ---
}