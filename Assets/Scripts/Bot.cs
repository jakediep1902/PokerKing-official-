using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Bot : MonoBehaviourPunCallbacks
{
    GameController gameController;
    PlayerController playerController;
    TimeCounter timeCounter;
    public float thinkingTime = 3f;
    public int randomOption;
    public bool isTurns = false;
    private void Awake()
    {
        gameController = GameController.Instance;
        playerController = GetComponent<PlayerController>();
        timeCounter = playerController.timeCounter;
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
   public void GamePlay()
    {
        randomOption = Random.Range(1, 3);
        switch (randomOption)
        {
            case 0:
                playerController.BtnBoBaiBot();              
              
                Debug.Log($"player {playerController.ID} Fold");
                break;
            case 1:
                playerController.BtnXemBaiBot();              
              
                Debug.Log($"player {playerController.ID} stand");
                break;
            case 2:
                playerController.BtnTheoCuocBot();              
               
                Debug.Log($"player {playerController.ID} follow");
                break;        
        }
        
    }
    public void DelayGamePlay()
    {
        float delay = Random.Range(1f, 3f);
        Invoke(nameof(GamePlay),delay);
    }
}
