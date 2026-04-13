using EmotivUnityPlugin;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BCITargetScanner : MonoBehaviour
{
    [Header("Player Controllers")]
    public StarterAssets.StarterAssetsInputs playerInputs;

    [Header("Control de Movimiento")]
    public StarterAssets.ThirdPersonController playerController;

    [Header("Configuración de Escáner")]
    public float detectionRadius = 8f;
    public LayerMask interactableLayer;
    public Transform originTransform;

    [Header("Animaciones del Jugador")]
    public Animator playerAnimator;

    private bool isChanneling = false;
    private BCIInteractable _currentTarget;


    private bool _wasMentalCommandActiveLastFrame = false;

    void Start()
    {
        if (originTransform == null) originTransform = transform;
    }

    void Update()
    {
        ScanForTarget();
        ProcessMentalCommand();
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

    private void ProcessMentalCommand()
    {
        string currentAction = "";
        float currentPower = 0f;


        GameModeManager.ControlMode mode  = GameModeManager.Instance != null ? GameModeManager.Instance.currentMode : GameModeManager.ControlMode.Hibrido;

        if (mode == GameModeManager.ControlMode.Teclado)
        {
            if (Input.GetKey(KeyCode.E)) { currentAction = "push"; currentPower = 1f; }
            if (Input.GetKey(KeyCode.Space)) { currentAction = "jump"; currentPower = 1f; }
        }
        else
        {
            if (EmotivUnityItf.Instance != null && EmotivUnityItf.Instance.IsProfileLoaded && EmotivUnityItf.Instance.LatestMentalCommand != null)
            {
                currentAction = EmotivUnityItf.Instance.LatestMentalCommand.act.ToLower();
                currentPower = (float)EmotivUnityItf.Instance.LatestMentalCommand.pow;
            }
            if (Application.isEditor)
            {
                if (Input.GetKey(KeyCode.E)) { currentAction = "push"; currentPower = 1f; }
                if (Input.GetKey(KeyCode.Space)) { currentAction = "jump"; currentPower = 1f; }
            }
        }

        if (mode == GameModeManager.ControlMode.Full_BCI && playerInputs != null) 
        { 
            playerInputs.move = Vector2.zero;
        }



        bool isMentalCommandActive = currentPower >= 0.5f && (currentAction == "push" || currentAction == "pull" || currentAction == "jump");


        if (isMentalCommandActive && !_wasMentalCommandActiveLastFrame)
        {
            if (GameMetricsLogger.Instance != null)
                GameMetricsLogger.Instance.StartMentalActivation(currentAction);
        }
        else if (!isMentalCommandActive && _wasMentalCommandActiveLastFrame)
        {
            if (GameMetricsLogger.Instance != null)
                GameMetricsLogger.Instance.StopMentalActivation();
        }


        _wasMentalCommandActiveLastFrame = isMentalCommandActive;



        if (currentAction == "jump" && currentPower >= 0.5f)
        {
            if (playerInputs != null)
            {
                playerInputs.jump = true;
            }
        }

        isChanneling = false;

        if (_currentTarget != null)
        {
            _currentTarget.ReceiveMentalData(currentAction, currentPower);

            if (currentPower >= 0.5f && (currentAction == "push" || currentAction == "pull") && playerInputs != null)
            {
                isChanneling = true;

                if (playerController != null) playerController.lockRotation = true;

                if (currentAction == "push")
                {
                    playerInputs.move = new Vector2(playerInputs.move.x, Mathf.Clamp(playerInputs.move.y, 0f, 1f));
                }
                else if (currentAction == "pull")
                {
                    playerInputs.move = new Vector2(playerInputs.move.x, Mathf.Clamp(playerInputs.move.y, -1f, 0f));
                }
            }
        }
        if (!isChanneling && playerController != null)
        {
            playerController.lockRotation = false;
        }

        if (playerAnimator != null)
        {
            playerAnimator.SetBool("IsChanneling", isChanneling && currentAction == "push");
            if (currentAction == "push") playerAnimator.SetInteger("AnimID", 1);
        }
    }

    void LateUpdate()
    {
        if (isChanneling && _currentTarget != null)
        {
            Vector3 directionToBox = _currentTarget.transform.position - transform.position;
            directionToBox.y = 0f;

            if (directionToBox.sqrMagnitude > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(directionToBox);
                transform.rotation = targetRotation;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 1, 0, 0.2f);
        Gizmos.DrawSphere(transform.position, detectionRadius);
    }
}