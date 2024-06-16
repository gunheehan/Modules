using System;
using System.Collections.Generic;

public class HumanMindInfo
{
    public const string STATE = "State";
    public const string STARTTIME = "StartTime";
    public const string KEYWORD = "KeyWord";
    public const string BANWORD = "BanWord";
    public const string ANSWERPLAYER = "AnswerPlayer";
    public const string RESPONSEDATA = "ResponseData";
    public const string STATEDATA = "Statedata";
    public const string TURNOWNERINDEX = "TrunOwnerIndex";
    public const string SCORE = "Score";
    public const string TIMEFORMAT = "yyyy-MM-dd HH:mm:ss";
    public const string IMAGEBASE64COUNT = "ImageBase64Count";
    public const string FAILANSWER = "FailAnswer";
    public const string LEFTROUND = "LeftRound";
    public const string TURNCHANGE = "TurnChange";
    
    public const int MAXSCORE = 40;
    public const int ROUNDSCORE = 5;
    public const int ROUNDCOUNT = 3;
    public const int FAILSCORE = -3;
}

public class HumanMindText
{
    public const string GAMESTART = "게임 시작 버튼을 눌러주세요.";
    public const string GAMEREADY = "게임 준비 중입니다.\n잠시만 기다려주세요.";
    public const string GAMEOVER = "모든 라운드가 종료되었습니다.\n하단 버튼으로 로비로 이동해 주세요.";
    public const string ROUNDOVER = "모든 기회가 소진되었습니다. 다음 키워드를 생각해주세요.";
    public const string ANSWERPLAYER = "정답자 : ";
    public const string KEYWORDSELECT = "제시어 선정";
    public const string BANWORDCOLLECT = "금지어 모집";
    public const string KEYWORDREADY = "방장이 제시어를 선택하고 있습니다.\n잠시만 기다려주세요 :)";
    public const string KEYWORDREADYGUIDE = "카테고리를 선택해 주세요!";
    public const string BANWORDREADYDTAIL = "선정된 금지어는 이번 제시어 설명에서 사용이 금지됩니다!\n선정된 제시어 : ";
    public const string PROMPTERROR = "금지어가 포함되어 있습니다. 다시 작성해 주세요!";
    public const string IMAGECREATING = "이미지를 생성중입니다.\n잠시만 기다려 주세요 :)";
    public const string CREATEIMAGE = "이미지를 생성해 주세요.";
    public const string WAITCREATEIMAGE = "다른 플레이어가 이미지를 생성중입니다. :)";
    public const string AIIMAGEREADING = "연상되는 단어를 AI가 판독하고 있어요!";
    public const string ROUND = " 라운드";
    public const string REMAINROUND = "남은 라운드 : ";
    public const string OPENAIPROMPT = "다음 그림을 보고 떠오르는 브랜드들을 ,로 구분해서 한글로 단어만 출력해줘";

}

public enum HumanMindStateType
{
    NONE,
    WORDSELECT,
    BANWORDCOLLECT,
    BANWORDSELECT,
    TURNCHANGE,
    CREATEIMAGE,
    CHOICETURN,
    WAITREQUEST,
    ENDMIND
}

public class HumanMindStateTime
{
    public const float WORDCOLLECT = 20f;
    public const float IMAGECREATE = 30f;
    public const float CHOICETURN = 8f;
    public const float NOTIANSWER = 5f;
    public const float WAITREQUEST = 60f;
}

public class HumanMindData
{
    public DateTime startTime;
    public HumanMindStateType state;
    public string[] banWordArr;
    public string keyWord;
    public HumanMindPlayerData OwnerHumanMindPlayer;
    public int score;
    public int roundsLeft;
}

public class HumanMindPlayerData
{
    public string nickname;
    public int actorID;
    public int score;
}

[Serializable]
public class HumainMindPlayData
{
    public List<HumanMindRoundData> data;
}
[Serializable]
public class HumanMindRoundData
{
    public int round;
    public string keyWord;
    public string banWord;
    public List<string> promptList;
}
