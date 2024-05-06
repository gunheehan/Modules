using System;
using UnityEngine;

public class WalkBehaviour : StateMachineBehaviour
{
    private GameObject avatar;
    //walk 0.1 , run 0.5
    [SerializeField]private float rotationFactorPerFrame = 0.1f;

    //속도 배율
    [SerializeField] private float speedMagnification = 1f;
    private Vector3 direction = Vector3.zero;
    private float moveSpeed = 0f;

    public float[] footDownThreshold;
    private int wasFootCount = 0;
    
     //OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        avatar = animator.gameObject;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        UpdateMoveState();

        float normalizeTime = stateInfo.normalizedTime % 1;

        if (Mathf.Abs(normalizeTime - footDownThreshold[wasFootCount])<0.05f)
        {
            wasFootCount++;
            if (wasFootCount > footDownThreshold.Length - 1)
                wasFootCount = 0;
        }
    }
    public void SetMoveInfo(Vector3 direct, float speed)
    {
        direction = direct;
        moveSpeed = speed;
    }
    
    private void UpdateMoveState()
    {
        if (direction.sqrMagnitude <= 0)
            return;
        
        Quaternion currentRotation = avatar.transform.rotation;
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        avatar.transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, rotationFactorPerFrame);
        avatar.transform.position = Vector3.Lerp(avatar.transform.position, avatar.transform.position + direction, (moveSpeed*speedMagnification )* Time.deltaTime);
    }
    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}
    
}
