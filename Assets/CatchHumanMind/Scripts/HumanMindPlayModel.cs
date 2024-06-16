using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine;

public class HumanMindPlayModel : MonoBehaviour
{
    public Action<HumanMindData> OnUpdateGameInfo = null;
    public Action<float> OnUpdateTimer = null;
    public Action<bool, HumanMindStateType> OnMyTrunBeging = null;
    public Action<bool> OnPlayViewActive = null;
    public Action<Sprite> OnUpdateImage = null;
    public Action<string> OnSendImageCheck = null;
    public Action<string> OnImageCreateError = null;
    public Action<bool> SetInputFieldOpen = null;

    private HumanMindStateType state;
    private readonly string ERRORTEXT = "부적절한 단어가 포함되어 이미지 생성에 실패하였습니다.";

    private string keyword;
    private string imageString = String.Empty;
    private bool isTurnOwner = false;
    
    private bool isTimeCheck = false;
    private DateTime startTime;
    private float waitTime = -1f;

    private void OnGetUploadImageUrl(string url)
    {
        Debug.LogWarning("Get Image Upload Url : " + url);
    }
    private void Update()
    {
        if (waitTime > 0 && isTimeCheck)
        {
            Timer();
        }
    }
    
    private void Timer()
    {
        double elapsedTimeInSeconds = (DateTime.Now - startTime).TotalSeconds;
        double remainingRatio = elapsedTimeInSeconds / waitTime;

        OnUpdateTimer?.Invoke((float)remainingRatio);
        if (elapsedTimeInSeconds >= waitTime)
        {
            isTimeCheck = false;
            UpdatePlayState();
        }
    }
    
    private void ResetData()
    {
        imageString = string.Empty;
        isTurnOwner = false;
        OnPlayViewActive?.Invoke(false);
    }

    public void UpdatePlayState(HumanMindData data)
    {
        state = data.state;
        waitTime = -1f;

        switch (state)
        {
            case HumanMindStateType.WORDSELECT:
                ResetData();
                break;
            case HumanMindStateType.BANWORDSELECT:
                keyword = data.keyWord;
                OnPlayViewActive?.Invoke(true);
                OnUpdateGameInfo?.Invoke(data);
                break;
            case HumanMindStateType.CREATEIMAGE:
                SetInputFieldOpen?.Invoke(true);
                waitTime = HumanMindStateTime.IMAGECREATE;
                isTurnOwner = CheckTurnOwner(data.OwnerHumanMindPlayer.actorID);
                OnMyTrunBeging?.Invoke(isTurnOwner, state);
                OnUpdateGameInfo?.Invoke(data);
                break;
            case HumanMindStateType.CHOICETURN:
                ShowImageFromProperty();
                waitTime = HumanMindStateTime.CHOICETURN;
                OnMyTrunBeging?.Invoke(isTurnOwner, state);
                OnUpdateGameInfo?.Invoke(data);
                break;
            case HumanMindStateType.WAITREQUEST:
                waitTime = HumanMindStateTime.WAITREQUEST;
                OnMyTrunBeging?.Invoke(false, state);
                break;
            case HumanMindStateType.ENDMIND:
                OnUpdateTimer?.Invoke(1f);
                SetInputFieldOpen?.Invoke(false);
                break;
        }

        startTime = data.startTime;
        if (waitTime > 0)
            isTimeCheck = true;
    }

    public void UpdatePlayState()
    {
        isTimeCheck = false;

        if (!PhotonNetwork.LocalPlayer.IsMasterClient)
            return;
        
        Hashtable properties = new Hashtable();

        switch (state)
        {
            case HumanMindStateType.CREATEIMAGE:
            case HumanMindStateType.CHOICETURN:
            case HumanMindStateType.WAITREQUEST:
                properties.Add(HumanMindInfo.STATE, HumanMindStateType.TURNCHANGE);
                break;
        }

        DateTime newTime = DateTime.Now;
        properties.Add(HumanMindInfo.STARTTIME, newTime.ToString(HumanMindInfo.TIMEFORMAT));
        PhotonNetwork.CurrentRoom.SetCustomProperties(properties);
    }

    public void RequestTurnChange()
    {
        Hashtable properties = new Hashtable();
        properties.Add(HumanMindInfo.TURNCHANGE, PhotonNetwork.LocalPlayer.NickName);
        PhotonNetwork.CurrentRoom.SetCustomProperties(properties);
    }

    public bool CheckTurnOwner(int ownerNickname)
    {
        if (ownerNickname == PhotonNetwork.LocalPlayer.ActorNumber)
            return true;
        
        return false;
    }

    public void RequestImageCreate(string prompt, string style)
    {
        SetInputFieldOpen?.Invoke(false);
        isTimeCheck = false;
        Hashtable properties = new Hashtable();
        DateTime newTime = DateTime.Now;
        properties.Add(HumanMindInfo.STARTTIME, newTime.ToString(HumanMindInfo.TIMEFORMAT));
        properties.Add(HumanMindInfo.RESPONSEDATA, prompt);
        properties.Add(HumanMindInfo.STATE, HumanMindStateType.WAITREQUEST);
        PhotonNetwork.CurrentRoom.SetCustomProperties(properties);
    }
    
    /// <summary>
    /// 인코딩된 이미지를 프로퍼티 MaxValue로 분할하여 룸프로퍼티에 전송
    /// </summary>
    /// <param name="base64">Texture Incoding string</param>
    public void UpdateImageProperty(int successCode, string base64)
    {
        if (state != HumanMindStateType.WAITREQUEST)
            return;
        
        if (successCode >= 400)
        {
            RequestTurnChange();
            OnImageCreateError?.Invoke(ERRORTEXT);
            return;
        }
        
        int chunkSize = 16000; 
        int pieceCount = Mathf.CeilToInt((float)base64.Length / chunkSize);

        for (int i = 0; i < pieceCount; i++)
        {
            int chunkStart = i * chunkSize;
            int chunkEnd = Mathf.Min(chunkStart + chunkSize, base64.Length);
            string chunkData = base64.Substring(chunkStart, chunkEnd - chunkStart);
            
            Hashtable props = new Hashtable
            {
                { $"ImageBase64_{i}", chunkData }
            };
            PhotonNetwork.CurrentRoom.SetCustomProperties(props);
        }

        Hashtable finalProps = new Hashtable
        {
            { HumanMindInfo.IMAGEBASE64COUNT, pieceCount }
        };
        
        PhotonNetwork.CurrentRoom.SetCustomProperties(finalProps);
    }
    
    /// <summary>
    /// 룸프로퍼티에 올라간 이미지 string 개수를 매개변수로 받아 데이터 조합
    /// </summary>
    private void ShowImageFromProperty()
    {
        Hashtable properties = PhotonNetwork.CurrentRoom.CustomProperties;
        List<string> imagePiece = new List<string>();

        int pieceCount = (int)properties[HumanMindInfo.IMAGEBASE64COUNT];

        for (int i = 0; i < pieceCount; i++)
        {
            string pieceKey = $"ImageBase64_{i}";
            if (properties.ContainsKey(pieceKey))
            {
                string pieceData = (string)properties[pieceKey];
                imagePiece.Add(pieceData);
            }
        }

        imageString = string.Join("", imagePiece);
        byte[] imageBytes = Convert.FromBase64String(imageString);
        Texture2D texture = new Texture2D(2, 2);
        if (texture.LoadImage(imageBytes))
        {
            texture.Compress(false);
            texture.Apply();
            Sprite sprite =  Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            OnUpdateImage?.Invoke(sprite);
        }
    }
    
    public void CheckImageAnswer()
    {
        isTimeCheck = false;
        Hashtable properties = new Hashtable();
        DateTime newTime = DateTime.Now;
        properties.Add(HumanMindInfo.STARTTIME, newTime.ToString(HumanMindInfo.TIMEFORMAT));
        properties.Add(HumanMindInfo.STATEDATA, HumanMindText.AIIMAGEREADING);
        properties.Add(HumanMindInfo.STATE, HumanMindStateType.WAITREQUEST);
        PhotonNetwork.CurrentRoom.SetCustomProperties(properties);
        OnSendImageCheck?.Invoke(imageString);
    }
    
    public void CheckAnswer(string answer)
    {
        if (state != HumanMindStateType.WAITREQUEST)
            return;
        
        Hashtable properties = new Hashtable();
        string[] answerArr = answer.Split(", ");

        properties.Add(HumanMindInfo.STATEDATA, answer);
        
        for (int i = 0; i < answerArr.Length; i++)
        {
            if (keyword.Equals(answerArr[i]))
            {
                properties.Add(HumanMindInfo.ANSWERPLAYER, PhotonNetwork.LocalPlayer.ActorNumber);
                PhotonNetwork.CurrentRoom.SetCustomProperties(properties);
                return;
            }
        }
        properties.Add(HumanMindInfo.FAILANSWER, PhotonNetwork.LocalPlayer.ActorNumber);
        PhotonNetwork.CurrentRoom.SetCustomProperties(properties);
    }
}
