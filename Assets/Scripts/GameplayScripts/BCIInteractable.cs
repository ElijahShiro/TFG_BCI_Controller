using UnityEngine;
using UnityEngine.Events;

public class BCIInteractable : MonoBehaviour
{
    [Header("Configuración BCI")]
    [Tooltip("El comando que activa este objeto (ej: push, pull, lift)")]
    public string targetCommand = "push";
    [Tooltip("Poder mental necesario para activarlo (0.0 a 1.0)")]
    public float activationThreshold = 0.5f;

    [Header("Eventos de Mirada (Visuales)")]
    public UnityEvent OnFocusEnter;
    public UnityEvent OnFocusExit;

    [Header("Eventos de Poder Mental (Acción)")]
    public UnityEvent OnMentalActivationStart;
    public UnityEvent OnMentalActivationEnd;

    private bool _isActivated = false;


    public void SetFocus(bool isFocused)
    {
        if (isFocused)
        {
            OnFocusEnter?.Invoke();
        }
        else
        {
            OnFocusExit?.Invoke();

            if (_isActivated)
            {
                _isActivated = false;
                OnMentalActivationEnd?.Invoke();
            }
        }
    }


    public void ReceiveMentalData(string currentCommand, float currentPower)
    {

        if (currentCommand.ToLower() == targetCommand.ToLower() && currentPower >= activationThreshold)
        {
            if (!_isActivated)
            {
                _isActivated = true;
                if (GameMetricsLogger.Instance != null) GameMetricsLogger.Instance.LogMentalActivationStart();
                OnMentalActivationStart?.Invoke();
            }
        }
        else 
        {
            if (_isActivated)
            {
                _isActivated = false;
                OnMentalActivationEnd?.Invoke();
            }
        }
    }
}