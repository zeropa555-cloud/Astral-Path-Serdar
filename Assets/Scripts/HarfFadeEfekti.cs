using UnityEngine;
using TMPro; // TextMeshPro'yu kullanmak için bu satýr þart
using System.Collections;

public class HarfFadeEfekti : MonoBehaviour
{
    // Her bir harfin kaybolma/belirme süresi
    [Tooltip("Her bir harfin kaç saniyede kaybolacaðý veya belireceði.")]
    public float harfFadeSuresi = 0.1f;

    // Efekt döngüleri arasýndaki bekleme süresi
    [Tooltip("Yazý tamamen kaybolduktan ve belirdikten sonra ne kadar beklenecek?")]
    public float donguBeklemeSuresi = 1.0f;

    private TextMeshProUGUI tmProYazi;

    void Awake()
    {
        // Script'in eklendiði objenin TextMeshProUGUI component'ini otomatik bul
        tmProYazi = GetComponent<TextMeshProUGUI>();
        if (tmProYazi == null)
        {
            Debug.LogError("Bu objede TextMeshProUGUI componenti bulunamadý!");
        }
    }

    void Start()
    {
        // Efekt döngüsünü baþlat
        StartCoroutine(HarfAnimasyonDongusu());
    }

    IEnumerator HarfAnimasyonDongusu()
    {
        // Bu döngü oyun boyunca sonsuza kadar devam edecek
        while (true)
        {
            // 1. ADIM: Yazýyý Harf Harf Kaybet (Fade Out)
            yield return StartCoroutine(HarfleriFadele(false)); // false = görünmez yap

            // Bekle
            yield return new WaitForSeconds(donguBeklemeSuresi);

            // 2. ADIM: Yazýyý Harf Harf Göster (Fade In)
            yield return StartCoroutine(HarfleriFadele(true)); // true = görünür yap

            // Bekle
            yield return new WaitForSeconds(donguBeklemeSuresi);
        }
    }

    IEnumerator HarfleriFadele(bool gorunurYap)
    {
        // Metin bilgilerini güncelleyip almamýz lazým
        tmProYazi.ForceMeshUpdate();
        TMP_TextInfo yaziBilgisi = tmProYazi.textInfo;
        int karakterSayisi = yaziBilgisi.characterCount;

        if (karakterSayisi == 0) yield break; // Yazý yoksa devam etme

        // Hedef alpha (opaklýk) deðerini belirle
        // gorunurYap true ise hedef 255 (tamamen görünür), false ise 0 (görünmez)
        byte hedefAlpha = (byte)(gorunurYap ? 255 : 0);

        // ******** DÜZELTME BURADA ********
        // Baþlangýç alpha deðerini TextMeshPro'dan okumak yerine kendimiz belirliyoruz.
        // Çünkü ForceMeshUpdate() sonrasý okunan deðer (255) yanlýþ olabiliyor.
        // gorunurYap true ise 0'dan baþla (görünmezden), false ise 255'ten baþla (görünürden).
        byte baslangicAlpha = (byte)(gorunurYap ? 0 : 255);
        // **********************************

        // Her bir karakter (harf) için döngü
        for (int i = 0; i < karakterSayisi; i++)
        {
            // Eðer karakter boþluk gibi görünmez bir þeyse, atla
            if (!yaziBilgisi.characterInfo[i].isVisible)
            {
                continue;
            }

            // Karakterin hangi materyale ve vertex'e ait olduðunu bul
            int materyalIndex = yaziBilgisi.characterInfo[i].materialReferenceIndex;
            int vertexIndex = yaziBilgisi.characterInfo[i].vertexIndex;

            // Karakterin vertex renklerini al
            Color32[] vertexRenkleri = yaziBilgisi.meshInfo[materyalIndex].colors32;

            // ESKÝ SATIR (SÝLDÝK): byte baslangicAlpha = vertexRenkleri[vertexIndex].a;

            // Bu TEK harfi yavaþça fade'le
            float gecenZaman = 0f;
            while (gecenZaman < harfFadeSuresi)
            {
                gecenZaman += Time.deltaTime;

                // Geçen zamana göre yeni alpha deðerini hesapla
                byte anlikAlpha = (byte)Mathf.Lerp(baslangicAlpha, hedefAlpha, gecenZaman / harfFadeSuresi);

                // Bir karakter 4 vertex'ten (köþeden) oluþur. Dördünün de alphasýný güncelle.
                vertexRenkleri[vertexIndex + 0].a = anlikAlpha;
                vertexRenkleri[vertexIndex + 1].a = anlikAlpha;
                vertexRenkleri[vertexIndex + 2].a = anlikAlpha;
                vertexRenkleri[vertexIndex + 3].a = anlikAlpha;

                // Metnin vertex verisini güncellediðimizi TextMeshPro'ya bildir
                tmProYazi.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);

                yield return null; // Bir sonraki frame'e kadar bekle
            }

            // Süre bittiðinde hedefe tam oturduðundan emin ol
            vertexRenkleri[vertexIndex + 0].a = hedefAlpha;
            vertexRenkleri[vertexIndex + 1].a = hedefAlpha;
            vertexRenkleri[vertexIndex + 2].a = hedefAlpha;
            vertexRenkleri[vertexIndex + 3].a = hedefAlpha;
            tmProYazi.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
        }
    }
}