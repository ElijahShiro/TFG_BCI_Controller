using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ObjectRespawner : MonoBehaviour
{
    private Vector3 _startPosition;
    private Quaternion _startRotation;
    private Rigidbody _rb;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _startPosition = transform.position;
        _startRotation = transform.rotation;
    }


    public void ResetToStart()
    {

        _rb.velocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;


        transform.position = _startPosition;
        transform.rotation = _startRotation;

        Debug.Log($"Objeto '{gameObject.name}' reseteado a su posicion original.");
    }
}