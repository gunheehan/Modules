using System;
using ExitGames.Client.Photon;
using Photon.Chat;
using Photon.Pun;
using UnityEngine;

public struct ChatMessage
{
    public string channel;
    public string sender;
    public string message;
}

public class ChatClientModel : MonoBehaviour, IChatClientListener
{
    public event Action<ChatMessage> OnGetMessage = null;
    public event Action<string, string> OnConnectNewChat = null;
    private ChatClient chatClient;

    private string[] channel;
    private string currentChaneel;

    private void OnDestroy()
    {
        chatClient?.Disconnect();
    }

    private void Update()
    {
        chatClient?.Service();
    }

    public void ConnectChat(string[] channelList)
    {
        channel = channelList;
        chatClient = new ChatClient(this);
        
        ChatAppSettings chatAppSettings = new ChatAppSettings
        {
            AppIdChat = PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat,
            AppVersion = PhotonNetwork.PhotonServerSettings.AppSettings.AppVersion
        };

        chatClient.ConnectUsingSettings(chatAppSettings);
    }

    public void OnSendMessage(string message)
    {
        chatClient?.PublishMessage(currentChaneel, message);
    }

    #region ChatClientOverride
    
    public void DebugReturn(DebugLevel level, string message) { }

    public void OnDisconnected() { }

    public void OnConnected()
    {
        currentChaneel = channel[0];
        chatClient.Subscribe(channel);
    }

    public void OnChatStateChange(ChatState state) { }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        for (int i = 0; i < senders.Length; i++)
        {
            ChatMessage chatData = new ChatMessage()
            {
                channel = channelName,
                sender = senders[i],
                message = (string)messages[i]
            };
            OnGetMessage?.Invoke(chatData);
        }
    }

    public void OnPrivateMessage(string sender, object message, string channelName) { }

    public void OnSubscribed(string[] channels, bool[] results)
    {
        foreach (string channel in channels)
        {
            OnConnectNewChat?.Invoke(currentChaneel, channel);
        }
    }

    public void OnUnsubscribed(string[] channels) { }

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message) { }

    public void OnUserSubscribed(string channel, string user) { }

    public void OnUserUnsubscribed(string channel, string user) { }
    
    #endregion
}
