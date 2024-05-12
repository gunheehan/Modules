using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChatView : MonoBehaviour
{
    public event Action<string> SendMessage = null;
    
    [SerializeField] private ChatItem item;
    [SerializeField] private Transform itemContents;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private Button sendButton;
    
    void Start()
    {
        sendButton.onClick.AddListener(OnClickSendMessage);
    }

    private void OnClickSendMessage()
    {
        if (string.IsNullOrEmpty(inputField.text))
            return;
        
        SendMessage?.Invoke(inputField.text);
        inputField.text = string.Empty;
    }

    public void UpdateChatItem(string sender, string message)
    {
        ChatItem newItem = Instantiate(item, itemContents);
        newItem.SetChatMessage(sender, message);
    }
}
