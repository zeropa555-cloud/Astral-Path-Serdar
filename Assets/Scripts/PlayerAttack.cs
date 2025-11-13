using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI; // 1. BU SATIRI EKLEMEK ÞART! (UI için)

public class PlayerAttack : MonoBehaviour
{
    [Header("Gerekli Objeler")]
    public Transform atisNoktasi;

    [Header("UI Ayarlarý")]
    public Image buyuIkonu;  // O ortadaki elmas þeklindeki "Buyu" objesini buraya atacaðýz
    public Color morRenk = new Color(0.6f, 0f, 1f); // Varsayýlan Mor
    public Color atesRengi = Color.red;            // Ateþ Kýrmýzýsý

    [Header("Büyü 1 (Standart)")]
    public GameObject morBuyuPrefab;
    public float morBuyuMana = 10f;

    [Header("Büyü 2 (Ateþ)")]
    public GameObject atesTopuPrefab;
    public float atesBuyuMana = 25f;

    // O an hangisini kullanýyoruz?
    private GameObject seciliBuyuPrefab;
    private float seciliManaMaliyeti;

    private Animator animator;
    private ManaSistemi manaSistemi;
    private static readonly int HashAttack = Animator.StringToHash("Attack");

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        manaSistemi = GetComponent<ManaSistemi>();

        // Oyun baþlayýnca varsayýlan olarak 1. büyüyü seç
        BuyuSec(1);
    }

    void Update()
    {
        if (Keyboard.current != null)
        {
            // 1'e basýnca Mor Büyüye geç
            if (Keyboard.current.digit1Key.wasPressedThisFrame)
            {
                BuyuSec(1);
            }
            // 2'ye basýnca Ateþ Topuna geç
            if (Keyboard.current.digit2Key.wasPressedThisFrame)
            {
                BuyuSec(2);
            }

            // Sol týk ile ateþ et
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

            // --- UI RENGÝNÝ MOR YAP ---
            if (buyuIkonu != null)
            {
                buyuIkonu.color = morRenk;
            }

            Debug.Log("Büyü 1 Seçildi: Standart Büyü");
        }
        else if (numara == 2)
        {
            seciliBuyuPrefab = atesTopuPrefab;
            seciliManaMaliyeti = atesBuyuMana;

            // --- UI RENGÝNÝ KIRMIZI YAP ---
            if (buyuIkonu != null)
            {
                buyuIkonu.color = atesRengi;
            }

            Debug.Log("Büyü 2 Seçildi: Ateþ Topu");
        }
    }

    void AtesEt()
    {
        if (seciliBuyuPrefab == null || atisNoktasi == null) return;

        if (manaSistemi != null)
        {
            if (manaSistemi.ManaHarca(seciliManaMaliyeti) == false)
            {
                return; // Mana yok
            }
        }

        if (animator != null) animator.SetTrigger(HashAttack);
        Instantiate(seciliBuyuPrefab, atisNoktasi.position, atisNoktasi.rotation);
    }
}