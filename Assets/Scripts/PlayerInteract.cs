using UnityEngine;
using UnityEngine.InputSystem; // Input Sistemi icin SART!

public class PlayerInteract : MonoBehaviour
{
    // O an etkilesime girebilecegimiz NPC'yi burada tutacagiz
    private NPCDialogue anlikNPC;

    void Update()
    {
        // Eger 'E' tusuna basildiysa VE etkilesim alaninda bir NPC varsa
        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame && anlikNPC != null)
        {
            // O NPC'nin konusmasini tetikle
            anlikNPC.KonusmayiTetikle();
        }
    }

    // Oyuncunun etkilesim alanina (Trigger) bir sey girdiginde calisir
    private void OnTriggerEnter(Collider other)
    {
        // Giren seyin "NPCDialogue" script'i var mi?
        NPCDialogue npc = other.GetComponent<NPCDialogue>();
        if (npc != null)
        {
            // EVET: Bu, konusulabilir bir NPC.
            Debug.Log("Konusulabilir NPC alana girdi: " + other.name);
            anlikNPC = npc; // Onu "anlikNPC" olarak kaydet
        }
    }

    // Oyuncunun etkilesim alanindan (Trigger) bir sey ciktiginda calisir
    private void OnTriggerExit(Collider other)
    {
        // Cikan sey, bizim o an konustugumuz NPC mi?
        if (anlikNPC != null && other.gameObject == anlikNPC.gameObject)
        {
            Debug.Log("Konusulabilir NPC alandan cikti.");
            anlikNPC = null; // Kaydi sil
        }
    }
}