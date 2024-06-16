using System;
using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine;

public class HumanMindEndModel : MonoBehaviour
{
    public Action<bool> OnEndViewActive = null;
    public Action<string> OnUpdateAnswer = null;
    public Action<string> OnUpdatEexplan = null;
    public Action<bool> OnStageEffect = null;
    public Action<string, float> OnPopupLeftRound = null;
    public Action OnEndRound = null;
    public Action OnEndGame = null;
    public Action<float> UpdateTimer = null;
    
    private int leftRound = 0;
    
    private DateTime startTime;
    private bool isTimeCheck = false;
    private bool isCloseGame = false;
    private float timelimit = HumanMindStateTime.NOTIANSWER;

    private void Update()
    {
        if (timelimit > 0 && isTimeCheck)
        {
            Timer();
        }
    }
    
    private void Timer()
    {
        double elapsedTimeInSeconds = (DateTime.Now - startTime).TotalSeconds;
        double remainingRatio = elapsedTimeInSeconds / timelimit;
        
        UpdateTimer?.Invoke((float)remainingRatio);


        if (elapsedTimeInSeconds >= timelimit)
        {
            isTimeCheck = false;

            ReStartGame();

            if(!isCloseGame)
                OnEndRound?.Invoke();
        }
    }
    
    public void UpdateEndState(HumanMindData data)
    {
        switch (data.state)
        {
            case HumanMindStateType.WORDSELECT:
                OnEndViewActive?.Invoke(false);
                break;
            case HumanMindStateType.ENDMIND:
                OnEndViewActive?.Invoke(true);
                if (data.score <= 0)
                {
                    OnEndRound?.Invoke();
                    OnUpdateAnswer?.Invoke(string.Empty);
                    OnUpdatEexplan?.Invoke(HumanMindText.ROUNDOVER);
                    OnStageEffect?.Invoke(false);
                }
                else
                {
                    string answerString = HumanMindText.ANSWERPLAYER + data.OwnerHumanMindPlayer.nickname;
                    OnUpdateAnswer?.Invoke(answerString);
                    OnStageEffect?.Invoke(true);
                }

                leftRound = data.roundsLeft - 1;
                startTime = data.startTime;
                OnPopupLeftRound?.Invoke(HumanMindText.REMAINROUND + leftRound, 3f);
                isTimeCheck = true;
                break;
        }
    }

    public void LeaveGame()
    {
        isTimeCheck = false;
    }
    
    private void ReStartGame()
    {
        if (leftRound <= 0) //Game End
        {
            isCloseGame = true;
            OnUpdateAnswer?.Invoke(string.Empty);
            OnUpdatEexplan?.Invoke(HumanMindText.GAMEOVER);
            LeaveGame();
            OnEndGame?.Invoke();
            return;
        }
        
        if (!PhotonNetwork.LocalPlayer.IsMasterClient)
            return;

        DateTime newTime = DateTime.Now;
        Hashtable properties = new Hashtable();
        properties.Add(HumanMindInfo.STATE, HumanMindStateType.WORDSELECT);
        properties.Add(HumanMindInfo.STARTTIME, newTime.ToString(HumanMindInfo.TIMEFORMAT));
        properties.Add(HumanMindInfo.SCORE, HumanMindInfo.MAXSCORE + HumanMindInfo.ROUNDSCORE);
        properties.Add(HumanMindInfo.LEFTROUND, leftRound);

        PhotonNetwork.CurrentRoom.SetCustomProperties(properties);
    }
}
