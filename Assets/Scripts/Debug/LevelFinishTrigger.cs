using UnityEngine;

public class LevelFinishTrigger : MonoBehaviour
{
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
            if (controller != null)
            {
                controller.enabled = false; 
            }

            StarterAssets.StarterAssetsInputs inputs = other.GetComponent<StarterAssets.StarterAssetsInputs>();
            if (inputs != null)
            {
                inputs.move = Vector2.zero; 
                inputs.jump = false;
            }

            // Aquí podrías activar un panel de UI que diga "¡Prueba Completada! Gracias."
            // if (uiPanelMenu != null) uiPanelMenu.SetActive(true);
        }
    }
}