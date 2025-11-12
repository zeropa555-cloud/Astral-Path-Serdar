using UnityEngine;
using TMPro; // TextMeshPro kullanmak icin bu SART!

[RequireComponent(typeof(TextMeshProUGUI))] // Bu script'i sadece TextMeshPro objelerine ekleyebilirsin
public class LocalizedText : MonoBehaviour
{
    [Tooltip("LocalizationManager'daki anahtar (key_basla_buton gibi)")]
    public string textKey; // Inspector'dan ayarlayacagiz

    private TextMeshProUGUI textComponent;

    // Obje aktif oldugunda (veya oyun basladiginda) calisir
    void OnEnable()
    {
        textComponent = GetComponent<TextMeshProUGUI>();
        UpdateText();
    }

    // Metni guncelleyen fonksiyon (LocalizationManager cagirir)
    public void UpdateText()
    {
        if (LocalizationManager.instance != null && textKey != null)
        {
            if (textComponent == null) // Eger referans yoksa tekrar al
            {
                textComponent = GetComponent<TextMeshProUGUI>();
            }

            // Manager'dan metni iste ve guncelle
            textComponent.text = LocalizationManager.instance.GetText(textKey);
        }
    }
}