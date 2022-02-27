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
            gameController.isFull = true;
        }
        //gameController.SetRandomCard(this.gameObject);
    }

    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, targetPos, 0.5f);
        
    }
}
