using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class PlayerAttack : MonoBehaviour
{
    [Header("Gerekli Objeler")]
    public GameObject buyuTopuPrefab;
    public Transform atisNoktasi;

    [Header("Mana Ayarý")]
    public float manaMaliyeti = 20f; // Her atýþ kaç mana yesin?

    private Animator animator;
    private static readonly int HashAttack = Animator.StringToHash("Attack");

    // Mana sistemine ulaþmak için deðiþken
    private ManaSistemi manaSistemi;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();

        // Ayný objenin üzerindeki ManaSistemi scriptini bul
        manaSistemi = GetComponent<ManaSistemi>();
        if (manaSistemi == null)
        {
            Debug.LogError("HATA: Player objesinde 'ManaSistemi' scripti yok! Lütfen ekle.");
        }
    }

    void Update()
    {
        if (Mouse.current == null || !Mouse.current.leftButton.wasPressedThisFrame) return;
        if (EventSystem.current.IsPointerOverGameObject()) return;

        AtesEt();
    }

    void AtesEt()
    {
        if (buyuTopuPrefab == null || atisNoktasi == null) return;

        // --- MANA KONTROLÜ ---
        // Eðer mana sistemi varsa VE mana harcamaya yetmiyorsa -> ATEÞ ETME, ÇIK.
        if (manaSistemi != null)
        {
            // ManaHarca fonksiyonu hem kontrol eder hem düþer.
            // Eðer false dönerse (mana yoksa) if'in içine girer ve return yapar.
            if (manaSistemi.ManaHarca(manaMaliyeti) == false)
            {
                return; // Mana yok, iptal.
            }
        }
        // ---------------------

        if (animator != null) animator.SetTrigger(HashAttack);
        Instantiate(buyuTopuPrefab, atisNoktasi.position, atisNoktasi.rotation);
    }
}