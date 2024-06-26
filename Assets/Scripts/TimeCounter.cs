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
    //public Text txtCheckingPlayer;
    
    private void Awake()
    {
        //gameController = GameController.Instance;
        imageFill = GetComponent<Image>();
        uIManager = UIManager.Instance;
    }
    public override void OnEnable()
    {
        gameController = GameController.Instance;

        //gameController.SyncIndexBigBlind();

        if (gameController.isCheckCard || gameController.isEndGame) return;
      
        playerController.isTurn = true;
        
        if (playerController.money == 0)
        {
            //playerController.isTurn = false;
            if (playerController.isBot) playerController.BtnXemBaiBot();
            else playerController.BtnXemBai();
            return;
        } 
        else if(playerController.isBot)  eEnable.Invoke();//Bot action

        else //player action
        {

            if (playerController.PvPlayer.IsMine)
            {
                uIManager.pnlGame.SetActive(true);
                playerController.SetClipToPlayDelay("chipIn",1.5f);
            }        

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
    }
    void Start()
    {      
        imageFill.fillAmount = 1;
        GameController.eventSyncIndexBigBlind.AddListener(() =>
        {
            playerChecking = gameController.indexBigBlind;
        });
    }  
    private void FixedUpdate()
    {
        if (playerController.isTurn && (gameController.isCheckCard == false) && (gameController.isShowDown == false))
        {
            if (imageFill.fillAmount > 0)
            {
                imageFill.fillAmount -= 0.0009f;
            }
            else
            {
                playerController.isTurn = false;              
                playerChecking = gameController.indexBigBlind;
                //Debug.Log(playerChecking);
                uIManager.pnlGame.SetActive(false);
                //check equal blind
                if ((playerController.moneyBlinded < gameController.bigestBlinded) && playerController.money > 0)
                {
                    playerController.GetComponent<Bot>().enabled = false;

                    if (playerController.isBot) playerController.BtnBoBaiBot();

                    else playerController.BtnBoBai();
                }
                NextPlayer(ref playerChecking);
                this.gameObject.SetActive(false);
                //Debug.Log($"called by {photonView.ViewID}");                           
            }
        }
    }
    public void NextPlayer(ref int CurrentPlayer)
    {
        CurrentPlayer--;
        //Debug.Log($"0 Current Player above is  {CurrentPlayer}");
        for (int i = 0; i < gameController.arrPlayer.Length; i++)
        {
            if (CurrentPlayer < 0 || CurrentPlayer>=gameController.arrPlayer.Length) CurrentPlayer = gameController.arrPlayer.Length - 1;
           
            if (gameController.arrPlayer[CurrentPlayer] == null || CurrentPlayer==gameController.arrPlayer.Length) CurrentPlayer--;

            else break;           
        }
        if (CurrentPlayer < 0)
        {
            //CurrentPlayer = gameController.arrPlayer.Length - 1;
            CurrentPlayer = gameController.playerPlaying - 1;
            gameController.indexBigBlind = CurrentPlayer;
            ////if (gameController.photonViews.IsMine)
            //    gameController.photonViews.RPC("RPC_OnlyIndexBigBlind", RpcTarget.All, CurrentPlayer); 
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
                    if (!gameController.isShowDown && gameController.photonViews.IsMine)
                    {
                        BtnDeal();
                        Thread.Sleep(3000);
                    }
                    //Invoke(nameof(BtnDeal), 2f);
                }
                else
                {
                    gameController.RefreshTimeCounter();
                }
            }
        }
        else
        {
            gameController.indexBigBlind = CurrentPlayer;                   
            if (gameController.arrPlayer[CurrentPlayer] && gameController.arrPlayer[CurrentPlayer]?.timeCounter.GetComponent<Image>().fillAmount > 0)
            {      
                gameController.arrPlayer[CurrentPlayer]?.timeCounter.gameObject.SetActive(true);
            }
            else if (!gameController.isCheckCard && !gameController.isShowDown)
            {
                if (gameController.CheckEqualBlind())
                {
                    //Thread.Sleep(10000);
                    //Debug.Log("Call deal");
                    if (!gameController.isShowDown && gameController.photonViews.IsMine)
                    {
                        BtnDeal();
                        Thread.Sleep(3000);
                    }
                    //Invoke(nameof(BtnDeal), 2f);                        
                }
                else
                {
                    gameController.RefreshTimeCounter();
                }              
            }
        }
    }
    public void BtnDeal() => gameController.BtnDeal(); //to delay       
    public void CheckNextPlayer()
    {
        playerChecking = gameController.indexBigBlind;       
        NextPlayer(ref playerChecking);
    }
}
