using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.Events;

public class PlayerController : MonoBehaviourPunCallbacks
{
    public static PlayerController Instance;

    //public BackCard backCard;
    GameController gameController;

    public GameObject card1,card2,cardTemplate1,cardTemplate2;   
    public GameObject backCard1,backCard2;
    public GameObject bigBlind;
    
    public TimeCounter timeCounter;
    public PhotonView PvPlayer;
    public UnityEvent eAddBackCard;
   // public UnityEvent eSetBigBlind;
    
    public Vector3 posCard1;
    public Vector3 posCard2;

    public Transform[] arrPosDefaul = new Transform[6];
    public List<GameObject> listCard = new List<GameObject>();
    public List<GameObject> listCardWin = new List<GameObject>();
    public int[] arrCardWin = new int[5];
    public float score = 0f;
    public int ID = 0;

    public bool isInvoke = false;
    public bool isStraightFlush = false;
    public bool isQuad = false;
    public bool isBoat = false;
    public bool isFlush = false;
    public bool isStraight = false;
    public bool isTrip = false;
    public bool isTwoPair = false;
    public bool isOnePair = false;
    public bool isHighCard = false;
    public bool isWinner = false;

    public bool isTurn = false;
   
    void Start()
    {
        if(Instance==null)
        Instance= this;

        gameController = GameController.Instance;

        //backCard = BackCard.Instance;
        //backCard.eArrange.AddListener(() => ArrangeCard());

        card1.GetComponent<SpriteRenderer>().sortingOrder = 7;
        card2.GetComponent<SpriteRenderer>().sortingOrder = 7;
      
        eAddBackCard.AddListener(() => ArrangeCard());      
        PvPlayer = GetComponent<PhotonView>();

        for (int i = 0; i < arrPosDefaul.Length; i++)
        {
            if (ID == i)
                transform.position = arrPosDefaul[i].position;
        }     
        gameObject.name = ID.ToString();       
    }

    public void ArrangeCard()
    {      
        if(!isInvoke)
        {              
            foreach (var item in gameController.listBackCard)
            {            
                Destroy(item.gameObject, 3f);
                item.gameObject.SetActive(false);             
            }
           
            posCard1 = card1.transform.position;
            posCard2 = card2.transform.position;          
            
            if (PvPlayer.IsMine)
            {
                int indexCard = Random.Range((int)0, gameController.cards.Length);
                PvPlayer.RPC("RPC_SetCard1", RpcTarget.All, indexCard);              
                //Debug.Log(indexCard);
                indexCard = Random.Range((int)0, gameController.cards.Length);
                PvPlayer.RPC("RPC_SetCard2", RpcTarget.All, indexCard);             
                //Debug.Log(indexCard);
                PvPlayer.RPC("CoverCardOtherClient", RpcTarget.Others, null);
            }  
            // gameController.SetSmallBigBlind(gameController.listPlayer);        
            isInvoke = true;
        }
    }
    [PunRPC]
    public void RPC_SetCard1(int index)
    {
        gameController.cards[index].SetActive(true);
        gameController.cards[index].transform.position = posCard1;
        gameController.cards[index].GetComponent<SpriteRenderer>().sortingOrder = 4;
        cardTemplate1 = card1;//just change card
        card1 = gameController.cards[index];
        listCard.Add(card1);//add card to list to check
        gameController.cards[index].transform.SetParent(this.transform);
        gameController.RemoveElement(ref gameController.cards, index);
    }
    [PunRPC]
    public void RPC_SetCard2(int index)
    {
        gameController.cards[index].SetActive(true);
        gameController.cards[index].transform.position = posCard2;
        gameController.cards[index].GetComponent<SpriteRenderer>().sortingOrder = 5;
        cardTemplate2 = card2;
        card2 = gameController.cards[index];
        listCard.Add(card2);
        gameController.cards[index].transform.SetParent(this.transform);
        gameController.RemoveElement(ref gameController.cards, index);
    }
    [PunRPC]
    public void CoverCardOtherClient()
    {
        cardTemplate1.GetComponent<SpriteRenderer>().sortingOrder = 7;
        cardTemplate2.GetComponent<SpriteRenderer>().sortingOrder = 7;
        cardTemplate1.SetActive(true);
        cardTemplate2.SetActive(true);
        card1.transform.position = cardTemplate1.transform.position;//fix sometime position not match;
        card2.transform.position = cardTemplate2.transform.position;
        cardTemplate1.transform.localScale = new Vector3(0.5f, 0.5f, 0.7f);
        cardTemplate2.transform.localScale = new Vector3(0.5f, 0.5f, 0.7f);

    }
}
