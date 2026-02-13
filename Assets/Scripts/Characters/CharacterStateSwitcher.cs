using UnityEngine;

public class CharacterStateSwitcher : MonoBehaviour
{
    public enum CharacterState
    {
        ThreeD,
        TwoD
    }

    public CharacterState CurrentState { get; private set; }  = CharacterState.TwoD;

    private DimensionStateSwitcher dimensionSwitcher;

    void Start()
    {
        dimensionSwitcher = GameObject.FindWithTag("Dimension").GetComponent<DimensionStateSwitcher>();
    }

    void Update()
    {
        if (dimensionSwitcher == null) return;

        CurrentState = dimensionSwitcher.CurrentState == DimensionStateSwitcher.DimensionState.ThreeD
            ? CharacterState.ThreeD
            : CharacterState.TwoD;
    }
}
