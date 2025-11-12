using UnityEngine;

// [System.Serializable] sayesinde bu kalip Inspector'da gorunur
[System.Serializable]
public class DiyalogSatiri // (Dialogue Line)
{
    public string konusmaciAdi; // Or: "Yeþil Uzaylý"
    public Sprite konusmaciPortresi; // PORTRE ÝÇÝN EKLENEN YENÝ SATIR

    [TextArea(3, 10)]
    public string metin; // Or: "Merhaba gezgin..."
}

[System.Serializable]
public class Konusma // (Conversation)
{
    // Bu, DiyalogSatiri'ndan olusan bir dizi (liste)
    public DiyalogSatiri[] satirlar;
}