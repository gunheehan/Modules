using UnityEngine;
using UnityEngine.InputSystem;

public class MoveInputController : MonoBehaviour
{
    private PlayerInputControl playerControl;
    private TouchInput touchInput;
    private CharacterAnimationModel _animationModel = null;
    private MoveModel moveModel;

    public CharacterAnimationModel animationModel
    {
        get
        {
            if (_animationModel == null)
                _animationModel = new CharacterAnimationModel();
            return _animationModel;
        }
    }
    
    private void Start()
    {
        animationModel.Animator = GetComponent<Animator>();
        playerControl = new PlayerInputControl();
        touchInput = new TouchInput();
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

        WalkBehaviour[] walk = animationModel.Animator.GetBehaviours<WalkBehaviour>();

        foreach (WalkBehaviour walkBehaviour in walk)
        {
            moveModel.OnChangeMoveInfo += walkBehaviour.SetMoveInfo;
        }
        moveModel.OnMoveUpdate += animationModel.PlayWalk;
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

        // touchInput.Enable();
        // touchInput.Touch.TouchPress.started += OnUpdateTabMove;
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

        // touchInput.Disable();
        // touchInput.Touch.TouchPress.started -= OnUpdateTabMove;
    }
    
    private void OnStartedMove(InputAction.CallbackContext context)
    {
        moveModel.OnChangeMoveState(context);
    }
    
    private void OnPerformedMove(InputAction.CallbackContext context)
    {
        moveModel.OnChangeMoveState(context);
    }
    
    private void OnCanceledMove(InputAction.CallbackContext context)
    {
        moveModel.OnChangeMoveState(context);
    }

    private void OnStartedJump(InputAction.CallbackContext context)
    {
        animationModel.PlayJump();
    }

    private void OnStartedRun(InputAction.CallbackContext context)
    {
        animationModel.PlayRun(true);
    }

    private void OnCancledRun(InputAction.CallbackContext context)
    {
        animationModel.PlayRun(false);
    }

    private void OnUpdateTabMove(InputAction.CallbackContext context)
    {
        Debug.Log("GetTabMove : " + touchInput.Touch.TouchPosition.ReadValue<Vector2>());
        
        Vector2 input = touchInput.Touch.TouchPosition.ReadValue<Vector2>();

        PointMoveController pointMoveController = Camera.main.gameObject.GetComponent<PointMoveController>();
        pointMoveController.CheckPointMove(input);
    }
}
