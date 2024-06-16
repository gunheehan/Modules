using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HumanMindEndView : MonoBehaviour
{
    public Action OnClickLeave = null;

    [SerializeField] private TMP_Text playerNickname;
    [SerializeField] private TMP_Text responseText;
    [SerializeField] private Slider timeSlider;
    [SerializeField] private Button leaveButton;


    private void Start()
    {
        timeSlider.maxValue = 1;
        timeSlider.value = 0;
        leaveButton.onClick.AddListener(() => OnClickLeave?.Invoke());
        leaveButton.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }

    public void UpdateTimer(float time)
    {
        timeSlider.value = time;
    }

    public void UpdateAnswerInfo(string nickname)
    {
        playerNickname.text = nickname;
    }
    public void SetDataResponse(string response)
    {
        responseText.text = response;
    }

    public void OnEndRound()
    {
        playerNickname.text = string.Empty;
        responseText.text = string.Empty;
    }

    public void OnEndGame()
    {
        leaveButton.gameObject.SetActive(true);
    }

    public void OnPlayStageEffect(bool isClear)
    {
        Debug.Log("stage effect " + isClear);
    }
}