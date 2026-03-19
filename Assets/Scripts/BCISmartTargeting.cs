using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BCISmartTargeting : MonoBehaviour
{
    [Header("Configuración de Detección")]
    public float detectionRadius = 10f;
    public LayerMask interactableLayer; 

    [Header("Objetivo Actual")]
    private IBCIInteractable currentTarget;

    void Update()
    {
        FindBestTarget();

        // Aquí iría tu lógica de BCI
        // if (BCIManager.Instance.CommandIs("Push") && currentTarget != null) { ... }
    }

    private void FindBestTarget()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius, interactableLayer);

        IBCIInteractable bestInteractable = null;
        float highestScore = -Mathf.Infinity;

        foreach (Collider col in colliders)
        {
            
            Vector3 directionToObject = (col.transform.position - transform.position).normalized;

            
            float lookAtScore = Vector3.Dot(transform.forward, directionToObject);

            
            if (lookAtScore > 0.5f)
            {
               
                float distance = Vector3.Distance(transform.position, col.transform.position);

                
                float finalScore = lookAtScore - (distance * 0.05f);

                if (finalScore > highestScore)
                {
                    highestScore = finalScore;
                    bestInteractable = col.GetComponent<IBCIInteractable>();
                }
            }
        }

        
        if (currentTarget != bestInteractable)
        {
            if (currentTarget != null) currentTarget.RemoveHighlight();

            currentTarget = bestInteractable;

            if (currentTarget != null) currentTarget.ApplyHighlight();
        }
    }
}