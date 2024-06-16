using System;
using System.Collections;
using System.Text;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class StabilityAIModel : MonoBehaviour
{
    public Action<Sprite> OnImageUrlSetting = null;
    public Action<int, string> OnReciveBase64Incoding = null;

    public Action<bool> OnSuccessAction = null;
    private OpenAIChatModel openAIChatModel;

    private readonly string OPENAIPROMPT = "다음 문자를 영어로 번역한 결과만 출력해줘\n";

    private void Start()
    {
        openAIChatModel = gameObject.AddComponent<OpenAIChatModel>();
    }

    public void RequestImageMake(string prompt, string style=null)
    {
        if (!string.IsNullOrEmpty(prompt))
        {
            if (string.IsNullOrEmpty(style))
                style = "none";

   
            string newPrompt = OPENAIPROMPT + prompt;
            openAIChatModel.RequestOpenAI(newPrompt, (translatprompt) =>
            {
                CreateImage(translatprompt, style);
            });
        }
        else
        {
            Debug.Log("스태빌리티 prompt 비어있음");
        }
    }

    private void CreateImage(string prompt, string style)
    {
        StartCoroutine(TextToImageCoroutine(prompt, style));
    }
    
    
    public IEnumerator TextToImageCoroutine(string prompt, string style)
    {
        string apiUrl = "https://api.stability.ai/v1/generation/stable-diffusion-v1-6/text-to-image";
        string apiKey = ""; // 실제 API 키로 교체해야 합니다.
        string json;

        StabilityRequestInfo requestInfo = new StabilityRequestInfo();
        requestInfo.text_prompts = new[]
        {
            new StabilityTextPrompt()
            {
                text = prompt
            }
        };

        json = JsonUtility.ToJson(requestInfo);

        if (!style.Equals("none"))
        {
            JObject jsonObject = JObject.Parse(json);
            jsonObject.Add("style_preset", style);

            json = jsonObject.ToString();
        }

        using (UnityWebRequest request = new UnityWebRequest(apiUrl, "POST"))
        {
            byte[] jsonToSend = new UTF8Encoding().GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(jsonToSend);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", $"Bearer {apiKey}");
            request.SetRequestHeader("Accept", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Error: {request.error}");
                OnReciveBase64Incoding?.Invoke(400, null);
            }
            else
            {
                Debug.Log(request.downloadHandler.text);
                StabilityResponse response =
                    JsonUtility.FromJson<StabilityResponse>(request.downloadHandler.text);
                Debug.Log("response.artifacts  확인 : " + response.artifacts );
                if(response.artifacts != null)
                    ConvertBase64ToSprite(response.artifacts[0].base64,0);
                else               
                    OnReciveBase64Incoding?.Invoke(400, null);
            }
        }
    }
    
    private void ConvertBase64ToSprite(string base64, int successCode)
    {
        byte[] imageBytes = Convert.FromBase64String(base64);
        Texture2D texture = new Texture2D(2, 2);

        if (texture.LoadImage(imageBytes))
        {
            Debug.Log("이미지 바이트 있음");
            OnSuccessAction?.Invoke(true);
            texture.Compress(false);
            texture.Apply();
            Sprite sprite =  Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            OnImageUrlSetting?.Invoke(sprite);
        }

        OnReciveBase64Incoding?.Invoke(successCode, base64);
    }
}
