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

    private void OnEnable()
    {
        imageFill = GetComponent<Image>();
        imageFill.fillAmount = 1;

    }
    void Start()
    {
        gameController = GameController.Instance;
        playerController.isTurn = true;
    }  
    void Update()
    {
        if (playerController.isTurn)
        {
            if (imageFill.fillAmount > 0)
                imageFill.fillAmount -= 0.003f;
            else
            {
                playerPlaying = gameController.indexSBBlind;
                playerController.isTurn = false;
                
                NextPlayer(playerPlaying);
                this.gameObject.SetActive(false);
            }
        }
        //else
        //    this.gameObject.SetActive(false);
    }
    public void NextPlayer(int CurrentPlayer)
    {       
        CurrentPlayer--;
        if (CurrentPlayer < 0)
        {
            CurrentPlayer = gameController.listPlayer.Length - 1;
            gameController.indexSBBlind = CurrentPlayer;          
            if (gameController.listPlayer[CurrentPlayer].timeCounter.GetComponent<Image>().fillAmount > 0)
            {
                gameController.listPlayer[CurrentPlayer].timeCounter.gameObject.SetActive(true);
            }

        }
        else
        {
            gameController.indexSBBlind = CurrentPlayer;
            if (gameController.listPlayer[CurrentPlayer].timeCounter.GetComponent<Image>().fillAmount > 0)
            {
                gameController.listPlayer[CurrentPlayer].timeCounter.gameObject.SetActive(true);
            }

        }
    }
}
