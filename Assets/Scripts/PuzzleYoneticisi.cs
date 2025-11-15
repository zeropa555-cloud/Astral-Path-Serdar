using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic; // Listeler için ÞART
using System.Linq; // Listeyi karýþtýrmak için ÞART

public class PuzzleYoneticisi : MonoBehaviour
{
    [Header("Baðlantýlar")]
    public SandikEtkilesim sandikScripti; // Sandýk scripti (Ayný kaldý)

    [Header("Dinamik Puzzle Ayarlarý")]
    [Tooltip("Az önce oluþturduðun 'KartPrefab'ýný (Button) buraya sürükle")]
    public GameObject kartPrefab; // Artýk butonlarý buradan yaratacaðýz

    [Tooltip("Kartlarýn oluþturulacaðý GridLayoutGroup'lu 'KartAlani' objesi")]
    public Transform kartAlani;

    [Tooltip("Kartlarýn 'kapalý'yken görünecek resmi (Sprite)")]
    public Sprite arkaResim;

    [Tooltip("Tüm olasý resimler (Örn: 21 tane). Kod bunlardan rastgele seçecek.")]
    public List<Sprite> TUM_PuzzleResimleri;

    [Header("Rastgelelik Ayarlarý")]
    [Tooltip("Bir oyunda en az kaç çift olsun? (Örn: 4 çift = 8 kart)")]
    public int minCiftSayisi = 4;
    [Tooltip("Bir oyunda en fazla kaç çift olsun? (Örn: 10 çift = 20 kart)")]
    public int maxCiftSayisi = 10;

    // --- Oyun Mekaniði Deðiþkenleri ---
    private List<Button> olusturulanKartlar = new List<Button>(); // Yaratýlan butonlarý burada tutacaðýz
    private List<Sprite> buRounddakiResimler = new List<Sprite>(); // Oynanacak resimler
    private List<int> karisikKartIDleri = new List<int>(); // Karýþtýrýlmýþ resim ID'leri

    private int ilkSecimIndex = -1;
    private int ikinciSecimIndex = -1;
    private bool secimYapilabilir = true;
    private int bulunanCiftSayisi = 0;
    private int toplamCiftSayisi;
    private System.Random rng = new System.Random(); // Karýþtýrma için

    void OnEnable()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // Gerekli þeyler ayarlanmýþ mý?
        if (TUM_PuzzleResimleri.Count < maxCiftSayisi)
        {
            Debug.LogError("HATA: 'TUM_PuzzleResimleri' listesinde yeterince resim yok! (En az 'maxCiftSayisi' kadar olmalý)");
            maxCiftSayisi = TUM_PuzzleResimleri.Count; // Hata vermemesi için sýnýrý düþür
        }

        OyunuKur();
    }

    void OyunuKur()
    {
        // 1. Önceki oyundan kalan kartlarý (varsa) temizle
        foreach (Transform child in kartAlani)
        {
            Destroy(child.gameObject);
        }
        olusturulanKartlar.Clear();
        buRounddakiResimler.Clear();
        karisikKartIDleri.Clear();

        // 2. Rastgele çift sayýsýný belirle
        toplamCiftSayisi = Random.Range(minCiftSayisi, maxCiftSayisi + 1);
        Debug.Log("Yeni puzzle baþlýyor. Çift sayýsý: " + toplamCiftSayisi);

        // 3. Oynanacak resimleri seç
        // Tüm resimleri karýþtýr ve listeden 'toplamCiftSayisi' kadarýný al
        List<Sprite> karisikTumResimler = TUM_PuzzleResimleri.OrderBy(a => rng.Next()).ToList();
        for (int i = 0; i < toplamCiftSayisi; i++)
        {
            buRounddakiResimler.Add(karisikTumResimler[i]);
        }

        // 4. Kart ID listesini oluþtur (Örn: 0, 0, 1, 1, 2, 2...)
        for (int id = 0; id < toplamCiftSayisi; id++)
        {
            karisikKartIDleri.Add(id);
            karisikKartIDleri.Add(id);
        }

        // 5. ID listesini karýþtýr
        karisikKartIDleri = karisikKartIDleri.OrderBy(a => rng.Next()).ToList();

        // 6. Kartlarý YARAT (Instantiate)
        for (int i = 0; i < karisikKartIDleri.Count; i++)
        {
            // Prefab'dan yeni kartý yarat ve 'kartAlani'nýn içine koy
            GameObject yeniKartObj = Instantiate(kartPrefab, kartAlani);
            yeniKartObj.name = "Kart " + i;

            // Arka resmini ayarla
            Image kartResmi = yeniKartObj.GetComponent<Image>();
            kartResmi.sprite = arkaResim;

            // Butonunu al
            Button yeniButon = yeniKartObj.GetComponent<Button>();
            yeniButon.interactable = true;

            // (EN ÖNEMLÝ KISIM) Butona týklandýðýnda hangi fonksiyonu çaðýracaðýný ayarla
            int kartIndexi = i; // Bu, C# 'closure' hatasýný önlemek için gerekli
            yeniButon.onClick.AddListener(() => KartSec(kartIndexi));

            // Yaratýlan kartý listeye ekle
            olusturulanKartlar.Add(yeniButon);
        }

        // 7. Grid'i ayarla (isteðe baðlý, daha iyi görünür)
        GridLayoutGroup grid = kartAlani.GetComponent<GridLayoutGroup>();
        if (grid != null)
        {
            // 12 veya daha az kartsa 4 sütun, daha fazlaysa 5 sütun yap
            grid.constraintCount = (toplamCiftSayisi * 2 <= 12) ? 4 : 5;
        }

        // 8. Oyun durumunu sýfýrla
        bulunanCiftSayisi = 0;
        secimYapilabilir = true;
        ilkSecimIndex = -1;
        ikinciSecimIndex = -1;
    }

    // Bu fonksiyon artýk dinamik olarak yaratýlan 'olusturulanKartlar' listesini kullanýyor
    public void KartSec(int kartIndex)
    {
        if (!secimYapilabilir || kartIndex == ilkSecimIndex) return;

        // ID'yi al
        int kartID = karisikKartIDleri[kartIndex];
        // O ID'ye ait olan resmi, bu round için seçtiðimiz resimlerden bul
        Sprite kartResmi = buRounddakiResimler[kartID];

        // Kartý aç (resmi göster)
        olusturulanKartlar[kartIndex].image.sprite = kartResmi;
        olusturulanKartlar[kartIndex].interactable = false;

        if (ilkSecimIndex == -1)
        {
            ilkSecimIndex = kartIndex;
        }
        else
        {
            ikinciSecimIndex = kartIndex;
            secimYapilabilir = false;
            EslesmeKontrolEt();
        }
    }

    // Bu fonksiyon da 'buRounddakiResimler' listesini kullanýyor
    void EslesmeKontrolEt()
    {
        int ilkKartID = karisikKartIDleri[ilkSecimIndex];
        int ikinciKartID = karisikKartIDleri[ikinciSecimIndex];

        if (ilkKartID == ikinciKartID) // EÞLEÞTÝ!
        {
            bulunanCiftSayisi++;
            secimYapilabilir = true;
            ilkSecimIndex = -1;
            ikinciSecimIndex = -1;

            if (bulunanCiftSayisi == toplamCiftSayisi) // KAZANDIN!
            {
                PuzzleiCoz();
            }
        }
        else // EÞLEÞMEDÝ!
        {
            StartCoroutine(KartlariGeriKapat());
        }
    }

    // Bu fonksiyon da 'olusturulanKartlar' listesini kullanýyor
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

    // (PuzzleiCoz ve PaneliKapat fonksiyonlarýn ayný kaldý)
    public void PuzzleiCoz()
    {
        Debug.Log("Puzzle Çözüldü! Sandýk açýlýyor...");
        gameObject.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        if (sandikScripti != null)
        {
            sandikScripti.SandigiAc();
        }
    }

    public void PaneliKapat()
    {
        gameObject.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}