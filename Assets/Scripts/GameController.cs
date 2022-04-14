using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Reflection;
using UnityEditor;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.Events;
using ExitGames.Client.Photon;
using System.Linq;

public class GameController : MonoBehaviourPunCallbacks,IPunObservable
{
    public static GameController Instance;

    UIManager uIManager;
    ManageNetwork manageNetwork;
    GameController2 gameController2;
    public PhotonView photonViews;

    public GameObject backCardPrefab;
    public GameObject commonCard;
    public GameObject congratulation;
    public GameObject playerPrefab;
    public GameObject timeCounterStart;
   // public GameObject test;
   // public object[] arrTest = new object[3];

    private Transform startPos;

    public Vector3 playerStartPos;

    //UnityAction<PlayerController> setOptionWinner;

    //public Sprite[] arrCards = new Sprite[52];
    public PlayerController[] arrPlayer;
    //public Transform[] posDefaul = new Transform[6];
    public PositionDefaul[] posDefaul = new PositionDefaul[6];
    public Transform[] commonPos = new Transform[5];
    public GameObject[] cards = new GameObject[52];
    //public GameObject[] cards = new GameObject[24];//use this value for test
    private GameObject[] cardsClone = new GameObject[52];

    //public Dictionary<int, Transform> dicPosDefaul = new Dictionary<int, Transform>(6);
    public Stack<GameObject> stackCheck = new Stack<GameObject>(7);

    private List<Flush> listFlush = new List<Flush>();
    public List<BackCard> listBackCard = new List<BackCard>();
    public List<int> listCardsRemoved = new List<int>();
    public List<int> listSaveIDCardToSync = new List<int>();
    //private List<int[]>listFlush = new List<int[]>();

    public long barTotalMoney = 0;

    public int commonIndex = 0;
    public int indexBigBlind;
    public int amountPlayer;
    public int playerInRoom = 0;
    public int playerPlaying = 0;
    public int amountCardInit = 0;
    public int NoTemplate = 0;
    public int NoCommonPos = 0;
    public int[] arrSaveIDCardToSync;
    public int[] arrCardsRemoved;
    int autoID = 0;

    public int countIndexSave = -1;
    int isFlush = 5;//edit with value 5 when finish
    int conStraightFlush = 5;//edit with value 5 when finish

    public long bigestBlinded;

    public bool isFullFiveCard = false;

    [SerializeField]private bool _isStartGame = false;
    public bool isStartGame
    {
        get => _isStartGame;
        set
        {
            _isStartGame = value;
            //gameController2.isStartGame = value;
        }
    }



    public bool isCheckCard = false;
    public bool isJoinedRoom = false;
    public bool isEndGame = false;
    public bool isFirstDeal = true;
    public bool isShowDown = false;

    private void Awake()
    {

        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        //DontDestroyOnLoad(this.gameObject);
        // cards = GameObject.FindGameObjectsWithTag("Card");//this method not sync when builded (differen index)
        //-> differen object spaw
        //var clone = cards.Clone();
        //cardsClone = clone as GameObject[];
    }
    public override void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += NetworkingClient_EventReceived;
    }

    public enum RaiseEventCode
    {
        SyncGameController = 1
    }
    public void SyncGameControllerJoinLate()
    {
        arrCardsRemoved = listCardsRemoved.ToArray();
        arrSaveIDCardToSync = listSaveIDCardToSync.ToArray();
        object[] datas = new object[]
        {
            photonViews.ViewID,//0
          arrSaveIDCardToSync,//1
          commonIndex,//2
          countIndexSave,//3
          arrCardsRemoved,//4
          playerPlaying,//5
          barTotalMoney
        };
        RaiseEventOptions option = new RaiseEventOptions()
        {
            CachingOption = EventCaching.DoNotCache,
            Receivers = ReceiverGroup.All
        };

        PhotonNetwork.RaiseEvent((byte)RaiseEventCode.SyncGameController, datas, option, SendOptions.SendUnreliable);
    }
    private void NetworkingClient_EventReceived(EventData obj)
    {
        if(obj.Code==(byte)RaiseEventCode.SyncGameController)
        {
            Debug.Log("GameController Received datas");
            object[] datas = obj.CustomData as object[];
            if((int)datas[0]==photonViews.ViewID)
            {
                arrSaveIDCardToSync = datas[1] as int[];
                foreach (var ID in arrSaveIDCardToSync)
                {
                    foreach (var item in cards)
                    {
                        if(ID==item.GetComponent<Card>().ID)
                        {
                            stackCheck.Push(item);
                            break;
                        }
                    }
                }
                
                commonIndex = (int)datas[2];
                countIndexSave = (int)datas[3];
                arrCardsRemoved = (int[])datas[4];
                RemoveCards();
                playerPlaying = (int)datas[5];
                barTotalMoney = (long)datas[6];
            }
        }
    }

    public override void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= NetworkingClient_EventReceived;
    }
    public void Start()
    {
        gameController2 = GameController2.Instance;
        uIManager = UIManager.Instance;
        uIManager.gameController = this;
        photonViews = GetComponent<PhotonView>();
        manageNetwork = ManageNetwork.Instance;
        startPos = backCardPrefab.transform;       
        //ClearConsole();
        //Debug.Log(arrTest.Length);

        //setOptionWinner += ReturnBlindedToWinner;
        //setOptionWinner += PayMoneyWonToWinner;

        //for (int i = 0; i < posDefaul.Length; i++)
        //{
        //    dicPosDefaul.Add(i, posDefaul[i]);
        //}
        
        foreach (var item in cards)
        {           
            item.SetActive(true);
            item.GetComponent<Card>().ID = autoID++;
        }

        //uIManager.pnlGame.SetActive(true);
        //Invoke(nameof(UpdatePlayerPlaying), 5f);   
        UpdatePlayerPlayings();
        UpdatePosDefaul();

        for (int i = 0; i < arrPlayer.Length; i++)
        {
            var item = arrPlayer[i];

            if (item.isBroke)
            {
                //do something;
            }

            item.gameController = this;

            item.cardTemplate1.SetActive(false);
            item.cardTemplate2.SetActive(false);
            item.timeCounter.imageFill.fillAmount = 1;
            //item.isTurn = true;
            item.bigBlindIcon.SetActive(false);
            item.listCard.Clear();
            //item.listCard.RemoveAll(GameObject => GameObject == null);
            item.listCardWin.Clear();
            item.card1 = new GameObject();
            item.card2 = new GameObject();
            System.Array.Clear(item.arrCardWin, 0, item.arrCardWin.Length);

            item.isStraightFlush = false;
            item.isQuad = false;
            item.isBoat = false;
            item.isFlush = false;
            item.isStraight = false;
            item.isTrip = false;
            item.isTwoPair = false;
            item.isOnePair = false;
            item.isHighCard = false;
            item.isWinner = false;
            item.timeCounter.isFirstGround = true;
            //item.isWaiting = false;
            //item.isFold = false;

            item.score = 0;
            item.moneyBlinding = 0;
            item.moneyBlinded = 0;

            Color tempColor = Color.white;
            tempColor.a = 1f;
            item.GetComponent<SpriteRenderer>().color = tempColor;
            item.cardTemplate1.GetComponent<SpriteRenderer>().color = tempColor;
            item.cardTemplate2.GetComponent<SpriteRenderer>().color = tempColor;

        }
    }
    private void Update()
    {
        if (manageNetwork.isJoinedRoom)//waiting for connected
        {

            if (isStartGame && (playerPlaying < 2 || playerInRoom < 2) && !isEndGame)
            {
                //BtnCheckCard(); //can't use because stackCheck = 0 (there is no Card to check)
                //Debug.Log($"arrPlayer Count is {arrPlayer.Length}");
                CheckWinner(arrPlayer);
                isEndGame = true;
                isCheckCard = true;
            }

            CheckStartGame();
          
        }
    }
    private void OnValidate()
    {
        // Debug.Log("OnValidate");

    }

    [PunRPC]
    public void CreateCommonCard(int numberOfCard = 3)//using
    {
        StartCoroutine(DelayCreateCard(numberOfCard, commonCard));
    }
    [PunRPC]
    public void CreateBackCard(int numberOfCard = 1)//using
    {
        StartCoroutine(DelayCreateCard(numberOfCard, backCardPrefab));
    }
    IEnumerator DelayCreateCard(int numberOfCard, GameObject cardInput)//using
    {
        for (int i = 0; i < numberOfCard; i++)
        {
            GameObject tempCard = Instantiate(cardInput, startPos.position, Quaternion.identity) as GameObject;
            //int indexCard = Random.Range((int)0, (int)cards.Length);
            //GameObject tempCard = cards[indexCard];
            //RemoveElement(ref cards, indexCard);
            if (tempCard.GetComponent<BackCard>() != null)
            {
                tempCard.GetComponent<BackCard>().enabled = true;
                listBackCard.Add(tempCard.GetComponent<BackCard>());
            }
            else
            {
                if (tempCard.activeSelf == false)
                    tempCard.SetActive(true);
                //int indexCard = Random.Range((int)0, (int)cards.Length);
                //RPC_SetCommonIndex(indexCard);
                tempCard.layer = cards[commonIndex].layer;
                int noLayer = tempCard.layer;
                switch (noLayer)
                {
                    case 7:
                        tempCard.AddComponent<Spades>();
                        break;
                    case 8:
                        tempCard.AddComponent<Hearts>();
                        break;
                    case 9:
                        tempCard.AddComponent<Clubs>();
                        break;
                    case 10:
                        tempCard.AddComponent<Diamonds>();
                        break;
                }

                tempCard.name = cards[commonIndex].name;           
                tempCard.GetComponent<SpriteRenderer>().sprite = cards[commonIndex].GetComponent<SpriteRenderer>().sprite;
                RemoveElement(ref cards, commonIndex);
                tempCard.GetComponent<SpriteRenderer>().sortingOrder = 7;

                //countIndexSave++;
                //Debug.Log($"countIndexSave is {countIndexSave}");
                //arrSaveIDCardToSync[countIndexSave] = tempCard.GetComponent<Card>().ID;
                Debug.Log("hello");
                stackCheck.Push(tempCard);
            }

            yield return new WaitForSeconds(0.2f);
        }
    }
    public void RemoveElement<T>(ref T[] arr, int index)//using
    //public void RemoveElement(ref Sprite[] arr,int index)
    {
        for (int i = index; i < arr.Length - 1; i++)
        {
            arr[i] = arr[i + 1];
        }
        System.Array.Resize(ref arr, arr.Length - 1);
    }
    public void BtnCheckCard()
    {
        if (photonViews.IsMine && !isCheckCard)
        {
            photonViews.RPC("StartCheckCard", RpcTarget.All, null);
            photonViews.RPC("InactiveTempCard", RpcTarget.All, null);
        }
    }//using
    [PunRPC]
    public void StartCheckCard()
    {
        isCheckCard = true;
        foreach (PlayerController player in arrPlayer)
        {
            CheckCard(player);
            player.timeCounter.imageFill.fillAmount = 0;
        }
        //CheckFlush(listFlush);//check Flush affter finish check all Player 
        CheckWinner(arrPlayer);
    }//using
    public void CheckCard(PlayerController player)
    {
        foreach (var item in stackCheck)
        {
            player.listCard.Add(item);
        }
        CheckStraightFlush(player);
        if (!player.isStraightFlush)
        {
            CheckQuad(player);
            if (!player.isQuad)
            {
                CheckBoatAndTrip(player);
                if (!player.isBoat)
                {
                    if (!player.isFlush)
                    {
                        CheckStraight(player);
                        if (!player.isStraight)
                        {
                            if (!player.isTrip)
                                CheckPairAndHighCard(player);
                        }
                    }

                }
            }
        }

        //CheckBoatAndTrip(player);
        //CheckStraight(player);
        //CheckStraightFlush(player);
    }//using
    public void BtnDeal()//using
    {
        if (photonViews.IsMine)
        {
            if (isFirstDeal)
            {
                RPC_Deal(3, 0.5f);
                RPC_SetNewGround(4f);
                photonViews.RPC("SetIsFirstDeal", RpcTarget.All, false);
            }
            else
            {
                RPC_SetCommonIndex();
                Deal();

                //photonViews.RPC("Deal", RpcTarget.AllBuffered, null);
                if (!isFullFiveCard) RPC_SetNewGround();
            }
        }
    }
    public void RPC_SetIsFullFiveCard(bool status)
    {
        photonViews.RPC("SetIsFullFiveCard", RpcTarget.All, status);
    }//using
    [PunRPC]
    public void SetIsFullFiveCard(bool status)
    {
        isFullFiveCard = status;
    }//using
    [PunRPC]
    public void SetIsFirstDeal(bool status)
    {
        isFirstDeal = status;
    }//using
    [PunRPC]
    public void PushCardToStackCheck(int commonIndex)
    {
        //countIndexSave++; // save ID card
        //Debug.Log($"countIndexSave is {countIndexSave}");
        //for (int i = countIndexSave; i < arrSaveIDCardToSync.Length; i++)
        //{
        //    arrSaveIDCardToSync[i] = cards[commonIndex].GetComponent<Card>().ID;
        //    break;
        //}
        listSaveIDCardToSync.Add(cards[commonIndex].GetComponent<Card>().ID);
        stackCheck.Push(cards[commonIndex]);

        int temp = cards[commonIndex].GetComponent<Card>().ID;
        listCardsRemoved.Add(temp);
        RemoveElement(ref cards, commonIndex);      
    }//using
    
    [PunRPC]
    public void Deal()
    {
        if (!isFullFiveCard)
        {
            //Debug.Log($"Common index is : {commonIndex}");
            cards[commonIndex].SetActive(true);
            cards[commonIndex].transform.position = startPos.position;
            cards[commonIndex].AddComponent<CommonCard>();
            photonViews.RPC("PushCardToStackCheck", RpcTarget.All, commonIndex);
            //stackCheck.Push(cards[commonIndex]);
            //RemoveElement(ref cards, commonIndex);
        }
        else
        {
            //Debug.Log("Now we have 5 card aldrealy!!");
            isShowDown = true;
            Invoke(nameof(BtnCheckCard), 3f);
        }
        
    }//using
    public void RPC_Deal(int times = 1, float delay = 1f)
    {
        StartCoroutine(DelayDeal(times, delay));
    }//using
    IEnumerator DelayDeal(int DealTimes = 1, float delay = 1f)//using
    {
        for (int i = 0; i < DealTimes; i++)
        {

            RPC_SetCommonIndex();
            yield return new WaitForSeconds(delay);
            Deal();
            // photonViews.RPC("Deal", RpcTarget.AllBuffered, null);
        }

    }//using
    public IEnumerator ResetTimeCounter(float timeDelay)
    {
        yield return new WaitForSeconds(timeDelay);
        UpdatePlayerPlayings();
        foreach (var item in arrPlayer)
        {
            if (item.timeCounter.imageFill == null)
            {
                item.timeCounter.imageFill = item.timeCounter.GetComponent<Image>();
                item.timeCounter.imageFill.fillAmount = 1;
            }
            else
            {
                item.timeCounter.imageFill.fillAmount = 1;
            }
        }
        if (!isShowDown)
        {
            if(arrPlayer[indexBigBlind]!=null)
            arrPlayer[indexBigBlind]?.timeCounter.gameObject.SetActive(true);
            else
            {
                Debug.Log($"arrPlayer[indexBigBlind] is null with indexBigBlind {indexBigBlind}");
            }
        }

    }//using
    [PunRPC]
    public void PlayAgain()
    {
        //arrPlayer = FindObjectsOfType<PlayerController>();
        photonViews.RPC("SetIsStartGame", RpcTarget.All, false);
        UpdatePlayer();//update to Set few variable before loadScene
        foreach (var item in arrPlayer)
        {
            item.isFold = false;
            item.isWaiting = false;
        }

        SceneManager.LoadScene(0);
    }//using
    public void BtnPlayAgain()
    {
        if(photonViews.IsMine)
        photonViews.RPC("PlayAgain", RpcTarget.All, null);
        //photonViews.RPC("PlayAgain", RpcTarget.AllBuffered, null);
    }//using
    public void CheckWinner(PlayerController[] arrPlayer)
    {
        BackCard[] arrCardHide = FindObjectsOfType<BackCard>();
        Card[] arrCardBlur = FindObjectsOfType<Card>();

        Color spriteColor = Color.white;
        spriteColor.a = 0.4f;

        foreach (var item in arrCardHide) item.gameObject.SetActive(false);

        foreach (var item in arrCardBlur) item.gameObject.transform.GetComponent<SpriteRenderer>().color = spriteColor;

        float bestScore = 0;
        List<PlayerController> listWinner = new List<PlayerController>();

        foreach (var player in arrPlayer)
        {
            Debug.Log($"Player {player.ID} have score is : {player.score}");
            if (!player.isFold)
            {
                if (player.score > bestScore)
                {
                    bestScore = player.score;
                    listWinner.Clear();
                    listWinner.Add(player);
                }
                else if (player.score == bestScore)
                {
                    listWinner.Add(player);
                }
                //foreach (var item in player.arrCardWin)
                //{
                //    Debug.Log($"Card {item}");
                //}          
            }
            ReturnBlindedToWinner(player);
        }

        StartCoroutine(RewardWinner());

        foreach (PlayerController winner in listWinner)
        {
            //AddMoneyToWiner(winner);
            //setOptionWinner?.Invoke(winner);
            //ReturnBlindedToWinner(winner);
             
            // devloping
            //RewardWinner(listWinner.ToArray());
           


            //HighLightCardWin(winner);
            //ScaleCardWin(winner, 1.1f);

            //Debug.Log($"Winner is Player {winner.gameObject.name} with Score : {winner.score}" +
            //    $" and Card ({winner.arrCardWin[0]} {winner.arrCardWin[1]} {winner.arrCardWin[2]} " +
            //    $"{winner.arrCardWin[3]} {winner.arrCardWin[4]})");

            ////congratulation.SetActive(true);
            ////congratulation.transform.position = winner.gameObject.transform.position;

            winner.timeCounter.imageFill.fillAmount = 0f;
            //Instantiate(congratulation, winner.gameObject.transform.position, Quaternion.identity);
            if (photonViews.IsMine)
                Invoke(nameof(BtnPlayAgain), 20f);
        }
    }//using
    public void CheckStraightFlush(PlayerController player)
    {
        stackCheck.Push(player.card1);
        stackCheck.Push(player.card2);

        int spades = 0;
        int hearts = 0;
        int clubs = 0;
        int diamonds = 0;
        foreach (var card in stackCheck)
        {
            switch (card?.layer)
            {
                case 7:
                    spades++;
                    break;
                case 8:
                    hearts++;
                    break;
                case 9:
                    clubs++;
                    break;
                case 10:
                    diamonds++;
                    break;
            }
        }

        if (spades >= isFlush || hearts >= isFlush || clubs >= isFlush || diamonds >= isFlush)
        {
            player.listCard.Clear();
            List<GameObject> listTempObj = new List<GameObject>();
            if (spades >= isFlush)
            {
                foreach (var item in stackCheck)
                {
                    if (item.GetComponent<Spades>() != null)
                    {
                        listTempObj.Add(item.gameObject);
                        player.listCard.Add(item);
                    }
                }
            }
            else if (hearts >= isFlush)
            {
                foreach (var item in stackCheck)
                {
                    if (item.GetComponent<Hearts>() != null)
                    {
                        listTempObj.Add(item.gameObject);
                        player.listCard.Add(item);
                    }
                }
            }
            else if (clubs >= isFlush)
            {
                foreach (var item in stackCheck)
                {
                    if (item.GetComponent<Clubs>() != null)
                    {
                        listTempObj.Add(item.gameObject);
                        player.listCard.Add(item);
                    }
                }
            }
            else if (diamonds >= isFlush)
            {
                foreach (var item in stackCheck)
                {
                    if (item.GetComponent<Diamonds>() != null)
                    {
                        listTempObj.Add(item.gameObject);
                        player.listCard.Add(item);
                    }
                }
            }
            CheckStraightOfFlush(listTempObj, player);
        }
        stackCheck.Pop();
        stackCheck.Pop();
    }//using
    public void CheckStraightOfFlush(List<GameObject> arr, PlayerController player)
    {
        List<int> tempList = new List<int>();
        bool isCheckAce = false;
        Debug.Log(5);
        foreach (var item in arr)
        {
            int vlue = int.Parse(item.name);
            tempList.Add(vlue);
        }
        tempList.Sort();
        tempList.Reverse();
        Debug.Log(6);
        List<int> container = new List<int>(5);
        container.Add(tempList[0]);
        bool isStraightFlush = false;

    pointGoto:
        for (int i = 1; i < tempList.Count; i++)//check Straight of Flush
        {
            if (tempList[i] + 1 == tempList[i - 1])
            {
                container.Add(tempList[i]);
                if (container.Count >= conStraightFlush)
                {
                    int[] container1 = new int[5];
                    container.CopyTo(container1);
                    Debug.Log($"player {player.gameObject.name} have straight flush {container1[0]} {container1[1]}" +
                        $" {container1[2]} {container1[3]} {container1[4]}");

                    for (int j = 0; j < container1.Length; j++)
                    {
                        player.arrCardWin[j] = container1[j];
                    }

                    int scoreFlush = 0;
                    foreach (var item in container)
                    {
                        scoreFlush += item;
                    }
                    player.score = 1000 + scoreFlush;
                    player.isStraightFlush = true;
                    isStraightFlush = true;
                    break;
                }
            }
            else
            {
                container.Clear();
                container.Add(tempList[i]);
            }
        }
        if (!isStraightFlush)
        {
            if (!isCheckAce)//change Ace value and check again
            {
                container.Clear();
                for (int j = 0; j < tempList.Count; j++)
                {
                    if (tempList[j] == 14)
                    {
                        tempList[j] = 1;
                        tempList.Sort();
                        tempList.Reverse();
                        container.Add(tempList[0]);
                        isCheckAce = true;
                        goto pointGoto;
                    }
                }
            }
            else
            {
                for (int j = 0; j < tempList.Count; j++)
                {
                    if (tempList[j] == 1)
                    {
                        tempList[j] = 14;
                        tempList.Sort();
                        tempList.Reverse();
                    }
                }
            }
            //use for check Flush
            int[] arrCopy = new int[7];
            tempList.CopyTo(arrCopy);
            player.gameObject.AddComponent<Flush>();
            player.GetComponent<Flush>().arrVlue = arrCopy;
            listFlush.Add(player.GetComponent<Flush>());
            //Debug.Log($"list Flush count is {listFlush.Count}");
            Debug.Log($"player {player.gameObject.name} have flush :{arrCopy[0]} {arrCopy[1]} {arrCopy[2]}" +
                $" {arrCopy[3]} {arrCopy[4]}");

            for (int j = 0; j < arrCopy.Length - 2; j++)
            {
                player.arrCardWin[j] = arrCopy[j];

            }
            Debug.Log(player.arrCardWin[0]);
            //comepare Flush by the way 2 ( check value)

            float scoreFlush = 0f;
            for (int i = 0; i < arrCopy.Length; i++)
            {
                switch (i)
                {
                    case 0:
                        scoreFlush += arrCopy[i];
                        break;
                    case 1:
                        scoreFlush += arrCopy[i] / 20f;
                        break;
                    case 2:
                        scoreFlush += arrCopy[i] / 200f;
                        break;
                    case 3:
                        scoreFlush += arrCopy[i] / 2000f;
                        break;
                    case 4:
                        scoreFlush += arrCopy[i] / 20000f;
                        break;
                }
            }

            if (player.score < 700)
                player.score = 700 + scoreFlush;
            player.isFlush = true;
        }
    }//using
    public void CheckQuad(PlayerController player)
    {
        stackCheck.Push(player.card1);
        stackCheck.Push(player.card2);

        List<int> listTemp = new List<int>();
        List<int> containerQuad = new List<int>();
        foreach (var item in stackCheck)
        {
            int vlue = item.gameObject.GetComponent<Card>().value;
            listTemp.Add(vlue);
        }
        listTemp.Sort();
        listTemp.Reverse();

        foreach (var item in listTemp)
        {
            foreach (var item1 in listTemp)
            {
                if (item == item1) containerQuad.Add(item1);
            }
            if (containerQuad.Count > 3)//edit = 3 when finish test
            {
                foreach (var item1 in listTemp)
                {
                    if (item1 == containerQuad[0]) continue;
                    else
                    {
                        containerQuad.Add(item1);
                        break;
                    }
                }

                Debug.Log($"player {player.ID} have Quad {containerQuad[0]} {containerQuad[1]} " +
                    $"{containerQuad[2]} {containerQuad[3]} {containerQuad[4]}");

                for (int j = 0; j < 5; j++)//add card win to arr to check highlight
                {
                    player.arrCardWin[j] = containerQuad[j];
                }

                int scoreQuad = 0;
                foreach (var item2 in containerQuad)
                {
                    scoreQuad += item2;
                }
                player.isQuad = true;
                player.score = 900 + scoreQuad;
                break;
            }
            else containerQuad.Clear();
        }
        stackCheck.Pop();
        stackCheck.Pop();
    }//using
    public void CheckBoatAndTrip(PlayerController player)
    {
        stackCheck.Push(player.card1);
        stackCheck.Push(player.card2);

        //stackCheck.Push(player.gameObject.transform.GetChild(4).gameObject);
        //stackCheck.Push(player.gameObject.transform.GetChild(5).gameObject);
        List<int> listTemp = new List<int>();
        List<int> containerBoat = new List<int>();
        List<int> containerBoat2 = new List<int>();
        int[] mainBoat = new int[5];
        int[] arrTrip = new int[5];
        foreach (var item in stackCheck)
        {
            int vlue = item.gameObject.GetComponent<Card>().value;
            listTemp.Add(vlue);
        }
        listTemp.Sort();
        listTemp.Reverse();//arrange list type  7 6 5 4 3 2 1
        //check Trips      
        foreach (var item in listTemp)
        {
            foreach (var item1 in listTemp)
            {
                if (item == item1)
                {
                    containerBoat.Add(item1);
                }
            }
            if (containerBoat.Count == 3)
            {
                listTemp.Remove(item);
                listTemp.Remove(item);
                listTemp.Remove(item);
                //Check Boat
                for (int i = 0; i < listTemp.Count; i++)
                {
                    foreach (var item3 in listTemp)
                    {
                        if (listTemp[i] == item3)
                        {
                            containerBoat2.Add(item3);
                        }
                    }
                    float scoreBoat = 0f;
                    if (containerBoat2.Count == 3)
                    {
                        Debug.Log($"{containerBoat[0]} {containerBoat2[0]} ");
                        if (containerBoat[0] > containerBoat2[0])
                        {
                            containerBoat.Add(containerBoat2[0]);
                            containerBoat.Add(containerBoat2[0]);
                            containerBoat.CopyTo(mainBoat);
                        }
                        else
                        {
                            //Debug.Log($"4");
                            containerBoat2.Add(containerBoat[0]);
                            containerBoat2.Add(containerBoat[0]);
                            containerBoat2.CopyTo(mainBoat);
                        }
                        // Debug.Log($"5");
                        Debug.Log($"player {player.ID} have Boat(6) {mainBoat[0]} " +
                                $"{mainBoat[1]} {mainBoat[2]} {mainBoat[3]} {mainBoat[4]}");

                        for (int j = 0; j < 5; j++)//add card win to arr to check highlight
                        {
                            player.arrCardWin[j] = mainBoat[j];
                        }

                        for (int j = 0; j < mainBoat.Length; j++)
                        {
                            //Debug.Log($"7");
                            if (j < 3)
                            {
                                scoreBoat += (float)mainBoat[j];
                            }
                            else
                            {
                                //Debug.Log($"8");
                                scoreBoat += ((float)(mainBoat[j]) / 100f);
                            }
                        }
                        player.isBoat = true;
                        player.score = (800f + scoreBoat);
                        //Debug.Log($"9");
                        goto pointgoto;
                    }

                    else if (containerBoat2.Count == 2)
                    {
                        //Debug.Log($"10");
                        containerBoat.Add(listTemp[i]);
                        containerBoat.Add(listTemp[i]);
                        containerBoat.CopyTo(mainBoat);
                        Debug.Log($"player {player.ID} have Boat(5) {mainBoat[0]} " +
                            $"{mainBoat[1]} {mainBoat[2]} {mainBoat[3]} {mainBoat[4]}");

                        for (int j = 0; j < 5; j++)//add card win to arr to check highlight
                        {
                            player.arrCardWin[j] = mainBoat[j];
                        }

                        for (int k = 0; k < mainBoat.Length; k++)
                        {
                            //Debug.Log($"11");
                            if (k < 3)
                            {
                                scoreBoat += (float)mainBoat[k];
                            }
                            else
                            {
                                scoreBoat += (float)(mainBoat[k] / 100f);
                            }
                        }
                        player.isBoat = true;
                        player.score = 800 + scoreBoat;
                        goto pointgoto;
                    }

                    else
                    {
                        //Debug.Log($"12");
                        containerBoat2.Clear();
                    }
                    // Debug.Log($"13");
                }
                containerBoat.Add(listTemp[0]);
                containerBoat.Add(listTemp[1]);
                Debug.Log($"player {player.ID} have Trips {containerBoat[0]} {containerBoat[1]} {containerBoat[2]} " +
                    $"{containerBoat[3]} {containerBoat[4]}");
                containerBoat.CopyTo(arrTrip);

                for (int j = 0; j < 5; j++)//add card win to arr to check highlight
                {
                    player.arrCardWin[j] = arrTrip[j];
                }

                float scoreTrip = 0f;
                for (int i = 0; i < arrTrip.Length; i++)
                {
                    switch (i)
                    {
                        case 0:
                            scoreTrip += arrTrip[i];
                            break;
                        case 1:
                            scoreTrip += arrTrip[i];
                            break;
                        case 2:
                            scoreTrip += arrTrip[i];
                            break;
                        case 3:
                            scoreTrip += arrTrip[i] / 100f;
                            break;
                        case 4:
                            scoreTrip += arrTrip[i] / 1000f;
                            break;
                    }
                }
                player.isTrip = true;
                if (player.score < 500)
                    player.score = 500 + scoreTrip;
                break;
            }
            else
            {
                containerBoat.Clear();
            }
        }
    pointgoto:

        stackCheck.Pop();
        stackCheck.Pop();
    }//using
    public void CheckPairAndHighCard(PlayerController player)
    {
        stackCheck.Push(player.card1);
        stackCheck.Push(player.card2);

        List<int> listTemp = new List<int>();
        List<int> containerPair = new List<int>(5);
        foreach (var item in stackCheck)
        {
            int vlue = item.gameObject.GetComponent<Card>().value;
            listTemp.Add(vlue);
        }
        listTemp.Sort();
        listTemp.Reverse();

        int countCheck = 2;
    pointgoto:
        if (containerPair.Count <= 4)
        {
            foreach (var item in listTemp)
            {
                for (int i = 0; i < listTemp.Count; i++)
                {
                    if (item == listTemp[i])
                    {
                        containerPair.Add(item);
                        if (containerPair.Count == 5)
                            goto pointgoto;
                        if (containerPair.Count == countCheck)
                        {
                            listTemp.Remove(item);
                            listTemp.Remove(item);
                            countCheck = 4;
                            goto pointgoto;
                        }
                    }
                }
                if (containerPair.Count < 2)
                    containerPair.Clear();
                else if (containerPair.Count == 3)
                {
                    containerPair.RemoveAt(2);
                }
            }
            if (containerPair.Count < 2)//Check High Card
            {
                Debug.Log($"player {player.ID} have High Card {listTemp[0]} " +
                    $"{listTemp[1]} {listTemp[2]} {listTemp[3]} {listTemp[4]}");

                for (int j = 0; j < 5; j++)//add card win to arr to check highlight
                {
                    player.arrCardWin[j] = listTemp[j];
                }

                float scoreHighCard = 0f;
                for (int i = 0; i < listTemp.Count; i++)
                {
                    switch (i)
                    {
                        case 0:
                            scoreHighCard += listTemp[i];
                            break;
                        case 1:
                            scoreHighCard += listTemp[i] / 20f;
                            break;
                        case 2:
                            scoreHighCard += listTemp[i] / 200f;
                            break;
                        case 3:
                            scoreHighCard += listTemp[i] / 2000f;
                            break;
                        case 4:
                            scoreHighCard += listTemp[i] / 20000f;
                            break;
                    }
                }
                player.isHighCard = true;
                player.score = 200 + scoreHighCard;
            }
            else
            {
                containerPair.Add(listTemp[0]);
                containerPair.Add(listTemp[1]);
                containerPair.Add(listTemp[2]);
                Debug.Log($"player {player.ID} have one pair {containerPair[0]} " +
                                $"{containerPair[1]} {containerPair[2]} {containerPair[3]} {containerPair[4]}");

                for (int j = 0; j < 5; j++)//add card win to arr to check highlight
                {
                    player.arrCardWin[j] = containerPair[j];
                }

                float scorePair = 0f;
                for (int i = 0; i < containerPair.Count; i++)
                {
                    switch (i)
                    {
                        case 0:
                            scorePair += containerPair[i];
                            break;
                        case 1:
                            scorePair += containerPair[i];
                            break;
                        case 2:
                            scorePair += containerPair[i] / 20f;
                            break;
                        case 3:
                            scorePair += containerPair[i] / 200f;
                            break;
                        case 4:
                            scorePair += containerPair[i] / 2000f;
                            break;
                    }
                }
                player.isOnePair = true;
                player.score = 300 + scorePair;
            }
        }
        else
        {
            Debug.Log($"player {player.ID} have two pair {containerPair[0]} " +
                $"{containerPair[1]} {containerPair[2]} {containerPair[3]} {containerPair[4]}");

            for (int j = 0; j < 5; j++)//add card win to arr to check highlight
            {
                player.arrCardWin[j] = containerPair[j];
            }

            float scorePair = 0f;
            for (int i = 0; i < containerPair.Count; i++)
            {
                if (i < 2)
                    scorePair += containerPair[i];
                else if (1 < i && i < 4)
                    scorePair += containerPair[i] / 100f;
                else
                    scorePair += containerPair[i] / 10000f;
            }
            player.isTwoPair = true;
            player.score = 400 + scorePair;
        }

        stackCheck.Pop();
        stackCheck.Pop();
    }//using
    public void CheckStraight(PlayerController player)
    {
        stackCheck.Push(player.card1);
        stackCheck.Push(player.card2);

        List<int> listTemp = new List<int>();
        List<int> containerStraight = new List<int>();
        bool isCheckAce = false;
        foreach (var item in stackCheck)
        {
            int vlue = item.gameObject.GetComponent<Card>().value;
            listTemp.Add(vlue);
        }
        listTemp.Sort();
        listTemp.Reverse();

    pointGoto:

        containerStraight.Add(listTemp[0]);
        for (int i = 1; i < listTemp.Count; i++)
        {
            if (listTemp[i] == listTemp[i - 1])
            {
                continue;
            }
            else if (listTemp[i] == listTemp[i - 1] - 1)
            {
                containerStraight.Add(listTemp[i]);
                if (containerStraight.Count > 4)
                {
                    Debug.Log($"player {player.gameObject.name} have straight " +
                    $"{containerStraight[0]} {containerStraight[1]} " +
                    $"{containerStraight[2]} {containerStraight[3]} {containerStraight[4]} ");

                    for (int j = 0; j < 5; j++)//add card win to arr to check highlight
                    {
                        player.arrCardWin[j] = containerStraight[j];
                    }

                    int scoreStraight = 0;
                    foreach (var item in containerStraight)
                    {
                        scoreStraight += item;
                    }
                    player.isStraight = true;
                    player.score = 600 + scoreStraight;
                    break;
                }
            }
            else
            {
                containerStraight.Clear();
                containerStraight.Add(listTemp[i]);
            }
        }
        if (!player.isStraight && !isCheckAce)//change Ace value and check again
        {
            containerStraight.Clear();
            for (int j = 0; j < listTemp.Count; j++)
            {
                if (listTemp[j] == 14)
                {
                    listTemp[j] = 1;
                    listTemp.Sort();
                    listTemp.Reverse();
                    isCheckAce = true;
                    goto pointGoto;
                }
            }
        }

        stackCheck.Pop();
        stackCheck.Pop();
    }//using
    public void CheckFlush(List<Flush> listFlushInput)
    {
        if (listFlush.Count > 1)
        {
            CompareFlush(listFlushInput);
        }
    }//check Flush by another way
    public void CompareFlush(List<Flush> listFlushInput)
    {
        Debug.Log($"There are {listFlushInput.Count} player have flush");
        //print value Flush in each player
        //for (int i = 0; i < listFlushInput.Count; i++)
        //{
        //    Debug.Log($"Player {listFlushInput[i].GetComponent<PlayerController>().ID} have flush: {listFlushInput[i].arrVlue[0]} {listFlushInput[i].arrVlue[1]}" +
        //        $" {listFlushInput[i].arrVlue[2]} {listFlushInput[i].arrVlue[3]}" +
        //        $" {listFlushInput[i].arrVlue[4]} {listFlushInput[i].arrVlue[5]}" +
        //        $" {listFlushInput[i].arrVlue[6]}  ");
        //}   
        for (int i = 0; i < listFlushInput.Count - 1; i++)
        {
            int a = 0;
            int j = i + 1;
        point:
            //Debug.Log("j is " + j);           
            if (listFlushInput[i].arrVlue[a] > listFlushInput[j].arrVlue[a])
            {
                int name = listFlushInput[i].GetComponent<PlayerController>().ID;
                listFlushInput[i].GetComponent<PlayerController>().score++;
                int name2 = listFlushInput[j].GetComponent<PlayerController>().ID;
                Debug.Log($"Flush player {name} bigger than player {name2}");
                if (j < listFlushInput.Count - 1)
                {
                    j++;
                    goto point;
                }
            }
            else if (listFlushInput[i].arrVlue[a] == listFlushInput[j].arrVlue[a])
            {
                int name = listFlushInput[i].GetComponent<PlayerController>().ID;
                int name2 = listFlushInput[j].GetComponent<PlayerController>().ID;
                if (a < listFlushInput[0].arrVlue.Length - 1)
                {
                    a++;
                    goto point;
                }
                else
                {
                    Debug.Log($"Flush player {name} equal player {name2}");
                    if (j < listFlushInput.Count - 1)
                    {
                        j++;
                        a = 0;
                        goto point;
                    }
                }
            }
            else
            {
                int name = listFlushInput[i].GetComponent<PlayerController>().ID;
                int name2 = listFlushInput[j].GetComponent<PlayerController>().ID;
                listFlushInput[j].GetComponent<PlayerController>().score++;
                Debug.Log($"Flush player {name} smaller than player {name2}");
                if (j < listFlushInput.Count - 1)
                {
                    j++;
                    goto point;
                }
            }
        }
    }//check Flush by another way
    public void HighLightCardWin(PlayerController player)
    {
        Color spriteColor = Color.white;
        spriteColor.a = 1f;

        for (int i = 0; i < player.arrCardWin.Length; i++)
        {
            for (int j = 0; j < player.listCard.Count; j++)
            {
                //int temp= int.Parse(player.listCard[j].name);
                int temp = (player.listCard[j].GetComponent<Card>().value);
                //bool boolTemp = int.TryParse(player.listCard[j].name, out temp);        
                if (temp == player.arrCardWin[i])
                {
                    player.listCardWin.Add(player.listCard[j]);
                    player.listCard[j].GetComponent<SpriteRenderer>().color = spriteColor;
                    player.listCard.Remove(player.listCard[j]);
                    break;
                }
            }
        }
        if (player.listCardWin.Count < 5)
        {
            if(player.card1!=null)
            {
                player.card1.GetComponent<SpriteRenderer>().color = spriteColor;
                player.card2.GetComponent<SpriteRenderer>().color = spriteColor;
            }
           
        }
    }//using
    public void ScaleCardWin(PlayerController player, float value = 1.5f)
    {
        float scale = value;
        Vector3 vlueScale = new Vector3(scale, scale, scale);
        foreach (GameObject item in player.listCardWin)
        {
            item.transform.localScale = vlueScale;
        }
        player.gameObject.GetComponent<SpriteRenderer>().size = new Vector2(1.5f, 1.5f);
    }//using
    public static void ClearConsole()
    {
        var assembly = Assembly.GetAssembly(typeof(SceneView));
        var type = assembly.GetType("UnityEditor.LogEntries");
        var method = type.GetMethod("Clear");
        method.Invoke(new object(), null);
    }//using

    IEnumerator RunTimeCounter(float timeDelay = 3f)//using
    {
        yield return new WaitForSeconds(timeDelay);
        SetSmallBigBlind(arrPlayer);
    }
    [PunRPC]
    public void SetCommonIndex(int index)//using
    {
        commonIndex = index;       
    }
    public void RPC_SetCommonIndex()//using
    {
        int index = Random.Range((int)0, (int)cards.Length);
        photonViews.RPC("SetCommonIndex", RpcTarget.All, index);
    }
    [PunRPC]
    public void SetIsStartGame(bool status)
    {
        isStartGame = status;
        //gameController2.isStartGame = status;
    }   //using
    public void BtnPlayGame()//using
    {
        if (photonViews.IsMine)
        {
            //photonViews.RPC("SetIsStartGame", RpcTarget.AllBuffered, true);
            photonViews.RPC("SetIsStartGame", RpcTarget.All, true);       
            photonViews.RPC("UpdatePlayer", RpcTarget.All, null);
            photonViews.RPC("CheckMoneyPlayer", RpcTarget.All, null);
            photonViews.RPC("InitBlind", RpcTarget.All,(long)10000);
            StartCoroutine(nameof(RunTimeCounter), 3f);
            //playerInRoom *= 2;
            amountCardInit = playerPlaying * 2;
            photonViews.RPC("CreateBackCard", RpcTarget.All, amountCardInit);
            //Debug.Log(playerPlaying);
        }
    }//using
    [PunRPC]
    public void UpdatePlayer()//using
    {
        arrPlayer = FindObjectsOfType<PlayerController>();
        for (int i = 0; i < arrPlayer.Length; i++)//sort arrPlayer 0->9
        {
            for (int j = i; j < arrPlayer.Length; j++)
            {
                if (arrPlayer[i].ID > arrPlayer[j].ID)
                {
                    var temp = arrPlayer[i];
                    arrPlayer[i] = arrPlayer[j];
                    arrPlayer[j] = temp;
                }
            }
        }
        playerInRoom = arrPlayer.Length;
       // playerInRoom = (int)PhotonNetwork.CurrentRoom.PlayerCount;
    }
    [PunRPC]
    public void UpdatePlayerPlayings()
    {
        UpdatePlayer();
        for (int i = 0; i < arrPlayer.Length; i++)
        {
            if ((arrPlayer[i].isFold) || (arrPlayer[i].isWaiting))
            {
                //Debug.Log($"index i  =  {i} and Player {arrPlayer[i].ID} isFold is {arrPlayer[i].isFold}");
                RemoveElement(ref arrPlayer, i);
                i--;
            }
        }       
        playerPlaying = arrPlayer.Length;
    }
    public void BtnReady()//using 
    {

        SpawPlayer();
        if (!isStartGame)
        {
            RPC_SetCommonIndex();
            photonViews.RPC("UpdatePlayer", RpcTarget.All, null);
            photonViews.RPC("UpdatePlayerPlayings", RpcTarget.All, null);
        }
        else
        {
            Debug.Log("game is started please wait");
            //UpdatePlayerPlayings();
        }

    }
    [PunRPC]
    public void InactivePos(int index) //using
    {
        //dicPosDefaul[index].gameObject.SetActive(false);
        //posDefaul[index].gameObject.SetActive(false);
        posDefaul[index].isEmpty = false;
    }
    [PunRPC]
    public void SpawPlayer()
    {
        int safeCount = 0;
        //for (int i = Random.Range((int)0, (int)dicPosDefaul.Count); i < dicPosDefaul.Count; i++)
        UpdatePosDefaul();
        for (int i = Random.Range((int)0, (int)posDefaul.Length); i < posDefaul.Length; i++)
        {
            if (posDefaul[i].isEmpty)
            {
                GameObject tempObj = PhotonNetwork.Instantiate(i.ToString(),
                    posDefaul[i].transform.position, Quaternion.identity) as GameObject;
                photonViews.RPC("InactivePos", RpcTarget.AllBuffered, i);
                break;
            }
            else
            {
                safeCount++;
                if (i == (posDefaul.Length - 1))
                    i = Random.Range((int)0, (int)posDefaul.Length);
            }
            if (safeCount > 500)
            {
                Debug.Log("All position is not empty");
                break;
            }
        }
    } //using
    public void SetSmallBigBlind(PlayerController[] arrPlayer)//using
    {
        int index = Random.Range(0, arrPlayer.Length);
        photonViews.RPC("RPC_IndexBigBlind", RpcTarget.All, index);
    }
    [PunRPC]
    public void RPC_IndexBigBlind(int index)//using
    {
        indexBigBlind = index;
        arrPlayer[indexBigBlind].timeCounter.gameObject.SetActive(true);
        arrPlayer[indexBigBlind].bigBlindIcon.SetActive(true);
    }
    [PunRPC]
    public void InactiveTempCard()//using
    {
        TemplateCard[] temp = FindObjectsOfType<TemplateCard>();
        foreach (var item in temp)
        {
            item.gameObject.SetActive(false);
        }

    }
    IEnumerator DelayUpdate()//not used
    {
        bool isUpdate = true;
        while (isUpdate)
        {
            playerInRoom = (int)PhotonNetwork.CurrentRoom.PlayerCount;
            if (!timeCounterStart.activeSelf)
            {
                if (playerInRoom >= 2 && !isStartGame)
                {
                    photonViews.RPC("RPC_ActiveTimeCounter", RpcTarget.All, null);
                }
                else
                {
                    photonViews.RPC("RPC_InactiveTimeCounter", RpcTarget.All, null);
                }
                if (isStartGame) isUpdate = false;
            }
            yield return new WaitForSeconds(0.5f);
        }
    }//not used
    [PunRPC]
    public void UpdatePlayerPlayingWithDelay()//not used
    {
        StartCoroutine(DelayUpdate());
    }//not used
    [PunRPC]
    public void ActiveTimeCounterStart()
    {
        timeCounterStart.SetActive(true);
    }//using
    [PunRPC]
    public void InactiveTimeCounterStart()
    {
        timeCounterStart.SetActive(false);
    }//using
    public void Btn_Quit()
    {
        Application.Quit();
        Debug.Log("quit");
    }//using
    public void CheckStartGame()
    {
        //playerInRoom = (int)PhotonNetwork.CurrentRoom.PlayerCount;
        if (playerInRoom >= 2)
        {
            if (!timeCounterStart.activeSelf && !isStartGame) ActiveTimeCounterStart();
        }
        else if (timeCounterStart.activeSelf)
        {
            InactiveTimeCounterStart();
        }
    }//using
    public void UpdatePosDefaul()
    {
        posDefaul = FindObjectsOfType<PositionDefaul>();
        foreach (var item in posDefaul)
        {
            for (int i = 0; i < posDefaul.Length; i++)
            {
                if (posDefaul[i].ID != i)
                {
                    var temp = posDefaul[posDefaul[i].ID];
                    posDefaul[posDefaul[i].ID] = posDefaul[i];
                    posDefaul[i] = temp;
                }
            }
        }

        for (int i = 0; i < posDefaul.Length; i++)
        {
            foreach (var player in arrPlayer)
            {
                if (player.ID == i)
                {
                    //posDefaul[i].gameObject.SetActive(false);
                    posDefaul[i].isEmpty = false;
                }
            }
        }

    }//using
    public void BtnTest()//Button test
    {
        Debug.Log($"StackCheck Count is {stackCheck.Count}");     
    }
    
    [PunRPC]
    public void Pause()
    {           
        if (Time.timeScale == 0.1f)
        {
            Time.timeScale = 1;
            Debug.Log("Resume !!");
        }          
        else
        {
            Time.timeScale = 0.1f;
            Debug.Log("Pause!!!");
        }
            
    }
    public void BtnPause()
    {
        
        photonViews.RPC("Pause", RpcTarget.All, null);
    }
    [PunRPC]
    public void InitBlind(long moneyVlue = 5000)//using
    {
        for (int i = 0; i < arrPlayer.Length; i++)
        {
            var player = arrPlayer[i];
            if (player.money >= moneyVlue)
            {
                player.money -= moneyVlue;
                player.moneyBlinded += moneyVlue;
                barTotalMoney += moneyVlue;
                bigestBlinded = moneyVlue;
            }
            else
            {
                //All in 
            }
        }
    }
    public string FormatVlueToString(long score)
    {
        string str = score.ToString();
        if (str.Length > 6 && str.Length <= 9)
        {
            score = score / 1000;
            Mathf.RoundToInt(score);
            str = score.ToString();
            str = str.Insert(str.Length - 3, ","); //string is immutable
            str = str + " M";
        }
        else if (str.Length > 3 && str.Length <= 6)
        {
            str = str.Insert(str.Length - 3, ",");
        }
        else if (str.Length > 9)
        {
            score = score / 1000000;
            Mathf.RoundToInt(score);
            str = score.ToString();
            str = str.Insert(str.Length - 3, ",");
            str = str + " B";
        }
        return str;
    }//using

    public void LoadScene()
    {
        SceneManager.LoadScene(0);
    }//using
    public bool CheckEqualBlind()//using
    {
        UpdatePlayerPlayings();

        List<PlayerController> temp = new List<PlayerController>();
        foreach (var item in arrPlayer)
        {
            if (item.money > 0)
            {
                temp.Add(item);
            }
        }


        //test
       // var qr = arrPlayer.Where(p => p.money > 0);
       // if(qr!=null)
       // {
       //    //qr.Where(p => p.moneyBlinded < bigestBlinded);
       //     return !qr.Any(p => p.moneyBlinded < bigestBlinded);
       // }
       //else
       // {

       // }
        //test

        if (temp.Count > 1)
        {
            foreach (var item1 in temp)
            {
                if (item1.moneyBlinded < bigestBlinded)
                {
                    return false;
                }
            }
        }
        else
        {
            if (temp.Count>0 && (temp[0].moneyBlinded < bigestBlinded))
            {
                return false;
            }
            else
            {
                //show card
                Debug.Log($"Let Show Down now !!!");
                BtnShowDown();
            }        
        }
        return true;
    }
    public void UpdateBlind()
    {
        foreach (var item in arrPlayer)
        {
            if (item.moneyBlinded > bigestBlinded)
            {
                bigestBlinded = item.moneyBlinded;
            }
        }
    }//using

    public void RefreshTimeCounter(float delay = 0.5f)
    {
        StartCoroutine(ResetTimeCounter(delay));
    }//using
    [PunRPC]
    public void SetNewGround(float delay = 1f)//using
    {
        UpdatePlayerPlayings();
        foreach (var item in arrPlayer)
        {
            item.timeCounter.isFirstGround = true;
            item.moneyBlinding = 0;
        }
        RefreshTimeCounter(delay);
    }
    public void RPC_SetNewGround(float delay =1f)//using
    {
        photonViews.RPC("SetNewGround", RpcTarget.AllBuffered, delay);
    }

    public void BtnShowDown()//using
    {
        //if (photonViews.IsMine) // alway only one player call therefor no need
            photonViews.RPC("RPC_ShowDown", RpcTarget.All, null);     
    }
    [PunRPC]
    public void RPC_ShowDown()//using
    {
        Debug.Log("Show Down!!!");
        isShowDown = true;
        if(photonViews.IsMine)
        {
            Debug.Log("Warning 2 !!!");
            switch (NoCommonPos)
            {
                case 0:
                    RPC_Deal(5);
                    RPC_SetNewGround(7);
                    break;
                case 3:
                    RPC_Deal(2);
                    RPC_SetNewGround(4);                  
                    break;
                case 4:
                    RPC_Deal(1);
                    RPC_SetNewGround(3);                 
                    break;
                case 5:              
                    break;
            }       
        }
        Invoke(nameof(BtnCheckCard), 9);
    }  
    public void ReturnBlindedToWinner(PlayerController winer)
    {
        winer.money += winer.moneyBlinded;
        barTotalMoney -= winer.moneyBlinded;
    }
    public void PayMoneyWonToWinner(PlayerController winner)
    {
        //UpdatePlayerPlayings();
        if (barTotalMoney <= winner.moneyBlinding)
        {
            winner.money += barTotalMoney;
            barTotalMoney = 0;
        }
        else
        {
            winner.money += winner.moneyBlinded;
            barTotalMoney -= winner.moneyBlinded;
        }
    }
    IEnumerator RewardWinner()
    {
        
        var query = arrPlayer.ToList();
        query.Sort((p1, p2) =>
        {
            if (p1.score == p2.score) return 0;
            if (p1.score < p2.score) return 1;
            return -1;
        });

        arrPlayer = query.ToArray();//sort arrPlayer by score 9 -> 0
      

        for (int i = 0; i < arrPlayer.Length; i++)
        {
            long totalWon = 0;           
            for (int j = i+1; j < arrPlayer.Length; j++)
            {
                long moneyWon = 0;
                if (arrPlayer[i].score>arrPlayer[j].score)
                {
                    if(arrPlayer[i].moneyBlinded>arrPlayer[j].moneyBlinded)
                    {
                        moneyWon = arrPlayer[j].moneyBlinded;
                    }
                    else
                    {
                        moneyWon = arrPlayer[i].moneyBlinded;
                    }

                    arrPlayer[i].money += moneyWon;
                    arrPlayer[j].money -= moneyWon;
                    arrPlayer[j].moneyBlinded -= moneyWon;
                    totalWon += moneyWon;                                    
                }
                else //handle when it hapen
                {
                    //how to share money between 2 winer[0] and winer[1]
                }
                //else if(arrPlayer[i].score < arrPlayer[j].score)//never happen
                //{
                //    Debug.Log("Warning!!!");
                //    if (arrPlayer[i].moneyBlinded > arrPlayer[j].moneyBlinded)
                //    {
                //        moneyWon = arrPlayer[j].moneyBlinded;
                //    }
                //    else
                //    {
                //        moneyWon = arrPlayer[i].moneyBlinded;
                //    }
                //    arrPlayer[j].money += moneyWon;
                //    arrPlayer[i].money -= moneyWon;
                //    arrPlayer[i].moneyBlinded -= moneyWon;
                //    totalWon += moneyWon;
                //}
            }

            if(totalWon>0)
            {
                HighLightCardWin(arrPlayer[i]);
                ScaleCardWin(arrPlayer[i], 1.15f);
                var temp = Instantiate(congratulation, arrPlayer[i].gameObject.transform.position, Quaternion.identity) as GameObject;
                Destroy(temp, 5);
                Debug.Log($"player {arrPlayer[i].name} win total {totalWon} $");
            }
          
            yield return new WaitForSeconds(5);
            
            //should be add money total won of player in this line
        }

    }
    [PunRPC]
    public void CheckMoneyPlayer()
    {
        //UpdatePlayerPlayings();
        foreach (var item in arrPlayer)
        {
            if (item.money <= 0)
            {
                item.isBroke = true;
                //item.gameObject.SetActive(false);
                item.money = 200000;
            }
        }
        //alternative method
        //arrPlayer.Where(p => p.money < 0).ToList().ForEach(p => { p.isBroke = true; p.gameObject.SetActive(false); });
    }
    #region SyncData
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(isStartGame);
        }
        else if (stream.IsReading)
        {               
            isStartGame = (bool)stream.ReceiveNext();        
        }
    }
    public void SyncPlayerDatasJoinLate()
    {
        if(photonViews.IsMine)
        {
            foreach (var item in arrPlayer)
            {               
                item.SyncPlayerJoinLate();
            }
            SyncGameControllerJoinLate();
        }    
    }
    #endregion
    public void RemoveCards()//used for Sync datas (remove cards in GameController join late
    {
        for (int i = 0; i < arrCardsRemoved.Length; i++)
        {
            for (int j = 0; j < cards.Length; j++)
            {
                if(arrCardsRemoved[i]==cards[j].GetComponent<Card>().ID)
                {
                    RemoveElement(ref cards, j);
                    break;
                }
            }
        }
    }
}
