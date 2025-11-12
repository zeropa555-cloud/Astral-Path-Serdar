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

    [Header("Kamera")]
    public Transform cameraTransform;

    [Header("Animator Ayarları")]
    [Tooltip("Animator'daki Jump state adın (state adı veya klip adı).")]
    public string jumpStateName = "jumping";

    CharacterController controller;
    Animator animator;
    float verticalVel;

    static readonly int HashSpeed = Animator.StringToHash("Speed");
    static readonly int HashJump  = Animator.StringToHash("Jump");

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator   = GetComponentInChildren<Animator>();

        if (cameraTransform == null) cameraTransform = Camera.main ? Camera.main.transform : null;
        if (animator) animator.applyRootMotion = false;

        verticalVel = 0f;

        if (animator)
        {
            animator.Rebind();
            animator.Update(0f);
            animator.ResetTrigger(HashJump);
        }
    }

    void Update()
    {
        var kb = Keyboard.current;
        if (kb == null || controller == null || cameraTransform == null) return;

        // --- 1) Girdi ---
        float h = (kb.aKey.isPressed || kb.leftArrowKey.isPressed)  ? -1f :
                  (kb.dKey.isPressed || kb.rightArrowKey.isPressed) ?  1f : 0f;
        float v = (kb.sKey.isPressed || kb.downArrowKey.isPressed)  ? -1f :
                  (kb.wKey.isPressed || kb.upArrowKey.isPressed)    ?  1f : 0f;

        Vector3 input = new Vector3(h, 0f, v).normalized;
        bool isMoving  = input.sqrMagnitude > 0.0001f;
        bool isRunning = isMoving && kb.leftShiftKey.isPressed;

        // --- 2) Kamera yönü ---
        Vector3 camF = cameraTransform.forward; camF.y = 0f; camF.Normalize();
        Vector3 camR = cameraTransform.right;   camR.y = 0f; camR.Normalize();
        Vector3 moveDir = (camF * input.z + camR * input.x).normalized;

        // --- 3) Hız belirle ---
        float worldSpeed = isRunning ? runSpeed : walkSpeed;
        Vector3 horizontal = moveDir * worldSpeed;

        // Havada yürümeyi kapat
        if (!controller.isGrounded) horizontal = Vector3.zero;

        // --- 4) Zıplama & Yerçekimi ---
        if (controller.isGrounded)
        {
            if (verticalVel <= 0f) verticalVel = -1f;

            if (kb.spaceKey.wasPressedThisFrame)
            {
                // a) Animasyonu AYNI FRAME başlat (gecikmeyi kes)
                if (animator)
                {
                    animator.ResetTrigger(HashJump);
                    // İki seçenekten biri (ikisi de 0 sürede):
                    // animator.CrossFade(jumpStateName, 0f, 0, 0f);
                    animator.Play(jumpStateName, 0, 0f);
                    animator.Update(0f);              // hemen evaluate et
                    animator.SetTrigger(HashJump);    // (opsiyonel) trigger kalsın
                    animator.SetFloat(HashSpeed, 0f); // bu karede Idle hissi
                }

                // b) Fiziksel zıplamayı AYNI FRAME uygula
                verticalVel = jumpForce;

                // c) Yatay hızı ve döndürmeyi bu karede sıfırla (tam senkron görünüm)
                horizontal = Vector3.zero;
                isMoving = false;
            }
        }
        else
        {
            verticalVel -= gravity * Time.deltaTime;
        }

        // --- 5) Hareket uygula ---
        Vector3 final = new Vector3(horizontal.x, verticalVel, horizontal.z);
        controller.Move(final * Time.deltaTime);

        // --- 6) Yönünü çevir (yalnızca yerdeyken) ---
        if (isMoving && controller.isGrounded)
        {
            Quaternion target = Quaternion.LookRotation(moveDir, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, target, rotationSpeed * Time.deltaTime);
        }

        // --- 7) Animator parametreleri ---
        if (animator)
        {
            // Idle=0, Walk=0.5, Run=1.0
            float targetSpeed = (controller.isGrounded && isMoving) ? (isRunning ? 1f : 0.5f) : 0f;
            float current     = animator.GetFloat(HashSpeed);
            float smoothed    = Mathf.Lerp(current, targetSpeed, 10f * Time.deltaTime);
            animator.SetFloat(HashSpeed, smoothed);
        }
    }
}
