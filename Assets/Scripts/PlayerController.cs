using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;

    private BackCard backCard;
    GameController gameController;

    public GameObject card1, card2;   
    public GameObject backCard1,backCard2;
    public TimeCounter timeCounter;
    
    public Vector3 posCard1;
    public Vector3 posCard2;

    public List<GameObject> listCard = new List<GameObject>();
    public List<GameObject> listCardWin = new List<GameObject>();
    public int[] arrCardWin = new int[5];
    public float score = 0f;
    public int ID = 0;

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
        backCard = BackCard.Instance;
        backCard.eArrange.AddListener(() => ArrangeCard());


        ID = int.Parse(gameObject.name);
    }
    public void ArrangeCard()
    {

        foreach (var item in gameController.listBackCard)
        {
            Destroy(item.gameObject);
        }
        //Debug.Log("Arrange");

        //transform.GetChild(2).transform.position = posCard1;
        //transform.GetChild(0).gameObject.SetActive(false);
        //transform.GetChild(3).transform.position = posCard2;
        //transform.GetChild(1).gameObject.SetActive(false);




        int indexCard = Random.Range((int)0, gameController.cards.Length);
        gameController.cards[indexCard].SetActive(true);
        gameController.cards[indexCard].transform.position = posCard1;
        gameController.cards[indexCard].GetComponent<SpriteRenderer>().sortingOrder = 4;
        card1 = gameController.cards[indexCard];
        //gameController.cards[indexCard].transform.SetParent(this.transform);
        gameController.RemoveElement(ref gameController.cards, indexCard);

        int indexCard2 = Random.Range((int)0, gameController.cards.Length);
        gameController.cards[indexCard2].SetActive(true);
        gameController.cards[indexCard2].transform.position = posCard2;
        gameController.cards[indexCard2].GetComponent<SpriteRenderer>().sortingOrder = 5;
        card2 = gameController.cards[indexCard2];
        //gameController.cards[indexCard2].transform.SetParent(this.transform);
        gameController.RemoveElement(ref gameController.cards, indexCard2);

        listCard.Add(card1);//add card to list to check
        listCard.Add(card2);

       // gameController.SetSmallBigBlind(gameController.listPlayer);

        // card1 = transform.GetChild(4).gameObject;
        //  card2 = transform.GetChild(5).gameObject;
    }   
}
