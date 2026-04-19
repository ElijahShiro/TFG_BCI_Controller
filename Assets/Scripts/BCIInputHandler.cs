using EmotivUnityPlugin;
using System.Collections.Generic;
using UnityEngine;

public class BCIInputHandler : MonoBehaviour
{
    [Header("Giroscopio (Full BCI Mode)")]
    public float gyroDeadzone = 0.15f;
    public float gyroSensitivity = 2.5f;


[Header("Monitor BCI en Vivo (Solo lectura)")]
    public string CurrentAction;
    public float CurrentPower;
    public Vector2 HeadMove;
    public bool IsJumpTriggered;
    public bool IsUsingHeadMovement;
    
 [Header ("Config MentalPow")]
 public float mentalPowerThreshhold = 0.3f;

    private InputState _state = new InputState();
    private Dictionary<GameModeManager.ControlMode, IControlStrategy> _strategies;
    private bool _wasMentalCommandActiveLastFrame = false;

    void Start()
    {
        _strategies = new Dictionary<GameModeManager.ControlMode, IControlStrategy>
        {
            { GameModeManager.ControlMode.Teclado, new KeyboardStrategy() },
            { GameModeManager.ControlMode.Hibrido, new HybridStrategy() },
            { GameModeManager.ControlMode.Full_BCI, new FullBCIStrategy() }
        };
    }

    void Update()
    {
        ProcessInputs();
    }

    private void ProcessInputs()
    {
        _state.Action = "";
        _state.Power = 0f;
        _state.HeadMove = Vector2.zero;
        _state.IsJumpTriggered = false;
        _state.IsUsingHeadMovement = false;

        GameModeManager.ControlMode mode = GameModeManager.Instance != null ? GameModeManager.Instance.currentMode : GameModeManager.ControlMode.Hibrido;
        bool isGymScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.StartsWith("Gym");
        if (isGymScene && mode == GameModeManager.ControlMode.Full_BCI) mode = GameModeManager.ControlMode.Hibrido;


        if (_strategies.ContainsKey(mode))
        {
            _strategies[mode].ReadInput(_state, this);
        }

        CurrentAction = _state.Action;
        CurrentPower = _state.Power;
        HeadMove = _state.HeadMove;
        IsJumpTriggered = _state.IsJumpTriggered;
        IsUsingHeadMovement = _state.IsUsingHeadMovement;

        bool isMentalCommandActive = _state.Power >= mentalPowerThreshhold && (_state.Action == "push" || _state.Action == "pull" || _state.Action == "jump");

        if (isMentalCommandActive && !_wasMentalCommandActiveLastFrame)
        {
            if (GameMetricsLogger.Instance != null) GameMetricsLogger.Instance.StartMentalActivation(_state.Action);
        }
        else if (!isMentalCommandActive && _wasMentalCommandActiveLastFrame)
        {
            if (GameMetricsLogger.Instance != null) GameMetricsLogger.Instance.StopMentalActivation();
        }
        _wasMentalCommandActiveLastFrame = isMentalCommandActive;
    }
}