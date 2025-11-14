using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections; // 1. BU SATIRI EKLEMEK ŞART! (Zamanlayıcı için)

public class PlayerAttack : MonoBehaviour
{
    [Header("Gerekli Objeler")]
    public Transform atisNoktasi;

    [Header("UI Ayarları")]
    public Image buyuIkonu;
    public Color morRenk = new Color(0.6f, 0f, 1f);
    public Color atesRengi = Color.red;

    [Header("Büyü 1 (Standart)")]
    public GameObject morBuyuPrefab;
    public float morBuyuMana = 10f;

    [Header("Büyü 2 (Ateş)")]
    public GameObject atesTopuPrefab;
    public float atesBuyuMana = 25f;

    // ----- 2. YENİ EKLENEN SATIR -----
    [Header("Senkronizasyon")]
    [Tooltip("Animasyon başladıktan KAÇ SANİYE SONRA top fırlasın?")]
    public float atesAnimasyonGecikmesi = 0.5f; // Animasyonun bitme süresi
    // ------------------------------------

    private GameObject seciliBuyuPrefab;
    private float seciliManaMaliyeti;

    private Animator animator;
    private ManaSistemi manaSistemi;
    private static readonly int HashAttack = Animator.StringToHash("Attack");

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        manaSistemi = GetComponent<ManaSistemi>();
        BuyuSec(1);
    }

    void Update()
    {
        if (Keyboard.current != null)
        {
            if (Keyboard.current.digit1Key.wasPressedThisFrame) { BuyuSec(1); }
            if (Keyboard.current.digit2Key.wasPressedThisFrame) { BuyuSec(2); }

            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                if (!EventSystem.current.IsPointerOverGameObject())
                {
                    AtesEt();
                }
            }
        }
    }

    void BuyuSec(int numara)
    {
        if (numara == 1)
        {
            seciliBuyuPrefab = morBuyuPrefab;
            seciliManaMaliyeti = morBuyuMana;
            if (buyuIkonu != null) { buyuIkonu.color = morRenk; }
            Debug.Log("Büyü 1 Seçildi: Standart Büyü");
        }
        else if (numara == 2)
        {
            seciliBuyuPrefab = atesTopuPrefab;
            seciliManaMaliyeti = atesBuyuMana;
            if (buyuIkonu != null) { buyuIkonu.color = atesRengi; }
            Debug.Log("Büyü 2 Seçildi: Ateş Topu");
        }
    }

    // ----- 3. GÜNCELLENEN FONKSİYON -----
    void AtesEt()
    {
        if (seciliBuyuPrefab == null || atisNoktasi == null) return;

        // 1. Mana kontrolü
        if (manaSistemi != null)
        {
            if (manaSistemi.ManaHarca(seciliManaMaliyeti) == false)
            {
                return; // Mana yoksa HİÇBİR ŞEY yapma
            }
        }

        // 2. Mana varsa, SADECE animasyonu tetikle
        if (animator != null) animator.SetTrigger(HashAttack);

        // 3. Topu fırlatmak için ZAMANLAYICIYI (Coroutine) başlat
        StartCoroutine(AtesGecikmesiRutini());
    }

    // ----- 4. YENİ EKLENEN ZAMANLAYICI -----
    IEnumerator AtesGecikmesiRutini()
    {
        // Kod, bu satırda 'atesAnimasyonGecikmesi' saniyesi kadar bekleyecek
        yield return new WaitForSeconds(atesAnimasyonGecikmesi);

        // Bekleme süresi bittikten sonra:
        // Topu (büyüyü) fırlat
        if (seciliBuyuPrefab != null && atisNoktasi != null)
        {
            Instantiate(seciliBuyuPrefab, atisNoktasi.position, atisNoktasi.rotation);
        }
    }
}