using UnityEngine;
using System.IO;
using System;

public class GameMetricsLogger : MonoBehaviour
{

    public static GameMetricsLogger Instance { get; private set; }

    [Header("Configuración del Test")]
    public string participantID = "User_01";
    public string currentControlMode = "Full_BCI"; 

    [Header("Métricas Recopiladas")]
    public float totalTimeElapsed = 0f;
    public int totalFalls = 0;
    public int successfulMentalActivations = 0;
    public float totalTimeConcentrated = 0f;

    private bool isLevelActive = false;

    private void Awake()
    {

        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
    }

    public void StartLevelRecording()
    {

        totalTimeElapsed = 0f;
        totalFalls = 0;
        successfulMentalActivations = 0;
        totalTimeConcentrated = 0f;
        isLevelActive = true;
        Debug.Log("Telemetría: Grabación iniciada para " + participantID);
    }

    private void Update()
    {
        if (isLevelActive)
        {
            totalTimeElapsed += Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.R))
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
        Debug.Log("Telemetría: Caída registrada. Total: " + totalFalls);
    }

    public void LogMentalActivationStart()
    {
        if (!isLevelActive) return;
        successfulMentalActivations++;
    }

    public void AddConcentrationTime(float time)
    {
        if (!isLevelActive) return;
        totalTimeConcentrated += time;
    }
    public void EndLevelAndSaveData()
    {
        isLevelActive = false;

        string folderPath = Application.dataPath + "/TelemetryResults/";
        if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

        string filePath = folderPath + "Results_" + participantID + ".csv";

        bool isNewFile = !File.Exists(filePath);

        using (StreamWriter writer = new StreamWriter(filePath, true)) 
        {
            if (isNewFile)
            {
                writer.WriteLine("ParticipantID,ControlMode,TotalTime(s),Falls,MentalActivations,TotalConcentrationTime(s),Date");
            }


            string dataRow = $"{participantID},{currentControlMode},{Mathf.Round(totalTimeElapsed)},{totalFalls},{successfulMentalActivations},{Mathf.Round(totalTimeConcentrated)},{DateTime.Now.ToString("dd/MM/yyyy HH:mm")}";
            writer.WriteLine(dataRow);
        }

        Debug.Log("Telemetría: Nivel superado. Datos guardados en Excel en: " + filePath);
    }
}