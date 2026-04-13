using UnityEngine;

public class LevelStartTrigger : MonoBehaviour
{
    private bool _hasStarted = false;

    private void OnTriggerEnter(Collider other)
    {
        if (_hasStarted) return;

        if (other.CompareTag("Player"))
        {
            _hasStarted = true;

 
            if (GameMetricsLogger.Instance != null)
            {
                GameMetricsLogger.Instance.StartLevelRecording();
                Debug.Log("¡Empieza la prueba!");
            }
            else
            {
                Debug.LogError("Error: No se ha encontrado el GameMetricsLogger en la escena.");
            }
        }
    }


    public void ResetStartLine()
    {
        _hasStarted = false;
    }
}