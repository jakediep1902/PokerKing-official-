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
    GameController2 gameController2;
    public static ManageNetwork Instance;
    AudioSource audioSource;
    PhotonView PvNetWork;
    public bool isJoinedRoom = false;
    public bool isJoinAble = true;
    public static int roomName = 0;
    private byte maxPlayersPerRoom = 2;
    private static bool isCheckOnce = false;
    

    private void Awake()
    {
        //old code
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
        //audioSource.PlayDelayed(4f);      
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
    private void CreateRoom()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 3;
        PhotonNetwork.CreateRoom(null, roomOptions, null);
    }
    public override void OnJoinedLobby()
    {
        //PhotonNetwork.JoinRandomOrCreateRoom(null, 0, MatchmakingMode.FillRoom, TypedLobby.Default, null, null, new RoomOptions { MaxPlayers = 3 });
        //PhotonNetwork.JoinRandomRoom();
        //Debug.LogWarning(PhotonNetwork.JoinRandomRoom());
        //Debug.Log($"Count of Rooms :  {PhotonNetwork.CountOfRooms}");
        //var count = PhotonNetwork.CountOfRooms;
        //if (count > 0)
        //{
        //    PhotonNetwork.JoinRandomRoom();
        //}
        //else
        //{
        //    PhotonNetwork.CreateRoom($"Room{Random.Range(0, 100)}", new RoomOptions { MaxPlayers = 1 }, TypedLobby.Default);
        //PhotonNetwork.JoinRandomOrCreateRoom(null, 0, MatchmakingMode.FillRoom, TypedLobby.Default, null, null, new RoomOptions { MaxPlayers = 3 });
        //}
        //JoinRoom();
        int room = Random.Range(0, 100);
        PhotonNetwork.JoinOrCreateRoom($"Room{room}", new RoomOptions { MaxPlayers = 2 }, TypedLobby.Default);
        //Debug.LogWarning(PhotonNetwork.JoinOrCreateRoom($"Room{roomName}", new RoomOptions { MaxPlayers = 1 }, TypedLobby.Default));
        //PhotonNetwork.JoinRandomRoom();
    }
    //void JoinNextRoom()
    //{
    //    //RoomOptions roomOptions = new RoomOptions
    //    //{
    //    //    MaxPlayers = maxPlayersPerRoom
    //    //};
    //    //PhotonNetwork.JoinRandomOrCreateRoom(null, maxPlayersPerRoom, MatchmakingMode.FillRoom, null, null, null, roomOptions);

    //    roomName++;
    //    PhotonNetwork.CreateRoom($"Room{roomName}", new RoomOptions { MaxPlayers = 1 }, TypedLobby.Default);

    //}
    public override void OnJoinedRoom()
    {
        Debug.Log($"Joined Room : {PhotonNetwork.CurrentRoom}");

        ////  if (PvNetWork.IsMine) gameController.GetComponent<GameController>().enabled = true;

        //// RPC_RequestSyncData();

        ////gameController.SyncPlayerDatasJoinLate();
        ////Debug.Log($"isStartGame is {gameController.isStartGame}  && isCheckCard is {gameController.isCheckCard}");
        ////BtnReady();
        ////gameController.playerInRoom = (int)PhotonNetwork.CurrentRoom.PlayerCount;

        ////var temp = gameController.TryGetAllPlayers();
        ////Debug.Log($"All Player Are :"+temp);

        ////StartCoroutine(gameController.ITryGetAllPlayers((result) => {
        ////    Debug.Log($"All players :" + result);
        ////    if (result >= 6)
        ////    {
        ////        PhotonNetwork.LeaveRoom();
        ////        //PhotonNetwork.JoinRandomOrCreateRoom(null, 0, MatchmakingMode.FillRoom, TypedLobby.Default, null, null, new RoomOptions { MaxPlayers = 6 });
        ////        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        ////        PhotonNetwork.CreateRoom($"Room{Random.Range(0, 100)}", new RoomOptions { MaxPlayers = 6 }, TypedLobby.Default);
        ////        CancelInvoke(nameof(BtnReady));
        ////    }
        ////    else
        ////    {
        ////        //Invoke(nameof(BtnReady), 4f);              
        ////    }
        ////}));
        //Invoke(nameof(BtnReady), 4f);
        isJoinedRoom = true;
        //Invoke(nameof(SetIsJoinedRoom), 4f);
        //Invoke(nameof(LoadSceneGame),4f);
        //SceneManager.LoadScene("Game");
        //Debug.Log($"Joined Room at :{Time.time}");
    }
    //public override void OnJoinRoomFailed(short returnCode, string message)
    //{
    //    if(!isCheckOnce)
    //    {
    //        JoinNextRoom();
    //        isCheckOnce = true;
    //    }
    //    ////Debug.Log(message);
    //    //roomName++;
    //    ////PhotonNetwork.JoinOrCreateRoom($"Room{roomName}", new RoomOptions { MaxPlayers = 3 }, TypedLobby.Default);

    //    //PhotonNetwork.CreateRoom($"Room{roomName}", new RoomOptions { MaxPlayers = 1 }, TypedLobby.Default);
    //}
    //public override void OnCreateRoomFailed(short returnCode, string message)
    //{
    //    Debug.Log(message);
    //    roomName++;
    //    PhotonNetwork.CreateRoom($"Room{roomName}", new RoomOptions { MaxPlayers = 1 }, TypedLobby.Default);
    //}
    //public override void OnJoinRoomFailed(short returnCode, string message)
    //{
    //    //PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 3 }, TypedLobby.Default);
    //}
    //public override void OnJoinRandomFailed(short returnCode, string message)
    //{
    //    //Debug.Log(message);
    //    //PhotonNetwork.CreateRoom($"Room{roomName}", new RoomOptions { MaxPlayers = 1 }, TypedLobby.Default);
    //}
    //PhotonNetwork.CreateRoom($"Room{Random.Range(0, 100)}", new RoomOptions { MaxPlayers = 3 }, TypedLobby.Default);
    //CreateRoom();


    public override void OnLeftRoom()
    {
        Debug.Log($"player has left room");
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
    public void LoadSceneGame()
    {
        SceneManager.LoadScene("Game");
    }
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
