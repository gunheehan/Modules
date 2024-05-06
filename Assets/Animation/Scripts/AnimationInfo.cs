public class AnimationInfo
{
    public static string ANI_SPEED = "Speed";
    public static string ANI_RUN = "Run";
    public static string ANI_JUMP = "Jump";
    public static string ANI_EMOTION = "Emotion";
    public static string ANI_CYCLEEMOTION = "CycleEmotion";
    public static string ANI_EFFECT = "Effect";

    public static string ANI_CHEERING = "IsCheering";
    
    public enum EmotionAnim
    {
        Greet,
        Flower,
        Fighting,
        Clap,
        Nod,
        Shame,
    }

    public enum CycleAnim
    {
        None = -1,
        Sit,
        Dance,
        Dance2,
        Cheering
    }

    public enum EffectAnim
    {
        None = -1,
        Hit
    }
}