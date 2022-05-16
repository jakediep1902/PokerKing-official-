using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using ExitGames.Client.Photon;
using System.Threading.Tasks;
using System.Threading;

public class ManageNetwork : MonoBehaviourPunCallbacks
{
    GameController gameController;
    GameController2 gameController2;
    public static ManageNetwork Instance;
    AudioSource audioSource;
    PhotonView PvNetWork;
    public bool isJoinedRoom = false;
    public bool isJoinAble = true;
    private void Awake()
    {
        if (Instance == null) Instance = this;

        else
        {
            //Destroy(Instance.gameObject);//Destroy old instance
            //Instance = this;
            Destroy(gameObject);//Destroy new instance
        }
        DontDestroyOnLoad(this.gameObject);

        PvNetWork = GetComponent<PhotonView>();
        gameController = GameController.Instance;
        gameController2 = GameController2.Instance;
        audioSource = GetComponent<AudioSource>();
        if (!PhotonNetwork.IsConnected)
            PhotonNetwork.ConnectUsingSettings();      
    }
    //private void OnEnable()
    //{
    //    //PhotonNetwork.NetworkingClient.EventReceived += NetworkingClient_EventReceived;
    //}

    //private void NetworkingClient_EventReceived(EventData obj)
    //{
    //    if (obj.Code == (byte)RaiseEventCode.SyncManageNetWork)
    //    {
    //        object[] datas = (object[])obj.CustomData;
    //        isJoinAble = (bool)datas[0];
    //    }
    //}

    //private void OnDisable()
    //{
    //    //PhotonNetwork.NetworkingClient.EventReceived -= NetworkingClient_EventReceived;
    //}
    void Start()
    {
        //PvNetWork = GetComponent<PhotonView>();
        //gameController = GameController.Instance;
        //gameController2 = GameController2.Instance;
        //if (!PhotonNetwork.IsConnected)
        //    PhotonNetwork.ConnectUsingSettings();

        audioSource.PlayDelayed(4f);
        

    }
    private void Update()
    {
        //if (isJoinAble && gameController.GetComponent<GameController>().enabled == false) ;
        //{
        //    gameController.GetComponent<GameController>().enabled = true;
        //}
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
        Debug.Log("Joined Room");
              
      //  if (PvNetWork.IsMine) gameController.GetComponent<GameController>().enabled = true;

       // RPC_RequestSyncData();

        //gameController.SyncPlayerDatasJoinLate();
        //Debug.Log($"isStartGame is {gameController.isStartGame}  && isCheckCard is {gameController.isCheckCard}");
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
            Invoke(nameof(SetIsJoinedRoom), 10f);
            Debug.Log($"Game alrealy played and isStartGame was true");
        }
        else
        {
            isJoinedRoom = true;
            gameController.BtnReady();
        }
    }
    public void SetIsJoinedRoom() => isJoinedRoom = true;
   // [PunRPC]
    //public void SetIsJoinAble(bool bul)
    //{
    //    isJoinAble = bul;
    //    Debug.Log($"isJoinAble in all client is {isJoinAble}");
        
    //}
   
    //public void RPC_SetIsJoinAble(bool bul)
    //{
    //    PvNetWork.RPC("SetIsJoinAble", RpcTarget.All, bul);
    //}

   

    //public async Task RPC_RequestSyncData()
    //{
    //    Task T1 = new Task(() =>
    //    {
    //        PvNetWork.RPC("RequestToMasterJoinAble", RpcTarget.MasterClient, null);
    //        Debug.Log("Running T1");
    //        Thread.Sleep(5000);
    //    });
    //    T1.Start();
    //    await T1;
    //    Debug.Log($"finish task T1 and isJoinAble is {isJoinAble}");
    //    if (isJoinAble)
    //    {
    //        Debug.Log("enabled GameController");
    //        gameController.GetComponent<GameController>().enabled = true;
    //    }
        

    //    //PvNetWork.RPC("RequestToMasterJoinAble", RpcTarget.All, null);
    //}
    //[PunRPC]
    //public void RequestToMasterJoinAble()
    //{
    //    Debug.Log($"isJoinAble in master before changed is {isJoinAble}");
    //    if (PvNetWork.IsMine)
    //    {
    //        PvNetWork.RPC("SetIsJoinAble", RpcTarget.All, isJoinAble);
    //    }

    //    Debug.Log($" isJoinAble in master after changed is {isJoinAble}");        

    //}
    //public void SyncManageNetWorkJoinLate()
    //{
    //    object[] datas = new object[]
    //    {
    //        isJoinAble,     //0
    //    };
    //    RaiseEventOptions option = new RaiseEventOptions()
    //    {
    //        CachingOption = EventCaching.DoNotCache,
    //        Receivers = ReceiverGroup.All
    //    };

    //    PhotonNetwork.RaiseEvent((byte)RaiseEventCode.SyncManageNetWork, datas, option, SendOptions.SendUnreliable);
    //}//using
    //enum RaiseEventCode
    //{
    //    SyncManageNetWork,
    //}
}
