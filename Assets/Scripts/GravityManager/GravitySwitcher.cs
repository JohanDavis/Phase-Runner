using UnityEngine;

public class GravitySwitcher : MonoBehaviour
{
    private DimensionStateSwitcher dimensionStateSwitcher;
    private DimensionStateSwitcher.DimensionState lastKnownState;

    void Start()
    {
        dimensionStateSwitcher = GameObject.FindWithTag("Dimension").GetComponent<DimensionStateSwitcher>();
        lastKnownState = dimensionStateSwitcher.CurrentState;
        ApplyGravity(lastKnownState);
    }

    void Update()
    {
        if (dimensionStateSwitcher.CurrentState == lastKnownState) return;

        lastKnownState = dimensionStateSwitcher.CurrentState;
        ApplyGravity(lastKnownState);
    }

    private void ApplyGravity(DimensionStateSwitcher.DimensionState state)
    {
        if (state == DimensionStateSwitcher.DimensionState.TwoD)
            Physics.gravity = new Vector3(0f, -9.81f, 0f);
        else
            Physics.gravity = new Vector3(9.81f, 0f, 0f);
    }
}