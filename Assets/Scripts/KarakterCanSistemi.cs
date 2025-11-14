using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement; // <-- Sahne yönetimi için GEREKLÝ
using UnityEngine.InputSystem;   // <-- Tuþ basýmý için GEREKLÝ

public class KarakterCanSistemi : MonoBehaviour
{
    [Header("Can Ayarlarý")]
    public float maxCan = 100f;

    [Header("Ölüm Ayarlarý")]
    [Tooltip("Karakter ölünce açýlacak UI Paneli")]
    public GameObject olumPaneli; // 3. Adýmda burayý dolduracaðýz

    [Header("Mevcut Durum")]
    [SerializeField]
    private float mevcutCan;
    public bool hayattaMi { get; private set; } // Düþmanlarýn durmasý için

    [Header("UI Sinyali")]
    public UnityEvent<float> OnCanDegisti;
    public UnityEvent OnOlum;

    void Start()
    {
        mevcutCan = maxCan;
        hayattaMi = true; // Oyuna hayatta baþla
        OnCanDegisti.Invoke(mevcutCan / maxCan);

        // Panelin baþta kapalý olduðundan %100 emin ol
        if (olumPaneli != null)
        {
            olumPaneli.SetActive(false);
        }
    }

    public void HasarAl(float miktar)
    {
        if (!hayattaMi) return; // Zaten öldüyse hasar alma

        mevcutCan -= miktar;
        mevcutCan = Mathf.Clamp(mevcutCan, 0f, maxCan);
        OnCanDegisti.Invoke(mevcutCan / maxCan);

        if (mevcutCan <= 0)
        {
            OlumFonksiyonu();
        }
    }

    public void Iyiles(float miktar)
    {
        if (!hayattaMi || mevcutCan == maxCan) return;
        mevcutCan += miktar;
        mevcutCan = Mathf.Clamp(mevcutCan, 0f, maxCan);
        OnCanDegisti.Invoke(mevcutCan / maxCan);
    }

    private void OlumFonksiyonu()
    {
        if (!hayattaMi) return; // Bu fonksiyonun sadece 1 kere çalýþmasýný garanti et

        hayattaMi = false; // ÖLDÜN!
        Debug.Log("Karakter ÖLDÜ!");
        OnOlum.Invoke(); // Ölüm animasyonunu tetikle

        // Hareket ve saldýrý scriptlerini devre dýþý býrak
        SimpleMove hareketScripti = GetComponent<SimpleMove>();
        if (hareketScripti != null) hareketScripti.enabled = false;
        PlayerAttack attackScripti = GetComponent<PlayerAttack>();
        if (attackScripti != null) attackScripti.enabled = false;

        // Ölüm Panelini aç
        if (olumPaneli != null)
        {
            olumPaneli.SetActive(true);
        }

        // Mouse'u görünür ve serbest yap
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    // --- SÝSTEMÝN KALBÝ BURASI ---
    // Her frame "acaba bir tuþa basýldý mý?" diye kontrol eder
    void Update()
    {
        // Eðer oyuncu hayattaysa, bu fonksiyonu HÝÇ ÇALIÞTIRMA
        if (hayattaMi) return;

        // Eðer oyuncu öldüyse (yani 'hayattaMi' false ise) 
        // ve (klavyeden VEYA fareden) herhangi bir tuþa BASTIYSA...
        if (Keyboard.current.anyKey.wasPressedThisFrame ||
            (Mouse.current != null && (Mouse.current.leftButton.wasPressedThisFrame || Mouse.current.rightButton.wasPressedThisFrame)))
        {
            // MainMenu'ye dön
            Debug.Log("Ana Menüye dönülüyor...");
            SceneManager.LoadScene("MainMenu");
        }
    }
}