using UnityEngine;


public interface IBCIInteractable
{
    
    void OnReceiveCommand(string commandType, float power);

    void ApplyHighlight();
    void RemoveHighlight();
}