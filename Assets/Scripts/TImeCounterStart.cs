using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TImeCounterStart : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject[] objNumber;
    GameController gameController;
    public PhotonView PvTimeCounterStart;

    private void Awake()
    {
        
    }

    private void OnEnable()
    {
        
        StartCoroutine(CoStartCount());
        //RPC_StartCount();
    } 


    private void Start()
    {
        gameController = GameController.Instance;

    }
    
    private void OnDisable()
    {
        foreach (var item in objNumber)
        {
            item.SetActive(false);
        }
    }
    
    private void Update()
    {
        if(gameController.isStartGame)
        {
            this.gameObject.SetActive(false);
        }
    }
    IEnumerator CoStartCount(int seconds = 10)
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
        gameController.InitBlind();
        gameController.RPC_SetIsStartGame(true);
        //gameController.isStartGame = true;
        gameController.BtnPlayGame();
        
        
    }
    
    //public void RPC_StartCount()
    //{
    //    PvTimeCounterStart.RPC("StartCount", RpcTarget.All, null);      
    //} 
    //[PunRPC]
    //public void StartCount()
    //{
    //    StartCoroutine(CoStartCount());
    //}
}
