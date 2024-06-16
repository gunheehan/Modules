using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Salin.OpenAI
{
    [Serializable]
    public class GPTRunRequestData
    {
        public string id;
        public string assistant_id;
        public string thread_id;
        public string status;
        public Action<string> onSuccess;
    }


    [Serializable]
    public class GPTMessageSendData
    {
        public string role;
        public string content;
    }

    [Serializable]
    public class GPTRequestRunData
    {
        public string assistant_id;
    }


    [Serializable]
    public class GPTThreadCreateResponseData
    {
        public string id;
        public int created_at;
    }


    [Serializable]
    public class GPTRunCreateResponseData
    {
        public string id;
        public int created_at;
        public string assistant_id;
        public string thread_id;
        public string status;
        public int expires_at;
        public string model;
        public string instructions;
    }

    [Serializable]
    public class GPTMessageDataList
    {
        public GPTMessageItemData[] data;
        public string first_id;
        public string last_id;
        public bool has_more;
    }

    [Serializable]
    public class GPTMessageItemData
    {
        public string id;
        public int created_at;
        public string assistant_id;
        public string thread_id;
        public string run_id;
        public string role;

        public GPTMessageContentsData[] content;
    }

    [Serializable]
    public class GPTMessageContentsData
    {
        public string type;
        public GPTMessageContentsTextData text;
    }

    [Serializable]
    public class GPTMessageContentsTextData
    {
        public string value;
        public string[] annotations;
    }

    public class OpenAICommunication : MonoBehaviour
    {
        //테스트 주석 추가
        private string apiKey = string.Empty;
        private string assistantId = string.Empty; // 어시스턴트 ID
        private string threadId = string.Empty;
        private string threadsURL = "https://api.openai.com/v1/threads"; // 스레드 생성 URL

        private List<GPTRunRequestData> RunRequestDatas;

        private void Start()
        {
            StartCoroutine(CheckMessageCoroutine());
        }

        IEnumerator CheckMessageCoroutine()
        {
            RunRequestDatas = new List<GPTRunRequestData>();
            while (true)
            {
                yield return new WaitUntil(() => 0 < RunRequestDatas.Count);

                GetMessages();

                yield return new WaitForSeconds(5f);
            }
        }

        // 새 스레드 생성
        public void CreateThread(string apiKey,string assistantId,Action<string> onSuccess, Action<string> onFailure)
        {
            this.apiKey = apiKey;
            this.assistantId = assistantId;
            string url = threadsURL;
            string jsonData = "{}"; // 새 스레드 생성을 위한 JSON 데이터

            SendToOpenAI(url, "POST", jsonData, (response) =>
            {
                OnSuccessCreateThread(response);
                onSuccess?.Invoke(response);
            }, onFailure);
        }

        public void SetApiKey(string apiKey)
        {
            this.apiKey = apiKey;
        }
        public void SetAssistantId(string assistantId)
        {
            this.assistantId = assistantId;
        }
        public void SetThreadId(string threadId)
        {
            this.threadId = threadId;
        }
        // 스레드에 메시지 추가 (데이터 전달용)
        public void AddMessageToThread(string threadId, string messageContent)
        {
            string url = $"{threadsURL}/{threadId}/messages";
            GPTMessageSendData requestData = new GPTMessageSendData();
            requestData.role = "user";
            requestData.content = messageContent;
            string jsonData = JsonUtility.ToJson(requestData);

            SendToOpenAI(url, "POST", jsonData, null, null);
        }

        // 스레드에 메시지 추가
        public void AddMessageToThreadAndRun(string threadId, string messageContent, Action<string> onSuccess,
            Action<string> onFailure)
        {
            string url = $"{threadsURL}/{threadId}/messages";
            GPTMessageSendData requestData = new GPTMessageSendData();
            requestData.role = "user";
            requestData.content = messageContent;
            string jsonData = JsonUtility.ToJson(requestData);

            SendToOpenAI(url, "POST", jsonData, (responseData) => CreateRun(threadId, onSuccess, onFailure), onFailure);
        }
        private void OnSuccessCreateThread(string responseData)
        {
            GPTThreadCreateResponseData threadData = JsonUtility.FromJson<GPTThreadCreateResponseData>(responseData);
            threadId = threadData.id;
        }
        private void GetMessages()
        {
            string url = $"{threadsURL}/{threadId}/messages";
            SendToOpenAI(url, "GET", null, OnGetMessages, Debug.LogError);
        }

        private void OnGetMessages(string responseData)
        {
            GPTMessageDataList messageDataList = JsonUtility.FromJson<GPTMessageDataList>(responseData);
            var messageList = messageDataList.data.ToList();

            for (int i = RunRequestDatas.Count - 1; i >= 0; i--)
            {
                GPTRunRequestData request = RunRequestDatas[i];
                if (messageList.Exists(o => o.run_id == request.id))
                {
                    GPTMessageItemData findMessage;
                    findMessage = messageList.Find(o => o.run_id == request.id);
                    StringBuilder stringBuilder = new StringBuilder();


                    if (findMessage.content != null)
                    {
                        bool isNotNull = false;
                        for (int j = 0; j < findMessage.content.Length; j++)
                        {
                            isNotNull = true;
                            stringBuilder.Append(findMessage.content[i].text.value);
                        }

                        if (isNotNull)
                        {
                            RunRequestDatas.Remove(request);
                            request.onSuccess?.Invoke(stringBuilder.ToString());
                        }
                    }
                }
            }
        }

        // 비동기 통신을 위한 SendToOpenAI 함수
        private void SendToOpenAI(string url, string httpMethod, string jsonData, Action<string> onSuccess,
            Action<string> onFailure)
        {
            StartCoroutine(SendRequestCoroutine(url, httpMethod, jsonData, onSuccess, onFailure));
        }

        // UnityWebRequest를 사용하여 요청을 보내고 응답을 처리하는 코루틴
        private IEnumerator SendRequestCoroutine(string url, string httpMethod, string jsonData,
            Action<string> onSuccess,
            Action<string> onFailure)
        {
            UnityWebRequest request = new UnityWebRequest(url, httpMethod);
            byte[] bodyRaw;
            if (!string.IsNullOrEmpty(jsonData))
            {
                request.SetRequestHeader("Content-Type", "application/json");
                bodyRaw = new System.Text.UTF8Encoding().GetBytes(jsonData);
                request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
            }

            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            request.SetRequestHeader("Authorization", "Bearer " + apiKey);
            request.SetRequestHeader("OpenAI-Beta", "assistants=v2");

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            { 
                onFailure?.Invoke(request.error);
                Debug.LogError(request.uri + " || " + request.error);
            }
            else
            {
                onSuccess?.Invoke(request.downloadHandler.text);
                Debug.Log(request.uri + " || " + request.downloadHandler.text);
            }
        }


      
        private void CreateRun(string threadId, Action<string> onSuccess, Action<string> onFailure)
        {
            string url = $"{threadsURL}/{threadId}/runs";
            GPTRequestRunData requestData = new GPTRequestRunData();
            requestData.assistant_id = assistantId;
            string jsonData = JsonUtility.ToJson(requestData);

            SendToOpenAI(url, "POST", jsonData, (responseData) => MessageRequestEnqueue(responseData, onSuccess),
                onFailure);
        }

        private void MessageRequestEnqueue(string responseData, Action<string> onSuccess)
        {
            GPTRunCreateResponseData runCreateResponseData =
                JsonUtility.FromJson<GPTRunCreateResponseData>(responseData);
            GPTRunRequestData newRequestData = new GPTRunRequestData();
            newRequestData.id = runCreateResponseData.id;
            newRequestData.assistant_id = runCreateResponseData.assistant_id;
            newRequestData.thread_id = runCreateResponseData.thread_id;
            newRequestData.onSuccess = onSuccess;

            RunRequestDatas.Add(newRequestData);
        }
    }
}