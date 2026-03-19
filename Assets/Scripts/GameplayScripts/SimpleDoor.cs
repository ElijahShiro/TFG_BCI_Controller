using UnityEngine;

public class SimpleDoor : MonoBehaviour
{
    [Header("Configuración de la Puerta")]
    [Tooltip("Distancia que bajará la puerta al abrirse (en el eje Y)")]
    public float openDistance = 3f;
    [Tooltip("Velocidad a la que se mueve la puerta")]
    public float moveSpeed = 2f;

    private Vector3 closedPosition;
    private Vector3 openPosition;
    private Vector3 targetPosition;

    void Start()
    {

        closedPosition = transform.position;


        openPosition = closedPosition - new Vector3(0f, openDistance, 0f);


        targetPosition = closedPosition;
    }

    void Update()
    {

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
    }



    public void OpenDoor()
    {
        targetPosition = openPosition;
    }

    public void CloseDoor()
    {
        targetPosition = closedPosition;
    }
}