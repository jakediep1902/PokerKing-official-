using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.Events;

public class PlayerController : MonoBehaviourPunCallbacks
{
    //public BackCard backCard;
    public GameController gameController;

    public GameObject card1,card2,cardTemplate1,cardTemplate2;   
    //public GameObject backCard1,backCard2;
    //public List<GameObject> listBackCard = new List<GameObject>(2);
    public GameObject bigBlind;

    public Button btnXemBai;
    public Button btnBoBai;
    public Button btnThemCuoc;

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
    public bool isFold = false;

    void Start()
    {
        //Debug.Log($"hello from Player {PvPlayer.ViewID}");       
        DontDestroyOnLoad(this.gameObject);
        gameController = GameController.Instance;
        cardTemplate1.GetComponent<SpriteRenderer>().sortingOrder = 7;
        cardTemplate2.GetComponent<SpriteRenderer>().sortingOrder = 7;
        PvPlayer = GetComponent<PhotonView>();
        eAddBackCard.AddListener(() => ArrangeCard());
        for (int i = 0; i < arrPosDefaul.Length; i++)
        {
            if (ID == i)
            {
                transform.position = arrPosDefaul[i].position;
            }
        }
        gameObject.name = ID.ToString();
        btnXemBai = gameController.btnXemBai;
        btnXemBai.onClick.AddListener(() => BtnXemBai());
        btnBoBai = gameController.btnBoBai;
        btnBoBai.onClick.AddListener(() => BtnBoBai());
        btnThemCuoc = gameController.btnThemCuoc;
        btnThemCuoc.onClick.AddListener(() => BtnThemCuoc());
    }

   

    public override void OnDisable()
    {
        //Debug.Log("Left Room");
        gameController.UpdatePlayer();
        timeCounter.imageFill.fillAmount = 0;
    }   

    public void ArrangeCard()
    {      
        if(gameController==null) gameController = GameController.Instance;
        
        if(!isInvoke)
        {          
            foreach (var item in gameController.listBackCard)
            {
                item.gameObject.SetActive(false);     
                //Destroy(item.gameObject, 3f);    
            }
            gameController.listBackCard.Clear();

             posCard1 = cardTemplate1.transform.position;
             posCard2 = cardTemplate2.transform.position;          
            
            if (PvPlayer.IsMine)
            {
                int indexCard = Random.Range((int)0, (int)gameController.cards.Length);
                if (indexCard > 51)
                {
                    Debug.LogError($"index of Card over 51 and the new value is {indexCard}");
                    indexCard--;
                }
                Debug.Log($"index of Card over 51 and vlue is {indexCard}");
                PvPlayer.RPC("RPC_SetCard1", RpcTarget.All, indexCard);              
                //Debug.Log(indexCard);
                indexCard = Random.Range((int)0,(int)gameController.cards.Length);
                PvPlayer.RPC("RPC_SetCard2", RpcTarget.All, indexCard);             
                //Debug.Log(indexCard);
                PvPlayer.RPC("CoverCardOtherClient", RpcTarget.Others, null);
            }  
            // gameController.SetSmallBigBlind(gameController.listPlayer);        
            isInvoke = true;
            Invoke(nameof(SetIsInvoke), 20f);
        }
    }
    [PunRPC]
    public void RPC_SetCard1(int index)
    {
        if (gameController == null)
        {
            gameController = GameController.Instance;
        }
        gameController.cards[index].SetActive(true);
        gameController.cards[index].transform.position = posCard1;
        gameController.cards[index].GetComponent<SpriteRenderer>().sortingOrder = 4;
        card1 = gameController.cards[index];
        listCard.Add(card1);//add card to list to check
        //gameController.cards[index].transform.SetParent(this.transform);
        gameController.RemoveElement(ref gameController.cards, index);
    }
    [PunRPC]
    public void RPC_SetCard2(int index)
    {
        gameController.cards[index].SetActive(true);
        gameController.cards[index].transform.position = posCard2;
        gameController.cards[index].GetComponent<SpriteRenderer>().sortingOrder = 5;
        card2 = gameController.cards[index];
        listCard.Add(card2);
        //gameController.cards[index].transform.SetParent(this.transform);
        gameController.RemoveElement(ref gameController.cards, index);
    }
    [PunRPC]
    public void CoverCardOtherClient()
    {      
        cardTemplate1.SetActive(true);
        cardTemplate2.SetActive(true);
        card1.transform.position = cardTemplate1.transform.position;//fix sometime position not match;
        card2.transform.position = cardTemplate2.transform.position;
        cardTemplate1.transform.localScale = new Vector3(0.5f, 0.5f, 0.7f);
        cardTemplate2.transform.localScale = new Vector3(0.5f, 0.5f, 0.7f);

    }
    public void SetIsInvoke()
    {
        isInvoke = false;
    }

    [PunRPC]
    public void XemBai()
    {
        timeCounter.imageFill.fillAmount = 0f;
    }
    public void BtnXemBai()
    {       
        if (PvPlayer.IsMine)
        {
            PvPlayer.RPC("XemBai", RpcTarget.All, null);
        }
    }
    [PunRPC]
    public void BoBai()
    {
        timeCounter.imageFill.fillAmount = 0f;
        Color tempColor = Color.white;
        tempColor.a = 0.4f;
        GetComponent<SpriteRenderer>().color = tempColor;
        tempColor.a = 0f;
        card1.GetComponent<SpriteRenderer>().color = tempColor;
        card2.GetComponent<SpriteRenderer>().color = tempColor;
        cardTemplate1.GetComponent<SpriteRenderer>().color = tempColor;
        cardTemplate2.GetComponent<SpriteRenderer>().color = tempColor;
        isFold = true;
        gameController.UpdatePlayerPlayings();
    }
    public void BtnBoBai()
    {
        if (PvPlayer.IsMine)
        {
            PvPlayer.RPC("BoBai", RpcTarget.All, null);
            Color tempColor = Color.white;
            tempColor.a = 0.4f;
            card1.GetComponent<SpriteRenderer>().color = tempColor;
            card2.GetComponent<SpriteRenderer>().color = tempColor;
                                
        }
    }
    public void BtnThemCuoc()
    {       
        if (PvPlayer.IsMine)
        {
            bool temp = !gameController.pnlThemCuoc.activeSelf;
            gameController.pnlThemCuoc.SetActive(temp);
        }
    }
}
