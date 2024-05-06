using UnityEngine;
using UnityEngine.InputSystem;

public class MoveInputController : MonoBehaviour
{
    private PlayerInputControl playerControl;
    private Animator animator = null;
    private MoveModel moveModel;

    private void Start()
    {
        animator = GetComponent<Animator>();
        playerControl = new PlayerInputControl();
        SetModel();
        InputSubscrive();
    }

    private void OnDestroy()
    {
        InputUnSubscrive();
    }

    private void SetModel()
    {
        moveModel = new MoveModel();
        moveModel.InitModel();
        WalkBehaviour[] walk = animator.GetBehaviours<WalkBehaviour>();

        foreach (WalkBehaviour walkBehaviour in walk)
        {
            moveModel.OnChangeMoveInfo += walkBehaviour.SetMoveInfo;
        }
    }

    private void InputSubscrive()
    {
        playerControl.Enable();
        playerControl.Player.Move.started += OnStartedMove;
        playerControl.Player.Move.performed += OnPerformedMove;
        playerControl.Player.Move.canceled += OnCanceledMove;
        playerControl.Player.Jump.started += OnStartedJump;
        playerControl.Player.Run.started += OnStartedRun;
        playerControl.Player.Run.canceled += OnCancledRun;
    }

    private void InputUnSubscrive()
    {
        playerControl.Disable();
        playerControl.Player.Move.started -= OnStartedMove;
        playerControl.Player.Move.performed -= OnPerformedMove;
        playerControl.Player.Move.canceled -= OnCanceledMove;
        playerControl.Player.Jump.started -= OnStartedJump;
        playerControl.Player.Run.started -= OnStartedRun;
        playerControl.Player.Run.canceled -= OnCancledRun;
    }
    
    private void OnStartedMove(InputAction.CallbackContext context)
    {
        animator.SetBool("Walk",true);
        moveModel.OnChangeMoveState(context);
    }
    
    private void OnPerformedMove(InputAction.CallbackContext context)
    {
        moveModel.OnChangeMoveState(context);
    }
    
    private void OnCanceledMove(InputAction.CallbackContext context)
    {
        moveModel.OnChangeMoveState(context);
        animator.SetBool("Walk",false);
    }

    private void OnStartedJump(InputAction.CallbackContext context)
    {
        animator.SetTrigger("Jump");
    }

    private void OnStartedRun(InputAction.CallbackContext context)
    {
        animator.SetBool("Run",true);
    }

    private void OnCancledRun(InputAction.CallbackContext context)
    {
        animator.SetBool("Run",false);
    }
}
