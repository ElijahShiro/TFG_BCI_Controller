using UnityEngine;

public class FallDetector : MonoBehaviour
{
    [Header("Telemetry Settings")]
    [Tooltip("¿Contabilizar como caída en el Excel?")]
    public bool logAsFall = true;

    [Header("Respawn Settings")]
    [Tooltip("¿Debe el jugador volver al checkpoint al tocar esto?")]
    public bool shouldRespawn = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {

            if (logAsFall && GameMetricsLogger.Instance != null)
            {
                GameMetricsLogger.Instance.LogFall();
                Debug.Log("Telemetría: Caída registrada (sin muerte).");
            }


            if (shouldRespawn)
            {
                RespawnPlayer(other.gameObject);
            }
        }
    }

    private void RespawnPlayer(GameObject player)
    {
        Transform target = Checkpoints.LastCheckpoint;
        if (target != null)
        {
            CharacterController cc = player.GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;
            player.transform.position = target.position;
            player.transform.rotation = target.rotation;
            if (cc != null) cc.enabled = true;
        }
    }
}
