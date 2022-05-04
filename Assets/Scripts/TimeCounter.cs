using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.Events;
using System.Threading;
public class TimeCounter : MonoBehaviourPunCallbacks
{
    public Image imageFill;
    public PlayerController playerController;
    public UnityEvent eEnable;
    //public PhotonView PvTimeCounter;
    GameController gameController;
    UIManager uIManager;
    [SerializeField] int playerChecking;
    public bool isFirstGround = true;
    
    private void Awake()
    {
        //gameController = GameController.Instance;
        imageFill = GetComponent<Image>();
        uIManager = UIManager.Instance;
    }
    public override void OnEnable()
    {
        gameController = GameController.Instance;
      
        playerController.isTurn = true;

        if (playerController.money == 0)
        {
            //playerController.isTurn = false;
            if (playerController.GetComponent<Bot>().enabled == true) playerController.BtnXemBaiBot();
            else playerController.BtnXemBai();
            return;
        } 
        else if(playerController.GetComponent<Bot>().enabled == true)  eEnable.Invoke();//Bot action

        else //player action
        {

            if (playerController.PvPlayer.IsMine) uIManager.pnlGame.SetActive(true);        

            if (gameController.bigestBlinded > playerController.moneyBlinded)
            {
                playerController.uIManager.btnTheoCuoc.gameObject.SetActive(true);
            }
            else
            {
                if (isFirstGround) playerController.uIManager.btnTheoCuoc.gameObject.SetActive(false);
               
                else playerController.BtnXemBai();                              
            }
            isFirstGround = false;
        }

        //if (gameController.isShowDown)
        //{
        //    playerController.isTurn = false;
        //    if (playerController.money == 0 && playerController.GetComponent<Bot>().enabled == true)//apply to Bot
        //        playerController.BtnXemBaiBot();
        //}
        //else eEnable.Invoke();

        //if (playerController.money == 0 && playerController.GetComponent<Bot>().enabled == false)//apply to player
        //{
        //    playerController.BtnXemBai();
        //}
        //else
        //{
        //    if (playerController.PvPlayer.IsMine && playerController.GetComponent<Bot>().enabled == false)
        //    {
        //        uIManager.pnlGame.SetActive(true);
        //    }

        //    //Debug.Log($"bigest Blinded is {gameController.bigestBlinded} money Blinded is {playerController.moneyBlinded}");
        //    if (gameController.bigestBlinded > playerController.moneyBlinded)
        //    {
        //        playerController.uIManager.btnTheoCuoc.gameObject.SetActive(true);

        //    }
        //    else
        //    {
        //        if (isFirstGround)
        //        {
        //            playerController.uIManager.btnTheoCuoc.gameObject.SetActive(false);
        //        }
        //        else
        //        {
        //            playerController.BtnXemBai();
        //        }
        //    }
        //    isFirstGround = false;
        //}

    }
    public override void OnDisable()
    {
        //if (playerController.PvPlayer.IsMine && playerController!=null && playerController.PvPlayer!=null)
        //{ 
        //gameController.pnlGame.SetActive(false);
        //}      
        //CheckNextPlayer();
    }

    void Start()
    {      
        imageFill.fillAmount = 1;
        
    }  
    private void FixedUpdate()
    {
        if (playerController.isTurn && (gameController.isCheckCard == false) && (gameController.isShowDown == false))
        {
            if (imageFill.fillAmount > 0) imageFill.fillAmount -= 0.0009f;
           
            else
            {         
                playerController.isTurn = false;
                playerChecking = gameController.indexBigBlind;
                //Debug.Log(playerChecking);
                uIManager.pnlGame.SetActive(false);

                //check equal blind
                if((playerController.moneyBlinded < gameController.bigestBlinded) && playerController.money>0)
                {
                    playerController.GetComponent<Bot>().enabled = false;                    
                    if (playerController.isBot) playerController.BtnBoBaiBot();
                    else playerController.BtnBoBai();
                }
              
                NextPlayer(playerChecking);              
                this.gameObject.SetActive(false);
            }
        }
    }
    public void NextPlayer(int CurrentPlayer)
    {
        CurrentPlayer--;
        for (int i = 0; i < gameController.arrPlayer.Length; i++)
        {
            if (CurrentPlayer < 0) CurrentPlayer = gameController.arrPlayer.Length - 1;
           
            if (gameController.arrPlayer[CurrentPlayer] == null) CurrentPlayer--;

            else break;           
        }
        
        if (CurrentPlayer < 0)
        {
            CurrentPlayer = gameController.arrPlayer.Length - 1;
            gameController.indexBigBlind = CurrentPlayer;
            if (gameController.arrPlayer[CurrentPlayer]?.timeCounter.GetComponent<Image>().fillAmount > 0)
            {
                gameController.arrPlayer[CurrentPlayer]?.timeCounter.gameObject.SetActive(true);
            }
            else if (!gameController.isCheckCard && !gameController.isShowDown)
            {
                //gameController.BtnDeal();
                //Invoke(nameof(BtnDeal), 2f);
                if (gameController.CheckEqualBlind())
                {
                    if(!gameController.isShowDown) Invoke(nameof(BtnDeal), 2f);
                    //Debug.Log("Call deal");
                    //BtnDeal();
                }
                else gameController.RefreshTimeCounter();             
            }

        }
        else
        {
            gameController.indexBigBlind = CurrentPlayer;
            if (gameController.arrPlayer[CurrentPlayer]?.timeCounter.GetComponent<Image>().fillAmount > 0)
            {
                gameController.arrPlayer[CurrentPlayer]?.timeCounter.gameObject.SetActive(true);
            }
            else if (!gameController.isCheckCard && !gameController.isShowDown)
            {
                if (gameController.CheckEqualBlind())
                {
                    //Thread.Sleep(10000);
                    //Debug.Log("Call deal");
                    if (!gameController.isShowDown) Invoke(nameof(BtnDeal), 2f);                  
                    //BtnDeal();
                }  
                else gameController.RefreshTimeCounter();                                           
            }
        }
    }
    public void BtnDeal() => gameController.BtnDeal(); //to delay       
    public void CheckNextPlayer()
    {
        playerChecking = gameController.indexBigBlind;       
        NextPlayer(playerChecking);
    }
}
