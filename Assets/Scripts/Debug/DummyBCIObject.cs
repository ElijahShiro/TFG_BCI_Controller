using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class DummyBCIObject : MonoBehaviour
{
    [Header("Configuración Visual")]
    public Color highlightColor = Color.cyan;
    private Color originalColor;
    private MeshRenderer meshRenderer;

    [Header("Físicas de Arrastre (NUEVO)")]
    [Tooltip("Fuerza con la que la caja intenta seguir tus manos")]
    public float followSpeed = 15f;
    [Tooltip("Fricción extra para que la caja no resbale como hielo al moverla")]
    public float draggingDrag = 5f;

    [Header("Estado Interno (Solo lectura)")]
    public bool isGrabbed = false;
    private Rigidbody rb;
    private Transform currentGrabPoint; 
    private float originalDrag;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        originalDrag = rb.drag; 

        meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            originalColor = meshRenderer.material.color;
        }
    }

    public void SetHighlight(bool isHighlighted)
    {
        if (meshRenderer != null && !isGrabbed)
        {
            meshRenderer.material.color = isHighlighted ? highlightColor : originalColor;
        }
    }

 
    public void EngancharCaja(Transform puntoDeAgarre)
    {
        if (rb != null && !isGrabbed)
        {
            isGrabbed = true;
            currentGrabPoint = puntoDeAgarre;

            rb.drag = draggingDrag;
        }
    }

    public void SoltarCaja()
    {
        if (isGrabbed)
        {
            isGrabbed = false;
            currentGrabPoint = null;

            if (rb != null)
            {
                rb.drag = originalDrag; 
            }

            SetHighlight(false);
        }
    }

    
    void FixedUpdate()
    {
        if (isGrabbed && currentGrabPoint != null)
        {
            
            Vector3 direction = currentGrabPoint.position - transform.position;
            float distance = direction.magnitude;

           
            rb.velocity = direction.normalized * (distance * followSpeed);

            Quaternion targetRotation = currentGrabPoint.rotation;
            rb.MoveRotation(Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * 5f));
        }
    }
}