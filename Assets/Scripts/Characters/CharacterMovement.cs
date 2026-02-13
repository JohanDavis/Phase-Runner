using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class CharacterMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 7f;
    public float lerpToDuration = 0.3f;
    public float deceleration = 20f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.1f;
    [SerializeField] private LayerMask floorLayer;

    private Rigidbody rb;
    private CharacterStateSwitcher stateSwitcher;

    private bool jumpPressed;

    // 2D lane lerp
    private const float TargetXIn2D = -3f;
    private bool isLerping = false;
    private float lerpTimer = 0f;
    private float lerpStartX;
    private bool lockX = false;

    private CharacterStateSwitcher.CharacterState lastKnownState;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        stateSwitcher = GetComponent<CharacterStateSwitcher>();
    }

    void Start()
    {
        lastKnownState = stateSwitcher.CurrentState;
    }

    void Update()
    {
        if (stateSwitcher.CurrentState != lastKnownState)
        {
            lastKnownState = stateSwitcher.CurrentState;

            if (lastKnownState == CharacterStateSwitcher.CharacterState.TwoD)
            {
                lerpStartX = transform.position.x;
                lerpTimer = 0f;
                isLerping = true;
                lockX = false;
            }
            else
            {
                lockX = false;
            }
        }

        if (isLerping)
        {
            lerpTimer += Time.deltaTime;
            float t = Mathf.Clamp01(lerpTimer / lerpToDuration);
            float st = t * t * (3f - 2f * t);

            Vector3 pos = transform.position;
            pos.x = Mathf.Lerp(lerpStartX, TargetXIn2D, st);
            transform.position = pos;

            if (t >= 1f)
            {
                isLerping = false;
                lockX = true;
            }
        }

        if (lockX)
        {
            Vector3 pos = transform.position;
            pos.x = TargetXIn2D;
            transform.position = pos;
        }
    }

    void FixedUpdate()
    {
        if (isLerping) return;

        if (stateSwitcher.CurrentState == CharacterStateSwitcher.CharacterState.TwoD)
            Handle2DMovement();
        else
            Handle3DMovement();

        if (jumpPressed)
        {
            jumpPressed = false;
            if (IsGrounded()) ApplyJump();
        }
    }

    private void Handle2DMovement()
    {
        float zDir = 0f;
        if (Keyboard.current.aKey.isPressed) zDir = 1f;
        if (Keyboard.current.dKey.isPressed) zDir = -1f;

        Vector3 velocity = rb.linearVelocity;
        velocity.z = Mathf.MoveTowards(velocity.z, zDir * moveSpeed, deceleration * Time.fixedDeltaTime);
        rb.linearVelocity = velocity;
    }

    private void Handle3DMovement()
    {
        float yDir = 0f;
        float zDir = 0f;

        if (Keyboard.current.wKey.isPressed) yDir = 1f;
        if (Keyboard.current.sKey.isPressed) yDir = -1f;
        if (Keyboard.current.aKey.isPressed) zDir = 1f;
        if (Keyboard.current.dKey.isPressed) zDir = -1f;

        Vector3 velocity = rb.linearVelocity;
        velocity.y = Mathf.MoveTowards(velocity.y, yDir * moveSpeed, deceleration * Time.fixedDeltaTime);
        velocity.z = Mathf.MoveTowards(velocity.z, zDir * moveSpeed, deceleration * Time.fixedDeltaTime);
        rb.linearVelocity = velocity;
    }

    private void ApplyJump()
    {
        if (stateSwitcher.CurrentState == CharacterStateSwitcher.CharacterState.TwoD)
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        else
            rb.AddForce(Vector3.left * jumpForce, ForceMode.Impulse);
    }

    void LateUpdate()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
            jumpPressed = true;
    }

    private bool IsGrounded()
    {
        return Physics.CheckSphere(groundCheck.position, groundCheckRadius, floorLayer);
    }
}