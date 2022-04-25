using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.Events;
//using System.Globalization;
using ExitGames.Client.Photon;

public class PlayerController : MonoBehaviourPunCallbacks//,IPunObservable
{
    public GameController gameController;
    public GameController2 gameController2;
    

    public GameObject card1, card2, cardTemplate1, cardTemplate2;
    public GameObject bigBlindIcon;
    public GameObject rewardTopup;
    
    public Text txtMoney;
    public Text txtMoneyBlind;

    public UIManager uIManager;
    public TimeCounter timeCounter;
    public PhotonView PvPlayer;
    public UnityEvent eAddBackCard;
    public Bot bot;

    public Vector3 posCard1;
    public Vector3 posCard2;

    public Transform[] arrPosDefaul = new Transform[6];
    public List<GameObject> listCard = new List<GameObject>();
    public List<GameObject> listCardWin = new List<GameObject>();
    public int[] arrCardWin = new int[5];

    public int card1ID;
    public int card2ID;

    public float score = 0f;
    public long money = 1000000;
    public long moneyBlinding = 0;
    public long moneyBlinded = 0;
    

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
    public bool isBroke = false;
    public bool isWaiting = false;
    public bool isBot = false;

    public enum PhotonEventCodes
    {
        SyncLateJoin = 0
    }
    private void Awake()
    {
        gameController = GameController.Instance;
        gameController2 = GameController2.Instance;
        DontDestroyOnLoad(this.gameObject);
        uIManager = FindObjectOfType<UIManager>();
        gameObject.name = ID.ToString();
        bot = GetComponent<Bot>();
    }
    void Start()
    {
       
        //if(rewardTopup!=null)
        //rewardTopup.SetActive(true);
        if (gameController.isStartGame)
        {
            if(!PvPlayer.IsMine)
            {
                Debug.Log($"Player {this.ID} waiting!!");             
                gameController.SyncPlayerDatasJoinLate();               
            }
            isWaiting = true;                     
        }
        else
        {
            gameController.UpdatePlayer();
        }    
        
        cardTemplate1.GetComponent<SpriteRenderer>().sortingOrder = 5;
        cardTemplate2.GetComponent<SpriteRenderer>().sortingOrder = 6;      
        PvPlayer = GetComponent<PhotonView>();
        eAddBackCard.AddListener(() => ArrangeCard());
        for (int i = 0; i < arrPosDefaul.Length; i++)
        {
            if (ID == i)
            {
                transform.position = arrPosDefaul[i].position;
            }
        }
        
        //money = 10000000;
        moneyBlinding = 0;

        if(PvPlayer.IsMine)
        {
            uIManager.pnlGame.SetActive(false);
            Invoke(nameof(SetImageConnecting), 2f);
        }


        uIManager.btnOKBlind.onClick.AddListener(() => BtnOkBlind());
        uIManager.btnTheoCuoc.onClick.AddListener(() => BtnTheoCuoc());
        uIManager.btnXemBai.onClick.AddListener(() => BtnXemBai());
        uIManager.btnBoBai.onClick.AddListener(() => BtnBoBai());
        uIManager.btnThemCuoc.onClick.AddListener(() => BtnAllIn());
        uIManager.btnAllIn.onClick.AddListener(() => BtnAllIn());

        if (bot.enabled) isBot = true;

    }


    
    private void Update()
    {
        if (isFold && card1!=null)
        {
            if (card1?.transform.position != Vector3.zero)//Fold card
            {
                card1.transform.position = Vector3.Lerp(card1.transform.position, Vector3.zero, 0.02f);
                card2.transform.position = Vector3.Lerp(card1.transform.position, Vector3.zero, 0.02f);
                card1.transform.Rotate(1, 2, 1);
                card2.transform.Rotate(2, 1, 2);
            }
        }
        txtMoney.text = gameController.FormatVlueToString(money);
        txtMoneyBlind.text = gameController.FormatVlueToString(moneyBlinding);

        if (uIManager.pnlThemCuoc.activeSelf && PvPlayer.IsMine && (GetComponent<Bot>().enabled == false))
        {
            float temp = uIManager.sliderVlue.value;
            moneyBlinding = (long)(temp * money);
            uIManager.txtSetBlindVlue.text = gameController.FormatVlueToString(moneyBlinding);
        }

        //if (timeCounter.gameObject.activeSelf)
        //{
        //    if (PvPlayer.IsMine)
        //        uIManager.pnlGame.SetActive(true);
        //}
        //else
        //{
        //    if (PvPlayer.IsMine)
        //        uIManager.pnlGame.SetActive(false);
        //}

    }
    public override void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += NetworkingClient_EventReceived;
    }
    public override void OnDisable()
    {
        // Apply for All Player in this client when OnDisable
        PhotonNetwork.NetworkingClient.EventReceived -= NetworkingClient_EventReceived;
        try
        {          
            timeCounter.imageFill.fillAmount = 0;
            gameController.UpdatePlayerPlayings();
                    
            if (card1 != null)
            {
                card1?.SetActive(false);
                card2?.SetActive(false);
            }
        }
        catch
        {
            Debug.Log($"Error in PlayerController ID {PvPlayer.ViewID} when Disabled !!!");
        }

    }
    private void NetworkingClient_EventReceived(EventData obj)
    {
        if(obj.Code == (byte)PhotonEventCodes.SyncLateJoin)
        {
            
            object[] datas = obj.CustomData as object[];
            int viewID = (int)datas[0];
            if(PvPlayer.ViewID == viewID)
            {
                if(!PvPlayer.IsMine)
                {
                    cardTemplate1.SetActive(true);
                    cardTemplate2.SetActive(true);
                    cardTemplate1.transform.localScale = new Vector3(1f,1f,1f);
                    cardTemplate2.transform.localScale = new Vector3(1f,1f,1f);
                }              

                foreach (var item in gameController.cards)
                {
                    if(item.GetComponent<Card>().ID == (int)datas[1] && card1==null)
                    {
                        card1 = item;
                        //card1.SetActive(true);
                        card1.GetComponent<SpriteRenderer>().sortingOrder = 2;                   
                       // Debug.Log($"card1 added");
                    }
                    if (item.GetComponent<Card>().ID == (int)datas[2] && card2 == null)
                    {
                        card2 = item;
                        //card2.SetActive(true);
                        card2.GetComponent<SpriteRenderer>().sortingOrder = 3;
                        // Debug.Log($"card2 added");
                        Debug.Log($"Player {this.ID} with ID {PvPlayer.ViewID} added card1 and card2 to PlayerController ");
                    }
                }
                moneyBlinded  = (long)datas[3];
                money         = (long)datas[4];
                moneyBlinding = (long)datas[5];
                bot.enabled   = (bool)datas[6];
                isBot         = (bool)datas[7];
            }
        }
    }
    public void SyncPlayerJoinLate()
    {      
        if(card1!=null)
        {
            card1ID = card1.GetComponent<Card>().ID;
            card2ID = card2.GetComponent<Card>().ID;
        }
        else
        {
            Debug.Log($"player {ID} disconnected !!");
        }


        object[] datas = new object[]
        {
            PvPlayer.ViewID,              //0
            card1ID,//1
            card2ID,//2
            moneyBlinded,                 //3
            money,                        //4
            moneyBlinding,                //5
            bot.enabled,                  //6
            isBot,                        //7


            //card1.GetComponent<SpriteRenderer>().sortingOrder,
            //card2.GetComponent<SpriteRenderer>().sortingOrder
        };
        RaiseEventOptions option = new RaiseEventOptions()
        {
            Receivers = ReceiverGroup.All,
            CachingOption = EventCaching.DoNotCache
        };
        //Debug.Log($"ID {PvPlayer.ViewID} Invoke RaiseEvent and sent card1");
        Debug.Log($"Player {this.ID} with ID {PvPlayer.ViewID} Send Datas");
        PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.SyncLateJoin, datas, option,SendOptions.SendUnreliable);

    }

   

    public void ArrangeCard()
    {
        if (gameController == null) gameController = GameController.Instance;

        if (!isInvoke)
        {
            foreach (var item in gameController.listBackCard)
            {
                item.gameObject.SetActive(false);
                Destroy(item.gameObject, 3f);    
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
                
                PvPlayer.RPC("RPC_SetCard1", RpcTarget.All, indexCard);
               
                //Debug.Log(indexCard);
                indexCard = Random.Range((int)0, (int)gameController.cards.Length);
                PvPlayer.RPC("RPC_SetCard2", RpcTarget.All, indexCard);

                //Debug.Log(indexCard);
                if (GetComponent<Bot>().enabled == true)
                {
                    PvPlayer.RPC("CoverCardOtherClient", RpcTarget.All, null);
                   
                }
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
       // RefreshListcard();
        if (gameController.cards[index] == null)
        {
            index--;
            //gameController = GameController.Instance;
            Debug.Log("gameController is null");
        }
        gameController.cards[index].SetActive(true);
        gameController.cards[index].transform.position = posCard1;
        gameController.cards[index].GetComponent<SpriteRenderer>().sortingOrder = 4;
        card1 = gameController.cards[index];
        listCard.Add(card1);//add card to list to check
        //gameController.cards[index].transform.SetParent(this.transform);
        gameController.listCardsRemoved.Add(card1.GetComponent<Card>().ID);
        gameController.RemoveElement(ref gameController.cards, index);
    }
    [PunRPC]
    public void RPC_SetCard2(int index)
    {
        
        if (gameController.cards[index] == null)
        {
            index--;          
            Debug.Log("gameController2 is null");
        }
        gameController.cards[index].SetActive(true);
        gameController.cards[index].transform.position = posCard2;
        gameController.cards[index].GetComponent<SpriteRenderer>().sortingOrder = 5;
        card2 = gameController.cards[index];
        listCard.Add(card2);
        //gameController.cards[index].transform.SetParent(this.transform);
        gameController.listCardsRemoved.Add(card2.GetComponent<Card>().ID);
        gameController.RemoveElement(ref gameController.cards, index);
    }
    [PunRPC]
    public void CoverCardOtherClient()
    {
        cardTemplate1.SetActive(true);
        cardTemplate2.SetActive(true);
        card1.transform.position = cardTemplate1.transform.position;//fix sometime position not match;
        card2.transform.position = cardTemplate2.transform.position;
        cardTemplate1.transform.localScale = new Vector3(1f,1f,1f);
        cardTemplate2.transform.localScale = new Vector3(1f,1f,1f);

    }
    public void SetIsInvoke()
    {
        isInvoke = false;
    }

    [PunRPC]
    public virtual void XemBai()
    {
        timeCounter.imageFill.fillAmount = 0f;
    }
    public virtual void BtnXemBai()
    {
        if (PvPlayer.IsMine && gameController.isStartGame)
        {
            if (GetComponent<Bot>().enabled == false)
            PvPlayer.RPC("XemBai", RpcTarget.All, null);
        }
    }
    public virtual void BtnXemBaiBot()
    {
        if (PvPlayer.IsMine && gameController.isStartGame)
        {
            PvPlayer.RPC("XemBai", RpcTarget.All, null);
        }
    }
    [PunRPC]
    public virtual void BoBai()
    {
        isFold = true;
        timeCounter.imageFill.fillAmount = 0f;
        Color tempColor = Color.white;
        tempColor.a = 0.3f;
        GetComponent<SpriteRenderer>().color = tempColor;
        Invoke("HandleBoBai", 0.4f);
        cardTemplate1.SetActive(false);
        cardTemplate2.SetActive(false);       
        gameController.UpdatePlayerPlayings();
    }
    public virtual void BtnBoBai()
    {
        if (PvPlayer.IsMine && gameController.isStartGame)
        {
            if (bot.enabled == false)
                PvPlayer.RPC("BoBai", RpcTarget.All, null);
            //Color tempColor = Color.white;
            //tempColor.a = 0.4f;
            //card1.GetComponent<SpriteRenderer>().color = tempColor;
            //card2.GetComponent<SpriteRenderer>().color = tempColor;
        }
    }
    public virtual void BtnBoBaiBot()
    {
        if (PvPlayer.IsMine && gameController.isStartGame)
        {          
            PvPlayer.RPC("BoBai", RpcTarget.All, null);        
        }
    }
    public void BtnAllIn()
    {
        if (PvPlayer.IsMine && (bot.enabled == false))
        {
            moneyBlinding = money;//Theo cuoc
            PvPlayer.RPC("SetValueBlind", RpcTarget.All, moneyBlinding);
           
            BtnXemBai();

            uIManager.pnlThemCuoc.SetActive(false);
            uIManager.pnlGame.SetActive(false);
        }
    }
    public void HandleBoBai()
    {
        //if (!PvPlayer.IsMine)
        //{
        if(card1!=null)
        {
            card1.SetActive(false);
            card2.SetActive(false);
        }
       
        //}
    }
    [PunRPC]
    public void SetValueBlind(long vlue)
    {
        if (vlue > money)
        {
            //All in
            vlue = money;
        }
        moneyBlinding = vlue;
        money -= vlue;
        gameController.barTotalMoney += vlue;
        moneyBlinded += vlue;
        gameController.UpdateBlind();

    }
    public void BtnOkBlind()
    {
        if (PvPlayer.IsMine && (bot.enabled == false))
        {
            moneyBlinding = gameController.bigestBlinded - moneyBlinded;//Theo cuoc
            PvPlayer.RPC("SetValueBlind", RpcTarget.All, moneyBlinding);

            float temp = uIManager.sliderVlue.value;
            moneyBlinding = (long)(temp * money);
            PvPlayer.RPC("SetValueBlind", RpcTarget.All, moneyBlinding);
            BtnXemBai();

            uIManager.pnlThemCuoc.SetActive(false);
            uIManager.pnlGame.SetActive(false);
        }
    }
    public virtual void BtnTheoCuoc()
    {
        if (PvPlayer.IsMine && (bot.enabled == false))
        {
            uIManager.pnlGame.SetActive(false);
            moneyBlinding = gameController.bigestBlinded - moneyBlinded;
            PvPlayer.RPC("SetValueBlind", RpcTarget.All, moneyBlinding);
            BtnXemBai();
        }
    }
    public void BtnTheoCuocBot()
    {
        if (PvPlayer.IsMine)
        {            
            moneyBlinding = gameController.bigestBlinded - moneyBlinded;
            PvPlayer.RPC("SetValueBlind", RpcTarget.All, moneyBlinding);
            BtnXemBaiBot();
        }
    }
    public void SetImageConnecting()
    {
        uIManager.imageConnecting.gameObject.SetActive(false);
    }

    //public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    //{
    //    // there is a crazy bug in this class "specifyed cast is not valid" because you click on the player owner first !!!
    //    if (stream.IsWriting)
    //    {
    //        Debug.Log($"Player {PvPlayer.ViewID} sends datas to Sync");
    //        stream.SendNext(moneyBlinded);            
    //    }
    //    else if (stream.IsReading)
    //    {
    //        Debug.Log($"Player {PvPlayer.ViewID} received datas to Sync");
    //        moneyBlinded = (long)stream.ReceiveNext();        
    //    }
    //}
}
