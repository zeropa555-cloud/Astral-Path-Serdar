using UnityEngine;
using UnityEngine.AI; // NavMesh için ÞART!

public class EnemySpawner : MonoBehaviour
{
    [Header("Düþman Ayarlarý")]
    public GameObject dusmanPrefab;

    [Header("Round Ayarlarý")]
    public int maxRound = 2; // Toplam kaç round olsun?
    public int dusmanSayisi = 5; // Her round'da kaç düþman spawn olacak?
    public float roundArasiBekleme = 3.0f; // Round bittikten sonra yenisi kaç sn sonra baþlasýn?

    private int mevcutRound = 0;
    private int hayattakiDusmanSayisi;
    private Bounds navMeshBounds;

    void Start()
    {
        // 1. NavMesh sýnýrlarýný bul (Senin eski kodunla ayný)
        NavMeshTriangulation navMeshData = NavMesh.CalculateTriangulation();
        if (navMeshData.vertices.Length > 0)
        {
            navMeshBounds = new Bounds(navMeshData.vertices[0], Vector3.zero);
            foreach (Vector3 vertex in navMeshData.vertices)
            {
                navMeshBounds.Encapsulate(vertex);
            }
        }
        else
        {
            Debug.LogError("NavMesh datasý bulunamadý! Lütfen NavMesh'i bake et.");
            return;
        }

        // 2. "Ben öldüm!" sinyalini dinlemeye baþla
        DusmanCanSistemi.OnDusmanOldu += DusmanOlduHaberiAldim;

        // 3. Ýlk Round'u baþlat
        RounduBaslat();
    }

    // Script yok edildiðinde sinyali dinlemeyi býrak (hafýza sýzýntýsý olmasýn)
    void OnDestroy()
    {
        DusmanCanSistemi.OnDusmanOldu -= DusmanOlduHaberiAldim;
    }

    void RounduBaslat()
    {
        mevcutRound++; // Round sayýsýný 1 artýr

        // Eðer son round'u da geçtiysek...
        if (mevcutRound > maxRound)
        {
            Debug.Log("OYUN BÝTTÝ! KAZANDIN!");
            this.enabled = false; // Spawner'ý durdur
            return;
        }

        Debug.Log("ROUND " + mevcutRound + " BAÞLIYOR!");

        // Bu round için hayattaki düþman sayýsýný ayarla
        hayattakiDusmanSayisi = dusmanSayisi;

        // 5 tane (veya "dusmanSayisi" kadar) düþman spawn et
        for (int i = 0; i < dusmanSayisi; i++)
        {
            SpawnEt(); // Düþman yaratan yardýmcý fonksiyonu çaðýr
        }
    }

    // Bir düþman öldüðünde 'DusmanCanSistemi'nden gelen sinyal bu fonksiyonu çalýþtýrýr
    void DusmanOlduHaberiAldim()
    {
        hayattakiDusmanSayisi--; // Hayattaki düþman sayýsýný 1 azalt

        // Eðer hayatta düþman kalmadýysa...
        if (hayattakiDusmanSayisi <= 0)
        {
            Debug.Log("ROUND " + mevcutRound + " TAMAMLANDI!");

            // 3 saniye bekle, sonra yeni round'u baþlat
            Invoke("RounduBaslat", roundArasiBekleme);
        }
    }

    // Bu fonksiyon sadece 1 tane düþman spawn eder
    void SpawnEt()
    {
        if (dusmanPrefab == null) return;

        // NavMesh sýnýrlarý içinde rastgele bir nokta bul (Senin eski kodunla ayný)
        float randomX = Random.Range(navMeshBounds.min.x, navMeshBounds.max.x);
        float randomZ = Random.Range(navMeshBounds.min.z, navMeshBounds.max.z);
        Vector3 rastgeleNokta = new Vector3(randomX, navMeshBounds.center.y, randomZ);

        NavMeshHit hit;
        if (NavMesh.SamplePosition(rastgeleNokta, out hit, 10.0f, NavMesh.AllAreas))
        {
            // Düþmaný o noktaya at
            Instantiate(dusmanPrefab, hit.position, Quaternion.identity);
        }
    }
}