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
        targetPos = gameController.commonPos[gameController.NoCommonPos].position;
        gameController.NoCommonPos++;
        if(gameController.NoCommonPos>=gameController.commonPos.Length)
        {          
            gameController.isFullFiveCard = true;
            gameController.NoCommonPos = 0;
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
