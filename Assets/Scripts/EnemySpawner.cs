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
    public GameObject[] arenaKapilari; // (SENÝN KODUN - DOKUNULMADI)

    // --- YENÝ EKLENDÝ (MÜZÝK) ---
    [Header("Müzik Ayarlarý")]
    [Tooltip("Savaþ baþlayýnca çalacak olan AudioSource (bu objede olmalý)")]
    public AudioSource savasMuzigiSource;
    [Tooltip("Savaþ baþlayýnca DURDURULACAK olan normal müzik (Oyuncuda veya Kamerada olabilir)")]
    public AudioSource normalMuzikSource;
    // ------------------------------------

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
    private BoxCollider spawnAlaniTrigger;

    void Start()
    {
        tumRoundlarBitti = false;
        spawnAlaniTrigger = GetComponent<BoxCollider>();
        if (spawnAlaniTrigger != null)
        {
            spawnAlaniTrigger.isTrigger = true;
        }

        DusmanCanSistemi.OnDusmanOldu += DusmanOlduHaberiAldim;

        // Kapýlarýn AÇIK olduðundan emin ol (SENÝN KODUN - DOKUNULMADI)
        foreach (GameObject kapi in arenaKapilari)
        {
            if (kapi != null) kapi.SetActive(false);
        }

        // --- YENÝ EKLENDÝ (MÜZÝK) ---
        // Savaþ müziðinin baþlangýçta kapalý ve döngüde olduðundan emin ol
        if (savasMuzigiSource != null)
        {
            savasMuzigiSource.Stop();
            savasMuzigiSource.loop = true; // Savaþ bitene kadar döngüde çal
        }
        // ----------------------------
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !roundBasladiMi)
        {
            Debug.Log("OYUNCU ARENAYA GÝRDÝ! KAPILAR KÝLÝTLENÝYOR!");
            roundBasladiMi = true;

            // Kapýlarý Kapat (barrier'larý aktif et) (SENÝN KODUN - DOKUNULMADI)
            foreach (GameObject kapi in arenaKapilari)
            {
                if (kapi != null) kapi.SetActive(true);
            }

            // --- YENÝ EKLENDÝ (MÜZÝK) ---
            // Normal müziði durdur
            if (normalMuzikSource != null && normalMuzikSource.isPlaying)
            {
                normalMuzikSource.Stop();
            }
            // Savaþ müziðini baþlat
            if (savasMuzigiSource != null)
            {
                savasMuzigiSource.Play();
            }
            // ---------------------------

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

                // Tüm roundlar bitti, Kapýlarý AÇ (SENÝN KODUN - DOKUNULMADI)
                foreach (GameObject kapi in arenaKapilari)
                {
                    if (kapi != null) kapi.SetActive(false);
                }

                // --- YENÝ EKLENDÝ (MÜZÝK) ---
                // Savaþ bitti, müziði deðiþtir
                if (savasMuzigiSource != null && savasMuzigiSource.isPlaying)
                {
                    savasMuzigiSource.Stop();
                }
                if (normalMuzikSource != null)
                {
                    normalMuzikSource.Play(); // Normal müziði geri aç
                }
                // ---------------------------
            }
            else
            {
                Invoke("RounduBaslat", roundArasiBekleme);
            }
        }
    }

    void RounduBaslat()
    {
        // (SENÝN KODUN - DOKUNULMADI)
        mevcutRound++;
        if (mevcutRound == 1) { spawnEdilecekSayi = Random.Range(minSpawnSayisi, maxSpawnSayisi + 1); }
        else { spawnEdilecekSayi += Random.Range(minArtis, maxArtis + 1); }
        Debug.Log("ROUND " + mevcutRound + " BAÞLIYOR! (RASTGELE " + spawnEdilecekSayi + " düþman spawn edilecek)");
        hayattakiDusmanSayisi = spawnEdilecekSayi;
        for (int i = 0; i < spawnEdilecekSayi; i++) { SpawnEt(); }
    }

    void SpawnEt()
    {
        // (SENÝN KODUN - DOKUNULMADI)
        if (dusmanPrefab == null || spawnAlaniTrigger == null) return;
        Bounds triggerBounds = spawnAlaniTrigger.bounds;
        float randomX = Random.Range(triggerBounds.min.x, triggerBounds.max.x);
        float randomZ = Random.Range(triggerBounds.min.z, triggerBounds.max.z);
        Vector3 rastgeleNokta = new Vector3(randomX, triggerBounds.center.y - triggerBounds.extents.y, randomZ);
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