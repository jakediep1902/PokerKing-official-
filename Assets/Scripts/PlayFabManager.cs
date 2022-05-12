using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using Newtonsoft.Json;
using UnityEngine.UI;
using System;



public class UserData
{
    public string userName;
    //public string userID;
    public long money;

    public UserData(string name, long money)
    {
        userName = name;
        //userID = ID;
        this.money = money;
    }


}

public class PlayFabManager : MonoBehaviour
{
    // Start is called before the first frame update
    public UserData[] userDatas = new UserData[6];

    public InputField inputID, inputPassWord, inputEmail;

    void Start()
    {
        //userDatas = FindObjectsOfType<UserData>();
       //userData = userDatas[0];
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public UserData ReturnClass()
    {
        //userData.userName = "JakeDiep";
        //int random = Random.Range((int)100000, (int)9999999);
        //userData.userID = random.ToString();
        //userData.money = Random.Range((int)10000000, (int)100000000);
        var random = UnityEngine.Random.Range((int)10000000, (int)100000000);
        return new UserData("jake",random);
    }
    public void ShowInfo(UserData userData)
    {
        Debug.Log($"name is {userData.userName}, money is {userData.money}");
    }
   
    public void SaveDatasUser()
    {
        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
            {
                //{"userName",userData.userName },
                //{"userID", userData.userID },
                //{"money", userData.money.ToString() },
                {"Player", JsonConvert.SerializeObject(ReturnClass())}
            }
        };
        PlayFabClientAPI.UpdateUserData(request, OnDatasSend, OnError);
    }
    public void LoadDataUser()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), OnRecievedData, OnError);
    }

    private void OnRecievedData(GetUserDataResult obj)
    {
        Debug.Log($"Recieved Player data");
        if (obj.Data != null && obj.Data.ContainsKey("Player"))
        {
            UserData userdata = JsonConvert.DeserializeObject<UserData>(obj.Data["Player"].Value);
            Debug.Log($"userName : {userdata.userName} , money : {userdata.money}");
        }
    }

    private void OnError(PlayFabError obj)
    {
        Debug.Log($"Datas Send Error {obj.ErrorMessage}");
    }

    private void OnDatasSend(UpdateUserDataResult obj)
    {
        Debug.Log($"Datas send to Server success");
        
    }


    public void Login()
    {
        var request = new LoginWithPlayFabRequest
        {
            Username = inputID.text,
            Password = inputPassWord.text,
        };
        PlayFabClientAPI.LoginWithPlayFab(request, OnLoginSuccess, OnLoginError);
    }

    private void OnLoginError(PlayFabError obj)
    {
        Debug.Log($"Wrong Name or Password {obj.ErrorMessage}");
    }

    private void OnLoginSuccess(LoginResult obj)
    {
        Debug.Log($"Login successful with ID {obj.PlayFabId}");
        LoadDataUser();
    }

    public void Register()
    {
        RegisterPlayFabUserRequest registerRequest = new RegisterPlayFabUserRequest
        {
            Email = inputEmail.text,
            Username = inputID.text,
            Password = inputPassWord.text,
            RequireBothUsernameAndEmail = false,
            DisplayName = inputID.text,
        };
        PlayFabClientAPI.RegisterPlayFabUser(registerRequest, RegisterSuccess, RegisterError);
    }
    private void RegisterError(PlayFabError obj)
    {
        Debug.Log($"Register Error {obj.ErrorMessage}");
    }
    private void RegisterSuccess(RegisterPlayFabUserResult obj)
    {
        Debug.Log($"Register Success with userName: {obj.Username}");
    }
}
