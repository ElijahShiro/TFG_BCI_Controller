using UnityEngine;

public class GameModeManager : MonoBehaviour
{
    public static GameModeManager Instance { get; private set; }

    public enum ControlMode
    {
        Full_BCI,   
        Hibrido,    
        Teclado     
    }

    [Header("Modo Actual")]
    public ControlMode currentMode = ControlMode.Hibrido; 

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
        }
    }

 
    public void SetControlMode(int modeIndex)
    {
        currentMode = (ControlMode)modeIndex;
        Debug.Log("SISTEMA: Modo de juego configurado a -> " + currentMode.ToString());


        if (GameMetricsLogger.Instance != null)
        {
            GameMetricsLogger.Instance.currentControlMode = currentMode.ToString();
        }
    }
}