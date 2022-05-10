using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Bot : MonoBehaviourPunCallbacks
{
    GameController gameController;
    PlayerController playerController;
    PhotonView PvBot;
    TimeCounter timeCounter;
    public float thinkingTime = 3f;
    public int randomOption;
    public bool isTurns = false;
    private void Awake()
    {
        gameController = GameController.Instance;
        playerController = GetComponent<PlayerController>();
        timeCounter = playerController.timeCounter;
        PvBot = GetComponent<PhotonView>();
    }
   
    private void Start()
    {
        timeCounter.eEnable.AddListener(() => DelayGamePlay());
    }


    private void Update()
    {
        //if(playerController.isTurn && !isTurns)
        //{
        //    thinkingTime = Random.Range(1f, 3f);
        //    Invoke(nameof(GamePlay), thinkingTime);
        //    //Debug.Log($"player {playerController.ID} count....");
        //    isTurns = true;
        //}
    }
    [PunRPC]
   public void BotAI(int option)
    {
        
        switch (option)
        {
            case 0:
                playerController.BtnBoBaiBot();                           
                Debug.Log($"player {playerController.ID} Fold");
                break;
            case 1:
                playerController.BtnXemBaiBot();                            
                Debug.Log($"player {playerController.ID} check");
                break;
            case 2:
                playerController.BtnTheoCuocBot();                            
                Debug.Log($"player {playerController.ID} follow");
                break;
            case 3:
                playerController.BtnAllInBot();
                Debug.Log($"player {playerController.ID} All In");
                break;
        }
        
    }
    public void DelayGamePlay()
    {
        float delay = Random.Range(3f, 6f);
        Invoke(nameof(RPC_BotAI),delay);       
    }
    public void RPC_BotAI()
    {
        if(PvBot.IsMine)
        {
            randomOption = Random.Range(0, 4);
            PvBot.RPC("BotAI", RpcTarget.All, randomOption);
        }        
    }
}
