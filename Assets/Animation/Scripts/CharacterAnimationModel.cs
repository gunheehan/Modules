using UnityEngine;

public class CharacterAnimationModel
{
    public Animator Animator;
    
    private readonly string Speed = "Speed";
    private readonly string RUN = "Run";
    private readonly string JUMP = "Jump";
    private readonly string EMOTIONSTATE = "Emotion";
    private readonly string CYCLEEMOTIONSTATE = "CycleEmotion"; 
    private readonly string EFFECT = "Effect";

    private AnimationInfo.CycleAnim currentCycle;

    public void PlayWalk(float moveSpeed)
    {
        Animator.SetFloat(Speed, moveSpeed);
    }

    public void PlayRun(bool isrun)
    {
        Animator.SetBool(RUN, isrun);
    }

    public void PlayJump()
    {
        Animator.SetTrigger(JUMP);
    }

    public void PlayEmotion(AnimationInfo.EmotionAnim emotion)
    {
        PlayWalk(0);
        Animator.SetInteger(EMOTIONSTATE, (int)emotion);
    }

    public void PlayCycleEmotion(AnimationInfo.CycleAnim cycleEmotion)
    {
        PlayWalk(0);
        if (currentCycle == cycleEmotion)
            currentCycle = AnimationInfo.CycleAnim.None;
        else
            currentCycle = cycleEmotion;

        Animator.SetInteger(CYCLEEMOTIONSTATE, (int)currentCycle);
    }

    public void PlayPressEmotion(string pressName, bool isPress)
    {
        Debug.Log("pressName : " + pressName);
        Animator.SetBool(pressName, isPress);
    }

    public void PlayEffect(AnimationInfo.EffectAnim effect)
    {
        PlayWalk(0);
        Animator.SetInteger(EFFECT, (int)effect);
    }
}