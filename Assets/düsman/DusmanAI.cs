using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class DusmanAI : MonoBehaviour
{
    [Header("Referanslar")]
    private NavMeshAgent agent;
    private Animator animator;
    public Transform player; // Player'ı otomatik bulur ama Inspector'dan da bakabilirsin

    [Header("Ayarlar")]
    public float devriyeYaricapi = 20f;
    public float gorusMesafesi = 15f;
    public float saldiriMesafesi = 2f;
    public float saldiriCooldown = 1.5f;
    public float saldiriHasari = 10f;

    [Header("Animasyon Ayarları")]
    public string hasarTriggerAdi = "Hit";
    public float hasarAnimasyonSuresi = 0.5f; // Düşman hasar alınca kaç sn dursun?

    private bool hayatta = true;
    private float sonSaldiriZamani = -99f;
    public LayerMask gorusLayerMask;
    private KarakterCanSistemi oyuncuCanSistemi;

    // --- YENİ EKLENEN: Hasar Kilidi ---
    private bool hasarAnimasyonunda = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        // Player'ı bulma (Otomatik)
        if (player == null)
        {
            try { player = GameObject.FindGameObjectWithTag("Player").transform; }
            catch { hayatta = false; }
        }

        if (player != null) oyuncuCanSistemi = player.GetComponent<KarakterCanSistemi>();

        YeniHedefBul();
    }

    void Update()
    {
        if (!hayatta || player == null) return;

        // Eğer hasar animasyonu oynuyorsa (donduysa) hiçbir şey yapma
        if (hasarAnimasyonunda) return;

        // Oyuncu öldüyse dur
        if (oyuncuCanSistemi != null && !oyuncuCanSistemi.hayattaMi)
        {
            agent.isStopped = true;
            animator.SetFloat("Speed", 0f);
            return;
        }

        // Basit Takip Mantığı (Önceki karmaşık State Machine yerine en garantisi)
        float mesafe = Vector3.Distance(transform.position, player.position);

        // 1. Saldırı Mesafesindeyse -> SALDIR
        if (mesafe <= saldiriMesafesi)
        {
            agent.isStopped = true;
            animator.SetFloat("Speed", 0f);
            SaldiriDavranisi();
        }
        // 2. Görüş Mesafesindeyse -> KOVALA
        else if (mesafe <= gorusMesafesi)
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);
            animator.SetFloat("Speed", agent.velocity.magnitude);
        }
        // 3. Uzaktaysa -> DEVRİYE (veya bekle)
        else
        {
            if (!agent.pathPending && agent.remainingDistance < agent.stoppingDistance)
            {
                YeniHedefBul();
            }
            animator.SetFloat("Speed", agent.velocity.magnitude);
        }
    }

    void SaldiriDavranisi()
    {
        // Oyuncuya dön
        Vector3 bakisYonu = (player.position - transform.position).normalized;
        if (bakisYonu != Vector3.zero)
        {
            Quaternion rot = Quaternion.LookRotation(new Vector3(bakisYonu.x, 0, bakisYonu.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * 5f);
        }

        if (Time.time > sonSaldiriZamani + saldiriCooldown)
        {
            animator.SetTrigger("Attack");
            sonSaldiriZamani = Time.time;
            if (oyuncuCanSistemi != null) oyuncuCanSistemi.HasarAl(saldiriHasari);
        }
    }

    void YeniHedefBul()
    {
        Vector3 randomPoint = transform.position + Random.insideUnitSphere * devriyeYaricapi;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, 10f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }

    // --- ÖNEMLİ: CAN SİSTEMİNDEN ÇAĞRILAN FONKSİYON ---
    public void HasarAnimasyonunuBaslat()
    {
        if (hayatta)
        {
            // Eğer zaten "Ah!" diyorsa tekrar başlatma (titremesin)
            if (!hasarAnimasyonunda)
            {
                StartCoroutine(HasarAlVeDur());
            }
        }
    }

    System.Collections.IEnumerator HasarAlVeDur()
    {
        hasarAnimasyonunda = true; // Kilidi aç

        // 1. Dur
        agent.isStopped = true;
        agent.velocity = Vector3.zero;

        // 2. Animasyonu Oynat
        animator.SetTrigger(hasarTriggerAdi);

        // 3. Bekle (Animasyon süresi kadar)
        yield return new WaitForSeconds(hasarAnimasyonSuresi);

        // 4. Devam Et
        if (hayatta)
        {
            agent.isStopped = false;
            hasarAnimasyonunda = false; // Kilidi kapat
        }
    }

    public void OlumAnimasyonunuBaslat()
    {
        if (!hayatta) return;
        hayatta = false;
        agent.isStopped = true;
        agent.enabled = false; // Agent'ı kapat
        animator.SetTrigger("Die");
        this.enabled = false; // Scripti kapat
    }
}