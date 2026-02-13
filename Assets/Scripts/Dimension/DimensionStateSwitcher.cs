using UnityEngine;
using UnityEngine.InputSystem;

public class DimensionStateSwitcher : MonoBehaviour
{
    public enum DimensionState
    {
        ThreeD,
        TwoD
    }

    public DimensionState CurrentState { get; private set; } = DimensionState.TwoD;

    void Update()
    {
        if (Keyboard.current.mKey.wasPressedThisFrame)
        {
            CurrentState = CurrentState == DimensionState.ThreeD
                ? DimensionState.TwoD
                : DimensionState.ThreeD;
        }
    }
}
