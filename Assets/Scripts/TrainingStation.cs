using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using StarterAssets;

public class TrainingStation : MonoBehaviour
{
    [Header("Configuracion de Entrenamiento")]
    public string ActionName = "";
    public int ActionID = 0;

    [Header("Referencias Visuales")]
    public CinemachineVirtualCamera StationCamera;
    public ParticleSystem AuraEffect;
    public Animator PlayerAnimator;

    private bool _playerInside = false;
    private bool _isActive = false;

    private PlayerInput _playerInput;
    private StarterAssetsInputs _mouseInput;
    private CharacterController _characterController;
    private bool _isLocked = false;


    [Header("Position in Station")]
    public Transform PlayerTargetPosition;

    void Start()
    {
        if (StationCamera) StationCamera.Priority = 0;
        if (AuraEffect) AuraEffect.Stop();

        MentalGymTrainer.OnTrainingUpdate += HandleTrainingState;
        if(MentalGymTrainer.Instance != null)
            MentalGymTrainer.Instance.OnResetGameplay += ExitStation;
    }

    private void OnDestroy()
    {
        MentalGymTrainer.OnTrainingUpdate -= HandleTrainingState;
        if(MentalGymTrainer.Instance != null)
            MentalGymTrainer.Instance.OnResetGameplay -= ExitStation;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) 
        {
            _playerInside = true;
            PlayerAnimator = other.GetComponent<Animator>();
            _playerInput = other.GetComponent<UnityEngine.InputSystem.PlayerInput>();
            _mouseInput = other.GetComponent<StarterAssets.StarterAssetsInputs>();

            if (_isActive) return;

            _characterController = other.GetComponent<CharacterController>();
            if (ActionName.Trim().ToLower() != "neutral" && !MentalGymTrainer.Instance.HasTrainedNeutral())
            {
                _isLocked = true;
                GameUIController.Instance.ToggleInteractPrompt(true, "BLOQUEADO: Entrena la estación 'NEUTRAL' primero", true);
            }
            else 
            {
                _isLocked = false;
                GameUIController.Instance.ToggleInteractPrompt(true, $"Pulsa E para entrenar {ActionName.ToUpper()}");
            }
                
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) 
        {
            _playerInside = false;
            if (!_isActive && GameUIController.Instance != null)
                GameUIController.Instance.ToggleInteractPrompt(false);
        }
    }

    void Update()
    {
        if (_playerInside && !_isActive && !_isLocked && Keyboard.current.eKey.wasPressedThisFrame)
        {
            OpenTrainingStation();
        }
    }

    void OpenTrainingStation() 
    {
        _isActive = true;
        GameUIController.Instance.ToggleInteractPrompt(false);

        if (PlayerTargetPosition != null && _characterController != null) 
        { 
            _characterController.enabled = false;

            _characterController.transform.position = PlayerTargetPosition.position;
            _characterController.transform.rotation = PlayerTargetPosition.rotation;
    
            _characterController.enabled = true;
        }

        if (StationCamera) StationCamera.Priority = 20;
        if (_playerInput) _playerInput.DeactivateInput();
        SetMouseState(true);

        GameUIController.Instance.OpenPreTrainingMenu(this);

    }

    public void StartTrainingVisuals()
    {
        SetMouseState(false);

        if (PlayerAnimator)
        {
            PlayerAnimator.SetBool("IsChanneling", true);
            PlayerAnimator.SetInteger("ActionID", ActionID);
        }
        if (AuraEffect)
        {
            var main = AuraEffect.main;
            main.startColor = Color.cyan;
            AuraEffect.Play();
        }
    }

    void HandleTrainingState(bool isTraining, string result, float score) 
    {
        if (!_isActive) return;

        if (!isTraining) 
        {
            if (PlayerAnimator) PlayerAnimator.SetBool("IsChanneling", false);

            if (result == "SUCCESS")
            {
                var main = AuraEffect.main;
                main.startColor = Color.green;
            }
            else 
            {
                var main = AuraEffect.main;
                main.startColor = Color.red;
            }         
            SetMouseState(true);

        }
    }

    public void ExitStation() 
    {
        _isActive = false;

        if(StationCamera) StationCamera.Priority = 0;

        if (_playerInput)
        {
            _playerInput.ActivateInput();
            _playerInput.SwitchCurrentActionMap("Player");
        }
        if(PlayerAnimator) PlayerAnimator.SetBool("IsChanneling", false);

        if (AuraEffect) 
        {
            AuraEffect.Stop();
            var main = AuraEffect.main;
            main.startColor = Color.cyan;
        }
        SetMouseState(false);
    }
    private void SetMouseState(bool showMouse)
    {
        if (_mouseInput)
        {
            _mouseInput.cursorLocked = !showMouse;
            _mouseInput.cursorInputForLook = !showMouse;
            _mouseInput.SetCursorState(!showMouse);
        }
        Cursor.visible = showMouse;
        Cursor.lockState = showMouse ? CursorLockMode.None : CursorLockMode.Locked;
    }

}
