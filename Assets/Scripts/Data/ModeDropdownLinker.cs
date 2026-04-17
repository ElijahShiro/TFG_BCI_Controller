using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_Dropdown))]
public class ModeDropdownLinker : MonoBehaviour
{
    private TMP_Dropdown _dropdown;

    private void Start()
    {
        _dropdown = GetComponent<TMP_Dropdown>();


        if (GameModeManager.Instance != null)
        {
            _dropdown.SetValueWithoutNotify((int)GameModeManager.Instance.currentMode);
        }

        _dropdown.onValueChanged.AddListener(OnDropdownChanged);
    }

    private void OnDropdownChanged(int newIndex)
    {
        if (GameModeManager.Instance != null)
        {
            GameModeManager.Instance.SetControlMode(newIndex);
        }
        else
        {
            Debug.LogError("Error: No se encontró el GameModeManager.");
        }
    }

    private void OnDestroy()
    {

        if (_dropdown != null)
        {
            _dropdown.onValueChanged.RemoveListener(OnDropdownChanged);
        }
    }
}