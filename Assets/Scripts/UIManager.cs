using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;


public class UIManager : MonoBehaviourPunCallbacks
{
    public static UIManager Instance;
    GameController gameController;
    PhotonView PvUI;

    public GameObject pnlThemCuoc;
    public GameObject pnlGame;

    public Button btnOKBlind;
    public Button btnTheoCuoc;
    public Slider sliderVlue;
    
    public Text txtSetBlindVlue;
    public float blindVlue = 0;

    private void Awake()
    {       
        if (Instance==null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
       // gameController = GameController.Instance;
    }
    private void Start()
    {
        gameController = GameController.Instance;
        btnTheoCuoc.gameObject.SetActive(false);
        btnOKBlind.onClick.AddListener(() => BtnOKBlind());
        btnTheoCuoc.onClick.AddListener(() => BtnTheoCuoc());
    }
    public void BtnOKBlind()
    {
        //Debug.Log("BtnOKBlind");
        pnlThemCuoc.SetActive(false);
        pnlGame.SetActive(false);
    }
    public void BtnTheoCuoc()
    {
        //do something
        gameController.pnlGame.SetActive(false);
        
    }    
}
