using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Button))]
public class CategorySelect : MonoBehaviour
{
    public Action<string> SendCategoryWord;
    public Action<string> OnSelectCategory;
    
    [SerializeField] private CategoryScriptable categoryScriptable;
    [SerializeField] private TextMeshProUGUI categoryText;

    private Button categoryButton;
    
    private void Awake()
    {
        categoryButton = GetComponent<Button>();
        categoryButton.onClick.AddListener(OnClickCategoryButton);
    }

    public void CategoryButtonInit()
    {
        categoryText.text = categoryScriptable.Category;
        categoryButton.enabled = true;
    }

    public void OnClickCategoryButton()
    {
        categoryButton.enabled = false;
        
        int rand = Random.Range(0, categoryScriptable.Words.Length);
        categoryText.text = categoryScriptable.Words[rand];
        
        SendCategoryWord?.Invoke(categoryScriptable.Category);
        OnSelectCategory?.Invoke(categoryScriptable.Words[rand]);
    }
}