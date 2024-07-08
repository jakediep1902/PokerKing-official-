using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using ExitGames.Client.Photon;
using System.Threading.Tasks;
using System.Threading;
using Photon.Pun.Demo.Cockpit;



public class ManageNetwork : MonoBehaviourPunCallbacks
{
    GameController gameController;
   
    public static ManageNetwork Instance;

    AudioSource audioSource;

    PhotonView PvNetWork;

    public bool isJoinedRoom = false;
    
    public static int roomName = 0;

    [SerializeField]
    private byte maxPlayersPerRoom = 3;
    //[SerializeField]
    private float delayJoinRoom = 1;

    private void Awake()
    {      
        Application.targetFrameRate = 60;//to set All device run at same frame rate 60fpt => every thing run in same speed(card moving....)

        if (Instance == null) Instance = this;

        else
        {
            //Destroy(Instance.gameObject);//Destroy old instance
            //Instance = this;
            Destroy(this.gameObject);//Destroy new instance
        }            
        DontDestroyOnLoad(this.gameObject);

        gameObject.AddComponent<PhotonView>();
        PvNetWork = GetComponent<PhotonView>();
        gameController = GameController.Instance;       
        audioSource = GetComponent<AudioSource>();
        Invoke(nameof(RequestConnectToMaster), delayJoinRoom);//If we request connect imediately then won't work on build mobile (but work fine on editor)
        return;            
    } 
    public override void OnConnectedToMaster()
    {     
        PhotonNetwork.JoinLobby();
    }  
    public override void OnJoinedLobby()
    {      
        PhotonNetwork.JoinRandomOrCreateRoom(null, 0, MatchmakingMode.FillRoom, TypedLobby.Default, null, null, new RoomOptions { MaxPlayers = maxPlayersPerRoom });      
    }   
    public override void OnJoinedRoom()
    {
        Debug.Log($"Joined Room : {PhotonNetwork.CurrentRoom}");      
        isJoinedRoom = true;       
    }
    public override void OnLeftRoom()
    {
        Debug.Log($"player has left room");
    }   
    public void LoadSceneGame()
    {
        SceneManager.LoadScene("Game");
    }
    public void RequestConnectToMaster()
    {
        if (!PhotonNetwork.IsConnected)
        {
            Debug.Log("Request join Photon server");
            PhotonNetwork.ConnectUsingSettings();
        }
    }
}
