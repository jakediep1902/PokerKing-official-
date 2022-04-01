using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeCounter : MonoBehaviour
{
    public Image imageFill;
    public PlayerController playerController;
    GameController gameController;
    UIManager uIManager;
    [SerializeField] int playerChecking;
    public bool isFirstGround = true;
    int test = 0;
    private void Awake()
    {
        imageFill = GetComponent<Image>();
        uIManager = UIManager.Instance;
    }
    private void OnEnable()
    {
        gameController = GameController.Instance;
        playerController.isTurn = true;
        //playerController.uIManager.pnlGame.SetActive(true);

        if(gameController.isShowDown)
        {
            playerController.isTurn = false;
        }

        if (playerController.money == 0)
        {
            playerController.BtnXemBai();
        }
        else
        {
            if (playerController.PvPlayer.IsMine)
            {
                uIManager.pnlGame.SetActive(true);
            }

            //Debug.Log($"bigest Blinded is {gameController.bigestBlinded} money Blinded is {playerController.moneyBlinded}");
            if (gameController.bigestBlinded > playerController.moneyBlinded)
            {
                playerController.uIManager.btnTheoCuoc.gameObject.SetActive(true);

            }
            else
            {
                if (isFirstGround)
                {
                    playerController.uIManager.btnTheoCuoc.gameObject.SetActive(false);
                }
                else
                {
                    playerController.BtnXemBai();
                }
            }
            isFirstGround = false;
        }
     
    }
    private void OnDisable()
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
    void Update()
    {
        //if (playerController.isTurn)
        //{
        //    if (imageFill.fillAmount > 0)
        //    {
        //        imageFill.fillAmount -= 0.0005f;
        //        Debug.Log(test++);
        //    }
        //    else
        //    {
        //        Time.timeScale = 0;
        //        playerController.isTurn = false;
        //        playerChecking = gameController.indexBigBlind;
        //        //Debug.Log(playerChecking);
        //        NextPlayer(playerChecking);
        //        this.gameObject.SetActive(false);
        //    }
            
        //}
        //else  this.gameObject.SetActive(false);
    }
    private void FixedUpdate()
    {
        if (playerController.isTurn)
        {
            if (imageFill.fillAmount > 0)
            {
                imageFill.fillAmount -= 0.005f;
                //Debug.Log(test++);
            }
            else
            {
                
                playerController.isTurn = false;
                playerChecking = gameController.indexBigBlind;
                //Debug.Log(playerChecking);
                NextPlayer(playerChecking);
                this.gameObject.SetActive(false);
            }

        }
    }
    public void NextPlayer(int CurrentPlayer)
    {       
        CurrentPlayer--;       
        if (CurrentPlayer < 0)
        {
            CurrentPlayer = gameController.arrPlayer.Length - 1;
            gameController.indexBigBlind = CurrentPlayer;

            if (gameController.arrPlayer[CurrentPlayer]!= null)
            {              
                if (gameController.arrPlayer[CurrentPlayer].timeCounter.GetComponent<Image>().fillAmount > 0)
                {
                    gameController.arrPlayer[CurrentPlayer].timeCounter.gameObject.SetActive(true);
                }
                else if (!gameController.isCheckCard && !gameController.isShowDown)
                {
                    //gameController.BtnDeal();
                    //Invoke(nameof(BtnDeal), 2f);

                    if (gameController.CheckEqualBlind())
                    {
                        Invoke(nameof(BtnDeal), 2f);
                    }
                    else
                    {
                        gameController.RefreshTimeCounter();
                        //Debug.Log(1);
                    }
                }
            }               
        }
        else
        {
            gameController.indexBigBlind = CurrentPlayer;
            if(gameController.arrPlayer[CurrentPlayer] !=null)
            {
                if (gameController.arrPlayer[CurrentPlayer].timeCounter.GetComponent<Image>().fillAmount > 0) //!gameController.arrPlayer[CurrentPlayer].isTurn &&
                {
                    gameController.arrPlayer[CurrentPlayer].timeCounter.gameObject.SetActive(true);
                }
                else if (!gameController.isCheckCard && !gameController.isShowDown)
                {
                    if (gameController.CheckEqualBlind())
                    {
                        Invoke(nameof(BtnDeal), 2f);
                    }
                    else
                    {
                        gameController.RefreshTimeCounter();
                        //Debug.Log(2);
                    }
                    //Invoke(nameof(BtnDeal), 2f);                
                }
            }    
        }
    }
    public void BtnDeal()//to delay
    {
        gameController.BtnDeal();      
    }    
    public void CheckNextPlayer()
    {
        playerChecking = gameController.indexBigBlind;       
        NextPlayer(playerChecking);
    }
}
