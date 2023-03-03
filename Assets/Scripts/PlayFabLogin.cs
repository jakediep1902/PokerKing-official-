using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
using System;

public class PlayFabLogin : MonoBehaviour
{
    //public InputField inputID, inputPassWord, inputEmail;

    //public void Login()
    //{
    //    var request = new LoginWithPlayFabRequest
    //    {
    //        Username = inputID.text,
    //        Password = inputPassWord.text,
    //    };
    //    PlayFabClientAPI.LoginWithPlayFab(request, OnLoginSuccess, OnLoginError);
    //}

    //private void OnLoginError(PlayFabError obj)
    //{
    //    Debug.Log($"Wrong Name or Password {obj.ErrorMessage}");
    //}

    //private void OnLoginSuccess(LoginResult obj)
    //{
    //    Debug.Log($"Login successful with ID {obj.PlayFabId}");
    //}

    //public void Register()
    //{
    //    RegisterPlayFabUserRequest registerRequest = new RegisterPlayFabUserRequest
    //    {
    //        Email = inputEmail.text,
    //        Username = inputID.text,
    //        Password = inputPassWord.text,
    //        RequireBothUsernameAndEmail = false,
    //        DisplayName = inputID.text,
    //    };
    //    PlayFabClientAPI.RegisterPlayFabUser(registerRequest, RegisterSuccess, RegisterError);
    //}
    //private void RegisterError(PlayFabError obj)
    //{
    //    Debug.Log($"Register Error {obj.ErrorMessage}");
    //}
    //private void RegisterSuccess(RegisterPlayFabUserResult obj)
    //{
    //    Debug.Log($"Register Success with userName: {obj.Username}");
    //}
}
