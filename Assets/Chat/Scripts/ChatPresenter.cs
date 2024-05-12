using System.Collections.Generic;
using UnityEngine;

public class ChatPresenter : MonoBehaviour
{
    [SerializeField] private ChatView view;
    [SerializeField] private ChatClientModel clientModel;
    private List<ChatMessageModel> messageModel;

    private string currentChannel;

    private void Start()
    {
        messageModel = new List<ChatMessageModel>();

        clientModel.ConnectChat(new string[] { "Default" });

        view.SendMessage += clientModel.OnSendMessage;
        clientModel.OnConnectNewChat += InitMessageModel;
    }
    
    public void InitMessageModel(string currentChannel, string channel)
    {
        Debug.Log("Connect New Channel : " + channel);
        ChatMessageModel model = new ChatMessageModel();
        model.ChannelName = channel;
        model.SetCurrentChatChannel(currentChannel);
        clientModel.OnGetMessage += model.OnUpdateChat;
        model.UpdateChatMessage += view.UpdateChatItem;
        
        messageModel.Add(model);
    }
}
