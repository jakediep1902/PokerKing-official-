using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PnlLogin : MonoBehaviour
{
    PlayFabManager playFabManager;

    public InputField inputID, inputPassWord, inputEmail;
    public Button btnLogin, btnRegister, btnLoginGuest;
    public Text txtNotification;
    public GameObject loadingVFX;
    public GameObject uIContainer;
    private void Start()
    {               
        playFabManager = PlayFabManager.Instance;
        playFabManager.inputID = inputID;
        playFabManager.inputPassWord = inputPassWord;
        playFabManager.inputEmail = inputEmail;
        btnLogin.onClick.AddListener(() => {
            playFabManager.Login();
            uIContainer.SetActive(false);
            loadingVFX.SetActive(true);
            txtNotification.text = "...LOG IN...";
        });
        btnLoginGuest.onClick.AddListener(() => playFabManager.LoginGuest());
        btnRegister.onClick.AddListener(() => {
            playFabManager.Register();
            uIContainer.SetActive(false);
            loadingVFX.SetActive(true);
            txtNotification.text = "...Processing...";
        });
        
        playFabManager.txtNotification = txtNotification;
    }
}
