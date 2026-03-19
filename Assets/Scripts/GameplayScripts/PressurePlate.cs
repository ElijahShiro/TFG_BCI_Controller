using UnityEngine;
using UnityEngine.Events;

public class PressurePlate : MonoBehaviour
{
    [Header("Configuración de Eventos")]
    [Tooltip("Acciones que ocurren cuando se pisa la placa")]
    public UnityEvent OnPressed;

    [Tooltip("Acciones que ocurren cuando se libera la placa")]
    public UnityEvent OnReleased;

    [Header("Efecto Visual (Opcional)")]
    [Tooltip("El modelo 3D de la placa que bajará físicamente al pisarse")]
    public Transform visualPlate;
    [Tooltip("Distancia que baja la placa al pisarse")]
    public float pressDownDistance = 0.05f;

    private Vector3 originalPos;
    private Vector3 pressedPos;

    private int objectsOnPlate = 0;

    void Start()
    {
        if (visualPlate != null)
        {
            originalPos = visualPlate.localPosition;
            pressedPos = originalPos - new Vector3(0, pressDownDistance, 0);
        }
    }

    private void OnTriggerEnter(Collider other)
    {

        DummyBCIObject box = other.GetComponent<DummyBCIObject>();


        bool isPlayer = other.CompareTag("Player");


        if (box != null || isPlayer)
        {
            objectsOnPlate++;

            if (objectsOnPlate == 1)
            {
                ActivarPlaca();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        DummyBCIObject box = other.GetComponent<DummyBCIObject>();
        bool isPlayer = other.CompareTag("Player");

        if (box != null || isPlayer)
        {
            objectsOnPlate--;

            if (objectsOnPlate <= 0)
            {
                objectsOnPlate = 0;
                DesactivarPlaca();
            }
        }
    }

    private void ActivarPlaca()
    {
        if (visualPlate != null) visualPlate.localPosition = pressedPos;
        OnPressed.Invoke();
        Debug.Log("¡Placa presionada!");
    }

    private void DesactivarPlaca()
    {
        if (visualPlate != null) visualPlate.localPosition = originalPos;
        OnReleased.Invoke();
        Debug.Log("Placa liberada.");
    }
}