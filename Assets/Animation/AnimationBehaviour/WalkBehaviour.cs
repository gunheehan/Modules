using System;
using Unity.VisualScripting;
using UnityEngine;

public class WalkBehaviour : StateMachineBehaviour
{
    public event Action<int> walkCountEvent = null;

    private GameObject avatar;
    private readonly float rotationFactorPerFrame = 0.05f;
    
    private Vector3 direction = Vector3.zero;
    private float moveSpeed = 0f;

    public float[] footDownThreshold; // 발이 땅에 닿는 순간 임계값
    private int wasFootCount = 0; // 발이 땅에 있었는지 여부 확인
    
     //OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        avatar = animator.GameObject();
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

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}

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
        avatar.transform.position = Vector3.Lerp(avatar.transform.position, avatar.transform.position + direction, moveSpeed * Time.deltaTime);
    }
}
