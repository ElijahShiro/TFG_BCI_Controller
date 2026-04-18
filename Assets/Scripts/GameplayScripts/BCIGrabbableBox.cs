using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BCIGrabbableBox : MonoBehaviour
{
    [Header("Visual Settings")]
    public Color highlightColor = Color.cyan;
    private Color originalColor;
    private MeshRenderer meshRenderer;

    [Header("Physics (Spatial Lock Tracker)")]
    public float followSpeed = 15f;
    public float draggingDrag = 5f;

    public float heightOffset = 1.2f;

    private bool isGrabbed = false;
    private Rigidbody rb;
    private float originalDrag;

    private Transform playerTransform;

    private Vector3 worldGrabOffset;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        originalDrag = rb.drag;

        meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null) originalColor = meshRenderer.material.color;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) playerTransform = playerObj.transform;
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
        if (!isGrabbed && playerTransform != null)
        {
            isGrabbed = true;
            rb.drag = draggingDrag;
            SetHighlightOff();


            Vector3 playerPosFlat = new Vector3(playerTransform.position.x, 0, playerTransform.position.z);
            Vector3 boxPosFlat = new Vector3(transform.position.x, 0, transform.position.z);


            worldGrabOffset = boxPosFlat - playerPosFlat;
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
        if (isGrabbed && playerTransform != null)
        {

            Vector3 targetPos = playerTransform.position + worldGrabOffset;
            targetPos.y = playerTransform.position.y + heightOffset;


            Vector3 direction = targetPos - transform.position;
            float distance = direction.magnitude;
            rb.velocity = direction.normalized * (distance * followSpeed);


            rb.MoveRotation(Quaternion.Slerp(transform.rotation, Quaternion.identity, Time.fixedDeltaTime * 2f));
        }
    }
}