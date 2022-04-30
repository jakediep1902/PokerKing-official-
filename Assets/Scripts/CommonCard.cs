using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommonCard : MonoBehaviour
{
    private GameController gameController;
    private Vector3 targetPos;
    void Start()
    {      
        gameController = GameController.Instance;
        //Debug.Log($"NoCommonPos in CommonCard : {gameController.NoCommonPos}");
        
        if (gameController.NoCommonPos >= gameController.commonPos.Length)
        {
            gameController.RPC_SetIsFullFiveCard(true);
            //this.gameObject.SetActive(false);            
        }
        else
        {
            targetPos = gameController.commonPos[gameController.NoCommonPos].position;
            gameController.NoCommonPos++;
        }      
    }
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, targetPos, 0.1f);       
    }
}
