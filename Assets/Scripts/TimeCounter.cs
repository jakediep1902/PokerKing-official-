using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeCounter : MonoBehaviour
{
    public Image imageFill;
    public PlayerController playerController;
    GameController gameController;
    int playerPlaying;

    private void Awake()
    {
        
        imageFill = GetComponent<Image>();
    }
    private void OnEnable()
    {
        
        gameController = GameController.Instance;
        imageFill.fillAmount = 1;      
        if(playerController.PvPlayer.IsMine)
        {
           gameController.pnlGame.SetActive(true);
        }
        Debug.Log($"bigest Blinded is {gameController.bigestBlinded}");
        Debug.Log($"money Blinded is {playerController.moneyBlinded}");
        if (gameController.bigestBlinded > playerController.moneyBlinded)
        {
            
            playerController.uIManager.btnTheoCuoc.gameObject.SetActive(true);
        }
        else
        {
            playerController.uIManager.btnTheoCuoc.gameObject.SetActive(false);
        }

    }
    private void OnDisable()
    {
        if (playerController.PvPlayer.IsMine)
        {
            gameController.pnlGame.SetActive(false);
        }
    }

    void Start()
    {
      
        playerController.isTurn = true;
    }  
    void Update()
    {
        if (playerController.isTurn)
        {
            if (imageFill.fillAmount > 0)
                imageFill.fillAmount -= 0.0003f;
            else
            {
                playerPlaying = gameController.indexBigBlind;
                playerController.isTurn = false;
                

                NextPlayer(playerPlaying);
                this.gameObject.SetActive(false);
            }
            
        }
        //else  this.gameObject.SetActive(false);
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
                else if (!gameController.isCheckCard)
                {
                    //gameController.BtnDeal();
                    Invoke(nameof(BtnDeal), 2f);

                    //if (gameController.CheckEqualBlind())
                    //{
                    //    Invoke(nameof(BtnDeal), 2f);
                    //}
                    //else
                    //{
                    //    Debug.Log(1);
                    //    StartCoroutine(gameController.ResetTimeCounter(0.5f));
                    //}                   
                }
            }               

        }
        else
        {
            gameController.indexBigBlind = CurrentPlayer;
            //Debug.Log($"current Player is :{CurrentPlayer}");
            if(gameController.arrPlayer[CurrentPlayer] !=null)
            {
                if (gameController.arrPlayer[CurrentPlayer].timeCounter.GetComponent<Image>().fillAmount > 0)
                {
                    gameController.arrPlayer[CurrentPlayer].timeCounter.gameObject.SetActive(true);
                    //Debug.Log(1);
                }
                else if (!gameController.isCheckCard)
                {
                    //if (gameController.CheckEqualBlind())
                    //{
                    //    Invoke(nameof(BtnDeal), 2f);
                    //}
                    //else
                    //{
                    //    Debug.Log(2);
                    //    StartCoroutine(gameController.ResetTimeCounter(0.5f));
                    //}
                    Invoke(nameof(BtnDeal), 2f);
                    //Debug.Log(2);
                }
            }
           
        }
    }
    public void BtnDeal()
    {
        gameController.BtnDeal();      
    }    
}
