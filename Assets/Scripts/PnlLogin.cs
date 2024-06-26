using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Events;

public class PnlLogin : MonoBehaviour
{
    PlayFabManager playFabManager;

    public InputField inputID, inputPassWord, inputEmail;
    public Button btnLogin, btnRegister, btnLoginGuest;
    public Text txtNotification;
    public GameObject loadingVFX;
    public GameObject uIContainer;

    public static UnityEvent<bool> eventSetPnlOnLogin = new UnityEvent<bool>();
    private void Start()
    {               
        playFabManager = PlayFabManager.Instance;
        playFabManager.inputID = inputID;
        playFabManager.inputPassWord = inputPassWord;
        playFabManager.inputEmail = inputEmail;
        btnLogin.onClick.AddListener(() => {
            playFabManager.Login();
            SetStatusBarsOnProcessing(false);
            txtNotification.text = "...LOG IN...";
        });
        btnLoginGuest.onClick.AddListener(() => playFabManager.LoginGuest());
        btnRegister.onClick.AddListener(() => {
            playFabManager.Register();
            SetStatusBarsOnProcessing(false);
            txtNotification.text = "...Processing...";
        });

        eventSetPnlOnLogin.AddListener((status) => {
            SetStatusBarsOnProcessing(status);
        });

        playFabManager.txtNotification = txtNotification;
    }
    public void SetStatusBarsOnProcessing(bool status)
    {       
        uIContainer.SetActive(status);
        loadingVFX.SetActive(!status);
    }
}
