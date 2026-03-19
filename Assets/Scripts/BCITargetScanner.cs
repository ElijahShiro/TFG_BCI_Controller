using EmotivUnityPlugin;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BCITargetScanner : MonoBehaviour
{
    [Header("Control de Movimiento")]
    [Tooltip("El controlador de Third Person del jugador")]
    public StarterAssets.ThirdPersonController playerController;
    [Tooltip("El script que lee el teclado/mando del jugador")]
    public StarterAssets.StarterAssetsInputs playerInputs;

    [Header("Configuración de Escáner")]
    public float detectionRadius = 8f;
    public LayerMask interactableLayer;
    public Transform originTransform;

    [Header("Referencias de Telequinesis")]
    public Transform grabPoint;
    public Animator playerAnimator;

    [Header("Configuración del Casco Emotiv")]
    [Range(0f, 1f)]
    public float currentConcentration = 0f;
    public float grabThreshold = 0.7f;
    public float dropThreshold = 0.4f;

    [Header("Estado de Detección (Solo Lectura)")]
    public DummyBCIObject currentTarget;

    private bool isGrabbing = false;
    private int currentActionID;

    void Start()
    {
        if (originTransform == null)
            originTransform = transform;
    }

    void Update()
    {

        float newConcentration = 0f;
        int newActionID = currentActionID; 

        if (EmotivUnityItf.Instance != null && EmotivUnityItf.Instance.LatestMentalCommand != null)
        {
            string currentAction = EmotivUnityItf.Instance.LatestMentalCommand.act.ToLower();
            float currentPower = (float)EmotivUnityItf.Instance.LatestMentalCommand.pow;

            if (currentAction == "push")
            {
                newConcentration = currentPower;
                newActionID = 1;
            }
            else if (currentAction == "pull")
            {
                newConcentration = currentPower;
                newActionID = 2;
            }
        }


        if (Input.GetKey(KeyCode.E)) 
        {
            newConcentration = 1f;
            newActionID = 1;
        }
        else if (Input.GetKey(KeyCode.Q))
        {
            newConcentration = 1f;
            newActionID = 2;
        }
        currentConcentration = newConcentration;
        if (currentConcentration > 0f)
        {
            currentActionID = newActionID;
        }

        if (!isGrabbing)
        {
            ScanForBestTarget();
            if (currentTarget != null && currentConcentration >= grabThreshold)
            {
                ActivarPoderMental();
            }
        }
        else
        {

            float distanceToGrabPoint = Vector3.Distance(currentTarget.transform.position, grabPoint.position);

            if (distanceToGrabPoint > 1.5f)
            {
                Debug.Log("¡Enlace mental roto! La caja se ha desincronizado.");
                currentConcentration = 0f; 
                DesactivarPoderMental();
                return; 
            }

            if (playerInputs != null)
            {
                if (currentActionID == 1) 
                {
                    playerInputs.move = new Vector2(playerInputs.move.x, Mathf.Clamp(playerInputs.move.y, 0f, 1f));
                }
                else if (currentActionID == 2) 
                {
                    playerInputs.move = new Vector2(playerInputs.move.x, Mathf.Clamp(playerInputs.move.y, -1f, 0f));
                }
                if (playerAnimator != null)
                {
                    playerAnimator.SetFloat("PushDirection", playerInputs.move.y);
                }
            }
            if (playerAnimator != null)
            {
                playerAnimator.SetFloat("TrainingPower", currentConcentration);
            }
            if (currentConcentration < dropThreshold)
            {
                DesactivarPoderMental();
            }
        }
    }
    private void ScanForBestTarget()
    {
        Collider[] colliders = Physics.OverlapSphere(originTransform.position, detectionRadius, interactableLayer);

        DummyBCIObject bestTarget = null;
        float highestScore = -Mathf.Infinity;

        foreach (Collider col in colliders)
        {
            DummyBCIObject interactable = col.GetComponent<DummyBCIObject>();
            if (interactable == null) continue;

            Vector3 directionToBox = (col.transform.position - originTransform.position).normalized;
            float distanceToBox = Vector3.Distance(originTransform.position, col.transform.position);
            float lookAtScore = Vector3.Dot(originTransform.forward, directionToBox);

            if (lookAtScore > 0.4f)
            {
                float currentScore = lookAtScore - (distanceToBox * 0.05f);

                if (currentScore > highestScore)
                {
                    highestScore = currentScore;
                    bestTarget = interactable;
                }
            }
        }

        if (currentTarget != bestTarget)
        {
            if (currentTarget != null) currentTarget.SetHighlight(false);
            currentTarget = bestTarget;
            if (currentTarget != null) currentTarget.SetHighlight(true);
        }
    }


    private void ActivarPoderMental()
    {
        isGrabbing = true;

        if (currentTarget != null)
        {
            currentTarget.EngancharCaja(grabPoint);
        }

        if (playerAnimator != null)
        {
            playerAnimator.SetBool("IsChanneling", true);
            playerAnimator.SetInteger("AnimID", currentActionID); 
            playerAnimator.SetFloat("TrainingPower", currentConcentration);
        }

        if (playerController != null) playerController.lockRotation = true;
    }

    private void DesactivarPoderMental()
    {
        isGrabbing = false;

        if (currentTarget != null)
        {
            currentTarget.SoltarCaja();
        }

        if (playerAnimator != null)
        {
            playerAnimator.SetBool("IsChanneling", false);
            playerAnimator.SetFloat("TrainingPower", 0f);
        }
        if (playerController != null) playerController.lockRotation = false;
    }
    void LateUpdate()
    {
        
        if (isGrabbing && currentTarget != null)
        {
            Vector3 directionToBox = currentTarget.transform.position - transform.position;
            directionToBox.y = 0f; 

            if (directionToBox.sqrMagnitude > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(directionToBox);             
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 15f);
            }
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 1, 0, 0.2f);
        Gizmos.DrawSphere(transform.position, detectionRadius);
    }
}