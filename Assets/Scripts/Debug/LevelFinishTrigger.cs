using UnityEngine;
using UnityEngine.SceneManagement; 
using System.Collections; 

public class LevelFinishTrigger : MonoBehaviour
{
    [Header("Configuración de Fin de Nivel")]
    [Tooltip("El nombre exacto de tu escena de menú o gym (ej: MainMenu)")]
    public string menuSceneName = "Gym_platformer";

    [Tooltip("Segundos que espera antes de cambiar de pantalla")]
    public float delayBeforeReturn = 3f;

    private bool _hasFinished = false;

    private void OnTriggerEnter(Collider other)
    {
        if (_hasFinished) return;

        if (other.CompareTag("Player"))
        {
            _hasFinished = true;


            if (GameMetricsLogger.Instance != null)
            {
                GameMetricsLogger.Instance.EndLevelAndSaveData();
                Debug.Log("Línea de meta cruzada: ¡Prueba finalizada y datos guardados!");
            }


            StarterAssets.ThirdPersonController controller = other.GetComponent<StarterAssets.ThirdPersonController>();
            if (controller != null) controller.enabled = false;

            StarterAssets.StarterAssetsInputs inputs = other.GetComponent<StarterAssets.StarterAssetsInputs>();
            if (inputs != null)
            {
                inputs.move = Vector2.zero;
                inputs.jump = false;
            }


            StartCoroutine(ReturnToMenuRoutine());
        }
    }

    private IEnumerator ReturnToMenuRoutine()
    {
        Debug.Log($"Volviendo al menú en {delayBeforeReturn} segundos...");


        yield return new WaitForSeconds(delayBeforeReturn);


        SceneManager.LoadScene(menuSceneName);
    }
}