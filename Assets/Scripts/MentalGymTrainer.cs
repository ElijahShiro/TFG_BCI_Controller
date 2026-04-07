using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EmotivUnityPlugin;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Threading.Tasks;

#if UNITY_ANDROID
using UnityEngine.Android;
#endif

public class MentalGymTrainer : MonoBehaviour
{

    public static MentalGymTrainer Instance { get; private set; }

    private bool _isEmotivUnityItfInitialized = false;  // flag para ver si EmotivUnityItf esta iniciado y conectando con Cortex
    EmotivUnityItf _eItf = EmotivUnityItf.Instance; //accede a todas las funciones de la API

    public string _currentProfileName { get; private set; }
    public string _currentGenre { get; private set; }

    //Timer para entrenamiento
    public float _TimerTrainer = 0;
    const float TimerDuration = 8f;
    [HideInInspector]public bool _isTraining = false;
    private string _currentTrainingAction = "";

    private string _lastMsg = "";

    public delegate void TrainingStateChange(bool isTraining, string resultMessage, float score);
    public static event TrainingStateChange OnTrainingUpdate;

    [SerializeField] private bool testingSimulatedDevice;

    [Header("Training Pct Var")]
    private float _totalPowerDuringTraining = 0f;
    private int _powerSamplesCount = 0;


    public event Action OnResetGameplay;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else 
        {
            Destroy(gameObject);
            return;
        } 
        
    }

    void Start()
    {
        _eItf.Init(AppConfig.ClientId, AppConfig.ClientSecret, AppConfig.AppName, AppConfig.AllowSaveLogToFile, AppConfig.IsDataBufferUsing, AppConfig.AppUrl);
        _eItf.Start();
        _isEmotivUnityItfInitialized = true;

        Invoke("DelayedQuery", 2.0f);
    }

    void DelayedQuery() { _eItf.QueryHeadsets(); }
    void Update()
    {

        if (_isTraining)
        {
            float l_currentPower = (float)EmotivUnityItf.Instance.LatestMentalCommand.pow;

            _totalPowerDuringTraining += l_currentPower;
            _powerSamplesCount++;
            _TimerTrainer -= Time.deltaTime;
            if (_TimerTrainer <= 0) 
            { 
                _isTraining = false;
                Debug.Log("Training ended");
                OnTrainingUpdate?.Invoke(false, "Tiempo Agotado", 0f);
            }
                
        }

        string currentMsg = _eItf.MessageLog;
        if (currentMsg != _lastMsg)
        {
            float l_finalScore = 0f;
            _lastMsg = currentMsg;
            if (currentMsg.Contains("MC_Succeeded"))
            {
                _isTraining = false;
                
                
                if (_powerSamplesCount > 0) 
                { 
                    l_finalScore = _totalPowerDuringTraining / _powerSamplesCount;
                }

                l_finalScore = Mathf.Clamp01(l_finalScore * 1.5f);

                int numberTrainings = _eItf.GetTrainingTimeForAction(_currentTrainingAction);
                if (_currentTrainingAction == "neutral" || numberTrainings == 0)
                {
                    l_finalScore = 1f;
                    Debug.Log($"Pase VIP aplicado. Acción: {_currentTrainingAction} | Veces previas: {numberTrainings}");
                }
                Debug.Log("Result: <color=green>¡ÉXITO!</color> Pulsa 'AcceptTraining' para guardar.");

                OnTrainingUpdate?.Invoke(false, "SUCCESS", l_finalScore);
            }
            else if (currentMsg.Contains("MC_Failed") || currentMsg.Contains("MC_Rejected"))
            {
                _isTraining = false;
                OnTrainingUpdate?.Invoke(false, "FAILED", 0f);
                Debug.LogError("Result: <color=red>FALLO</color>. Hubo mucho ruido o movimiento. Inténtalo de nuevo.");
            }
            else if (currentMsg.Contains("MC_Completed"))
            {
                Debug.Log("Sistema: Patrón aprendido y guardado en memoria.");
                SaveProfile();
            }
        }
    }

    public void ConnectToHeadset()
    {
        
        var listaCascos = _eItf.GetDetectedHeadsets();

        if (listaCascos.Count > 0)
        {
            _eItf.CreateSessionWithHeadset(listaCascos[0].HeadsetID);
        }
        else 
        {
            Debug.LogWarning("Lista vacía. Escaneando... Inténtalo de nuevo en 2 segundos.");
            _eItf.QueryHeadsets("");
        }
    }

    public bool IsSessionReady() { return _eItf.IsSessionCreated; }

    public bool IsProfileLoaded() { return _eItf.IsProfileLoaded; }

    public void ShowQueryHeadset() 
    {
        _eItf.QueryHeadsets(); //enseña en el log de los headsets conectados.
    }

    public void LoadProfile(string l_name, string l_genre) 
    {
        if (string.IsNullOrEmpty(l_name))
        {
            Debug.Log("Need to insert a profile");
            return;
        }
        _currentGenre = l_genre;
        string baseName = l_name.Trim();
        _currentProfileName = baseName + "_"+ l_genre;
        _eItf.LoadProfile(_currentProfileName);   
        _eItf.SubscribeData(new List<string> { "com", "sys" });

        Debug.Log("Loaded profile: " + _currentProfileName);
    }
    public void UnloadProfile()
    {
        Debug.Log("Profile _unloaded_");
        _eItf.UnLoadProfile();
    }
    public void LoadLevel() 
    {
        string l_sceneName = "Gym_" + _currentGenre;
        Debug.Log("Loding scenari: " + l_sceneName);
        SceneManager.LoadScene(l_sceneName);
    }
    public void LoadTestLevel()
    {
        Debug.Log("Loading test scenario: Level1");
        SceneManager.LoadScene("Level1");
    }
    public void ReturnToMenu() 
    { 
        _eItf.UnLoadProfile();
        SceneManager.LoadScene("MainMenu");

    }

    public void StartTrainingAction(string action) 
    { 
        action = action.ToLower().Trim();
        _currentTrainingAction = action;
        Debug.Log("Iniciando entrenamiento: " + action);
        _eItf.StartMCTraining(action);

        _TimerTrainer = TimerDuration;
        _isTraining = true;

        _totalPowerDuringTraining = 0f;
        _powerSamplesCount = 0;

        OnTrainingUpdate?.Invoke(true, "Training: " + action, 0f);
    }

    public void AcceptTraining()
    {
        Debug.Log("Training Accepted");
        _eItf.AcceptMCTraining();

        OnResetGameplay?.Invoke();
    }

    public void RejectTraining()
    {
        Debug.Log("Training Rejected");
        _eItf.RejectMCTraining();
        OnResetGameplay?.Invoke();
    }

    public void SaveProfile()
    {
        _eItf.SaveProfile(_currentProfileName);
        Debug.Log("Saved profile: " + _currentProfileName);
    }

    public bool CanAcceptResult() {  return _eItf.IsMCTrainingSuccess;}
    public bool CanSaveResult() { return _eItf.IsProfileLoaded && !_isTraining; }

    public int GetTrainingCount(string actionName)
    {
        if (!_eItf.IsProfileLoaded) return 0;

        return _eItf.GetTrainingTimeForAction(actionName.ToLower().Trim());
    }
    public float GetCurrentPower()
    {
        if (!_isTraining || !_eItf.IsProfileLoaded) return 0f;
        return (float)_eItf.LatestMentalCommand.pow;
    }
    public bool HasTrainedNeutral() 
    {
        if (!_eItf.IsProfileLoaded) return false;

        int neutralTrainingCount = _eItf.GetTrainingTimeForAction("neutral");
        return neutralTrainingCount > 0;
    }
    void OnApplicationQuit()
    {
        if (_isEmotivUnityItfInitialized)
            _eItf.Stop();
    }
}
