using UnityEngine;

public class NPCDialogue : MonoBehaviour
{
    // O an aktif olarak konuþulan NPC'yi hafýzada tutar
    private static NPCDialogue aktifKonusmaci;

    [Header("Konuþma Verisi")]
    public Konusma konusma;

    [Header("Baðlý Mekanikler")]
    [Tooltip("Bu konuþma BÝTTÝÐÝNDE GÝZLENECEK/AÇILACAK kapý objesi")]
    public GameObject acilacakKapi; // (Artýk bu objeyi SetActive(false) yapacaðýz)

    // Bu fonksiyon disaridan cagirildiginda konusmayi baslatir
    public void KonusmayiTetikle()
    {
        // DialogueManager'a "Hey, þu an aktif konuþmacý benim" diye haber ver
        aktifKonusmaci = this;

        DialogueManager.instance.KonusmayiBaslat(konusma);
    }

    // --- BU FONKSÝYON GÜNCELLENDÝ ---

    /// <summary>
    /// DialogueManager bu fonksiyonu çaðýrdýðýnda, 
    /// o an konuþan NPC'nin kapýsýný AÇAR (yani objeyi KAPATIR).
    /// </summary>
    public static void KonusmaBittiSinyali()
    {
        // Hafýzada bir konuþmacý var mý?
        if (aktifKonusmaci != null)
        {
            // O konuþmacýnýn açmak istediði bir kapý var mý?
            if (aktifKonusmaci.acilacakKapi != null)
            {
                Debug.Log("Diyalog bitti, kapý AÇILIYOR (obje gizleniyor): " + aktifKonusmaci.acilacakKapi.name);

                // KAPISINI AÇ (yani objeyi kapat/gizle)
                aktifKonusmaci.acilacakKapi.SetActive(false); // <-- DEÐÝÞÝKLÝK BURADA
            }

            // Hafýzayý temizle (iþimiz bitti)
            aktifKonusmaci = null;
        }
    }
    // --- GÜNCELLEME BÝTTÝ ---
}