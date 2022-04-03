using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameController2 : MonoBehaviourPunCallbacks,IPunObservable
{
    public static GameController2 Instance;
    GameController gameController;
    public bool isStartGame = false;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
        gameController = GameController.Instance;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
       if(stream.IsWriting)
        {
            stream.SendNext(isStartGame);
        }
       else if(stream.IsReading)
        {
            isStartGame = (bool)stream.ReceiveNext();
        }
    }
}
