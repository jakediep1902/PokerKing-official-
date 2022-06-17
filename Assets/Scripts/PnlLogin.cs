using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PnlLogin : MonoBehaviour
{
    PlayFabManager playFabManager;

    public InputField inputID, inputPassWord, inputEmail;
    public Button btnLogin, btnRegister, btnLoginGuest;
    public Text txtNotification;
    private void Start()
    {               
        playFabManager = PlayFabManager.Instance;
        playFabManager.inputID = inputID;
        playFabManager.inputPassWord = inputPassWord;
        playFabManager.inputEmail = inputEmail;
        btnLogin.onClick.AddListener(() => playFabManager.Login());
        btnLoginGuest.onClick.AddListener(() => playFabManager.LoginGuest());
        btnRegister.onClick.AddListener(() => playFabManager.Register());
        playFabManager.txtNotification = txtNotification;
    }
}
