using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GameUIController : MonoBehaviour
{
    public static GameUIController Instance { get; private set; }
    
    
    [Header("UI Interation")]
    public GameObject InteractPromptObj; // Objeto que contiene el texto de "Pulsa E"
    public Text InteractText;
    

    [Header("UI Pre-Training")]
    public GameObject PreTrainingPanel;
    public Text Countdown_Text;
    private TrainingStation _currentStation;

    [Header("UI Training Results")]
    public GameObject TrainingResultPanel;
    public Text TimerText;
    public Text StatusText;
    public Button AcceptBtn;
    public Button RejectBtn;

    [Header("Quality Bar UI")]
    public Slider ScoreSlider;
    public Text QualityText;
    public Image QualityBarFill;

    [Header("Pause Menu")]
    public GameObject PausePanel;
    public Button SaveBtn;
    public Button ExitBtn;
    public Button ResumeBtn;
    public Text SaveFeedbackText;
    private bool _isPaused;

    private StarterAssetsInputs _mouseInputs;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    private void OnEnable(){MentalGymTrainer.OnTrainingUpdate += HandleTrainingUpdate;}
    private void OnDisable(){MentalGymTrainer.OnTrainingUpdate -= HandleTrainingUpdate;}

    void Start()
    {      
        _mouseInputs = FindObjectOfType<StarterAssetsInputs>();
        if (InteractPromptObj) InteractPromptObj.SetActive(false); 
        if (PreTrainingPanel) PreTrainingPanel.SetActive(false);
        if (Countdown_Text) Countdown_Text.gameObject.SetActive(false);
        if (TrainingResultPanel) TrainingResultPanel.SetActive(false);

        if (PausePanel) PausePanel.SetActive(false);
        if (SaveFeedbackText) SaveFeedbackText.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(MentalGymTrainer.Instance._isTraining && TimerText != null)
            TimerText.text = MentalGymTrainer.Instance._TimerTrainer.ToString("F1");

        AcceptBtn.interactable = MentalGymTrainer.Instance.CanAcceptResult();
        RejectBtn.interactable = true;
        SaveBtn.interactable = MentalGymTrainer.Instance.CanSaveResult();


        bool canPause = !MentalGymTrainer.Instance._isTraining &&
                        !PreTrainingPanel.activeSelf &&
                        !TrainingResultPanel.activeSelf;

        if (canPause && Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            TogglePauseMenu();
        }
    }

    public void TogglePauseMenu() 
    { 
        _isPaused = !_isPaused;

        if(PausePanel) PausePanel.SetActive(_isPaused);
        if(SaveFeedbackText) SaveFeedbackText.gameObject.SetActive(false);

        if (_mouseInputs != null)
        {
            _mouseInputs.cursorLocked = !_isPaused;       
            _mouseInputs.cursorInputForLook = !_isPaused;  
            _mouseInputs.SetCursorState(!_isPaused);
        }

        Cursor.visible = _isPaused;
        Cursor.lockState = _isPaused ? CursorLockMode.None : CursorLockMode.Locked;

        Time.timeScale = _isPaused ? 0f : 1f;
    }

    public void OnResumeClick()
    {
        TogglePauseMenu(); 
    }

    public void ToggleInteractPrompt(bool show, string message = "", bool isLocked = false)
    {
        if (InteractPromptObj) InteractPromptObj.SetActive(show);
        if (show && InteractText)
        {
            InteractText.text = message;
            InteractText.color = isLocked? Color.red : Color.white;
        }
    }
    public void OpenPreTrainingMenu(TrainingStation station) 
    { 
        _currentStation = station;
        PreTrainingPanel.SetActive(true);

    }
    public void OnCancelPreTraining() 
    {
        PreTrainingPanel.SetActive(false);
        _currentStation.ExitStation();
    }

    public void OnStartTraining() 
    {
        PreTrainingPanel.SetActive(false);
        StartCoroutine(CountdownRoutine());
    }
    IEnumerator CountdownRoutine()
    {
        if (Countdown_Text)
        {
            Countdown_Text.gameObject.SetActive(true);
            Countdown_Text.text = "3";
            yield return new WaitForSeconds(1f);
            Countdown_Text.text = "2";
            yield return new WaitForSeconds(1f);
            Countdown_Text.text = "1";
            yield return new WaitForSeconds(1f);
            Countdown_Text.text = "¡CONCÉNTRATE!";
            yield return new WaitForSeconds(0.5f);
            Countdown_Text.gameObject.SetActive(false);
        }
        if (_currentStation) _currentStation.StartTrainingVisuals();

        MentalGymTrainer.Instance.StartTrainingAction(_currentStation.ActionName);
    }
    void HandleTrainingUpdate(bool isTraining, string message, float score) 
    {
        if (isTraining)
        {
            TrainingResultPanel.SetActive(true);
            StatusText.text = message;
            StatusText.color = Color.white;

            if(ScoreSlider) ScoreSlider.gameObject.SetActive(false);
            if (QualityText) QualityText.text = "";
        }
        else 
        {
            if (message == "SUCCESS")
            {
                StatusText.text = "¡ÉXITO!";
                StatusText.color = Color.green;
                if (ScoreSlider != null)
                {
                    ScoreSlider.gameObject.SetActive(true);
                    ScoreSlider.value = score; 

                   
                    int porcentaje = Mathf.RoundToInt(score * 100);

                    if (score >= 0.76f) 
                    {
                        if (QualityBarFill) QualityBarFill.color = Color.green;
                        if (QualityText) { QualityText.text = $"Calidad: {porcentaje}% - EXCELENTE"; QualityText.color = Color.green; }
                    }
                    else if (score >= 0.40f) 
                    {
                        if (QualityBarFill) QualityBarFill.color = Color.yellow;
                        if (QualityText) { QualityText.text = $"Calidad: {porcentaje}% - NORMAL"; QualityText.color = Color.yellow; }
                    }
                    else 
                    {
                        if (QualityBarFill) QualityBarFill.color = Color.red;
                        if (QualityText) { QualityText.text = $"Calidad: {porcentaje}% - MALO\nRecomendado: RECHAZAR."; QualityText.color = Color.red; }
                    }
                }
            }
            else if (message == "FAILED")
            {
                StatusText.text = "FALLO";
                StatusText.color = Color.red;

                if (ScoreSlider) ScoreSlider.gameObject.SetActive(false);
                if (QualityText) QualityText.text = "Demasiado ruido. Inténtalo de nuevo.";
            }
        }
    }

    public void OnAcceptClick() 
    {
        MentalGymTrainer.Instance.AcceptTraining();
        TrainingResultPanel.SetActive(false);
    }
    public void OnRejectClick()
    {
        MentalGymTrainer.Instance.RejectTraining();
        TrainingResultPanel.SetActive(false);
    }

    public void OnSaveClick() 
    { 
        MentalGymTrainer.Instance.SaveProfile();
        
        if (SaveFeedbackText)
        {
            SaveFeedbackText.gameObject.SetActive(true);
            SaveFeedbackText.text = "¡Perfil guardado correctamente!";
        }
    }
    public void OnExitClick() 
    { 
        MentalGymTrainer.Instance.ReturnToMenu();
        Time.timeScale = 1f;
    }
}
