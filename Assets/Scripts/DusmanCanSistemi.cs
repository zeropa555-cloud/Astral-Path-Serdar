using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DusmanCanSistemi : MonoBehaviour
{
    public float maxCan = 50f;
    private float mevcutCan;
    public Slider canBariSlider; // Varsa slider'ı bağla

    void Start()
    {
        mevcutCan = maxCan;
    }

    // Normal Hasar Alma
    public void HasarAl(float miktar)
    {
        mevcutCan -= miktar;
        if (canBariSlider != null) canBariSlider.value = mevcutCan / maxCan;

        if (mevcutCan <= 0)
        {
            StopAllCoroutines(); // Yanmayı durdur
            Destroy(gameObject); // Düşmanı yok et
        }
    }

    // --- ATEŞ TOPU İÇİN GEREKLİ OLAN KISIM BURASI ---

    // Dışarıdan (Top'tan) çağrılan fonksiyon
    public void YanmaBaslat(float saniyeBasiHasar, int kacSaniyeSurucek)
    {
        // Üst üste yanmasın diye önce eskisi varsa durduruyoruz
        StopCoroutine("YanmaDongusu");
        // Yeni yanmayı başlatıyoruz
        StartCoroutine(YanmaDongusu(saniyeBasiHasar, kacSaniyeSurucek));
    }

    // Saniye saniye can azaltan zamanlayıcı
    IEnumerator YanmaDongusu(float hasar, int sure)
    {
        for (int i = 0; i < sure; i++)
        {
            yield return new WaitForSeconds(1f); // 1 saniye bekle

            if (mevcutCan > 0)
            {
                HasarAl(hasar); // Can azalt
                Debug.Log(gameObject.name + " yanıyor! Canı: " + mevcutCan);
            }
        }
    }
}