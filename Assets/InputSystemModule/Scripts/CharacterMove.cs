using UnityEngine;

public class CharacterMove : MonoBehaviour
{
    private readonly float rotationFactorPerFrame = 0.05f;
    private Vector3 direct = Vector3.zero;
    private float moveSpeed = 0f;

    public void UpdateMoveState(Vector3 direction, float speed)
    {
        // if (direction == direct)
        //     return;
        //
        direct = direction;
        moveSpeed = speed;
    }

    private void Update()
    {
        if (direct.sqrMagnitude <= 0)
            return;
        
        Quaternion currentRotation = transform.rotation;
        Quaternion targetRotation = Quaternion.LookRotation(direct);

        transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, rotationFactorPerFrame);
        transform.position = Vector3.Lerp(transform.position, transform.position + direct, moveSpeed * Time.deltaTime);
    }
}
