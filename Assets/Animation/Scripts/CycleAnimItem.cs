using System;
using UnityEngine;
using UnityEngine.UI;

public class CycleAnimItem : MonoBehaviour
{
    public Action<AnimationInfo.CycleAnim> PlayCycleEmotion = null;

    [SerializeField] private AnimationInfo.CycleAnim cycleEmotion;
    [SerializeField] private Button emotionButton;

    private void Start()
    {
        emotionButton.onClick.AddListener(OnClickEmotionButton);
    }

    private void OnClickEmotionButton()
    {
        PlayCycleEmotion?.Invoke(cycleEmotion);
    }
}
