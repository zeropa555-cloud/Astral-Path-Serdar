using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem; // Yeni Input Sistemi

public class IntroYoneticisi : MonoBehaviour
{
    public VideoPlayer videoPlayer;

    [Tooltip("Video bitince (veya geçilince) hangi sahne açýlsýn?")]
    public string sonrakiSahneAdi = "MainMenu"; // Buraya gitmek istediðin sahne adýný yaz

    void Start()
    {
        // Video Player componentini otomatik bulmaya çalýþ
        if (videoPlayer == null)
            videoPlayer = GetComponent<VideoPlayer>();

        // Video bittiðinde çalýþacak fonksiyonu ayarla
        videoPlayer.loopPointReached += VideoBitti;
    }

    // Video doðal yolla biterse bu çalýþýr
    void VideoBitti(VideoPlayer vp)
    {
        SonrakiSahneyeGec();
    }

    void Update()
    {
        // Oyuncu herhangi bir tuþa basarsa videoyu geç (Skip)
        if (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame)
        {
            SonrakiSahneyeGec();
        }

        // Fare týklamasý ile de geçmek istersen:
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            SonrakiSahneyeGec();
        }
    }

    void SonrakiSahneyeGec()
    {
        Debug.Log("Intro bitti/geçildi. Sahne yükleniyor: " + sonrakiSahneAdi);
        SceneManager.LoadScene(sonrakiSahneAdi);
    }
}