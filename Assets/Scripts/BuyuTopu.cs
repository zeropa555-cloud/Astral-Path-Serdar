using UnityEngine;

public class BuyuTopu : MonoBehaviour
{
    [Header("Temel Ayarlar")]
    public float hiz = 20f;
    public float hasarMiktari = 5f; // Çarpınca vereceği anlık hasar (Ateş topu için az olsun)
    public float yasamSuresi = 4f;

    [Header("Ateş (Yanma) Ayarları")]
    [Tooltip("Bu kutuyu işaretlersen düşman yanmaya başlar")]
    public bool yakiciOlsunMu = true; // ATEŞ TOPUNDA BU KUTUYU İŞARETLE!
    public float yanmaHasari = 5f;    // Saniyede kaç can yakacak?
    public int kacSaniyeYansin = 3;   // Toplam kaç saniye sürecek?

    [System.Obsolete]
    void Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = transform.forward * hiz;
        }
        Destroy(gameObject, yasamSuresi);
    }

    void OnTriggerEnter(Collider other)
    {
        // Çarptığımız şey Düşman mı?
        DusmanCanSistemi dusman = other.GetComponent<DusmanCanSistemi>();

        if (dusman != null)
        {
            // 1. Anlık hasarı ver (GÜM!)
            dusman.HasarAl(hasarMiktari);

            // 2. Eğer yakıcı özellik açıksa YAK (CISS!)
            if (yakiciOlsunMu)
            {
                dusman.YanmaBaslat(yanmaHasari, kacSaniyeYansin);
            }

            Destroy(gameObject); // Topu yok et
        }
        else if (!other.CompareTag("Player"))
        {
            Destroy(gameObject); // Duvara çarpınca yok et
        }
    }
}