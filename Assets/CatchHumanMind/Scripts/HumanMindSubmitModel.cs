using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;


public class HumanMindSubmitModel : MonoBehaviour
{
    public Action<string> OnReceiveSubmitResult = null;

    public void RequestImageToOpenAI(string base64Image)
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
                    ""text"": ""다음 그림을 보고 떠오르는 단어만 출력해줘""
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
        StartCoroutine(RequestOpenAIChat(jsonData, null));
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
            Debug.Log(uwr.downloadHandler.text);
            OpenAIVisionResponse response = JsonUtility.FromJson<OpenAIVisionResponse>(uwr.downloadHandler.text);
            string resultMessage = GetOpenAIMessage(response);
            Debug.Log(resultMessage);
            OnReceiveSubmitResult?.Invoke(resultMessage);
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
