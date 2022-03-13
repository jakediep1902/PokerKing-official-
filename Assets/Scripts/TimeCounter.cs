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
                playerPlaying = gameController.indexBigBlind;
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
            CurrentPlayer = gameController.arrPlayer.Length - 1;
            gameController.indexBigBlind = CurrentPlayer;          
            if (gameController.arrPlayer[CurrentPlayer].timeCounter.GetComponent<Image>().fillAmount > 0)
            {
                gameController.arrPlayer[CurrentPlayer].timeCounter.gameObject.SetActive(true);
            }

        }
        else
        {
            gameController.indexBigBlind = CurrentPlayer;
            if (gameController.arrPlayer[CurrentPlayer].timeCounter.GetComponent<Image>().fillAmount > 0)
            {
                gameController.arrPlayer[CurrentPlayer].timeCounter.gameObject.SetActive(true);
            }

        }
    }
}
