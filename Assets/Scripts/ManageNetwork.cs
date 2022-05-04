using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class ManageNetwork : MonoBehaviourPunCallbacks
{
    GameController gameController;
    GameController2 gameController2;
    public static ManageNetwork Instance;
    public bool isJoinedRoom = false;
    private void Awake()
    {
        if(Instance==null) Instance = this;
       
        else
        {
            //Destroy(Instance.gameObject);//Destroy old instance
            //Instance = this;
            Destroy(gameObject);//Destroy new instance
        }
        DontDestroyOnLoad(this.gameObject);
    }
    void Start()
    {
        gameController = GameController.Instance;
        gameController2 = GameController2.Instance;
        if(!PhotonNetwork.IsConnected)
        PhotonNetwork.ConnectUsingSettings();        
    }
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }
    public override void OnJoinedLobby()
    {
        PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions { MaxPlayers = 6 }, TypedLobby.Default);
    }
    public override void OnJoinedRoom()
    {
        // Debug.Log($"isStartGame is {gameController.isStartGame}");
        //BtnReady();
        //gameController.playerInRoom = (int)PhotonNetwork.CurrentRoom.PlayerCount;
        Invoke(nameof(BtnReady), 4f);                          
    }
    public override void OnLeftRoom()
    {      
        //Debug.Log($"player ID {photonViews.ViewID} has left room");
        //gameController.CheckPlayerExit();
    }
    public void BtnReady()
    {
        //gameController.SpawPlayer();
        if (gameController.isStartGame)
        {
            gameController.SpawPlayer();                 
            Invoke(nameof(SetIsJoinedRoom),10f);
            Debug.Log($"Game alrealy played and isStartGame was true");
        }
        else
        {
            isJoinedRoom = true;
            gameController.BtnReady();
        }      
    }
    public void SetIsJoinedRoom() => isJoinedRoom = true; 
}
