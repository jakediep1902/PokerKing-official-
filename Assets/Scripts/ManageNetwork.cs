using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class ManageNetwork : MonoBehaviourPunCallbacks
{

    // Start is called before the first frame update
    void Start()
    {
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
        GetComponent<GameController>().BtnReady();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
