using UnityEngine;
using UnityEngine.AI;
// using System.Collections.Generic; // Artýk List'e gerek yok

[RequireComponent(typeof(BoxCollider))]
public class EnemySpawner : MonoBehaviour
{
    [Header("Düþman Prefab'ý")]
    [Tooltip("Spawn edilecek düþman prefab'ý")]
    public GameObject dusmanPrefab;

    [Header("Round Ayarlarý")]
    public int ilkRoundSpawnSayisi = 5;
    public int sonrakiRoundArtisi = 3;
    public float roundArasiBekleme = 3.0f;

    private int mevcutRound = 0;
    private int hayattakiDusmanSayisi;
    private int spawnEdilecekSayi;
    private Bounds navMeshBounds;

    private bool roundBasladiMi = false;

    void Start()
    {
        // 1. Trigger'ý Ayarla (Arena giriþini)
        BoxCollider triggerAlan = GetComponent<BoxCollider>();
        if (triggerAlan != null)
        {
            triggerAlan.isTrigger = true;
        }

        // 2. NavMesh sýnýrlarýný bul
        NavMeshTriangulation navMeshData = NavMesh.CalculateTriangulation();
        if (navMeshData.vertices.Length > 0)
        {
            navMeshBounds = new Bounds(navMeshData.vertices[0], Vector3.zero);
            foreach (Vector3 vertex in navMeshData.vertices)
            {
                navMeshBounds.Encapsulate(vertex);
            }
        }
        else { Debug.LogError("NavMesh datasý bulunamadý!"); return; }

        // 3. "Ben öldüm!" sinyalini dinle
        DusmanCanSistemi.OnDusmanOldu += DusmanOlduHaberiAldim;

        // 4. Havuz doldurma kodu SÝLÝNDÝ.

        // 5. Round 1 sayýsýný ayarla, baþlamayý bekle
        spawnEdilecekSayi = ilkRoundSpawnSayisi;
    }

    // Karakter (Player) bu objenin trigger'ýna girdiðinde çalýþýr
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !roundBasladiMi)
        {
            Debug.Log("OYUNCU ARENAYA GÝRDÝ! ROUND 1 BAÞLIYOR!");
            roundBasladiMi = true;
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
            Invoke("RounduBaslat", roundArasiBekleme);
        }
    }

    void RounduBaslat()
    {
        if (mevcutRound != 0)
        {
            spawnEdilecekSayi += sonrakiRoundArtisi;
        }
        mevcutRound++;
        Debug.Log("ROUND " + mevcutRound + " BAÞLIYOR! (" + spawnEdilecekSayi + " düþman spawn edilecek)");

        hayattakiDusmanSayisi = spawnEdilecekSayi;

        for (int i = 0; i < spawnEdilecekSayi; i++)
        {
            SpawnEt();
        }
    }

    // --- BU FONKSÝYON INSTANTIATE'E GERÝ DÖNDÜ ---
    void SpawnEt()
    {
        if (dusmanPrefab == null) return;

        // 1. Rastgele bir spawn noktasý bul (NavMesh üzerinde)
        float randomX = Random.Range(navMeshBounds.min.x, navMeshBounds.max.x);
        float randomZ = Random.Range(navMeshBounds.min.z, navMeshBounds.max.z);
        Vector3 rastgeleNokta = new Vector3(randomX, navMeshBounds.center.y, randomZ);

        NavMeshHit hit;
        if (NavMesh.SamplePosition(rastgeleNokta, out hit, 10.0f, NavMesh.AllAreas))
        {
            // 2. DÜÞMANI YARAT (Instantiate)
            Instantiate(dusmanPrefab, hit.position, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("Geçerli bir spawn noktasý bulunamadý, bu düþman spawn olamadý.");
        }
    }
}