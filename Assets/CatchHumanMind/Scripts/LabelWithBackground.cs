using TMPro;
using UnityEngine;

public class LabelWithBackground : MonoBehaviour
{
    [SerializeField] private TMP_Text label;

    public void UpdateLabel(string text)
    {
        label.text = text;
    }
}
