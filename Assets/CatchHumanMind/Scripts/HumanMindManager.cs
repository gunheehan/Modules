using System;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class HumanMindManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject guideView;
    [SerializeField] private Button guideSkipButton;
    [SerializeField] private HumanMindPlayer[] playerIcons;
    public event Action<HumanMindData> OnReadyProcess = null;
    public event Action<HumanMindData> OnPlayingProcess = null;
    public event Action<HumanMindData> OnEndProcess = null;
    public event Action<string> OnRecivePromptResponse = null;

    public Action OnClickGuideSkip = null;
    
    private HumanMindStateType gameState = HumanMindStateType.NONE;
    private DateTime startTime;
    private HashSet<string> banWordList;
    private string keyWord;
    private Dictionary<int, HumanMindPlayerData> playerDic = new Dictionary<int, HumanMindPlayerData>();
    private int turnOwnerIndex = -1;
    private HumanMindPlayerData turnOwner;
    private HumainMindPlayData roundDataList = new HumainMindPlayData();
    private HumanMindRoundData roundData;

    private int banWordCount = 0;
    private int score;
    private int roundLeft = 0;
    private int round = 1;

    private void Start()
    {
        score = HumanMindInfo.MAXSCORE + HumanMindInfo.ROUNDSCORE;
        banWordList = new HashSet<string>();
        roundDataList.data = new List<HumanMindRoundData>();
        guideView.SetActive(true);
        guideSkipButton.onClick.AddListener(OnClickGuideTab);
    }

    private void resetData()
    {
        roundData = new HumanMindRoundData();
        roundData.promptList = new List<string>();
        banWordCount = 0;
    }

    private void OnClickGuideTab()
    {
        OnClickGuideSkip?.Invoke();
        HumanMindData newGameData = GetGameData();
        OnReadyProcess?.Invoke(newGameData);
        guideView.SetActive(false);
    }

    private void UpdatePlayerList()
    {
        foreach (HumanMindPlayer humanMindPlayer in playerIcons)
        {
            humanMindPlayer.Reset();
        }

        int i = 0;

        foreach (HumanMindPlayerData player in playerDic.Values)
        {
            playerIcons[i].SetPlayerData(player);
            i++;
        }
    }

    private HumanMindData GetGameData()
    {
        HumanMindData newGameData = new HumanMindData()
        {
            startTime = startTime,
            state = gameState,
            banWordArr = banWordList.ToArray(),
            OwnerHumanMindPlayer = turnOwner,
            keyWord = keyWord,
            score = score,
            roundsLeft = roundLeft
        };

        return newGameData;
    }

    private void UpdateGameState(HumanMindStateType state)
    {
        if (state == gameState)
            return;

        gameState = state;

        HumanMindData newGameData = GetGameData();

        switch (state)
        {
            case HumanMindStateType.NONE:
                OnReadyProcess?.Invoke(newGameData);
                break;
            case HumanMindStateType.WORDSELECT:
                resetData();
                guideView.SetActive(false);
                OnEndProcess?.Invoke(newGameData);
                OnReadyProcess?.Invoke(newGameData);
                OnPlayingProcess?.Invoke(newGameData);
                break;
            case HumanMindStateType.BANWORDCOLLECT:
                OnReadyProcess?.Invoke(newGameData);
                break;
            case HumanMindStateType.BANWORDSELECT:
                OnReadyProcess?.Invoke(newGameData);
                OnPlayingProcess?.Invoke(newGameData);
                SetNextState();
                break;
            case HumanMindStateType.TURNCHANGE:
                CheckInvaildTurn();
                break;
            case HumanMindStateType.CREATEIMAGE:
                OnPlayingProcess?.Invoke(newGameData);
                break;
            case HumanMindStateType.CHOICETURN:
                OnPlayingProcess?.Invoke(newGameData);
                break;
            case HumanMindStateType.WAITREQUEST:
                OnPlayingProcess?.Invoke(newGameData);
                break;
            case HumanMindStateType.ENDMIND:
                OnPlayingProcess?.Invoke(newGameData);
                OnEndProcess?.Invoke(newGameData);
                RefreshGame();
                break;
        }
    }

    private void SetNextState()
    {
        if (!PhotonNetwork.LocalPlayer.IsMasterClient)
            return;
        
        HumanMindStateType stateType = gameState;
        stateType += 1;
        Hashtable properties = new Hashtable();
        DateTime newTime = DateTime.Now;
        properties.Add(HumanMindInfo.STARTTIME, newTime.ToString(HumanMindInfo.TIMEFORMAT));
        properties.Add(HumanMindInfo.STATE, stateType);

        PhotonNetwork.CurrentRoom.SetCustomProperties(properties);
    }

    private void RoundEnd()
    {
        if (!PhotonNetwork.LocalPlayer.IsMasterClient)
            return;
        
        Hashtable properties = new Hashtable();
        DateTime newTime = DateTime.Now;
        properties.Add(HumanMindInfo.STARTTIME, newTime.ToString(HumanMindInfo.TIMEFORMAT));
        properties.Add(HumanMindInfo.STATE, HumanMindStateType.ENDMIND);

        PhotonNetwork.CurrentRoom.SetCustomProperties(properties);
    }

    private void SetTurnOwner()
    {
        turnOwnerIndex++;

        if (turnOwnerIndex >= playerDic.Count)
            turnOwnerIndex = 0;
        
        Hashtable properties = new Hashtable();
        properties.Add(HumanMindInfo.TURNOWNERINDEX, turnOwnerIndex);
        DateTime newTime = DateTime.Now;
        properties.Add(HumanMindInfo.STARTTIME, newTime.ToString(HumanMindInfo.TIMEFORMAT));
        properties.Add(HumanMindInfo.STATE, HumanMindStateType.CREATEIMAGE);
        properties.Add(HumanMindInfo.SCORE, score);
        PhotonNetwork.CurrentRoom.SetCustomProperties(properties);
    }

    private void SetOwnerPlayerMark(int ownerID)
    {
        foreach (HumanMindPlayer humanMindPlayer in playerIcons)
        {
            humanMindPlayer.SetPlayerTurn(ownerID);
        }
    }

    private void CheckInvaildTurn()
    {
        if (!PhotonNetwork.LocalPlayer.IsMasterClient)
            return;

        score -= HumanMindInfo.ROUNDSCORE; 

        if (score <= 0)
        {
            RoundEnd();
            return;
        }
        SetTurnOwner();
    }

    private void UpdatePlayerScore(int userid, int score)
    {
        HumanMindPlayerData humanMindPlayerData;
        
        if (playerDic.TryGetValue(userid, out humanMindPlayerData))
        {
            humanMindPlayerData.score += score;

            foreach (HumanMindPlayer playerIcon in playerIcons)
            {
                playerIcon.UpdateScore(userid, humanMindPlayerData.score);
            }
        }
    }

    private void CheckCollectCount()
    {
        if (gameState != HumanMindStateType.BANWORDCOLLECT)
            return;

        banWordCount++;
        if (banWordCount >= playerDic.Keys.Count)
        {
            banWordCount = 0;
            SetNextState();
        }
    }

    private void RefreshGame()
    {
        roundData.round = round;
        roundData.keyWord = keyWord;
        roundData.banWord = string.Join(", ", banWordList);
        roundDataList.data.Add(roundData);
        
        if (roundLeft <= 1)
            SendPlayDataOnBoard();
        
        banWordList.Clear();
        keyWord = string.Empty;
        round++;
    }

    private void SendPlayDataOnBoard()
    {
        string contentsData = JsonUtility.ToJson(roundDataList);
        //boardModel.RequestOnBoard("그리고 맞춰봐!","Game", contentsData);
    }

    #region Photon Override

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        object odata;
        
        if (propertiesThatChanged.TryGetValue(HumanMindInfo.STARTTIME, out odata))
        {
            startTime = DateTime.ParseExact((string)odata, HumanMindInfo.TIMEFORMAT,
                System.Globalization.CultureInfo.InvariantCulture);
        }
        
        if (propertiesThatChanged.TryGetValue(HumanMindInfo.SCORE, out odata))
        {
            score = (int)odata;
        }

        if (propertiesThatChanged.TryGetValue(HumanMindInfo.LEFTROUND, out odata))
        {
            roundLeft = (int)odata;
        }

        if (propertiesThatChanged.TryGetValue(HumanMindInfo.KEYWORD, out odata))
        {
            keyWord = (string)odata;
            SetNextState();
        }

        if (propertiesThatChanged.TryGetValue(HumanMindInfo.BANWORD, out odata))
        {
            string banWord = (string)odata;
            if (!string.IsNullOrEmpty(banWord))
                banWordList.Add(banWord);
            CheckCollectCount();
        }

        if (propertiesThatChanged.TryGetValue(HumanMindInfo.TURNOWNERINDEX, out odata))
        {
            int index = (int)odata;
            turnOwnerIndex = index;
            HumanMindPlayerData[] dataArr = playerDic.Values.ToArray();
            turnOwner = dataArr[index];
            
            SetOwnerPlayerMark(turnOwner.actorID);
        }

        if (propertiesThatChanged.TryGetValue(HumanMindInfo.RESPONSEDATA, out odata))
        {
            string responseData = (string)odata;
            OnRecivePromptResponse?.Invoke(responseData);
            roundData.promptList.Add(responseData);
        }
        
        if (propertiesThatChanged.TryGetValue(HumanMindInfo.STATEDATA, out odata))
        {
            string stateData = (string)odata;
            OnRecivePromptResponse?.Invoke(stateData);
        }
        
        if (propertiesThatChanged.TryGetValue(HumanMindInfo.IMAGEBASE64COUNT, out odata))
        {
            gameState = HumanMindStateType.CREATEIMAGE;
            SetNextState();
        }
 
        if (propertiesThatChanged.TryGetValue(HumanMindInfo.ANSWERPLAYER, out odata))
        {
            int answerID = (int)odata;
            UpdatePlayerScore(answerID, score);
            RoundEnd();
        }

        if (propertiesThatChanged.TryGetValue(HumanMindInfo.FAILANSWER, out odata))
        {
            int failAnswerID = (int)odata;
            UpdatePlayerScore(failAnswerID, HumanMindInfo.FAILSCORE);
            CheckInvaildTurn();
        }
        
        if (propertiesThatChanged.TryGetValue(HumanMindInfo.TURNCHANGE, out odata))
        {
            CheckInvaildTurn();
        }

        if (propertiesThatChanged.TryGetValue(HumanMindInfo.STATE, out odata))
        {
            HumanMindStateType state = (HumanMindStateType)odata;
            UpdateGameState(state);
        }
    }
    
    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        
        if (string.IsNullOrEmpty(PhotonNetwork.LocalPlayer.NickName))
            PhotonNetwork.LocalPlayer.NickName = PhotonNetwork.LocalPlayer.ActorNumber.ToString();

        Photon.Realtime.Player[] photonPlayer = PhotonNetwork.PlayerList;
        for (int index = 0; index < photonPlayer.Length; index++)
        {
            if (string.IsNullOrEmpty(photonPlayer[index].NickName))
                photonPlayer[index].NickName = photonPlayer[index].ActorNumber.ToString();
            
            HumanMindPlayerData newHumanMindPlayerData = new HumanMindPlayerData()
            {
                actorID = photonPlayer[index].ActorNumber,
                nickname = photonPlayer[index].NickName
            };

            playerDic.Add(newHumanMindPlayerData.actorID, newHumanMindPlayerData);
        }

        UpdatePlayerList();
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        if (string.IsNullOrEmpty(newPlayer.NickName))
            newPlayer.NickName = newPlayer.ActorNumber.ToString();
        
        HumanMindPlayerData newHumanMindPlayerData = new HumanMindPlayerData()
        {
            actorID = newPlayer.ActorNumber,
            nickname = newPlayer.NickName
        };
        playerDic.Add(newHumanMindPlayerData.actorID, newHumanMindPlayerData);
        UpdatePlayerList();
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        playerDic.Remove(otherPlayer.ActorNumber);

        UpdatePlayerList();

        if (!PhotonNetwork.LocalPlayer.IsMasterClient)
            return;
        
        if (string.IsNullOrEmpty(otherPlayer.NickName))
            otherPlayer.NickName = otherPlayer.ActorNumber.ToString();
        
        if (gameState >= HumanMindStateType.CREATEIMAGE && gameState <= HumanMindStateType.WAITREQUEST)
        {
            Hashtable properties = new Hashtable();

            if (otherPlayer.NickName != turnOwner.nickname)
            {
                turnOwnerIndex = 0;
                foreach (int key in playerDic.Keys)
                {
                    if (turnOwner.actorID == key)
                        break;
                    turnOwnerIndex++;
                }
            }
            else
            {
                DateTime newTime = DateTime.Now;
                properties.Add(HumanMindInfo.STARTTIME, newTime.ToString(HumanMindInfo.TIMEFORMAT));
                properties.Add(HumanMindInfo.STATE, HumanMindStateType.TURNCHANGE);
            
                turnOwnerIndex--;
                if (turnOwnerIndex < 0)
                    turnOwnerIndex = playerDic.Count - 1;
            }
            
            properties.Add(HumanMindInfo.TURNOWNERINDEX, turnOwnerIndex);
            PhotonNetwork.CurrentRoom.SetCustomProperties(properties);
        }
    }

    public override void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
    {
        if (gameState != HumanMindStateType.NONE)
            return;
        HumanMindData newGameData = GetGameData();
        OnReadyProcess?.Invoke(newGameData);
    }

    #endregion
}
