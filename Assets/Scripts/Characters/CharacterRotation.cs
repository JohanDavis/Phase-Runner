using UnityEngine;

public class CharacterRotation : MonoBehaviour
{
    [Header("Rotation Settings")]
    public float rotationDuration = 0.3f;

    private CharacterStateSwitcher stateSwitcher;

    private Quaternion targetRotation;
    private Quaternion startRotation;
    private float rotationTimer = 0f;
    private bool isRotating = false;

    private CharacterStateSwitcher.CharacterState lastKnownState;

    private static readonly Quaternion Rotation2D = Quaternion.Euler(0f, 0f, 0f);
    private static readonly Quaternion Rotation3D = Quaternion.Euler(0f, 0f, 90f);

    void Awake()
    {
        stateSwitcher = GetComponent<CharacterStateSwitcher>();
    }

    void Start()
    {
        lastKnownState = stateSwitcher.CurrentState;
        transform.rotation = stateSwitcher.CurrentState == CharacterStateSwitcher.CharacterState.TwoD
            ? Rotation2D
            : Rotation3D;
    }

    void Update()
    {
        if (stateSwitcher.CurrentState != lastKnownState)
        {
            lastKnownState = stateSwitcher.CurrentState;

            startRotation = transform.rotation;
            targetRotation = lastKnownState == CharacterStateSwitcher.CharacterState.TwoD
                ? Rotation2D
                : Rotation3D;

            rotationTimer = 0f;
            isRotating = true;
        }

        if (isRotating)
        {
            rotationTimer += Time.deltaTime;
            float t = Mathf.Clamp01(rotationTimer / rotationDuration);
            float st = t * t * (3f - 2f * t); // smoothstep

            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, st);

            if (t >= 1f)
            {
                transform.rotation = targetRotation;
                isRotating = false;
            }
        }
    }
}