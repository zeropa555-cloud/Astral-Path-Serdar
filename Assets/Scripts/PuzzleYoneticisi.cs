using UnityEngine;

public class PuzzleYoneticisi : MonoBehaviour
{
    [Header("Baðlantýlar")]
    public SandikEtkilesim sandikScripti; // Hangi sandýðý açacak?

    // Bu fonksiyonu panel açýlýnca mouse'u görünür yapmak için kullanýyoruz
    void OnEnable()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    // Bu fonksiyonu butonun OnClick özelliðine baðlayacaksýn
    public void PuzzleiCoz()
    {
        Debug.Log("Puzzle Çözüldü! Sandýk açýlýyor...");

        // 1. Paneli Kapat
        gameObject.SetActive(false);

        // 2. Mouse'u tekrar gizle (Oyuna dön)
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        // 3. Sandýða "Açýl" emrini ver
        if (sandikScripti != null)
        {
            sandikScripti.SandigiAc(); // Sandýktaki özel fonksiyonu çaðýrýyoruz
        }
    }

    // "Kapat" butonu için (Ýsteðe baðlý)
    public void PaneliKapat()
    {
        gameObject.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}