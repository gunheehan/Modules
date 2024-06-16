using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HumanMindPlayView : MonoBehaviour
{
    public Action UpdatePlayState = null;
    public Action OnClickSubmit = null;

    public Action<string,string> OnClickImageRequest = null;

    [SerializeField] private TMP_Text keyWordText;
    [SerializeField] private LabelWithBackground[] banWordLabelArr;
    [SerializeField] private RectTransform banWordListRect;
    [SerializeField] private TMP_Text processStateText;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private Image viewImage;
    [SerializeField] private Text promptGuideText;
    [SerializeField] private InputField promptInputField;
    [SerializeField] private Button requestButton;
    [SerializeField] private Slider timeSlider;
    [SerializeField] private GameObject choiceTurnButtonView;
    [SerializeField] private Button choicePassTrunButton;
    [SerializeField] private Button choiceSubmitButton;

    private string[] banwordArr;
    
    private void Start()
    {
        Init();
        ResetBanWord();
        timeSlider.maxValue = 1;
        timeSlider.value = 0;
        choicePassTrunButton.onClick.AddListener(OnClickPassTurnButton);
        choiceSubmitButton.onClick.AddListener(OnClickCheckImageAnswer);
        timeSlider.gameObject.SetActive(false);
        choiceTurnButtonView.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        viewImage.sprite = null;
        processStateText.text = string.Empty;
    }

    private void Init()
    {
        requestButton.onClick.AddListener(OnClickRequest);
    }

    private void ResetSilde()
    {
        timeSlider.value = 0;
    }

    private void ResetBanWord()
    {
        foreach (LabelWithBackground label in banWordLabelArr)
        {
            label.UpdateLabel(string.Empty);
            label.gameObject.SetActive(false);
        }
    }

    private void OnClickRequest()
    {
        if (string.IsNullOrEmpty(promptInputField.text))
            return;
        
        string style = string.Empty;

        bool isInvaild = CheckInValidPrompt(promptInputField.text);

        if (!isInvaild)
        {
            processStateText.text = HumanMindText.PROMPTERROR;
            return;
        }

        OnClickImageRequest?.Invoke(promptInputField.text, style);
        processStateText.text = HumanMindText.IMAGECREATING;
        promptInputField.text = string.Empty;
        requestButton.gameObject.SetActive(false);
    }
    
    public void UpdateTimer(float time)
    {
        timeSlider.value = time;
    }

    public void SetPlayInfo(HumanMindData gameData)
    {
        ResetBanWord();
        timeSlider.gameObject.SetActive(true);

        keyWordText.text = gameData.keyWord;

        banwordArr = gameData.banWordArr;

        for (int i = 0; i < banwordArr.Length; i++)
        {
            banWordLabelArr[i].UpdateLabel(banwordArr[i]);
            banWordLabelArr[i].gameObject.SetActive(true);
        }
        
        scoreText.text = $"{gameData.score}";
        LayoutRebuilder.ForceRebuildLayoutImmediate(banWordListRect);
    }

    public void SetDataResponse(string response)
    {
        processStateText.text = response;
    }

    public void SetErrorText(string errortext)
    {
        processStateText.text = errortext;
    }

    public void SetOwnerTurnView(bool isOn, HumanMindStateType gameState)
    {
        if (gameState == HumanMindStateType.CREATEIMAGE)
        {
            requestButton.gameObject.SetActive(isOn);
            SetChoiceView(false);
            viewImage.sprite = null;
            if (isOn)
                promptGuideText.text = HumanMindText.CREATEIMAGE;
            else
                promptGuideText.text = HumanMindText.WAITCREATEIMAGE;
        }
        else if (gameState == HumanMindStateType.CHOICETURN)
            SetChoiceView(isOn);
        else if(gameState == HumanMindStateType.WAITREQUEST)
        {
            requestButton.gameObject.SetActive(false);
            SetChoiceView(false);
        }
    }

    public void SetImage(Sprite sprite)
    {
        processStateText.text = String.Empty;
        viewImage.sprite = sprite;
    }

    private bool CheckInValidPrompt(string prompt)
    {
        for (int i = 0; i < banwordArr.Length; i++)
        {
            if (prompt.Contains(banwordArr[i]))
                return false;
        }

        if (prompt.Contains(keyWordText.text))
            return false;

        return true;
    }

    private void SetChoiceView(bool isOn)
    {
        choiceTurnButtonView.SetActive(isOn);
    }

    private void OnClickPassTurnButton()
    {
        UpdatePlayState?.Invoke();
        ResetSilde();
        SetChoiceView(false);
    }

    private void OnClickCheckImageAnswer()
    {
        OnClickSubmit?.Invoke();
        ResetSilde();
        SetChoiceView(false);
    }
}
