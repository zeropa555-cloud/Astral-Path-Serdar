using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(BoxCollider))]
public class EnemySpawner : MonoBehaviour
{
    public static bool tumRoundlarBitti = false;

    [Header("Düþman Prefab'ý")]
    public GameObject dusmanPrefab;

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

    // --- YENÝ EKLENDÝ ---
    // Bütün haritanýn NavMesh'i yerine, bu trigger'ýn collider'ýný kullanacaðýz
    private BoxCollider spawnAlaniTrigger;
    // -------------------

    void Start()
    {
        tumRoundlarBitti = false;

        // 1. Trigger'ý Ayarla (ve hafýzaya al)
        spawnAlaniTrigger = GetComponent<BoxCollider>(); // Collider'ý bul
        if (spawnAlaniTrigger != null)
        {
            spawnAlaniTrigger.isTrigger = true;
        }

        // 2. NavMesh sýnýrlarýný bulma kodu SÝLÝNDÝ.
        //    Artýk spawnAlaniTrigger.bounds kullanacaðýz.

        // 3. "Ben öldüm!" sinyalini dinle
        DusmanCanSistemi.OnDusmanOldu += DusmanOlduHaberiAldim;
    }

    void OnTriggerEnter(Collider other)
    {
        // (Burasý ayný kaldý)
        if (other.CompareTag("Player") && !roundBasladiMi)
        {
            Debug.Log("OYUNCU ARENAYA GÝRDÝ! ROUND 1 BAÞLIYOR!");
            roundBasladiMi = true;
            RounduBaslat();
        }
    }

    void OnDestroy()
    {
        // (Burasý ayný kaldý)
        DusmanCanSistemi.OnDusmanOldu -= DusmanOlduHaberiAldim;
    }

    void DusmanOlduHaberiAldim()
    {
        // (Burasý ayný kaldý)
        hayattakiDusmanSayisi--;
        if (hayattakiDusmanSayisi <= 0)
        {
            Debug.Log("ROUND " + mevcutRound + " TAMAMLANDI!");
            if (mevcutRound >= toplamRoundSayisi)
            {
                Debug.Log("TÜM DÜÞMANLAR YENÝLDÝ! Sandýk kilidi açýldý.");
                tumRoundlarBitti = true;
            }
            else
            {
                Invoke("RounduBaslat", roundArasiBekleme);
            }
        }
    }

    void RounduBaslat()
    {
        // (Burasý ayný kaldý)
        mevcutRound++;
        if (mevcutRound == 1)
        {
            spawnEdilecekSayi = Random.Range(minSpawnSayisi, maxSpawnSayisi + 1);
        }
        else
        {
            spawnEdilecekSayi += Random.Range(minArtis, maxArtis + 1);
        }

        Debug.Log("ROUND " + mevcutRound + " BAÞLIYOR! (RASTGELE " + spawnEdilecekSayi + " düþman spawn edilecek)");

        hayattakiDusmanSayisi = spawnEdilecekSayi;
        for (int i = 0; i < spawnEdilecekSayi; i++)
        {
            SpawnEt();
        }
    }

    // --- BU FONKSÝYON TAMAMEN GÜNCELLENDÝ ---
    void SpawnEt()
    {
        if (dusmanPrefab == null || spawnAlaniTrigger == null) return;

        // 1. Trigger'ýn (BoxCollider) sýnýrlarýný al
        Bounds triggerBounds = spawnAlaniTrigger.bounds;

        // 2. O kutunun sýnýrlarý içinde rastgele bir X ve Z noktasý seç
        float randomX = Random.Range(triggerBounds.min.x, triggerBounds.max.x);
        float randomZ = Random.Range(triggerBounds.min.z, triggerBounds.max.z);

        // Y (yükseklik) olarak trigger'ýn zeminini al (merkezi - yarým yüksekliði)
        Vector3 rastgeleNokta = new Vector3(randomX, triggerBounds.center.y - triggerBounds.extents.y, randomZ);

        // 3. Bu noktanýn NavMesh üzerinde (duvar içinde deðil) olduðundan emin ol
        NavMeshHit hit;

        // 'rastgeleNokta'ya en yakýn (3m içinde) yürünebilir noktayý bul
        if (NavMesh.SamplePosition(rastgeleNokta, out hit, 3.0f, NavMesh.AllAreas))
        {
            // Geçerli bir nokta bulundu! Düþmaný oraya at.
            Instantiate(dusmanPrefab, hit.position, Quaternion.identity);
        }
        else
        {
            // Eðer o rastgele nokta (ve 3m çevresi) yürünebilir deðilse, tekrar dene (veya uyarý ver)
            Debug.LogWarning("Geçerli bir spawn noktasý bulunamadý. Trigger'ýn NavMesh üzerinde olduðundan emin ol.");
            // (Ýleri seviye: Burada 'SpawnEt' fonksiyonunu tekrar çaðýrabilirsin ama þimdilik gerek yok)
        }
    }
}