using UnityEngine;

public class Checkpoints : MonoBehaviour
{
    [Tooltip("El punto exacto donde reaparecerá el jugador (ej: un Transform vacío). Si está vacío, usará la posición de este Trigger.")]
    public Transform respawnPoint;

   
    public static Transform LastCheckpoint { get; private set; }

    private void Start()
    {

        if (respawnPoint == null) respawnPoint = transform;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {

            if (LastCheckpoint != respawnPoint)
            {
                LastCheckpoint = respawnPoint;
                Debug.Log("¡Checkpoint guardado!");
            }
        }
    }
}