using System;
using UnityEngine;
using UnityEngine.UI;

public class EmotionAnimItem : MonoBehaviour
{
    public Action<AnimationInfo.EmotionAnim> PlayEmotion = null;

    [SerializeField] private AnimationInfo.EmotionAnim emotion;
    [SerializeField] private Button emotionButton;

    private void Start()
    {
        emotionButton.onClick.AddListener(OnClickEmotionButton);
    }

    private void OnClickEmotionButton()
    {
        PlayEmotion?.Invoke(emotion);
    }
}
