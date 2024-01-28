using UnityEngine;

public class CharacterMove : MonoBehaviour
{
    private readonly float rotationFactorPerFrame = 0.05f;

    public void UpdateMoveState(Vector3 direction, float speed)
    {
        if (direction.sqrMagnitude <= 0)
            return;
        
        Quaternion currentRotation = transform.rotation;
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, rotationFactorPerFrame);
        transform.position = Vector3.Lerp(transform.position, transform.position + direction, speed * Time.deltaTime);
    }
}
