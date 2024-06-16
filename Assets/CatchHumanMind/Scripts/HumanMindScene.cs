using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class HumanMindScene : MonoBehaviourPunCallbacks
{
    [SerializeField] private LoginController loginController;
    [SerializeField] private HumanMindManager manager;
    [SerializeField] private bool isDeveloper = false;
    
    private readonly short ROOM_DOES_NOT_EXIST = 32758;
    private readonly short ROOM_WAS_CLOSED = 32764;
    private readonly short ROOM_IS_FULL = 32765;
    
    private string sceneName;
    private int channelNumber = 0;
    
    private void Start()
    {
        sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        loginController.OnClickJoin = OnClickJoinButton;
        
        if(!isDeveloper)
            manager.OnJoinedRoom();
    }

    private void OnClickJoinButton()
    {
        if (isDeveloper)
            Photon.Pun.PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        if(!isDeveloper)
            return;
        base.OnConnectedToMaster();
        PhotonNetwork.JoinLobby();
    }
    
    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        PhotonNetwork.JoinOrCreateRoom($"{sceneName}{++channelNumber}", new RoomOptions { MaxPlayers = 4}, TypedLobby.Default, null);
    }
    
    public override void OnJoinedRoom()
    {
        channelNumber = 0;
    }
    
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        if(returnCode == ROOM_IS_FULL || returnCode == ROOM_WAS_CLOSED)
            PhotonNetwork.JoinOrCreateRoom($"{sceneName}{++channelNumber}", new RoomOptions { MaxPlayers = 4}, TypedLobby.Default, null);
        else if(returnCode == ROOM_DOES_NOT_EXIST)
            PhotonNetwork.JoinOrCreateRoom(sceneName, new RoomOptions { MaxPlayers = 4}, TypedLobby.Default);
        else
            Debug.LogError($"OnJoinRoomFailed :  code : {returnCode}  / {message}");
    }
}
