using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BackCard : MonoBehaviour
{
    //public TemplateCard[] arrayStarPosCard;
    private GameController gameController;
    public static BackCard Instance;
    public UnityEvent eArrange;
    
    private Vector3 targetPos;
  
    //public PlayerTemplate[] arrayPlayerTemplate = new PlayerTemplate[6];

    

    public int emptyPlayer;
    public bool isArranged = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {    
        gameController = GameController.Instance;
       
        targetPos = gameController.listPlayer[gameController.NoTemplate].transform.position;
        if(gameController.listPlayer[gameController.NoTemplate].backCard1==null)
        {
            gameController.listPlayer[gameController.NoTemplate].backCard1 = this.gameObject;
        }
        else
        {
            gameController.listPlayer[gameController.NoTemplate].backCard2 = this.gameObject;
        }

       // gameObject.transform.SetParent(gameController.listPlayer[gameController.NoTemplate].transform);
        gameController.NoTemplate++;
        if (gameController.NoTemplate >= gameController.listPlayer.Length)
        {
            gameController.NoTemplate = 0;
        }       
    }
    private void Update()
    {     
        MoveToPlayer();     
    }
    public void MoveToPlayer()
    {
        if (!isArranged)
        {
            if (transform.position != targetPos)
            {
                transform.position = Vector3.Lerp(transform.position, targetPos, 0.09f);
            }
            else
            {
                isArranged = true;
                float delayTime = 2f;
                if (gameController.amountPlayer > 3)
                {
                    delayTime = 3f;
                }
                else
                {
                    delayTime = 1.2f;
                }
                Invoke(nameof(SetIsArrange), delayTime);
            }
        }
    }
    public void SetIsArrange()
    {
        //Debug.Log("alarm");
        eArrange.Invoke();       
        gameController.ChangeSpriteRenderer(this.gameObject);
        
    }
}
