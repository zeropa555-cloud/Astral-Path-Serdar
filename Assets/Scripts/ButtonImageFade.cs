using UnityEngine;
using UnityEngine.UI; // Image component'i icin bu satir SART!
using UnityEngine.EventSystems; // Fare hareketleri icin bu sart
using System.Collections; // Coroutine (yavaslatma) icin bu sart

// Bu script'in eklendigi objede mutlaka Image olmasini zorunlu kýlar
[RequireComponent(typeof(Image))]
public class ButtonImageFade : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Tooltip("Butonun ne kadar hizli kaybolacagi veya geri gelecegi (saniye)")]
    public float fadeSuresi = 0.3f;

    private Image buttonImage;
    private Color originalImageColor;
    private Coroutine aktifFadeCoroutine;

    void Awake()
    {
        buttonImage = GetComponent<Image>();
        if (buttonImage != null)
        {
            // Butonun R-G-B rengini kaydet (sadece alpha'yi degistirecegiz)
            originalImageColor = buttonImage.color;
        }
    }

    // 1. Fare Butonun Uzerine Geldiginde
    public void OnPointerEnter(PointerEventData eventData)
    {
        StartFade(0f); // Alpha'yi 0 yap (gorunmez)
    }

    // 2. Fare Butonun Uzerinden Cekildiginde
    public void OnPointerExit(PointerEventData eventData)
    {
        StartFade(1f); // Alpha'yi 1 yap (gorunur)
    }

    // Yavaslatma (Fade) islemini baslatan yardýmcý fonksiyon
    private void StartFade(float hedefAlpha)
    {
        if (aktifFadeCoroutine != null)
        {
            StopCoroutine(aktifFadeCoroutine);
        }
        aktifFadeCoroutine = StartCoroutine(Fade(hedefAlpha));
    }

    // Asýl yavaslatma (Fade) islemini yapan Coroutine
    IEnumerator Fade(float hedefAlpha)
    {
        Color baslangicColor = buttonImage.color;
        Color hedefColor = new Color(originalImageColor.r, originalImageColor.g, originalImageColor.b, hedefAlpha);

        float gecenZaman = 0f;

        while (gecenZaman < fadeSuresi)
        {
            gecenZaman += Time.deltaTime;
            buttonImage.color = Color.Lerp(baslangicColor, hedefColor, gecenZaman / fadeSuresi);
            yield return null;
        }

        buttonImage.color = hedefColor;
        aktifFadeCoroutine = null;
    }
}