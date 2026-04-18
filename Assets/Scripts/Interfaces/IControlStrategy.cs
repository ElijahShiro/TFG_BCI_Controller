using UnityEngine.InputSystem.LowLevel;

public interface IControlStrategy
{
    void ReadInput(InputState state, BCIInputHandler context);
}