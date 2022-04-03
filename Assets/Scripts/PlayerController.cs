using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.Events;
using System.Globalization;

public class PlayerController : MonoBehaviourPunCallbacks
{
    public GameController gameController;

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
    public int[] arrCardWin = new int[5];

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

    private void Awake()
    {
        gameController = GameController.Instance;
        DontDestroyOnLoad(this.gameObject);
        uIManager = FindObjectOfType<UIManager>();
    }
    void Start()
    {
        if (gameController.isStartGame)
        {
            isWaiting = true;
        }
        gameController.UpdatePlayer();
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

        uIManager.pnlGame.SetActive(false);
        uIManager.btnOKBlind.onClick.AddListener(() => BtnOkBlind());
        uIManager.btnTheoCuoc.onClick.AddListener(() => BtnTheoCuoc());
        uIManager.btnXemBai.onClick.AddListener(() => BtnXemBai());
        uIManager.btnBoBai.onClick.AddListener(() => BtnBoBai());
        uIManager.btnThemCuoc.onClick.AddListener(() => BtnThemCuoc());



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


    void OnDisable()
    {
        gameController.UpdatePlayer();
        if (!PvPlayer.IsMine)
        {
            timeCounter.imageFill.fillAmount = 0;
            card1?.SetActive(false);
            card2?.SetActive(false);

        }
        //Debug.Log("Left Room 2");

        //if (card1 != null && card2 != null)
        //{
        //    card1.SetActive(false);
        //    card2.SetActive(false);
        //}
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
                //Debug.Log(indexCard);
                indexCard = Random.Range((int)0, (int)gameController.cards.Length);
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
        RefreshListcard();
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
        gameController.RemoveElement(ref gameController.cards, index);
    }
    [PunRPC]
    public void RPC_SetCard2(int index)
    {
        RefreshListcard();
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
    [PunRPC]
    public void RefreshListcard()
    {
        for (int j = 0; j < listCard.Count; j++)
        {
            if (listCard[j] == null)
                listCard.RemoveAt(j);
        }
    }
}
