using UnityEngine;
using UnityEngine.InputSystem; // YENI Input Sistemi icin SART!
using UnityEngine.EventSystems; // UI'a tiklayip tiklamadigini anlamak icin SART!

public class ClickToDamage : MonoBehaviour
{
    public float hasarMiktari = 10f;
    private Camera anaKamera;

    void Start()
    {
        anaKamera = Camera.main; // Kamerayi basta bir kere bul
    }

    void Update()
    {
        // Gerekli sistemler yoksa veya fareye basýlmadýysa devam etme
        if (anaKamera == null || Mouse.current == null || !Mouse.current.leftButton.wasPressedThisFrame)
        {
            return;
        }

        // Eger fare bir UI elementinin (Buton, Panel vs.) uzerindeyse, ISLEM YAPMA.
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        // Fare pozisyonundan dunyaya dogru bir isin olustur
        Ray isin = anaKamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        // Isini gonder (en fazla 100 metre)
        if (Physics.Raycast(isin, out RaycastHit hitInfo, 100f))
        {
            // Isin bir seye carpti. Carptigi seyde "DusmanCanSistemi" var mi?
            DusmanCanSistemi dusman = hitInfo.collider.GetComponent<DusmanCanSistemi>();

            if (dusman != null)
            {
                // EVET! Dusman bulduk. Hasar ver.
                Debug.Log("Dusmana vuruldu: " + hitInfo.collider.name);
                dusman.HasarAl(hasarMiktari);
            }
        }
    }
}