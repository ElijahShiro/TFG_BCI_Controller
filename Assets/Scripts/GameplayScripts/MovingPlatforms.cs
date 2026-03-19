using UnityEngine;

public class MovingPlatforms : MonoBehaviour
{

    [Tooltip("Velocidad a la que se mueve la plataforma")]
    public float speed = 3f;

    public Transform pointA;
    public Transform pointB;

    private Vector3 targetPosition;

    void Start()
    {
        if (pointA == null || pointB == null)
        {
            Debug.LogError("¡Faltan puntos de ruta en la plataforma " + gameObject.name + "!");
            return;
        }


        targetPosition = pointB.position;
    }


    void Update()
    {
        if (pointA == null || pointB == null) return;


        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);


        if (Vector3.Distance(transform.position, targetPosition) < 0.05f)
        {
            if (targetPosition == pointA.position)
            {
                targetPosition = pointB.position;
            }
            else
            {
                targetPosition = pointA.position;
            }
        }
    }



    private void OnTriggerEnter(Collider other)
    {
     
        if (other.CompareTag("Player"))
        {
     
            other.transform.SetParent(transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {

        if (other.CompareTag("Player"))
        {
            other.transform.SetParent(null);
        }
    }


    private void OnDrawGizmos()
    {
        if (pointA != null && pointB != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(pointA.position, pointB.position);
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(pointA.position, 0.2f);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(pointB.position, 0.2f);
        }
    }
}