using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BackCard : MonoBehaviour
{
    private GameController gameController;
    public  UnityEvent eArrange;
    private Vector3 targetPos;
    public int noTemplate = 0;    
    public bool isArranged = false;

    private void Start()
    {    
        gameController = GameController.Instance;     
        noTemplate = gameController.NoTemplate;

        if(targetPos==null) Destroy(this.gameObject);
       
        else
        {
            try
            {
                targetPos = gameController.arrPlayer[gameController.NoTemplate].transform.position;
            }
            catch
            {
                Destroy(this.gameObject);
            }
            
        }
        
        //if (gameController.arrPlayer[gameController.NoTemplate].backCard1 == null)
        //{
        //    gameController.arrPlayer[gameController.NoTemplate].backCard1 = this.gameObject;
        //}
        //else
        //{
        //    gameController.arrPlayer[gameController.NoTemplate].backCard2 = this.gameObject;
        //}

        //gameObject.transform.SetParent(gameController.arrPlayer[gameController.NoTemplate].transform);

        //if (transform.GetComponentInParent<PlayerController>().backCard==null)
        //{
        //    transform.GetComponentInParent<PlayerController>().backCard = this;
        //    Debug.Log("add");
        //}            
        
       gameController.NoTemplate++;
       if (gameController.NoTemplate >= gameController.arrPlayer.Length) gameController.NoTemplate = 0;                
    }
    private void Update()
    {     
        MoveToPlayer();     
    }
    public void MoveToPlayer()
    {
        if (!isArranged)
        {
            if (transform.position != targetPos) transform.position = Vector3.Lerp(transform.position, targetPos, 0.09f);
                     
            else
            {               
                isArranged = true;
                float delayTime = 2f;
                if (gameController.playerPlaying > 3) delayTime = 3f;
              
                else delayTime = 1.2f;
               
                Invoke(nameof(SetIsArrange), delayTime);
            }
        }
    }
    public void SetIsArrange()
    {
        //Debug.Log("alarm");
        //eArrange.Invoke();
        //transform.GetComponentInParent<PlayerController>().eAddBackCard.Invoke();
        try
        {
            gameController.arrPlayer[noTemplate].eAddBackCard.Invoke();
        }
        catch
        {
            Debug.Log("Error on Exit");         
        }      
    }   
}
