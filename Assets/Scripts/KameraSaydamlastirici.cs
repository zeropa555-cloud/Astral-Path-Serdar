using UnityEngine;

public class KameraSaydamlastirici : MonoBehaviour
{
    [Header("Ayarlar")]
    public Transform oyuncu; // Karakterini buraya sürükle
    public LayerMask duvarLayer; // Sadece 'Duvar' layer'ýný seçeceðiz
    [Range(0.1f, 1f)]
    public float saydamlikMiktari = 0.3f; // Duvar ne kadar þeffaf olsun?

    private Renderer suankiDuvar;
    private Color orijinalRenk;

    void Update()
    {
        if (oyuncu == null) return;

        // Kameradan oyuncuya doðru yön ve mesafe
        Vector3 yon = (oyuncu.position - transform.position).normalized;
        float mesafe = Vector3.Distance(transform.position, oyuncu.position);

        RaycastHit hit;

        // Oyuncuya doðru ýþýn at (Sadece duvarLayer'ýna çarpacak þekilde)
        if (Physics.Raycast(transform.position, yon, out hit, mesafe, duvarLayer))
        {
            Renderer hitRenderer = hit.collider.GetComponent<Renderer>();

            if (hitRenderer != null)
            {
                // Eðer yeni bir duvara çarptýysak
                if (suankiDuvar != hitRenderer)
                {
                    // Önceki duvarý eski haline getir (varsa)
                    EskiDuvariDuzelt();

                    // Yeni duvarý kaydet
                    suankiDuvar = hitRenderer;
                    orijinalRenk = suankiDuvar.material.color; // Rengini sakla

                    // ÞEFFAFLAÞTIR
                    Color yeniRenk = orijinalRenk;
                    yeniRenk.a = saydamlikMiktari; // Alpha'yý düþür
                    suankiDuvar.material.color = yeniRenk;
                }
            }
        }
        else
        {
            // Arada engel yoksa eski duvarý düzelt
            EskiDuvariDuzelt();
        }
    }

    void EskiDuvariDuzelt()
    {
        if (suankiDuvar != null)
        {
            suankiDuvar.material.color = orijinalRenk; // Rengi geri yükle
            suankiDuvar = null;
        }
    }
}