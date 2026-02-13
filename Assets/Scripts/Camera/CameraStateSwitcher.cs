using UnityEngine;

public class CameraStateSwitcher : MonoBehaviour
{
    public enum CameraState { ThreeD, TwoD }
    public CameraState CurrentState { get; private set; } = CameraState.TwoD;

    private DimensionStateSwitcher dimensionSwitcher;

    void Awake()
    {
        dimensionSwitcher = GameObject.FindWithTag("Dimension").GetComponent<DimensionStateSwitcher>();
    }

    void Update()
    {
        if (dimensionSwitcher == null) return;

        CurrentState = dimensionSwitcher.CurrentState == DimensionStateSwitcher.DimensionState.ThreeD
            ? CameraState.ThreeD
            : CameraState.TwoD;
    }
}