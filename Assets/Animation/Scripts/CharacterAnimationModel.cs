using UnityEngine;

public class CharacterAnimationModel
{
    public Animator Animator;

    private AnimationInfo.CycleAnim currentCycle;

    public void PlayWalk(float moveSpeed)
    {
        Animator.SetFloat(AnimationInfo.ANI_SPEED, moveSpeed);
    }

    public void PlayRun(bool isrun)
    {
        Animator.SetBool(AnimationInfo.ANI_RUN, isrun);
    }

    public void PlayJump()
    {
        Animator.SetTrigger(AnimationInfo.ANI_JUMP);
    }

    public void PlayEmotion(AnimationInfo.EmotionAnim emotion)
    {
        PlayWalk(0);
        Animator.SetInteger(AnimationInfo.ANI_EMOTION, (int)emotion);
    }

    public void PlayCycleEmotion(AnimationInfo.CycleAnim cycleEmotion)
    {
        PlayWalk(0);
        if (currentCycle == cycleEmotion)
            currentCycle = AnimationInfo.CycleAnim.None;
        else
            currentCycle = cycleEmotion;

        Animator.SetInteger(AnimationInfo.ANI_CYCLEEMOTION, (int)currentCycle);
    }

    public void PlayPressEmotion(string pressName, bool isPress)
    {
        Debug.Log("pressName : " + pressName);
        Animator.SetBool(pressName, isPress);
    }

    public void PlayEffect(AnimationInfo.EffectAnim effect)
    {
        PlayWalk(0);
        Animator.SetInteger(AnimationInfo.ANI_EFFECT, (int)effect);
    }
}