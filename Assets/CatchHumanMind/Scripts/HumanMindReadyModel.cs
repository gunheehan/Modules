using System;
using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine;

public class HumanMindReadyModel : MonoBehaviour
{
    public Action<string, string> OnUpdateReadyInfo = null;
    public Action<float> OnUpdateTimer = null;
    public Action<string, float> OnPopupToast = null;
    public Action<bool> OnViewStartFlow = null;
    public Action<bool, string> SetReadyView = null;
    public Action<bool> SetCategoryList = null;
    public Action<bool> SetInputFieldOpen = null;
    public Action SetRandomKeyword = null;

    private HumanMindStateType state;
    private float waitTime = HumanMindStateTime.WORDCOLLECT;
    private bool isTimeCheck = false;
    private DateTime startTime;
    private int round = 0;

    private void Update()
    {
        if (isTimeCheck)
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
            UpdateReadyState();
        }
    }

    public void UpdateReadyState(HumanMindData data)
    {
        state = data.state;

        switch (state)
        {
            case HumanMindStateType.NONE:
                SetWaitView();
                break;
            case HumanMindStateType.WORDSELECT:
                round++;
                OnPopupToast?.Invoke(round + HumanMindText.ROUND, 3f);
                OnViewStartFlow?.Invoke(true);
                bool isMaster = PhotonNetwork.LocalPlayer.IsMasterClient;
                string guide = isMaster ? HumanMindText.KEYWORDREADYGUIDE : HumanMindText.KEYWORDREADY;
                OnUpdateReadyInfo?.Invoke(HumanMindText.KEYWORDSELECT, guide);
                SetCategoryList?.Invoke(isMaster);
                break;
            case HumanMindStateType.BANWORDCOLLECT:
                OnUpdateReadyInfo?.Invoke(HumanMindText.BANWORDCOLLECT, HumanMindText.BANWORDREADYDTAIL + data.keyWord);
                SetInputFieldOpen?.Invoke(true);
                break;
            case HumanMindStateType.BANWORDSELECT:
                OnViewStartFlow?.Invoke(false);
                SetInputFieldOpen?.Invoke(false);
                break;
        }

        startTime = data.startTime;
        isTimeCheck = true;
    }

    public void SendPlayerText(string text)
    {
        Hashtable properties = new Hashtable();

        if (state == HumanMindStateType.WORDSELECT)
            properties.Add(HumanMindInfo.KEYWORD, text);
        
        else if (state == HumanMindStateType.BANWORDCOLLECT)
            properties.Add(HumanMindInfo.BANWORD, text);
        
        PhotonNetwork.CurrentRoom.SetCustomProperties(properties);
    }
    
    public void OnClickStartGame()
    {
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;

        DateTime newTime = DateTime.Now;
        Hashtable properties = new Hashtable();
        properties.Add(HumanMindInfo.STATE, HumanMindStateType.WORDSELECT);
        properties.Add(HumanMindInfo.LEFTROUND, HumanMindInfo.ROUNDCOUNT);
        properties.Add(HumanMindInfo.STARTTIME, newTime.ToString(HumanMindInfo.TIMEFORMAT));

        PhotonNetwork.CurrentRoom.SetCustomProperties(properties);
    }

    private void UpdateReadyState()
    {
        if (!PhotonNetwork.LocalPlayer.IsMasterClient)
            return;
        
        Hashtable properties = new Hashtable();
        switch (state)
        {
            case HumanMindStateType.WORDSELECT:
                SetRandomKeyword?.Invoke();
                break;
            case HumanMindStateType.BANWORDCOLLECT:
                DateTime newTime = DateTime.Now;
                properties.Add(HumanMindInfo.STARTTIME, newTime.ToString(HumanMindInfo.TIMEFORMAT));
                properties.Add(HumanMindInfo.STATE, HumanMindStateType.BANWORDSELECT);
                break;
        }

        PhotonNetwork.CurrentRoom.SetCustomProperties(properties);
    }

    private void SetWaitView()
    {
        bool isMaster = PhotonNetwork.LocalPlayer.IsMasterClient;
        string infoText = string.Empty;

        if (isMaster)
            infoText = HumanMindText.GAMESTART;
        else
            infoText = HumanMindText.GAMEREADY;

        SetReadyView?.Invoke(isMaster, infoText);
    }
}
