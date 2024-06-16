using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WordCategory", menuName = "WordCategory", order = 0)]
public class CategoryScriptable : ScriptableObject
{
    [SerializeField] private string category;
    public string Category => category;
    
    [SerializeField] private string[] words;
    public string[] Words => words;

}