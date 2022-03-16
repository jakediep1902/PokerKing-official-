using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class ManageNetwork : MonoBehaviourPunCallbacks
{
    GameController gameController;
    public static ManageNetwork Instance;
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
        //Debug.Log("hello from Network");
        //pnlGame.SetActive(true);
        // SceneManager.LoadScene(0);
        Invoke(nameof(BtnReady),4f);       
    }
    public override void OnLeftRoom()
    {
        //Debug.Log($"player ID {photonViews.ViewID} has left room");
    }
    public void BtnReady()
    {
        gameController.BtnReady();
    }  
}
