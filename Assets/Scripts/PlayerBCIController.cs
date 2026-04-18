using UnityEngine;

public class PlayerBCIController : MonoBehaviour
{
    [Header("Referencias de Arquitectura (Nuestros nuevos scripts)")]
    public BCITargetScanner radar;
    public BCIInputHandler inputHandler;

    [Header("Referencias del Starter Assets")]
    public StarterAssets.StarterAssetsInputs playerInputs;
    public StarterAssets.ThirdPersonController playerController;
    public Animator playerAnimator;

    private bool isChanneling = false;

    void Update()
    {

        if (radar == null || inputHandler == null || playerInputs == null) return;
        if(MentalGymTrainer.Instance != null && MentalGymTrainer.Instance._isTraining) 
            return; 

        string action = inputHandler.CurrentAction;
        float power = inputHandler.CurrentPower;


        UnityEngine.InputSystem.PlayerInput pi = playerInputs.GetComponent<UnityEngine.InputSystem.PlayerInput>();

        if (inputHandler.IsUsingHeadMovement)
        {
            
            if (pi != null && pi.actions != null) pi.actions.FindAction("Move").Disable();
            playerInputs.move = inputHandler.HeadMove;
        }
        else
        {
            
            if (pi != null && pi.actions != null) pi.actions.FindAction("Move").Enable();
        }

        if (inputHandler.IsJumpTriggered) playerInputs.jump = true;


        isChanneling = false;
        BCIInteractable target = radar.CurrentTarget; 

        if (target != null)
        {

            target.ReceiveMentalData(action, power);


            if (power >= 0.5f && (action == "push" || action == "pull"))
            {
                isChanneling = true;


                if (playerController != null) playerController.lockRotation = true;


                if (action == "push")
                {
                    playerInputs.move = new Vector2(playerInputs.move.x, Mathf.Clamp(playerInputs.move.y, 0f, 1f));
                }
                else if (action == "pull")
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
            bool isPushing = (isChanneling && action == "push");
            playerAnimator.SetBool("IsChanneling", isPushing);

            if (isPushing) playerAnimator.SetInteger("AnimID", 1);
            else playerAnimator.SetInteger("AnimID", 0);
        }
    }

    void LateUpdate()
    {

        if (isChanneling && radar != null && radar.CurrentTarget != null)
        {
            Vector3 directionToBox = radar.CurrentTarget.transform.position - transform.position;
            directionToBox.y = 0f; 

            if (directionToBox.sqrMagnitude > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(directionToBox);
                transform.rotation = targetRotation;
            }
        }
    }
}