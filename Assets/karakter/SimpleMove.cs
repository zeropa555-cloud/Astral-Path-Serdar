using UnityEngine;
using UnityEngine.InputSystem;

public class SimpleMove : MonoBehaviour
{
    [Header("Hızlar")]
    public float walkSpeed = 3.5f;
    public float runSpeed  = 6.5f;
    public float rotationSpeed = 500f;

    [Header("Zıplama")]
    public float jumpForce = 7f;
    public float gravity   = 20f;
    [Tooltip("Jump klibinde takeoff anına denk gelen normalizedTime (0-1). Örn: 0.15")]
    public float takeoffNormalizedTime = 0.5f;

    [Header("Kamera")]
    public Transform cameraTransform;

    CharacterController controller;
    Animator animator;
    Vector3 horizontalVel;
    float verticalVel;
    bool jumpQueued;      // animasyon state'ine girildi, takeoff bekleniyor mu?
    bool hasTakenOff;     // takeoff uygulandı mı?

    static readonly int HashSpeed = Animator.StringToHash("Speed");
    static readonly int HashJump  = Animator.StringToHash("Jump");

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator   = GetComponentInChildren<Animator>();
        if (cameraTransform == null) cameraTransform = Camera.main.transform;
        if (animator != null) animator.applyRootMotion = false;
    }

    void Update()
    {
        var kb = Keyboard.current; if (kb == null) return;

        // --- Input ---
        float h = (kb.aKey.isPressed || kb.leftArrowKey.isPressed)  ? -1f :
                  (kb.dKey.isPressed || kb.rightArrowKey.isPressed) ?  1f : 0f;
        float v = (kb.sKey.isPressed || kb.downArrowKey.isPressed)  ? -1f :
                  (kb.wKey.isPressed || kb.upArrowKey.isPressed)    ?  1f : 0f;

        Vector3 input = new Vector3(h, 0f, v).normalized;
        bool isMoving  = input.sqrMagnitude > 0.0001f;
        bool isRunning = isMoving && kb.leftShiftKey.isPressed;

        // --- Kameraya göre yön ---
        Vector3 camF = cameraTransform.forward; camF.y = 0; camF.Normalize();
        Vector3 camR = cameraTransform.right;   camR.y = 0; camR.Normalize();
        Vector3 moveDir = (camF * input.z + camR * input.x).normalized;

        float worldSpeed = isRunning ? runSpeed : walkSpeed;
        horizontalVel = moveDir * worldSpeed;

        // --- Space: Jump tetikle (yalnızca yerdeyken) ---
        if (controller.isGrounded && kb.spaceKey.wasPressedThisFrame && !jumpQueued)
        {
            animator.SetTrigger(HashJump);  // animasyonu başlat
            jumpQueued  = true;             // state'e girince takeoff bekle
            hasTakenOff = false;
        }

        // --- Animasyonla senkron takeoff ---
        if (jumpQueued)
        {
            var st = animator.GetCurrentAnimatorStateInfo(0);
            // "Jump" state adına kendi klibinin adını yazabilirsin (Contains kullanıyoruz esnek olsun diye)
            if (st.IsTag("Jump") || st.IsName("Jump") || st.shortNameHash != 0)
            {
                if (!hasTakenOff && st.normalizedTime >= takeoffNormalizedTime)
                {
                    verticalVel = jumpForce; // tam bu anda fiziksel zıplamayı uygula
                    hasTakenOff = true;
                    jumpQueued  = false;
                }
            }
        }

        // --- Yerçekimi / yere basma ---
        if (controller.isGrounded && verticalVel <= 0f)
            verticalVel = -1f; // yere sabitle
        else
            verticalVel -= gravity * Time.deltaTime;

        // --- Hareket uygula ---
        Vector3 final = horizontalVel; final.y = verticalVel;
        controller.Move(final * Time.deltaTime);

        // --- Yönünü çevir ---
        if (isMoving)
        {
            Quaternion target = Quaternion.LookRotation(moveDir, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, target, rotationSpeed * Time.deltaTime);
        }

        // --- Animator Speed (Idle/Walk/Run için) ---
        float targetAnimSpeed = 0f;
        if (isMoving) targetAnimSpeed = isRunning ? 1f : 0.5f;
        float current = animator.GetFloat(HashSpeed);
        float smooth  = Mathf.Lerp(current, targetAnimSpeed, 10f * Time.deltaTime);
        animator.SetFloat(HashSpeed, smooth);
    }
}
