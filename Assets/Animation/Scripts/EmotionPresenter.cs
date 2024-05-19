using UnityEngine;

public class EmotionPresenter : MonoBehaviour
{
    [SerializeField] private EmotionAnimItem[] emotionItemAnims;
    [SerializeField] private CycleAnimItem[] cycleAnimItmes;
    private CharacterAnimationModel animationModel;

    public void InitAnimationModel(GameObject player)
    {
        animationModel = player.GetComponent<MoveInputController>().animationModel;
        if (animationModel == null)
        {
            Debug.Log("Animation Model Init Error");
            return;
        }
        
        InitItemes();
    }

    private void InitItemes()
    {
        foreach (EmotionAnimItem emotionItem in emotionItemAnims)
        {
            emotionItem.PlayEmotion += animationModel.PlayEmotion;
        }

        foreach (CycleAnimItem cycleItem in cycleAnimItmes)
        {
            cycleItem.PlayCycleEmotion += animationModel.PlayCycleEmotion;
        }
    }
}
