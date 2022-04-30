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
        Debug.Log($"NoCommonPos in CommonCard : {gameController.NoCommonPos}");
        
        if (gameController.NoCommonPos >= gameController.commonPos.Length)
        {
            gameController.RPC_SetIsFullFiveCard(true);
            this.gameObject.SetActive(false);
            Debug.Log($"NoCommonPos in CommonCard : {gameController.NoCommonPos}");

        }
        else
        {
            targetPos = gameController.commonPos[gameController.NoCommonPos].position;
            gameController.NoCommonPos++;
        }

       

        //gameController.SetRandomCard(this.gameObject);
        // int indexCard = Random.Range((int)0, (int)gameController.cards.Length);
        //spriteTemp = gameController.cards[indexCard].GetCompo
    }
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, targetPos, 0.1f);       
    }
}
