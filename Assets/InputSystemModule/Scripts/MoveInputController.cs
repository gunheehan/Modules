using UnityEngine;
using UnityEngine.InputSystem;

public class MoveInputController : MonoBehaviour
{
    private PlayerInputControl playerControl;

    private void Start()
    {
        playerControl = new PlayerInputControl();
        InputSubscrive();
    }

    private void OnDestroy()
    {
        InputUnSubscrive();
    }

    private void InputSubscrive()
    {
        playerControl.Enable();
        playerControl.Player.Move.started += OnStartedMove;
        playerControl.Player.Move.performed += OnPerformedMove;
        playerControl.Player.Move.canceled += OnCanceledMove;
    }

    private void InputUnSubscrive()
    {
        playerControl.Disable();
        playerControl.Player.Move.started -= OnStartedMove;
        playerControl.Player.Move.performed -= OnPerformedMove;
        playerControl.Player.Move.canceled -= OnCanceledMove;
    }
    
    private void OnStartedMove(InputAction.CallbackContext context)
    {
        Debug.Log("On Start Move");
    }
    
    private void OnPerformedMove(InputAction.CallbackContext context)
    {
        Debug.Log("On Move Chnage");
    }
    
    private void OnCanceledMove(InputAction.CallbackContext context)
    {
        Debug.Log("End Move");
    }
}
