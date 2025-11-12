using UnityEngine;

public class NPCDialogue : MonoBehaviour
{
    [Header("Konuþma Verisi")]
    // [System.Serializable] dedigimiz icin bu Inspector'da gorunecek
    public Konusma konusma;

    // Bu fonksiyon disaridan cagirildiginda konusmayi baslatir
    public void KonusmayiTetikle()
    {
        // DialogueManager'in singleton'ina (instance) ulas ve konusmayi baslat
        DialogueManager.instance.KonusmayiBaslat(konusma);
    }
}