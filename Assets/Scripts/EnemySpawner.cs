using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(BoxCollider))]
public class EnemySpawner : MonoBehaviour
{
    public static bool tumRoundlarBitti = false;

    [Header("Düþman Prefab'ý")]
    public GameObject dusmanPrefab;

    [Header("Kapi Ayarlarý")]
    [Tooltip("Arena baþlayýnca kapanacak (aktif olacak) kapý/bariyer objeleri")]
    public GameObject[] arenaKapilari;

    [Header("Round Ayarlarý")]
    public int toplamRoundSayisi = 2;
    public float roundArasiBekleme = 3.0f;
    [Header("Rastgele Spawn Sayýlarý")]
    public int minSpawnSayisi = 3;
    public int maxSpawnSayisi = 6;
    public int minArtis = 2;
    public int maxArtis = 4;

    private int mevcutRound = 0;
    private int hayattakiDusmanSayisi;
    private int spawnEdilecekSayi;
    private bool roundBasladiMi = false;

    // Collider'ý hafýzada tutacaðýz
    private BoxCollider spawnAlaniTrigger;

    void Start()
    {
        tumRoundlarBitti = false;

        // 1. Trigger'ý Ayarla (ve hafýzaya al)
        spawnAlaniTrigger = GetComponent<BoxCollider>();
        if (spawnAlaniTrigger != null)
        {
            spawnAlaniTrigger.isTrigger = true;
        }

        // --- (GEREKSÝZ KOD SÝLÝNDÝ) ---
        // NavMesh sýnýrlarýný bulma kodu buradan kaldýrýldý.
        // Artýk spawnAlaniTrigger.bounds kullanýyoruz.
        // --------------------------------

        // 3. "Ben öldüm!" sinyalini dinle
        DusmanCanSistemi.OnDusmanOldu += DusmanOlduHaberiAldim;

        // 4. Kapýlarýn AÇIK (kapalý) olduðundan emin ol
        foreach (GameObject kapi in arenaKapilari)
        {
            if (kapi != null) kapi.SetActive(false);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !roundBasladiMi)
        {
            Debug.Log("OYUNCU ARENAYA GÝRDÝ! KAPILAR KÝLÝTLENÝYOR!");
            roundBasladiMi = true;

            // Kapýlarý Kapat (barrier'larý aktif et)
            foreach (GameObject kapi in arenaKapilari)
            {
                if (kapi != null) kapi.SetActive(true);
            }

            RounduBaslat();
        }
    }

    void OnDestroy()
    {
        DusmanCanSistemi.OnDusmanOldu -= DusmanOlduHaberiAldim;
    }

    void DusmanOlduHaberiAldim()
    {
        hayattakiDusmanSayisi--;
        if (hayattakiDusmanSayisi <= 0)
        {
            Debug.Log("ROUND " + mevcutRound + " TAMAMLANDI!");
            if (mevcutRound >= toplamRoundSayisi)
            {
                Debug.Log("TÜM DÜÞMANLAR YENÝLDÝ! Sandýk kilidi ve kapýlar açýldý.");
                tumRoundlarBitti = true;

                // Tüm roundlar bitti, Kapýlarý AÇ
                foreach (GameObject kapi in arenaKapilari)
                {
                    if (kapi != null) kapi.SetActive(false);
                }
            }
            else
            {
                Invoke("RounduBaslat", roundArasiBekleme);
            }
        }
    }

    void RounduBaslat()
    {
        mevcutRound++;
        if (mevcutRound == 1) { spawnEdilecekSayi = Random.Range(minSpawnSayisi, maxSpawnSayisi + 1); }
        else { spawnEdilecekSayi += Random.Range(minArtis, maxArtis + 1); }
        Debug.Log("ROUND " + mevcutRound + " BAÞLIYOR! (RASTGELE " + spawnEdilecekSayi + " düþman spawn edilecek)");
        hayattakiDusmanSayisi = spawnEdilecekSayi;
        for (int i = 0; i < spawnEdilecekSayi; i++) { SpawnEt(); }
    }

    void SpawnEt()
    {
        if (dusmanPrefab == null || spawnAlaniTrigger == null) return;

        // 1. Trigger'ýn (BoxCollider) sýnýrlarýný al
        Bounds triggerBounds = spawnAlaniTrigger.bounds;

        // 2. O kutunun sýnýrlarý içinde rastgele bir X ve Z noktasý seç
        float randomX = Random.Range(triggerBounds.min.x, triggerBounds.max.x);
        float randomZ = Random.Range(triggerBounds.min.z, triggerBounds.max.z);
        Vector3 rastgeleNokta = new Vector3(randomX, triggerBounds.center.y - triggerBounds.extents.y, randomZ);

        // 3. Bu noktanýn NavMesh üzerinde olduðundan emin ol
        NavMeshHit hit;
        if (NavMesh.SamplePosition(rastgeleNokta, out hit, 3.0f, NavMesh.AllAreas))
        {
            Instantiate(dusmanPrefab, hit.position, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("Geçerli bir spawn noktasý bulunamadý. Trigger'ýn NavMesh üzerinde olduðundan emin ol.");
        }
    }
}