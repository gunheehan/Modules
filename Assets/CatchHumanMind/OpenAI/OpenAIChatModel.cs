using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class OpenAIChatModel : MonoBehaviour
{
    public void RequestOpenAI(string prompt, Action<string> callback)
    {
        OpenAIMessage[] ChatGPTMessage = new OpenAIMessage[]
        {
            new OpenAIMessage()
            {
                role = OpenAIInfo.USER_ROLE,
                content = prompt,
            },
        };

        StartCoroutine(RequestOpenAIChat(ChatGPTMessage, callback));
    }
    
    IEnumerator RequestOpenAIChat(OpenAIMessage[] ChatGPTMessage, Action<string> callback)
    {
        string modelType = OpenAIInfo.PreviewModel;
        string jsonData = ConvertChatmessageDataToJson(ChatGPTMessage);
        string modelData = $"{{\"model\": \"{modelType}\", \"messages\": {jsonData}}}";
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(modelData);
        
        string url = OpenAIInfo.OpenAIRootUrl + OpenAIInfo.OpenAICompletions;
        using UnityWebRequest uwr = new UnityWebRequest(url, "POST");
        uwr.uploadHandler = new UploadHandlerRaw(jsonToSend);
        uwr.downloadHandler = new DownloadHandlerBuffer();
        uwr.SetRequestHeader("Content-Type", "application/json");
        uwr.SetRequestHeader("Authorization", "Bearer " + OpenAIInfo.OpenAiAuthKey);
        uwr.SetRequestHeader("OpenAI-Organization", OpenAIInfo.OpenAiOrganization);
        
        yield return uwr.SendWebRequest();

        if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log(uwr.downloadHandler.text);
        }
        else
        {
            OpenAIResponse response = JsonUtility.FromJson<OpenAIResponse>(uwr.downloadHandler.text);
            string message = GetOpenAIMessage(response);
            callback?.Invoke(message);
        }
    }

    private string ConvertChatmessageDataToJson(OpenAIMessage[] message)
    {
        string returnValue = "[";
        for (int i = 0; i < message.Length; i++)
        {
            returnValue += JsonUtility.ToJson(message[i]);
            if (i < message.Length - 1)
            {
                returnValue += ",";
            }
        }
        returnValue += "]";
        return returnValue;
    }

    private string GetOpenAIMessage(OpenAIResponse response)
    {
        if (response.choices.Length < 2)
            return response.choices[0].message.content;
        
        string message = String.Empty;

        foreach (OpenAIResult choiceses in response.choices)
        {
            message += choiceses.message;
        }

        return message;
    }
}
