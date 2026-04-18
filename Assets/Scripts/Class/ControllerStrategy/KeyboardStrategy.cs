using UnityEngine;


public class KeyboardStrategy : IControlStrategy
{
    public void ReadInput(InputState state, BCIInputHandler context)
    {
        if (Input.GetKey(KeyCode.E)) { state.Action = "push"; state.Power = 1f; }
        if (Input.GetKey(KeyCode.Space)) { state.Action = "jump"; state.Power = 1f; state.IsJumpTriggered = true; }
    }
}