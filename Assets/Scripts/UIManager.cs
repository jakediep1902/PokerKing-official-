using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;


public class UIManager : MonoBehaviourPunCallbacks
{
    public static UIManager Instance;
    public GameController gameController;
    PhotonView PvUI;
    ManageNetwork manageNetwork;

    public GameObject pnlThemCuoc;
    public GameObject pnlGame;

    public Button btnOKBlind;
    public Button btnTheoCuoc; 
    public Button btnXemBai;
    public Button btnBoBai;
    public Button btnThemCuoc;
    public Button btnAllIn;
    public Button btnQuit;

    public Button btnPause;
    public Button btnTest;

    public Text txtIndex;
    public Text txtBarTotalMoney;
    public Text txtSetBlindVlue;
    public Text txtDisPlayIndexBigBlind;

    public Slider sliderVlue;

    public Image imageConnecting;
   
    public float blindVlue = 0;

    private void Awake()
    {       
        if (Instance==null) Instance = this;
        
        else Destroy(this.gameObject);
        
        DontDestroyOnLoad(this.gameObject);
    }
    private void Start()
    {
        gameController = GameController.Instance;
        btnTheoCuoc.gameObject.SetActive(false);

        btnTest.onClick.AddListener(gameController.BtnTest);
        btnPause.onClick.AddListener(gameController.BtnPause);
    }
    private void Update()
    {
        txtIndex.text = gameController.commonIndex.ToString();
        txtBarTotalMoney.text = gameController.FormatVlueToString(gameController.barTotalMoney);
        //txtDisPlayIndexBigBlind.text = gameController.indexBigBlind.ToString();
    }   
}
