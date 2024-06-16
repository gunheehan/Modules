using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class OpenAIImageModel : MonoBehaviour
{
    public void RequestImageToOpenAI(string prompt, string base64Image, Action<string> callback)
    {
        string jsonData = $@"
{{
    ""model"": ""{OpenAIInfo.VisionModel}"",
    ""messages"": [
        {{
            ""role"": ""user"",
            ""content"": [
                {{
                    ""type"": ""text"",
                    ""text"": ""{prompt}""
                }},
                {{
                    ""type"": ""image_url"",
                    ""image_url"": {{
                        ""url"": ""data:image/jpeg;base64,{base64Image}""
                    }}
                }}
            ]
        }}
    ],
    ""max_tokens"": 300
}}";
        StartCoroutine(RequestOpenAIChat(jsonData, callback));
    }
    
    IEnumerator RequestOpenAIChat(string jsonData, Action<string> callback)
    {
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonData);
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
            OpenAIVisionResponse response = JsonUtility.FromJson<OpenAIVisionResponse>(uwr.downloadHandler.text);
            string resultMessage = GetOpenAIMessage(response);
            callback?.Invoke(resultMessage);
        }
    }
    
    private string GetOpenAIMessage(OpenAIVisionResponse response)
    {
        if (response.choices.Count < 2)
            return response.choices[0].message.content;
        
        string message = String.Empty;

        foreach (OpenAIResult choiceses in response.choices)
        {
            message += choiceses.message;
        }

        return message;
    }
}
