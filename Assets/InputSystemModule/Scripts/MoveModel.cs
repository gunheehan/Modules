using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class MoveModel
{
    public event Action<Vector3, float> OnMoveEvent = null; 
    
    private WalkBehaviour walkBehaviour = null;
    private Vector2 inputDirection;
    private Vector3 direction;
    private float speed;
    private Camera cam;
    
    private readonly float MAXSPEED = 3f;
    private readonly float BOOSTSPEED = 2f;
    private readonly float MAXSPEEDRANGE = 0.75f;
    private readonly float MINSPEEDRANGE = 0.25f;

    public void SetMoveModel(WalkBehaviour behaviour)
    {
        walkBehaviour = behaviour;
        cam = Camera.main;
    }
    
    public void OnChangeMoveState(InputAction.CallbackContext context)
    {
        if (cam == null)
            return;
        
        Vector2 input = context.ReadValue<Vector2>();

        if (inputDirection == input)
            return;
        
        float distance = Vector2.Distance(Vector2.zero, input);
        distance = distance > 1 ? 1 : distance;

        float animationSpeed = 0f;

        if (distance > 0.01f)
            animationSpeed = distance / 2f + 0.5f;

        input = context.ReadValue<Vector2>();
        float newSpeed = animationSpeed * MAXSPEED * MINSPEEDRANGE;
        speed = newSpeed + (MAXSPEED * MAXSPEEDRANGE);

        direction.x = input.x;
        direction.z = input.y;
        direction = direction.normalized;

        Quaternion v3Rotation = Quaternion.Euler(0f, cam.transform.localEulerAngles.y, 0f);
        direction = v3Rotation * direction;

        inputDirection = input;
        OnMoveEvent?.Invoke(direction, speed);
    }
}
