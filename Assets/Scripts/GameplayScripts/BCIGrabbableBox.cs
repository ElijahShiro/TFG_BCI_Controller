using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BCIGrabbableBox : MonoBehaviour
{
    [Header("Visual Settings")]
    public Color highlightColor = Color.cyan;
    private Color originalColor;
    private MeshRenderer meshRenderer;

    [Header("Physics (Grab Point Tracker)")]
    [Tooltip("How fast the box follows the grab point")]
    public float followSpeed = 15f;
    [Tooltip("Extra friction so it doesn't slide like ice")]
    public float draggingDrag = 5f;

    private bool isGrabbed = false;
    private Rigidbody rb;
    private float originalDrag;
    private Transform targetGrabPoint;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        originalDrag = rb.drag;

        meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            originalColor = meshRenderer.material.color;
        }

        GameObject grabPointObj = GameObject.Find("MentalGrabPoint");
        if (grabPointObj != null)
        {
            targetGrabPoint = grabPointObj.transform;
        }
        else
        {
            Debug.LogWarning("BCIGrabbableBox: No 'MentalGrabPoint' found in the scene.");
        }
    }
    public void SetHighlightOn()
    {
        if (meshRenderer != null && !isGrabbed) meshRenderer.material.color = highlightColor;
    }

    public void SetHighlightOff()
    {
        if (meshRenderer != null) meshRenderer.material.color = originalColor;
    }

    public void Grab()
    {
        if (targetGrabPoint != null && !isGrabbed)
        {
            isGrabbed = true;
            rb.drag = draggingDrag;
            SetHighlightOff();
        }
    }

    public void Release()
    {
        if (isGrabbed)
        {
            isGrabbed = false;
            rb.drag = originalDrag; 
        }
    }


    void FixedUpdate()
    {
        if (isGrabbed && targetGrabPoint != null)
        {
            
            Vector3 direction = targetGrabPoint.position - transform.position;
            float distance = direction.magnitude;

            
            rb.velocity = direction.normalized * (distance * followSpeed);

            
            Quaternion targetRotation = targetGrabPoint.rotation;
            rb.MoveRotation(Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * 5f));
        }
    }
}