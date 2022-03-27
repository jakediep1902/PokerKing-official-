using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TImeCounterStart : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject[] objNumber;
    GameController gameController;   
    private void Start()
    {
        gameController = GameController.Instance;
    }
    private void OnEnable()
    {
        StartCoroutine(StartCount());
    }
    private void OnDisable()
    {
        foreach (var item in objNumber)
        {
            item.SetActive(false);
        }
    }

    IEnumerator StartCount(int seconds = 10)
    {
        if (seconds >= objNumber.Length)
        {
            seconds = objNumber.Length - 1;
            for (int i = seconds; i >= 0 && i < objNumber.Length; i--)
            {
                objNumber[i].SetActive(true);
                yield return new WaitForSeconds(1);
                objNumber[i].SetActive(false);
            }
        }          
        else
        {
            for (int i = seconds; i >= 0 && i < objNumber.Length; i--)
            {
                objNumber[i].SetActive(true);
                yield return new WaitForSeconds(1);
                objNumber[i].SetActive(false);
            }
        }       
        this.gameObject.SetActive(false);
        gameController.isStartGame = true;
        gameController.InitBlind();
        gameController.BtnPlayGame();
    }
}
