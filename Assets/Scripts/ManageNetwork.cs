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
        if(Instance==null)
        {
            Instance = this;
        }
        else
        {
            //Destroy(Instance.gameObject);//Destroy old instance
            //Instance = this;
            Destroy(gameObject);//Destroy new instance
        }
        DontDestroyOnLoad(this.gameObject);
    }
    // Start is called before the first frame update
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
        Invoke(nameof(BtnReady), 2f);
                        
        //SceneManager.LoadScene(0);
        
    }
    public override void OnLeftRoom()
    {
        //Debug.Log($"player ID {photonViews.ViewID} has left room");
        //gameController.CheckPlayerExit();
    }
    public void BtnReady()
    {
        if (gameController2.isStartGame)
        {
            gameController.SpawPlayer();
            Debug.Log($"isStartGame is true");
        }
        else
        {
            isJoinedRoom = true;
            gameController.BtnReady();
            Debug.Log($"isStartGame is fasle");
        }
       
              
    }
}
