using TMPro;
using UnityEngine;

public class ChatItem : MonoBehaviour
{
    [SerializeField] private TMP_Text senderText;
    [SerializeField] private TMP_Text messageText;

    public void SetChatMessage(string sender, string message)
    {
        senderText.text = sender;
        messageText.text = message;
        gameObject.SetActive(true);
    }
}
