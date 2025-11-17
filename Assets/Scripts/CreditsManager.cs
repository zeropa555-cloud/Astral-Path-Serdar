using UnityEngine;
using UnityEngine.SceneManagement; // Sahne deðiþtirmek için ÞART
using UnityEngine.InputSystem;     // Tuþa basýnca geçmek için

public class CreditsManager : MonoBehaviour
{
    [Header("Ayarlar")]
    [Tooltip("Yazýnýn yukarý kayma hýzý")]
    public float kaymaHizi = 50f;

    [Tooltip("Yazý bu Yüksekliðe (Y) gelince Menüye dönsün")]
    public float bitisPozisyonuY = 2000f; // Bunu test edip ayarlayacaðýz

    [Header("Referanslar")]
    public RectTransform yaziTransformu; // KayanYazi objesini buraya atacaðýz

    void Update()
    {
        // 1. Yazýyý yukarý kaydýr
        if (yaziTransformu != null)
        {
            yaziTransformu.anchoredPosition += Vector2.up * kaymaHizi * Time.deltaTime;

            // 2. Yazý belirlediðimiz yüksekliði geçti mi?
            if (yaziTransformu.anchoredPosition.y > bitisPozisyonuY)
            {
                MenuyeDon();
            }
        }

        // 3. Oyuncu tuþa basarsa direkt geç (Skip)
        if (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame)
        {
            MenuyeDon();
        }

        // Fareye týklarsa da geçsin
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            MenuyeDon();
        }
    }

    void MenuyeDon()
    {
        Debug.Log("Credits bitti, Ana Menüye dönülüyor...");
        SceneManager.LoadScene("MainMenu");
    }
}