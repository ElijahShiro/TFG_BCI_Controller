using EmotivUnityPlugin;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BCITargetScanner : MonoBehaviour
{
    [Header("Configuración de Escáner")]
    public float detectionRadius = 8f;
    public LayerMask interactableLayer;
    public Transform originTransform;

    private BCIInteractable _currentTarget;
    public BCIInteractable CurrentTarget => _currentTarget;

    void Start()
    {
        if (originTransform == null) originTransform = transform;
    }

    void Update()
    {
        ScanForTarget();
    }

    private void ScanForTarget()
    {
        Collider[] colliders = Physics.OverlapSphere(originTransform.position, detectionRadius, interactableLayer);
        BCIInteractable bestTarget = null;
        float highestScore = -Mathf.Infinity;

        foreach (Collider col in colliders)
        {
            BCIInteractable interactable = col.GetComponent<BCIInteractable>();
            if (interactable == null) continue;

            Vector3 directionToTarget = (col.transform.position - originTransform.position).normalized;
            float distanceToTarget = Vector3.Distance(originTransform.position, col.transform.position);
            float lookAtScore = Vector3.Dot(originTransform.forward, directionToTarget);

            if (lookAtScore > 0.4f)
            {
                float currentScore = lookAtScore - (distanceToTarget * 0.05f);
                if (currentScore > highestScore)
                {
                    highestScore = currentScore;
                    bestTarget = interactable;
                }
            }
        }

        if (_currentTarget != bestTarget)
        {
            if (_currentTarget != null) _currentTarget.SetFocus(false);
            _currentTarget = bestTarget;
            if (_currentTarget != null) _currentTarget.SetFocus(true);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 1, 0, 0.2f);
        Gizmos.DrawSphere(transform.position, detectionRadius);
    }
}