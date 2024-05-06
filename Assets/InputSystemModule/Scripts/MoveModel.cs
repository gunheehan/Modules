using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

[RequireComponent (typeof(PlayerInput))]
public class MoveModel
{
    public event Action<Vector3, float> OnChangeMoveInfo = null;
    public event Action<float> OnMoveUpdate = null;
    private Vector3 direction;
    private float speed;
    private bool isBoost = false;

    private Camera cam => Camera.main;
    
    private readonly float MAXSPEED = 3f;
    private readonly float BOOSTSPEED = 2f;
    private readonly float MAXSPEEDRANGE = 0.75f;
    private readonly float MINSPEEDRANGE = 0.25f;


    public void OnChangeMoveState(InputAction.CallbackContext context)
    {
        if (cam == null)
            return;
        
        Vector2 input = context.ReadValue<Vector2>();
        float distance = Vector2.Distance(Vector2.zero, input);
        distance = distance > 1 ? 1 : distance;

        float animationSpeed = 0f;

        if (distance > 0.01f)
            animationSpeed = distance / 2f + 0.5f;

        float newSpeed = animationSpeed * MAXSPEED * MINSPEEDRANGE;
        speed = newSpeed + (MAXSPEED * MAXSPEEDRANGE);

        direction.x = input.x;
        direction.z = input.y;
        direction = direction.normalized;

        Quaternion v3Rotation = Quaternion.Euler(0f, cam.transform.localEulerAngles.y, 0f);
        direction = v3Rotation * direction;
        
        OnChangeMoveInfo?.Invoke(direction, speed);
        OnMoveUpdate?.Invoke(animationSpeed);
    }
    
    public void OnChangeMoveState(Vector2 input)
    {
        if (cam == null)
            return;
        
        float distance = Vector2.Distance(Vector2.zero, input);
        distance = distance > 1 ? 1 : distance;

        float animationSpeed = 0f;

        if (distance > 0.01f)
            animationSpeed = distance / 2f + 0.5f;

        float newSpeed = animationSpeed * MAXSPEED * MINSPEEDRANGE;
        speed = newSpeed + (MAXSPEED * MAXSPEEDRANGE);

        direction.x = input.x;
        direction.z = input.y;
        direction = direction.normalized;

        Quaternion v3Rotation = Quaternion.Euler(0f, cam.transform.localEulerAngles.y, 0f);
        direction = v3Rotation * direction;

        CheckBoostMode(input);
        OnChangeMoveInfo?.Invoke(direction, speed);
        OnMoveUpdate?.Invoke(animationSpeed);
    }
    
    private void CheckBoostMode(Vector2 newPos)
    {
        float distance = Vector2.Distance(Vector2.zero, new Vector2(newPos.x, newPos.y));

        if (distance > 0.9f)
        {
            if (isBoost)
                return;
            isBoost = true;

            KeyboardState newKeyboardState = new KeyboardState(Key.LeftShift);
            InputSystem.QueueStateEvent(Keyboard.current, newKeyboardState); 
        }
        else
        {
            if (!isBoost)
                return;
            isBoost = false;
            
            KeyboardState newKeyboardState = new KeyboardState(Key.LeftShift);
            newKeyboardState.Set(Key.LeftShift, false);
            InputSystem.QueueStateEvent(Keyboard.current, newKeyboardState); 
        }
    }
}
