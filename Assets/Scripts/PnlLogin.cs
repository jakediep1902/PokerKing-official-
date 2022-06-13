using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PnlLogin : MonoBehaviour
{
    PlayFabManager playFabManager;

    public InputField inputID, inputPassWord, inputEmail;
    public Button btnLogin, btnRegister;
    public Text txtNotification;
    private void Start()
    {               
        playFabManager = PlayFabManager.Instance;
        playFabManager.inputID = inputID;
        playFabManager.inputPassWord = inputPassWord;
        playFabManager.inputEmail = inputEmail;
        btnLogin.onClick.AddListener(() => playFabManager.Login());
        btnRegister.onClick.AddListener(() => playFabManager.Register());
        playFabManager.txtNotification = txtNotification;
    }
}
