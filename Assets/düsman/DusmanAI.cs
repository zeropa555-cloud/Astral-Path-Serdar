using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class DusmanAI : MonoBehaviour
{
    [Header("Referanslar")]
    private NavMeshAgent agent;
    private Animator animator;
    [Tooltip("Düşmanın kovalayacağı ve saldıracağı oyuncu")]
    public Transform player;

    [Header("Devriye Ayarları")]
    public float devriyeYaricapi = 20f;

    [Header("Takip & Saldırı Ayarları")]
    public float gorusMesafesi = 15f;
    public float gorusAcisi = 90f;
    public float saldiriMesafesi = 2f;
    public float saldiriCooldown = 1.5f;
    public float saldiriHasari = 10f;

    private enum Durum { Devriye, Takip, Saldiri }
    private Durum mevcutDurum;
    private bool hayatta = true;
    private float sonSaldiriZamani = -99f;

    [Header("Optimizasyon")]
    public LayerMask gorusLayerMask;


    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        if (player == null)
        {
            try { player = GameObject.FindGameObjectWithTag("Player").transform; }
            catch (System.Exception) { Debug.LogError("SAHİP: Oyuncu bulunamadı."); hayatta = false; }
        }
        mevcutDurum = Durum.Devriye;
        YeniHedefBul();
    }

    void Update()
    {
        if (!hayatta || player == null) return;

        switch (mevcutDurum)
        {
            case Durum.Devriye: DevriyeDavranisi(); break;
            case Durum.Takip: TakipDavranisi(); break;
            case Durum.Saldiri: SaldiriDavranisi(); break;
        }

        animator.SetFloat("Speed", agent.velocity.magnitude);
    }

    void DevriyeDavranisi()
    {
        if (OyuncuyuGorebiliyorMu())
        {
            mevcutDurum = Durum.Takip;
            agent.isStopped = false;
            return;
        }

        if (!agent.pathPending && agent.remainingDistance < agent.stoppingDistance)
        {
            YeniHedefBul();
        }
    }

    void TakipDavranisi()
    {
        if (!OyuncuyuGorebiliyorMu())
        {
            mevcutDurum = Durum.Devriye;
            return;
        }

        agent.SetDestination(player.position);
        float mesafe = Vector3.Distance(transform.position, player.position);

        if (mesafe <= saldiriMesafesi)
        {
            mevcutDurum = Durum.Saldiri;
            agent.isStopped = true;
        }
    }

    void SaldiriDavranisi()
    {
        agent.isStopped = true;

        // 1. KONTROL: Oyuncu tamamen kayıp mı (veya öldü mü)?
        if (!OyuncuyuGorebiliyorMu())
        {
            mevcutDurum = Durum.Devriye;
            agent.isStopped = false;
            return;
        }

        float mesafe = Vector3.Distance(transform.position, player.position);

        // ----- 2. YENİ EKLENEN KONTROL (HATA DÜZELTMESİ) -----
        // Oyuncu saldırı menzilinden kaçtı mı?
        if (mesafe > saldiriMesafesi)
        {
            // Evet, kaçtı. Tekrar "Takip" durumuna dön.
            mevcutDurum = Durum.Takip;
            agent.isStopped = false;
            return; // Bu frame'de başka bir şey yapma
        }
        // ----------------------------------------------------

        // 3. KONTROL: Oyuncu hâlâ menzildeyse, ona dön
        Vector3 bakisYonu = (player.position - transform.position).normalized;
        Quaternion bakisRotasyonu = Quaternion.LookRotation(new Vector3(bakisYonu.x, 0, bakisYonu.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, bakisRotasyonu, Time.deltaTime * 5f);

        // 4. KONTROL: Saldırı süresi dolduysa saldır
        if (Time.time > sonSaldiriZamani + saldiriCooldown)
        {
            animator.SetTrigger("Attack");
            sonSaldiriZamani = Time.time;

            KarakterCanSistemi oyuncuCan = player.GetComponent<KarakterCanSistemi>();
            if (oyuncuCan != null)
            {
                oyuncuCan.HasarAl(saldiriHasari);
            }
        }
    }

    // --- (Diğer fonksiyonlar: OyuncuyuGorebiliyorMu, YeniHedefBul, OlumAnimasyonunuBaslat) ---
    // Bu fonksiyonlar bir öncekiyle aynı kalabilir.

    bool OyuncuyuGorebiliyorMu()
    {
        if (player == null) return false;
        float mesafe = Vector3.Distance(transform.position, player.position);
        if (mesafe > gorusMesafesi) return false;
        Vector3 yonToPlayer = (player.position - transform.position).normalized;
        float aci = Vector3.Angle(transform.forward, yonToPlayer);
        if (aci > gorusAcisi / 2) return false;
        RaycastHit hit;
        Vector3 gozHizasi = transform.position + Vector3.up * 1.5f;
        if (Physics.Raycast(gozHizasi, yonToPlayer, out hit, gorusMesafesi, gorusLayerMask))
        {
            if (hit.transform == player) return true;
        }
        return false;
    }

    void YeniHedefBul()
    {
        Vector3 rastgeleYon = Random.insideUnitSphere * devriyeYaricapi;
        rastgeleYon += transform.position;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(rastgeleYon, out hit, devriyeYaricapi, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
        else { Debug.LogWarning(gameObject.name + " NavMesh üzerinde yeni bir hedef bulamadı!"); }
    }

    public void OlumAnimasyonunuBaslat()
    {
        if (!hayatta) return;
        hayatta = false;
        agent.isStopped = true;
        agent.velocity = Vector3.zero;
        animator.SetTrigger("Die");
        this.enabled = false;
    }
}