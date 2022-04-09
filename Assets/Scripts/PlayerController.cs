using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.Events;
using System.Globalization;
using ExitGames.Client.Photon;

public class PlayerController : MonoBehaviourPunCallbacks//,IPunObservable
{
    public GameController gameController;
    public GameController2 gameController2;

    public GameObject card1, card2, cardTemplate1, cardTemplate2;
    public GameObject bigBlindIcon;
    
    public Text txtMoney;
    public Text txtMoneyBlind;

    public UIManager uIManager;
    public TimeCounter timeCounter;
    public PhotonView PvPlayer;
    public UnityEvent eAddBackCard;

    public Vector3 posCard1;
    public Vector3 posCard2;

    public Transform[] arrPosDefaul = new Transform[6];
    public List<GameObject> listCard = new List<GameObject>();
    public List<GameObject> listCardWin = new List<GameObject>();
    //public Dictionary<int,object> dicBufferedCard= new Dictionary<int,object>();
    public int[] arrCardWin = new int[5];

    public float score = 0f;
    public long money = 1000000;
    public long moneyBlinding = 0;
    public long moneyBlinded = 0;

    public int ID = 0;
    public int test = 0;
    

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
    }
    void Start()
    {
        if (gameController.isStartGame)
        {
            if(!PvPlayer.IsMine)
            {
                Debug.Log($"Player waiting!!");
                //Debug.Log(dicBufferedCard.Count);
                gameController.SyncCardLateJoin();

                
            }
            isWaiting = true;
                       
        }
        else
        {
            gameController.UpdatePlayer();
        }
        //gameController.UpdatePlayer();
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
        money = 10000000;
        moneyBlinding = 0;

        if(PvPlayer.IsMine)
        uIManager.pnlGame.SetActive(false);

        uIManager.btnOKBlind.onClick.AddListener(() => BtnOkBlind());
        uIManager.btnTheoCuoc.onClick.AddListener(() => BtnTheoCuoc());
        uIManager.btnXemBai.onClick.AddListener(() => BtnXemBai());
        uIManager.btnBoBai.onClick.AddListener(() => BtnBoBai());
        uIManager.btnThemCuoc.onClick.AddListener(() => BtnThemCuoc());

        if(PvPlayer.IsMine)
        {
            test = Random.Range(0, 100);
            PvPlayer.RPC("SetTest", RpcTarget.All, test);          
        }
        Debug.Log($"Random test is {test}");

    }


    [PunRPC]
    public void SetTest(int vlue)
    {
        test = vlue;
    }
    private void Update()
    {
        if (isFold && !PvPlayer.IsMine)
        {
            if (card1.transform.position != Vector3.zero)//Fold card
            {
                card1.transform.position = Vector3.Lerp(card1.transform.position, Vector3.zero, 0.02f);
                card2.transform.position = Vector3.Lerp(card1.transform.position, Vector3.zero, 0.02f);
                card1.transform.Rotate(1, 2, 1);
                card2.transform.Rotate(2, 1, 2);
            }
        }
        txtMoney.text = gameController.FormatVlueToString(money);
        txtMoneyBlind.text = gameController.FormatVlueToString(moneyBlinding);

        if (uIManager.pnlThemCuoc.activeSelf && PvPlayer.IsMine)
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
    private void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= NetworkingClient_EventReceived;


        //gameController.UpdatePlayer();
        //if (!PvPlayer.IsMine)
        //{
        //    timeCounter.imageFill.fillAmount = 0;
        //    card1?.SetActive(false);
        //    card2?.SetActive(false);

        //}
        //Debug.Log("Left Room 2");

        //if (card1 != null && card2 != null)
        //{
        //    card1.SetActive(false);
        //    card2.SetActive(false);
        //}
    }
    private void NetworkingClient_EventReceived(EventData obj)
    {
        if(obj.Code == (byte)PhotonEventCodes.SyncLateJoin)
        {
            
            object[] datas = obj.CustomData as object[];
            int viewID = (int)datas[0];
            if(PvPlayer.ViewID == viewID)
            {
                //card1 = (GameObject)datas[1];
                //Debug.Log($"ID { PvPlayer.ViewID} Received Event and card1 name is {card1.name}");
                foreach (var item in gameController.cards)
                {
                    if(item.GetComponent<Card>().ID == (int)datas[1])
                    {
                        card1 = item;
                        Debug.Log($"card1 added");
                    }
                    if (item.GetComponent<Card>().ID == (int)datas[2])
                    {
                        card2 = item;
                        Debug.Log($"card2 added");
                    }
                }

                //int[] temp = (int[])datas[3];
                //foreach (var item in gameController.cards)
                //{
                //    Debug.Log($"ID to Push is {temp[0]}");
                //    if(item.GetComponent<Card>().ID == temp[0])
                //        gameController.stackCheck.Push(item);
                //    Debug.Log($"ID to Push is {temp[1]}");
                //    if (item.GetComponent<Card>().ID == temp[1])
                //        gameController.stackCheck.Push(item);
                //    Debug.Log($"ID to Push is {temp[2]}");
                //    if (item.GetComponent<Card>().ID == temp[2])
                //        gameController.stackCheck.Push(item);
                //    break;
                //}

                //gameController.stackCheck.Push(gameController.arrSaveIDCardToSync[1]);
                //Debug.Log($"ID { PvPlayer.ViewID} Received Event and test is {test}");

            }          
            //card1 = (GameObject)datas[0];
            //card2 = (GameObject)datas[1];
        }
    }
    public void SyncPlayerJoinLate()
    {

        object[] datas = new object[]
        {
            PvPlayer.ViewID,
            card1.GetComponent<Card>().ID,
            card2.GetComponent<Card>().ID,
            //gameController.arrSaveIDCardToSync  // it should be a new RaiseEvent
        };
        RaiseEventOptions option = new RaiseEventOptions()
        {
            Receivers = ReceiverGroup.All,
            CachingOption = EventCaching.DoNotCache
        };
        //Debug.Log($"ID {PvPlayer.ViewID} Invoke RaiseEvent and sent card1");
        Debug.Log($"ID {PvPlayer.ViewID} Invoke RaiseEvent and sent test {test}");
        PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.SyncLateJoin, datas, option,
            ExitGames.Client.Photon.SendOptions.SendUnreliable);

    }

   

    public void ArrangeCard()
    {
        if (gameController == null) gameController = GameController.Instance;

        if (!isInvoke)
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
                
                PvPlayer.RPC("RPC_SetCard1", RpcTarget.All, indexCard);
                //PvPlayer.RPC("BuffererCard", RpcTarget.All,1,indexCard);
                //Debug.Log(indexCard);
                indexCard = Random.Range((int)0, (int)gameController.cards.Length);
                PvPlayer.RPC("RPC_SetCard2", RpcTarget.All, indexCard);
                //PvPlayer.RPC("BuffererCard", RpcTarget.All,2, indexCard);
                //Debug.Log(indexCard);
                PvPlayer.RPC("CoverCardOtherClient", RpcTarget.Others, null);
            }
            // gameController.SetSmallBigBlind(gameController.listPlayer);        
            isInvoke = true;
            Invoke(nameof(SetIsInvoke), 20f);
        }
    }
    //[PunRPC]
    //public void BuffererCard(int name, int index)
    //{
    //    dicBufferedCard.Remove(name);
    //    dicBufferedCard.Add(name, gameController.cards[index]);
    //}
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
        //RefreshListcard();
        if (gameController.cards[index] == null)
        {
            index--;
            //gameController = GameController.Instance;
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
        cardTemplate1.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
        cardTemplate2.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);

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
        if (PvPlayer.IsMine && gameController.isStartGame)
        {
            PvPlayer.RPC("XemBai", RpcTarget.All, null);
        }
    }
    [PunRPC]
    public void BoBai()
    {
        timeCounter.imageFill.fillAmount = 0f;
        Color tempColor = Color.white;
        tempColor.a = 0.3f;
        GetComponent<SpriteRenderer>().color = tempColor;
        Invoke("HandleBoBai", 0.4f);
        cardTemplate1.SetActive(false);
        cardTemplate2.SetActive(false);
        isFold = true;
        gameController.UpdatePlayerPlayings();
    }
    public void BtnBoBai()
    {
        if (PvPlayer.IsMine && gameController.isStartGame)
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
        if (PvPlayer.IsMine && gameController.isStartGame)
        {
            bool temp = !uIManager.pnlThemCuoc.activeSelf;
            uIManager.pnlThemCuoc.SetActive(temp);
        }
    }
    public void HandleBoBai()
    {
        if (!PvPlayer.IsMine)
        {
            card1.SetActive(false);
            card2.SetActive(false);
        }
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
        if (PvPlayer.IsMine)
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
    public void BtnTheoCuoc()
    {
        if (PvPlayer.IsMine)
        {
            uIManager.pnlGame.SetActive(false);
            moneyBlinding = gameController.bigestBlinded - moneyBlinded;
            PvPlayer.RPC("SetValueBlind", RpcTarget.All, moneyBlinding);
            BtnXemBai();
        }
    }

    //public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    //{
    //    if(stream.IsWriting)
    //    {
    //        stream.SendNext(dicBufferedCard);
    //        //Debug.Log("Send");
    //    }
    //    else if(stream.IsReading)
    //    {
    //        dicBufferedCard = (Dictionary<int,object>)stream.ReceiveNext();
    //        //Debug.Log("Receive");
    //    }
    //}
    //[PunRPC]
    //public void RefreshListcard()
    //{
    //    for (int j = 0; j < listCard.Count; j++)
    //    {
    //        if (listCard[j] == null)
    //            listCard.RemoveAt(j);
    //    }
    //}
    
}
