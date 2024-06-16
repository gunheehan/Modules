using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HumanMindPlayer : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private TMP_Text playerText;
    [SerializeField] private TMP_Text playerScore;

    private int playerID;

    private void Start()
    {
        image.color = Color.gray;
    }

    public void Reset()
    {
        playerText.text = string.Empty;
        playerScore.text = string.Empty;
        playerID = -1;
        gameObject.SetActive(false);
    }

    public void SetPlayerData(HumanMindPlayerData data)
    {
        playerID = data.actorID;
        
        playerText.text = data.nickname;
        playerScore.text = $"{data.score}";
        image.color = Color.gray;
        gameObject.SetActive(true);
    }

    public void SetPlayerTurn(int playerid)
    {
        if(playerID == playerid)
            image.color = Color.yellow;
        else
            image.color = Color.gray;
    }

    public void UpdateScore(int playerid, int score)
    {
        if (playerID != playerid)
            return;

        playerScore.text = $"{score}";
    }
}
