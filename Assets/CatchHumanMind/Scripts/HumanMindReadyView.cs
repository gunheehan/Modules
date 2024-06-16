using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class HumanMindReadyView : MonoBehaviour
{
    public Action<string> SendPlayerInput = null;
    public Action OnGameStart = null;
    
    [SerializeField] private CategorySelect[] categorySelects;
    [SerializeField] private GameObject categoryContents;
    [SerializeField] private TMP_Text stateTitleText;
    [SerializeField] private TMP_Text stateDtailText;
    [SerializeField] private InputField inputField;
    [SerializeField] private Button gameStartButton;
    [SerializeField] private Button sendInputTextButton;
    [SerializeField] private Slider timeSlider;

    private void Start()
    {
        sendInputTextButton.onClick.AddListener(OnClickSendInputText);
        gameStartButton.onClick.AddListener(OnClickGameStart);
        timeSlider.maxValue = 1;
        timeSlider.value = 0;

        foreach (CategorySelect categorySelect in categorySelects)
        {
            categorySelect.CategoryButtonInit();
            categorySelect.OnSelectCategory = OnClickCategory;
        }
        gameObject.SetActive(false);
    }
    
    public void UpdateTimer(float time)
    {
        timeSlider.value = time;
    }

    public void UpdateStateInfo(string title, string subtitle)
    {
        stateTitleText.text = title;
        stateDtailText.text = subtitle;
        inputField.text = string.Empty;
        sendInputTextButton.gameObject.SetActive(true);
        gameStartButton.gameObject.SetActive(false);
        categoryContents.SetActive(false);
        gameObject.SetActive(true);
    }

    public void SetCategoryList(bool isMaster)
    {
        foreach (CategorySelect categorySelect in categorySelects)
        {
            categorySelect.CategoryButtonInit();
        }
        
        categoryContents.SetActive(isMaster);
    }

    public void SetStartButton(bool isOn, string gameInfo)
    {
        stateDtailText.text = gameInfo;
        gameStartButton.gameObject.SetActive(isOn);
        categoryContents.SetActive(false);
    }

    public void SetRandomKeyword()
    {
        int rand = Random.Range(0, categorySelects.Length);
        categorySelects[rand].OnClickCategoryButton();
    }

    private void OnClickGameStart()
    {
        OnGameStart?.Invoke();
        gameStartButton.gameObject.SetActive(false);
    }

    private void OnClickSendInputText()
    {
        sendInputTextButton.gameObject.SetActive(false);
        SendPlayerInput?.Invoke(inputField.text);
        inputField.text = String.Empty;
    }

    private void OnClickCategory(string keyWord)
    {
        SendPlayerInput?.Invoke(keyWord);
        categoryContents.SetActive(false);
    }
}
