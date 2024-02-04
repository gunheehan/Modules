using System;
using UnityEngine;

public class RunBehaviour : StateMachineBehaviour
{
    public event Action RunEvent = null;
    public event Action<int> RunCountEvent = null;
    
    public float[] footDownThreshold; // 발이 땅에 닿는 순간 임계값
    private int wasFootCount = 0; // 발이 땅에 있었는지 여부 확인
    
     //OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    // override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    // {
    //     
    // }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        RunEvent?.Invoke();

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
}
