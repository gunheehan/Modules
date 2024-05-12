using System;
using System.Collections.Generic;

public class ChatMessageModel
{
    public Action<string, string> UpdateChatMessage = null;

    private List<ChatMessage> messageList = new List<ChatMessage>();

    private string channelName;
    public string ChannelName
    {
        set { channelName = value; }
    }
    private string currentChat;

    public void SetCurrentChatChannel(string channel)
    {
        currentChat = channel;

        if (!channelName.Equals(currentChat))
            return;

        foreach (ChatMessage chatData in messageList)
            UpdateChatMessage?.Invoke(chatData.sender, chatData.message);
    }

    public void OnUpdateChat(ChatMessage chatData)
    {
        if (!channelName.Equals(chatData.channel))
            return;

        messageList.Add(chatData);

        if (!currentChat.Equals(chatData.channel))
            return;
        
        UpdateChatMessage?.Invoke(chatData.sender, chatData.message);
    }
}
