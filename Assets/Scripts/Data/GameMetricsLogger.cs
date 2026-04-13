using UnityEngine;
using System.IO;
using System;

public class GameMetricsLogger : MonoBehaviour
{
    public static GameMetricsLogger Instance { get; private set; }

    [Header("Configuración del Test")]
    public string participantID = "User_01";
    public string currentControlMode = "Full_BCI";

    [Header("Métricas de Tiempo y Errores")]
    public float totalTimeElapsed = 0f;
    public float totalIdleTime = 0f; 
    public int totalFalls = 0;

    [Header("Métricas de Comandos (Independientes)")]
    public int pushActivations = 0;
    public int jumpActivations = 0;
    public int unstableActivations = 0; 

    [Header("Métricas de Concentración")]
    public float totalTimeConcentrated = 0f;
    public float maxContinuousConcentration = 0f; 


    private bool isLevelActive = false;
    private float currentContinuousTime = 0f;
    private bool isCurrentlyConcentrating = false;
    private Transform playerTransform;
    private Vector3 lastPlayerPosition;

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
    }

    private void Start()
    {

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
            lastPlayerPosition = playerTransform.position;
        }
    }

    public void StartLevelRecording()
    {

        if (MentalGymTrainer.Instance != null && !string.IsNullOrEmpty(MentalGymTrainer.Instance._currentProfileName))
        {
            participantID = MentalGymTrainer.Instance._currentProfileName;
            Debug.Log("Telemetría: Nombre de perfil obtenido del Trainer -> " + participantID);
        }
        else if (string.IsNullOrEmpty(participantID) || participantID == "User_01")
        {
            participantID = "Tester_" + UnityEngine.Random.Range(1000, 9999);
        }

        if (GameModeManager.Instance != null)
        {
            currentControlMode = GameModeManager.Instance.currentMode.ToString();
            Debug.Log("Telemetría: Modo de control detectado -> " + currentControlMode);
        }

        totalTimeElapsed = 0f;
        totalIdleTime = 0f;
        totalFalls = 0;
        pushActivations = 0;
        jumpActivations = 0;
        unstableActivations = 0;
        totalTimeConcentrated = 0f;
        maxContinuousConcentration = 0f;

        isLevelActive = true;
    }

    private void Update()
    {
        if (!isLevelActive) return;

        totalTimeElapsed += Time.deltaTime;


        if (playerTransform != null)
        {
            float distanceMoved = Vector3.Distance(playerTransform.position, lastPlayerPosition);
            if (distanceMoved < 0.01f) 
            {
                totalIdleTime += Time.deltaTime;
            }
            lastPlayerPosition = playerTransform.position;
        }


        if (isCurrentlyConcentrating)
        {
            currentContinuousTime += Time.deltaTime;
            totalTimeConcentrated += Time.deltaTime;
        }

      
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            ObjectRespawner[] allRespawners = FindObjectsOfType<ObjectRespawner>();
            foreach (ObjectRespawner obj in allRespawners)
            {
                obj.ResetToStart();
            }
            Debug.Log("El jugador ha forzado el reseteo del puzle.");
        }
    }



    public void LogFall()
    {
        if (!isLevelActive) return;
        totalFalls++;
    }


    public void StartMentalActivation(string commandName)
    {
        if (!isLevelActive) return;

        isCurrentlyConcentrating = true;
        currentContinuousTime = 0f; 


        string cmd = commandName.ToLower();
        if (cmd == "push") pushActivations++;
        else if (cmd == "jump") jumpActivations++;
    }


    public void StopMentalActivation()
    {
        if (!isLevelActive || !isCurrentlyConcentrating) return;

        isCurrentlyConcentrating = false;


        if (currentContinuousTime < 0.5f)
        {
            unstableActivations++;
        }


        if (currentContinuousTime > maxContinuousConcentration)
        {
            maxContinuousConcentration = currentContinuousTime;
        }
    }


    public void EndLevelAndSaveData()
    {
        isLevelActive = false;


        if (isCurrentlyConcentrating && currentContinuousTime > maxContinuousConcentration)
        {
            maxContinuousConcentration = currentContinuousTime;
        }

        string folderPath = Application.dataPath + "/TelemetryResults/";
        if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);
        string filePath = folderPath + "Results_" + participantID + ".csv";

        bool isNewFile = !File.Exists(filePath);

        using (StreamWriter writer = new StreamWriter(filePath, true))
        {
            if (isNewFile)
            {

                writer.WriteLine("ParticipantID,ControlMode,TotalTime(s),IdleTime(s),Falls,PushCount,JumpCount,UnstableFlickers,TotalConcentration(s),MaxContinuousTime(s),Date");
            }


            string dataRow = $"{participantID},{currentControlMode},{Math.Round(totalTimeElapsed, 2)},{Math.Round(totalIdleTime, 2)},{totalFalls},{pushActivations},{jumpActivations},{unstableActivations},{Math.Round(totalTimeConcentrated, 2)},{Math.Round(maxContinuousConcentration, 2)},{DateTime.Now.ToString("dd/MM/yyyy HH:mm")}";
            writer.WriteLine(dataRow);
        }

        Debug.Log("Telemetría: Excel generado con métricas avanzadas.");
    }
}