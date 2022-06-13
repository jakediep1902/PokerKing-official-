using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.Events;

public class GameController2 : MonoBehaviourPunCallbacks, IPunObservable
{
   // UnityEvent eChangIsStartGame = new UnityEvent();
    public static GameController2 Instance;
    GameController gameController;
    PhotonView PvGameController2;
    //[SerializeField] private bool _isStartGame = false;
    public bool isStartGame = false;
   // public int Input;
   // [SerializeField]private int _test;
    //public int test
    //{
    //    get { return _test; }
    //    set
    //    {
    //        _test = value;
    //        eChangIsStartGame?.Invoke();
    //    }
    //}
    private void Awake()
    {
        //if (Instance == null)
        //{
        //    Instance = this;
        //}
        //else
        //{
        //    Destroy(this.gameObject);
        //}
        //DontDestroyOnLoad(this.gameObject);
        //gameController = GameController.Instance;
      
    }
    void Start()
    {
        
        //GetComponent<PhotonView>().ViewID = 99;
        //if(PvGameController2 ==null)
        //{
        //    PvGameController2 = gameObject.AddComponent<PhotonView>();
        //    PvGameController2.ViewID = 100;
        //    PvGameController2.observableSearch = PhotonView.ObservableSearch.AutoFindAll;
        //}

        //eChangIsStartGame.AddListener(() => ChangeIsStartGame());
    }

    // Update is called once per frame
    void Update()
    {
        //if (gameController.arrPlayer.Length > 0)
        //test = Input;
        
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //if (stream.IsWriting)
        //{
        //    stream.SendNext(isStartGame);
        //}
        //else if(stream.IsReading)
        //{
        //    isStartGame = (bool)stream.ReceiveNext();
        //}
    }
    //public void ChangeIsStartGame()
    //{
    //    Debug.Log($"Changed test {test}");
    //}
    
}
