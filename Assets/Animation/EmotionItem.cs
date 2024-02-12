using UnityEngine;
using UnityEngine.UI;

public class EmotionItem : MonoBehaviour
{
    [SerializeField] private int animationNumber;

    [SerializeField] private Animator animator;
    private Button clickButton;

    private readonly string EMOTION = "Emotion";
    // Start is called before the first frame update
    void Start()
    {
        clickButton = gameObject.GetComponent<Button>();
        clickButton.onClick.AddListener(()=>OnClickEmotion(animationNumber));
    }

    private void OnClickEmotion(int number)
    {
        animator.SetInteger(EMOTION, number);
    }
}
