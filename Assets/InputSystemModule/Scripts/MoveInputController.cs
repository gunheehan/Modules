using UnityEngine;
using UnityEngine.InputSystem;

public class MoveInputController : MonoBehaviour
{
    private PlayerInputControl playerControl;
    private Animator animator = null;
    private MoveModel moveModel;
    private CharacterMove characterMove;

    private void Start()
    {
        animator = GetComponent<Animator>();
        characterMove = gameObject.AddComponent<CharacterMove>();
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
        WalkBehaviour walk = animator.GetBehaviour<WalkBehaviour>();
        moveModel.SetMoveModel(walk);
    }

    private void InputSubscrive()
    {
        playerControl.Enable();
        playerControl.Player.Move.started += OnStartedMove;
        playerControl.Player.Move.performed += OnPerformedMove;
        playerControl.Player.Move.canceled += OnCanceledMove;
        playerControl.Player.Jump.started += OnStartedJump;

        moveModel.OnMoveEvent += characterMove.UpdateMoveState;
    }

    private void InputUnSubscrive()
    {
        playerControl.Disable();
        playerControl.Player.Move.started -= OnStartedMove;
        playerControl.Player.Move.performed -= OnPerformedMove;
        playerControl.Player.Move.canceled -= OnCanceledMove;
        playerControl.Player.Jump.started -= OnStartedJump;

        moveModel.OnMoveEvent -= characterMove.UpdateMoveState;
    }
    
    private void OnStartedMove(InputAction.CallbackContext context)
    {
        Debug.Log("On Start Move");
        animator.SetBool("Walk",true);
        moveModel.OnChangeMoveState(context);
    }
    
    private void OnPerformedMove(InputAction.CallbackContext context)
    {
        Debug.Log("On Move Chnage");
        moveModel.OnChangeMoveState(context);
    }
    
    private void OnCanceledMove(InputAction.CallbackContext context)
    {
        Debug.Log("End Move");
        moveModel.OnChangeMoveState(context);
        animator.SetBool("Walk",false);
    }

    private void OnStartedJump(InputAction.CallbackContext context)
    {
        animator.SetTrigger("Jump");
    }
}
