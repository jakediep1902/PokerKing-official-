using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.Events;
//using System.Globalization;
using ExitGames.Client.Photon;
using Photon.Pun.Demo.Cockpit;

public class PlayerController : MonoBehaviourPunCallbacks//, IPunObservable
{
    public GameController gameController;
    public GameController2 gameController2;
    public PlayFabManager playFabManager;
    AudioSource audioSource;
    AudioSource audioSource2;
    public List<AudioClip> listAudio = new List<AudioClip>();

    public GameObject card1, card2, cardTemplate1, cardTemplate2;
    public GameObject bigBlindIcon;
    public GameObject rewardTopup;

    public Text txtMoney;
    public Text txtMoneyBlind;
    public Text txtDisplayName;

    public UIManager uIManager;
    public TimeCounter timeCounter;
    public PhotonView PvPlayer;
    public UnityEvent eAddBackCard;
    public Bot bot;
    public UserData userData = new UserData();

    public Vector3 posCard1;
    public Vector3 posCard2;

    public Transform[] arrPosDefaul = new Transform[6];
    public List<GameObject> listCard = new List<GameObject>();
    public List<GameObject> listCardWin = new List<GameObject>();

    public int[] arrCardWin = new int[5];

    public int card1ID;
    public int card2ID;
    public int ID = 0;
    public static int countIndex = 0;

    public string strNameDisplay {
        get => txtDisplayName.text;
        set { txtDisplayName.text = value; } }

    public float score = 0f;
    public long money = 1000000;
    public long moneyBlinding = 0;
    public long moneyBlinded = 0;

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
    public bool isFirstPlayer = false;

    public bool isTurn = false;
    public bool isFold = false;
    public bool isBroke = false;
    public bool isWaiting = false;
    public bool isBot = false;


    public enum PhotonEventCodes
    {
        SyncLateJoin = 0,
        SyncOnLoadScene = 1,
        SyncDataPlayerFromMaster = 2,
    }
    private void Awake()
    {
        gameController = GameController.Instance;
        gameController2 = GameController2.Instance;
        playFabManager = PlayFabManager.Instance;
        SetupAudioSource();
        DontDestroyOnLoad(this.gameObject);
        uIManager = FindObjectOfType<UIManager>();
        gameObject.name = ID.ToString();
        bot = GetComponent<Bot>();
    }
    void Start()
    {             
        if (PvPlayer.IsMine)
        {          
            strNameDisplay = playFabManager.userData.userName;
            money = playFabManager.userData.money;            
            if (playFabManager.userData.userName.Equals("ADMINS") ||
                playFabManager.userData.userName.Equals("ADMINS1"))
            {
                money = 8888888;//Defaul money for admin test
            }
            uIManager.pnlGame.SetActive(false);
            Invoke(nameof(SetImageConnecting), 2f);
        }
        //else
        //{
        //    RPC_RequestSyncDataFromRemote();// only remote request Sync data
        //}  
        if (gameController.isStartGame)
        {
            if (!PvPlayer.IsMine)
            {
                Debug.Log($"Player {this.ID} waiting!!");
                gameController.SyncPlayersDatasJoinLate();
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
            if (ID == i) transform.position = arrPosDefaul[i].position;
        }
        
        moneyBlinding = 0;

        #region add Button
        uIManager.btnOKBlind.onClick.AddListener(() => BtnOkBlind());
        uIManager.btnTheoCuoc.onClick.AddListener(() => BtnTheoCuoc());
        uIManager.btnXemBai.onClick.AddListener(() => BtnXemBai());
        uIManager.btnBoBai.onClick.AddListener(() => BtnBoBai());
        uIManager.btnThemCuoc.onClick.AddListener(() => BtnThemCuoc());
        uIManager.btnAllIn.onClick.AddListener(() => BtnAllIn());
        #endregion

        if (bot.enabled && !isWaiting && PvPlayer.IsMine)
        {
            isBot = true;
            money = (long)Random.Range(200000, 5000000);
            int random = Random.Range((int)0, (int)userData.namesTemplate.Length);
            txtDisplayName.text = userData.namesTemplate[random];
            //SyncPlayerOnLoadScene();
            //Debug.Log($"{gameObject.name}");
        }
        //else if (PvPlayer.IsMine)
        //{
        //    UpdateDataPlayerFromServer();// use on build
        //}

        if(!PvPlayer.IsMine && !isBot) RPC_RequestSyncDataFromRemote();// only realPlayer remote request Sync data
    }
    private void Update()
    {
        if (isFold && card1 != null) FoldCard();

        txtMoney.text = gameController.FormatVlueToString(money);
        txtMoneyBlind.text = gameController.FormatVlueToString(moneyBlinding);

        if (uIManager.pnlThemCuoc.activeSelf && PvPlayer.IsMine && (isBot == false))
        {
            float temp = uIManager.sliderVlue.value;
            moneyBlinding = (long)(temp * money);
            uIManager.txtSetBlindVlue.text = gameController.FormatVlueToString(moneyBlinding);
        }
    }
    public override void OnEnable()
    {        
        PhotonNetwork.NetworkingClient.EventReceived += NetworkingClient_EventReceived;
    }
    public override void OnDisable()
    {
        // Apply for All Player in this client when OnDisable
        PhotonNetwork.NetworkingClient.EventReceived -= NetworkingClient_EventReceived;
        if (!isBot) RequestSaveData();
        try
        {
            timeCounter.CheckNextPlayer();

            BtnBoBai();
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
            if (!PvPlayer.IsMine) gameController.BtnDeal();
        }
    }
    private void NetworkingClient_EventReceived(EventData obj)
    {
        if(PvPlayer.IsMine)
        {
            return;
        }
        else
        {
            if (obj.Code == (byte)PhotonEventCodes.SyncLateJoin)
            {
                object[] datas = obj.CustomData as object[];
                //Debug.Log($"datas lenght is {datas.Length} + {countIndex++} + {gameObject.name}");
                int viewID = (int)datas[0];
                if (PvPlayer.ViewID == viewID)
                {
                    if (!PvPlayer.IsMine)
                    {                      
                        cardTemplate1.SetActive(true);
                        cardTemplate2.SetActive(true);
                        cardTemplate1.transform.localScale = new Vector3(1f, 1f, 1f);
                        cardTemplate2.transform.localScale = new Vector3(1f, 1f, 1f);
                    }

                    foreach (var item in gameController.cards)
                    {
                        if (item.GetComponent<Card>().ID == (int)datas[1] && card1 == null)
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
                    moneyBlinded = (long)datas[3];
                    money = (long)datas[4];
                    moneyBlinding = (long)datas[5];
                    bot.enabled = (bool)datas[6];
                    isBot = (bool)datas[7];
                    strNameDisplay = datas[8].ToString();
                    isTurn = (bool)datas[9];
                    isWaiting = (bool)datas[10];
                }
            }

            if (obj.Code == (byte)PhotonEventCodes.SyncOnLoadScene)
            {
                object[] datas = obj.CustomData as object[];
                int viewID = (int)datas[0];
                if (PvPlayer.ViewID == viewID)
                {
                    txtDisplayName.text = datas[1].ToString();
                    money = (long)datas[2];
                }
            }

            if (obj.Code == (byte)PhotonEventCodes.SyncDataPlayerFromMaster)
            {
                object[] datas = obj.CustomData as object[];
                int viewID = (int)datas[0];
                if (PvPlayer.ViewID == viewID)
                {
                    strNameDisplay = datas[1].ToString();
                    money = (long)datas[2];
                }
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
        else Debug.Log($"player {ID} disconnected !!");       

        object[] datas = new object[]
        {
            PvPlayer.ViewID,              //0
            card1ID,                      //1
            card2ID,                      //2
            moneyBlinded,                 //3
            money,                        //4
            moneyBlinding,                //5
            bot.enabled,                  //6
            isBot,                        //7
            strNameDisplay,               //8
            isTurn,                       //9
            isWaiting,                    //10

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
    public void SyncPlayerOnLoadScene()
    {
        object[] datas = new object[]
        {
            PvPlayer.ViewID,          //0
            txtDisplayName.text,      //1
            money,                    //2
        };
        RaiseEventOptions option = new RaiseEventOptions()
        {
            Receivers = ReceiverGroup.All,
            CachingOption = EventCaching.DoNotCache
        };
        PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.SyncOnLoadScene, datas, option, SendOptions.SendUnreliable);
    }
    public void RequestSyncDataFromMaster()
    {
    //    if (card1 != null)
    //    {
    //        card1ID = card1.GetComponent<Card>().ID;
    //        card2ID = card2.GetComponent<Card>().ID;
    //    }
        object[] datas = new object[]
        {
            PvPlayer.ViewID,              //0
            strNameDisplay,               //1
            money,                        //2

            //PvPlayer.ViewID,              //0
            //card1ID,                      //1
            //card2ID,                      //2
            //moneyBlinded,                 //3
            //money,                        //4
            //moneyBlinding,                //5
            //bot.enabled,                  //6
            //isBot,                        //7
            //strNameDisplay,               //8
            //isTurn,                       //9
            //isWaiting,                    //10


        };
        RaiseEventOptions option = new RaiseEventOptions()
        {
            Receivers = ReceiverGroup.Others,
            CachingOption = EventCaching.DoNotCache,
        };
        PhotonNetwork.RaiseEvent((byte)PhotonEventCodes.SyncDataPlayerFromMaster, datas, option, SendOptions.SendReliable);
    }
    [PunRPC]
    public void RequestSyncDataFromRemote()
    {
        //Debug.Log($"{PvPlayer.ViewID} Request Sync Data from remote");
        RequestSyncDataFromMaster();
    }
    
    public void RPC_RequestSyncDataFromRemote()
    {
        PvPlayer.RPC(nameof(RequestSyncDataFromRemote), RpcTarget.All, null);
    }
    public void UpdateDataPlayerFromServer()
    {
        if (playFabManager != null)
        {
            userData = playFabManager.userData;
            long temp = userData.money;

            if (userData.userName.Equals("ADMINS"))
            {
                money = 8888888;
                temp = money;
            }
            else
            {
                money = userData.money;
            }
           
            txtDisplayName.text = userData.userName;
            string name = userData.userName;
            Debug.Log("Set user name :" + userData.userName);
            PvPlayer.RPC("SetNameDisplayPlayer", RpcTarget.All,name);
            PvPlayer.RPC("SetMoneyPlayer", RpcTarget.All, temp);
           
            //int random = Random.Range((int)0, (int)9);
           
            //PvPlayer.RPC("SetDataPlayer", RpcTarget.Others, money, txtDisplayName.text);
        }      
    }
    [PunRPC]
    public void SetNameDisplayPlayer(string userName)
    {          
        txtDisplayName.text = userName;
    }
    [PunRPC]
    public void SetMoneyPlayer(long money)
    {
        this.money = money;      
    }
    public void FoldCard()
    {     
        if (card1?.transform.position != Vector3.zero && gameController.photonViews.IsMine && card1.activeSelf)//Fold card
        {
            //Debug.Log($"Player {this.ID}");
            card1.transform.position = Vector3.Lerp(card1.transform.position, Vector3.zero, 0.02f);
            card2.transform.position = Vector3.Lerp(card1.transform.position, Vector3.zero, 0.02f);
            card1.transform.Rotate(1, 2, 1);
            card2.transform.Rotate(2, 1, 2);
        }
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

                gameController.cards[indexCard].transform.position = posCard1;
                PvPlayer.RPC("RPC_SetCard1", RpcTarget.All, indexCard);
               
                //Debug.Log(indexCard);
                indexCard = Random.Range((int)0, (int)gameController.cards.Length);
                gameController.cards[indexCard].transform.position = posCard2;
                PvPlayer.RPC("RPC_SetCard2", RpcTarget.All, indexCard);

                //Debug.Log(indexCard);
                if (isBot) PvPlayer.RPC("CoverCardOtherClient", RpcTarget.All, null);
              
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
        if (gameController.cards[index] == null)
        {
            index--;
            Debug.Log("gameController is null");
        }
        card1 = gameController.cards[index];
        //card1.SetActive(false);
        card1.GetComponent<SpriteRenderer>().enabled = false;
        Invoke(nameof(DelayActiveCard1), 1.3f);
        gameController.cards[index].GetComponent<SpriteRenderer>().sortingOrder = 4;
        listCard.Add(card1);//add card to list to check
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
        card2 = gameController.cards[index];
        //card2.SetActive(false);
        card2.GetComponent<SpriteRenderer>().enabled = false;
        Invoke(nameof(DelayActiveCard2), 1.3f);
        gameController.cards[index].GetComponent<SpriteRenderer>().sortingOrder = 5;
        listCard.Add(card2);
        gameController.listCardsRemoved.Add(card2.GetComponent<Card>().ID);
        gameController.RemoveElement(ref gameController.cards, index);
    }
    public void DelayActiveCard1()
    {
        card1.GetComponent<SpriteRenderer>().enabled = true;
    }
    public void DelayActiveCard2()
    {
        card2.GetComponent<SpriteRenderer>().enabled = true;
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
    public void SetIsInvoke() => isInvoke = false;

    [PunRPC]
    public virtual void XemBai()
    {
        timeCounter.imageFill.fillAmount = 0f;
        SetClipToPlay("check");
    }
    [PunRPC]
    public virtual void XemBaiSetAudio(string clipName="")
    {
        timeCounter.imageFill.fillAmount = 0f;
        SetClipToPlay(clipName,"dice_chipin");
    }
    public virtual void BtnXemBai()
    {
        if (PvPlayer.IsMine && gameController.isStartGame && !isBot) PvPlayer.RPC("XemBai", RpcTarget.All, null);     
    }
    public virtual void BtnXemBaiBot()
    {
        if (PvPlayer.IsMine && gameController.isStartGame && isBot)
        {
            if(moneyBlinded<gameController.bigestBlinded)
            {
                BtnBoBaiBot();
            }
            else
            {
                PvPlayer.RPC("XemBai", RpcTarget.All, null);
            }      
        }
    }
    [PunRPC]
    public virtual void BoBai()
    {
        isFold = true;
        timeCounter.imageFill.fillAmount = 0f;
        SetClipToPlay("fold","giveup");
        Color tempColor = Color.white;
        tempColor.a = 0.3f;
        GetComponent<SpriteRenderer>().color = tempColor;
        Invoke("HandleBoBai", 0.7f);
        if(card1 && card2)
        {
            card1.GetComponent<SpriteRenderer>().sprite = cardTemplate1.GetComponent<SpriteRenderer>().sprite;
            card2.GetComponent<SpriteRenderer>().sprite = cardTemplate2.GetComponent<SpriteRenderer>().sprite;
        }
        cardTemplate1.SetActive(false);
        cardTemplate2.SetActive(false);       
        gameController.UpdatePlayerPlayings();
    }
    public virtual void BtnBoBai()
    {
        if (PvPlayer.IsMine && gameController.isStartGame)
        {
            if (isBot == false) PvPlayer.RPC("BoBai", RpcTarget.All, null);
            //Color tempColor = Color.white;
            //tempColor.a = 0.4f;
            //card1.GetComponent<SpriteRenderer>().color = tempColor;
            //card2.GetComponent<SpriteRenderer>().color = tempColor;
        }
    }
    public virtual void BtnBoBaiBot()
    {
        if (PvPlayer.IsMine && gameController.isStartGame && isBot && isTurn) PvPlayer.RPC("BoBai", RpcTarget.All, null);       
    }
    public void BtnThemCuoc()//player only
    {
        if (PvPlayer.IsMine && !isBot)
        {
            //moneyBlinding = money;//Theo cuoc
            //PvPlayer.RPC("SetValueBlind", RpcTarget.All, moneyBlinding);
            //BtnXemBai();
           // Debug.Log("Pressed");
            uIManager.pnlThemCuoc.SetActive(!uIManager.pnlThemCuoc.activeSelf);          
        }
    }
    public void BtnAllIn()
    {
        if (PvPlayer.IsMine && !isBot)
        {
            moneyBlinding = money;//Theo cuoc
            PvPlayer.RPC("SetValueBlind", RpcTarget.All, moneyBlinding);
            if (gameController.isStartGame) PvPlayer.RPC("XemBaiSetAudio", RpcTarget.All, "allin");
            //BtnXemBai();
            uIManager.pnlThemCuoc.SetActive(false);
            uIManager.pnlGame.SetActive(false);
        }
    }
    public void BtnAllInBot()
    {
        if (PvPlayer.IsMine && isBot)
        {
            moneyBlinding = money;//Theo cuoc
            PvPlayer.RPC("SetValueBlind", RpcTarget.All, moneyBlinding);
            if (gameController.isStartGame) PvPlayer.RPC("XemBaiSetAudio", RpcTarget.All,"allin");

            //BtnXemBaiBot();       
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
        if (PvPlayer.IsMine && !isBot)
        {
            moneyBlinding = gameController.bigestBlinded - moneyBlinded;//Theo cuoc
            PvPlayer.RPC("SetValueBlind", RpcTarget.All, moneyBlinding);

            float temp = uIManager.sliderVlue.value;
            moneyBlinding = (long)(temp * money);
            PvPlayer.RPC("SetValueBlind", RpcTarget.All, moneyBlinding);

            if (gameController.isStartGame) PvPlayer.RPC("XemBaiSetAudio", RpcTarget.All,  "raise");
            //BtnXemBai();
            uIManager.pnlThemCuoc.SetActive(false);
            uIManager.pnlGame.SetActive(false);
        }
    }
    public virtual void BtnTheoCuoc()
    {
        if (PvPlayer.IsMine && !isBot)
        {
            uIManager.pnlGame.SetActive(false);
            moneyBlinding = gameController.bigestBlinded - moneyBlinded;
            PvPlayer.RPC("SetValueBlind", RpcTarget.All, moneyBlinding);

            if (gameController.isStartGame) PvPlayer.RPC("XemBaiSetAudio", RpcTarget.All, "call");
            //BtnXemBai();
        }
    }
    public void BtnTheoCuocBot()
    {
        if (PvPlayer.IsMine && isBot)
        {            
            moneyBlinding = gameController.bigestBlinded - moneyBlinded;
            PvPlayer.RPC("SetValueBlind", RpcTarget.All, moneyBlinding);

            if (gameController.isStartGame) PvPlayer.RPC("XemBaiSetAudio", RpcTarget.All, "call");
            //BtnXemBaiBot();
        }
    }
    public void SetImageConnecting()=> uIManager.imageConnecting.gameObject.SetActive(false);

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
    //        Debug.Log($"Player {PvPlayer.ViewID} received datas to ");
    //        moneyBlinded = (long)stream.ReceiveNext();
    //    }
    //}
    public void RequestSaveData()
    {
        userData.money = money;
        userData.userName = txtDisplayName.text;
        playFabManager.userData = userData;
        playFabManager.SaveDatasUser();
    }
    public void SetupAudioSource()
    {
        gameObject.AddComponent<AudioSource>();
        audioSource = GetComponent<AudioSource>();
        audioSource2 = gameObject.AddComponent<AudioSource>();
        var listAudios = FindObjectOfType<AudioListPlayer>();
        listAudio = listAudios.listAudio;     
    }    
    public void SetClipToPlay(string clipName="",string clipName2 ="")
    {
        foreach (var item in listAudio)
        {
            if(item.name==clipName)
            {
                audioSource.clip = item;
                audioSource.Play();
                break;
            }            
        }
        foreach (var item2 in listAudio)
        {
            if (item2.name == clipName2)
            {
                audioSource2.clip = item2;
                audioSource2.Play();
                break;
            }
        }
        //audioSource.clip = listAudio[index];       
    }
    public void SetClipToPlayDelay(string clipName = "",float delay=2f)
    {
        foreach (var item in listAudio)
        {
            if (item.name == clipName)
            {
                audioSource.clip = item;
                audioSource.PlayDelayed(delay);
                break;
            }
        }                 
    }
}
