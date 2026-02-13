using UnityEngine;

public class CameraStateManager : MonoBehaviour
{
    [Header("References")]
    public GameObject corridor;

    // Rig tranforms
    private Transform cameraRoot;
    private Transform cameraPivot;
    private Transform mainCamera;

    // Target anchor transforms found from Corridor's children
    private Transform pos3D;
    private Transform pos2D;

    // The switcher on this same GmeObject
    private CameraStateSwitcher stateSwitcher;

    // Transition config
    private const float TransitionDuration = 0.2f;
    private float transitionTimer = 0f;
    private bool isTransitioning = false;

    // Snapshots at the start of each transition
    private Vector3 startRootPos;
    private Quaternion startPivotRot;
    private Quaternion startCamRot;

    private Vector3 targetRootPos;
    private Quaternion targetPivotRot;
    private Quaternion targetCamRot;

    // Track which state we transitioned to last
    private CameraStateSwitcher.CameraState lastKnownState;

    // Camera
    private Camera cam;

    void Awake()
    {
        // Find the rig – MainCamera is this object, walk up
        mainCamera = transform;
        cameraPivot = mainCamera.parent;
        cameraRoot = cameraPivot.parent;

        stateSwitcher = GetComponent<CameraStateSwitcher>();

        cam = mainCamera.GetComponent<Camera>();
        if (stateSwitcher == null)
            Debug.LogError("CameraStateManager: No CameraStateSwitcher found on this GameObject!");

        // Find position anchors inside Corridor by name
        if (corridor != null)
        {
            Transform t3D = corridor.transform.Find("3DCameraPos");
            Transform t2D = corridor.transform.Find("2DCameraPos");

            if (t3D == null) Debug.LogError("CameraStateManager: Could not find child '3DCameraPos' on Corridor.");
            if (t2D == null) Debug.LogError("CameraStateManager: Could not find child '2DCameraPos' on Corridor.");

            pos3D = t3D;
            pos2D = t2D;
        }
        else
        {
            Debug.LogError("CameraStateManager: Corridor reference is not assigned!");
        }

        lastKnownState = stateSwitcher != null ? stateSwitcher.CurrentState : CameraStateSwitcher.CameraState.ThreeD;
    }

    void Update()
    {
        if (stateSwitcher == null) return;

        // State change is there
        if (stateSwitcher.CurrentState != lastKnownState)
        {
            lastKnownState = stateSwitcher.CurrentState;
            BeginTransition(lastKnownState);
        }

        // Tick the transition
        if (isTransitioning)
        {
            transitionTimer += Time.deltaTime;
            float t = Mathf.Clamp01(transitionTimer / TransitionDuration);

            // Smooth-step for a snappier feel
            float st = t * t * (3f - 2f * t);

            cameraRoot.position = Vector3.Lerp(startRootPos, targetRootPos, st);
            cameraPivot.rotation = Quaternion.Slerp(startPivotRot, targetPivotRot, st);
            //mainCamera.localRotation = Quaternion.Slerp(startCamRot, targetCamRot, st);

            if (t >= 1f)
                isTransitioning = false;
        }
    }

    private void BeginTransition(CameraStateSwitcher.CameraState newState)
    {
        Transform target = newState == CameraStateSwitcher.CameraState.ThreeD ? pos3D : pos2D;
        if (target == null) return;

        // Snapshot current state
        startRootPos = cameraRoot.position;
        startPivotRot = cameraPivot.rotation;
        startCamRot = mainCamera.localRotation;

        // The anchor's world position drives CameraRoot; its rotation drives CameraPivot.
        // MainCamera local rotation resets to identity so the anchor rotation is expressed
        // purely through the pivot.
        targetRootPos = target.position;
        targetPivotRot = target.rotation;
        targetCamRot = Quaternion.identity;

        cam.orthographic = (newState == CameraStateSwitcher.CameraState.TwoD);

        transitionTimer = 0f;
        isTransitioning = true;
    }
}