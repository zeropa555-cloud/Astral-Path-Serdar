using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Video; // <-- VÝDEO ÝÇÝN BU ÞART!

[RequireComponent(typeof(AudioSource))]
public class PuzzleYoneticisi : MonoBehaviour
{
    [Header("Baðlantýlar")]
    public SandikEtkilesim sandikScripti;

    [Header("Dinamik Puzzle Ayarlarý")]
    public GameObject kartPrefab;
    public Transform kartAlani;
    public Sprite arkaResim;
    public List<Sprite> TUM_PuzzleResimleri;

    [Header("Rastgelelik Ayarlarý")]
    public int minCiftSayisi = 4;
    public int maxCiftSayisi = 10;

    [Header("Jumpscare (Video) Ayarlarý")]
    [Tooltip("Oyuncu en fazla kaç yanlýþ yapabilir?")]
    public int maxYanlisHakki = 4;

    [Tooltip("Jumpscare videosunun oynayacaðý RawImage objesi")]
    public GameObject jumpscareVideoEkrani;

    [Tooltip("Videoyu oynatacak olan VideoPlayer bileþeni")]
    public VideoPlayer videoOynatici;

    // (Ýsteðe baðlý: Videoda ses yoksa ekstra ses çaldýrmak istersen)
    public AudioClip ekstraKorkuSesi;

    private List<Button> olusturulanKartlar = new List<Button>();
    private List<Sprite> buRounddakiResimler = new List<Sprite>();
    private List<int> karisikKartIDleri = new List<int>();

    private int ilkSecimIndex = -1;
    private int ikinciSecimIndex = -1;
    private bool secimYapilabilir = true;
    private int bulunanCiftSayisi = 0;
    private int toplamCiftSayisi;
    private System.Random rng = new System.Random();
    private int suankiYanlisSayisi = 0;
    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void OnEnable()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // Jumpscare ekranýný gizle
        if (jumpscareVideoEkrani != null) jumpscareVideoEkrani.SetActive(false);
        suankiYanlisSayisi = 0;

        if (TUM_PuzzleResimleri.Count < maxCiftSayisi)
        {
            maxCiftSayisi = TUM_PuzzleResimleri.Count;
        }

        OyunuKur();
    }

    void OyunuKur()
    {
        foreach (Transform child in kartAlani) { Destroy(child.gameObject); }
        olusturulanKartlar.Clear();
        buRounddakiResimler.Clear();
        karisikKartIDleri.Clear();

        toplamCiftSayisi = Random.Range(minCiftSayisi, maxCiftSayisi + 1);

        List<Sprite> karisikTumResimler = TUM_PuzzleResimleri.OrderBy(a => rng.Next()).ToList();
        for (int i = 0; i < toplamCiftSayisi; i++) { buRounddakiResimler.Add(karisikTumResimler[i]); }
        for (int id = 0; id < toplamCiftSayisi; id++) { karisikKartIDleri.Add(id); karisikKartIDleri.Add(id); }
        karisikKartIDleri = karisikKartIDleri.OrderBy(a => rng.Next()).ToList();

        for (int i = 0; i < karisikKartIDleri.Count; i++)
        {
            GameObject yeniKartObj = Instantiate(kartPrefab, kartAlani);
            Image kartResmi = yeniKartObj.GetComponent<Image>();
            kartResmi.sprite = arkaResim;
            Button yeniButon = yeniKartObj.GetComponent<Button>();
            yeniButon.interactable = true;
            int kartIndexi = i;
            yeniButon.onClick.AddListener(() => KartSec(kartIndexi));
            olusturulanKartlar.Add(yeniButon);
        }

        GridLayoutGroup grid = kartAlani.GetComponent<GridLayoutGroup>();
        if (grid != null) { grid.constraintCount = (toplamCiftSayisi * 2 <= 12) ? 4 : 5; }

        bulunanCiftSayisi = 0;
        secimYapilabilir = true;
        ilkSecimIndex = -1;
        ikinciSecimIndex = -1;
    }

    public void KartSec(int kartIndex)
    {
        if (!secimYapilabilir || kartIndex == ilkSecimIndex) return;
        int kartID = karisikKartIDleri[kartIndex];
        Sprite kartResmi = buRounddakiResimler[kartID];
        olusturulanKartlar[kartIndex].image.sprite = kartResmi;
        olusturulanKartlar[kartIndex].interactable = false;

        if (ilkSecimIndex == -1) { ilkSecimIndex = kartIndex; }
        else { ikinciSecimIndex = kartIndex; secimYapilabilir = false; EslesmeKontrolEt(); }
    }

    void EslesmeKontrolEt()
    {
        int ilkKartID = karisikKartIDleri[ilkSecimIndex];
        int ikinciKartID = karisikKartIDleri[ikinciSecimIndex];

        if (ilkKartID == ikinciKartID)
        {
            bulunanCiftSayisi++;
            secimYapilabilir = true;
            ilkSecimIndex = -1;
            ikinciSecimIndex = -1;
            if (bulunanCiftSayisi == toplamCiftSayisi) { PuzzleiCoz(); }
        }
        else
        {
            suankiYanlisSayisi++;
            Debug.Log("Yanlýþ Eþleþme! Hata: " + suankiYanlisSayisi + "/" + maxYanlisHakki);

            if (suankiYanlisSayisi >= maxYanlisHakki)
            {
                StartCoroutine(VideoJumpscareBaslat());
            }
            else
            {
                StartCoroutine(KartlariGeriKapat());
            }
        }
    }

    // --- YENÝ VÝDEO JUMPSCARE FONKSÝYONU ---
    IEnumerator VideoJumpscareBaslat()
    {
        // 1. Video ekranýný aç
        if (jumpscareVideoEkrani != null)
        {
            jumpscareVideoEkrani.SetActive(true);
        }

        // 2. Varsa ekstra ses çal
        if (audioSource != null && ekstraKorkuSesi != null)
        {
            audioSource.PlayOneShot(ekstraKorkuSesi);
        }

        // 3. Videoyu oynat
        if (videoOynatici != null)
        {
            videoOynatici.Play();

            // 4. Videonun uzunluðu kadar bekle (veya 2 saniye)
            // (Video uzunluðunu otomatik alýr)
            yield return new WaitForSeconds((float)videoOynatici.length);
        }
        else
        {
            // Video yoksa 2 sn bekle
            yield return new WaitForSeconds(2f);
        }

        // 5. Paneli komple kapat (Puzzle baþarýsýz oldu)
        PaneliKapat();
    }
    // --------------------------------------

    IEnumerator KartlariGeriKapat()
    {
        yield return new WaitForSeconds(1f);
        olusturulanKartlar[ilkSecimIndex].image.sprite = arkaResim;
        olusturulanKartlar[ilkSecimIndex].interactable = true;
        olusturulanKartlar[ikinciSecimIndex].image.sprite = arkaResim;
        olusturulanKartlar[ikinciSecimIndex].interactable = true;
        secimYapilabilir = true;
        ilkSecimIndex = -1;
        ikinciSecimIndex = -1;
    }

    public void PuzzleiCoz()
    {
        Debug.Log("Puzzle Çözüldü! Sandýk açýlýyor...");
        gameObject.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        if (sandikScripti != null) sandikScripti.SandigiAc();
    }

    public void PaneliKapat()
    {
        gameObject.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}