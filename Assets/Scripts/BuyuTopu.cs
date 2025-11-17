using UnityEngine;

public class BuyuTopu : MonoBehaviour
{
    public float hiz = 20f;
    public float hasarMiktari = 10f;
    public float yasamSuresi = 4f;

    [Header("Yanma Ayarları")]
    public bool yakiciOlsunMu = false;
    public float yanmaHasari = 5f;
    public int kacSaniyeYansin = 3;

    [System.Obsolete]
    void Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null) rb.velocity = transform.forward * hiz;
        Destroy(gameObject, yasamSuresi);
    }

    void OnTriggerEnter(Collider other)
    {
        DusmanCanSistemi dusman = other.GetComponent<DusmanCanSistemi>();
        if (dusman != null)
        {
            // 1. Normal hasarı ver (Animasyon oynatır)
            dusman.HasarAl(hasarMiktari, true);

            // 2. Eğer ateş topuysa yanmayı başlat
            if (yakiciOlsunMu)
            {
                dusman.YanmaBaslat(yanmaHasari, kacSaniyeYansin);
            }

            Destroy(gameObject);
        }
        else if (!other.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}