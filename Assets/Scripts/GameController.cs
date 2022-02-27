using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Reflection;
using UnityEditor;

public class GameController : MonoBehaviour
{
    public static GameController Instance;

    public GameObject backCard;
    public GameObject commonCard;
    public GameObject congratulation;
        
    private Transform startPos;
   // public Transform playerCard_1;
    //public Transform playerCard_2;
    public Vector3 playerStartPos;

    PlayerController playerController;
    public PlayerController player;

    public List<BackCard> listBackCard = new List<BackCard>();
    public Sprite[] arrCards = new Sprite[52];
    public PlayerController[] listPlayer;
    public Transform[] posDefaul = new Transform[6];
    public Transform[] commonPos = new Transform[5];
    //public GameObject[] cards = new GameObject[24];//use this value for test
    public GameObject[] cards = new GameObject[52];

    //private List<int[]>listFlush = new List<int[]>();
    private List<Flush> listFlush = new List<Flush>();  
    private Stack<GameObject> stackCheck = new Stack<GameObject>(7);  
  
    public int amountPlayer;
    private int amountCardInit;
    public int NoPlayer = 0;
    public int NoTemplate = 0;
    public int NoCommonPos = 0;
    int isFlush = 5;//edit with value 5 when finish
    int conStraightFlush = 5;//edit with value 5 when finish

    public bool isFull = false;
    

    private void Awake()
    {
        if(Instance==null)
        Instance = this;

        cards = GameObject.FindGameObjectsWithTag("Card");     
    }
    void Start()
    {
        playerController = PlayerController.Instance;

        ClearConsole();

        foreach (var item in cards)
        {
            item.SetActive(false);
        }

        amountPlayer = Random.Range((int)2, (int)7);      
        for(int i =0;i<amountPlayer;i++)
        {
            PlayerController tempPlayer = Instantiate(player, playerStartPos, Quaternion.identity) as PlayerController;
            tempPlayer.gameObject.SetActive(true);
            tempPlayer.name = NoPlayer.ToString();
            NoPlayer++;               
            for(int j =i;j<posDefaul.Length;j+=10)
            {
                tempPlayer.transform.position = posDefaul[j].position;
                tempPlayer.posCard1 = tempPlayer.card1.transform.position;
                Destroy(tempPlayer.card1);
                tempPlayer.posCard2 = tempPlayer.card2.transform.position;
                Destroy(tempPlayer.card2);
            }           
        }

        listPlayer = FindObjectsOfType<PlayerController>();
        amountPlayer = listPlayer.Length;
        amountCardInit = amountPlayer * 2;
          
        startPos = backCard.transform;
        CreateCommonCard(3);
        CreateBackCard(amountCardInit);                   
    }  
    public void CreateCommonCard(int numberOfCard = 3)
    {
        StartCoroutine(DelayCreateCard(numberOfCard, commonCard));
    }
    public void CreateBackCard(int numberOfCard= 1)
    {     
        StartCoroutine(DelayCreateCard(numberOfCard,backCard));            
    }   
      IEnumerator DelayCreateCard(int numberOfCard,GameObject cardInput)
    {      
        for (int i = 0; i < numberOfCard; i++)
        {
            GameObject tempCard = Instantiate(cardInput, startPos.position, Quaternion.identity) as GameObject;
            //int indexCard = Random.Range((int)0, (int)cards.Length);
            //GameObject tempCard = cards[indexCard];
            //RemoveElement(ref cards, indexCard);
            if(tempCard.GetComponent<BackCard>()!=null)
            {
                tempCard.GetComponent<BackCard>().enabled = true;
                listBackCard.Add(tempCard.GetComponent<BackCard>());
            }
            else
            {               
                if(tempCard.activeSelf==false)
                tempCard.SetActive(true);
               
                int indexCard = Random.Range((int)0, (int)cards.Length);
                tempCard.layer = cards[indexCard].layer;

                int noLayer = tempCard.layer;
                switch(noLayer)
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

                tempCard.name = cards[indexCard].name;
                tempCard.GetComponent<SpriteRenderer>().sprite = cards[indexCard].GetComponent<SpriteRenderer>().sprite;
                RemoveElement(ref cards, indexCard);          
                stackCheck.Push(tempCard);                           
            }
            tempCard.GetComponent<SpriteRenderer>().sortingOrder = 7;
            yield return new WaitForSeconds(0.2f);
        }   
    }
    public void ChangeSpriteRenderer(GameObject card)
    {
        int indexCard = Random.Range((int)0, arrCards.Length);
        card.GetComponent<SpriteRenderer>().sprite = arrCards[indexCard];
        RemoveElement(ref arrCards, indexCard);
        //Debug.Log(arrCards.Length);                   
    }
    public void RemoveElement<T>(ref T[] arr,int index)
    //public void RemoveElement(ref Sprite[] arr,int index)
    {
        for (int i = index; i < arr.Length - 1; i++)
        {
            arr[i] = arr[i + 1];
        }
        System.Array.Resize(ref arr, arr.Length - 1);
    }       

    public void StartCheckCard()
    {
        foreach (PlayerController player in listPlayer)
        {
            CheckCard(player);
        }
        //CheckFlush(listFlush);//check Flush affter finish check all Player 
        
        CheckWinner(listPlayer);
    }

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
    }
    public void Deal()
    {       
        if (!isFull)
        {
            int indexCard = Random.Range((int)0, (int)cards.Length);
            //Debug.Log($"index:"+cards.Length);
            cards[indexCard].SetActive(true);
            cards[indexCard].transform.position = startPos.position;
            cards[indexCard].AddComponent<CommonCard>();
            stackCheck.Push(cards[indexCard]);           
            RemoveElement(ref cards, indexCard);                                
        }
        else
            Debug.Log("Now we have 5 card aldrealy!!");
    }

    public void PlayAgain()
    {
        SceneManager.LoadScene(0);
    }

    public void CheckWinner(PlayerController[] arrPlayer)
    {
        Card[] arrCardBlur;
        BackCard[] arrCardHide;
        arrCardBlur = FindObjectsOfType<Card>();
        arrCardHide = FindObjectsOfType<BackCard>();
        Color spriteColor = Color.white;
        spriteColor.a = 0.4f;

        foreach (var item in arrCardHide)
        {
            item.gameObject.SetActive(false);
        }
      
        foreach (var item in arrCardBlur)
        {          
            item.gameObject.transform.GetComponent<SpriteRenderer>().color = spriteColor;
        }

        float bestScore = 0;
        List<PlayerController> listWinner = new List<PlayerController>() ;
      
        foreach (var player in arrPlayer)
        {
            Debug.Log($"Player {player.ID} have score is : {player.score}");
            if(player.score>bestScore)
            {
                bestScore = player.score;
                listWinner.Clear();
                listWinner.Add(player);
            }
            else if(player.score == bestScore)
            {
                listWinner.Add(player);                                        
            }
            //foreach (var item in player.arrCardWin)
            //{
            //    Debug.Log($"Card {item}");
            //}          
        }
        foreach (var winner in listWinner)
        {
            HighLightCardWin(winner);
            ScaleCardWin(winner,1.1f);
            Debug.LogError($"Winner is Player {winner.gameObject.name} with Score : {winner.score}" +
                $" and Card ({winner.arrCardWin[0]} {winner.arrCardWin[1]} {winner.arrCardWin[2]} " +
                $"{winner.arrCardWin[3]} {winner.arrCardWin[4]})");
            Instantiate(congratulation, winner.gameObject.transform.position, Quaternion.identity);
        }       
    }
    
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
            switch (card.layer)
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
            CheckStraightOfFlush(listTempObj,player);          
        }
        else
        {
            //Debug.Log($"player {player.gameObject.name} don't have Flush");
        }
        stackCheck.Pop();
        stackCheck.Pop();
        
    }
    public void CheckStraightOfFlush(List<GameObject> arr,PlayerController player)
    {
        List<int> tempList = new List<int>();
        bool isCheckAce = false;
        foreach (var item in arr)
        {
            int vlue = int.Parse(item.name);
            tempList.Add(vlue);
        }
        tempList.Sort();
        tempList.Reverse();       
               
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
                    player.score = 1000+scoreFlush;
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

            for (int j = 0; j < arrCopy.Length-2; j++)
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
                        scoreFlush += arrCopy[i]/20f;
                        break;
                    case 2:
                        scoreFlush += arrCopy[i]/200f;
                        break;
                    case 3:
                        scoreFlush += arrCopy[i] / 2000f;
                        break;
                    case 4:
                        scoreFlush += arrCopy[i] / 20000f;
                        break;
                }
            }

            if (player.score<700)
            player.score = 700 + scoreFlush;
            player.isFlush = true;                 
        }                
    }
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
    }
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
                containerBoat.CopyTo( arrTrip);

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
                if(player.score<500)
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
    }
    
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
                if(containerPair.Count<2)
                containerPair.Clear();
                else if(containerPair.Count==3)
                {
                    containerPair.RemoveAt(2);
                }
            }
            if(containerPair.Count<2)//Check High Card
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
                    switch(i)
                    {
                        case 0:
                            scoreHighCard += listTemp[i];
                            break;
                        case 1:
                            scoreHighCard += listTemp[i]/20f;
                            break;
                        case 2:
                            scoreHighCard += listTemp[i]/200f;
                            break;
                        case 3:
                            scoreHighCard += listTemp[i]/2000f;
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
                else if(1<i&&i<4)
                    scorePair += containerPair[i] / 100f;
                else
                    scorePair += containerPair[i] / 10000f;
            }
            player.isTwoPair = true;
            player.score = 400+ scorePair;
        }

        stackCheck.Pop();
        stackCheck.Pop();
    }
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
            if (listTemp[i]==listTemp[i-1])
            {
                continue;              
            }
            else if(listTemp[i]==listTemp[i-1]-1)
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
    }
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
        //Queue<GameObject> queueTemp = new Queue<GameObject>();
        
        for (int i = 0; i < player.arrCardWin.Length; i++)
        {          
            for (int j = 0; j < player.listCard.Count; j++)
            {              
                    int temp = int.Parse(player.listCard[j].name);
                    if (temp == player.arrCardWin[i])
                    {                      
                        player.listCardWin.Add(player.listCard[j]);
                        Color spriteColor = Color.white;
                        spriteColor.a = 1f;
                        player.listCard[j].GetComponent<SpriteRenderer>().color = spriteColor;
                        player.listCard.Remove(player.listCard[j]);
                        break;
                    }                              
            }          
        }             
    }
    public void ScaleCardWin(PlayerController player,float value = 1.5f)
    {
        float scale = value;
        Vector3 vlueScale = new Vector3(scale,scale,scale);
        foreach (GameObject item in player.listCardWin)
        {
            item.transform.localScale = vlueScale;
        }
        player.gameObject.GetComponent<SpriteRenderer>().size = new Vector2(1.5f, 1.5f);
    }
    public static void ClearConsole()
    {
        var assembly = Assembly.GetAssembly(typeof(SceneView));
        var type = assembly.GetType("UnityEditor.LogEntries");
        var method = type.GetMethod("Clear");
        method.Invoke(new object(), null);
    }



}
