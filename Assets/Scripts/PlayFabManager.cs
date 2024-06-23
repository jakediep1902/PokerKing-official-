using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using Newtonsoft.Json;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;



//public class UserData
//{
//    public string userName;
//    //public string userID;
//    public long money;

//    public UserData(string name, long money)
//    {
//        userName = name;
//        //userID = ID;
//        this.money = money;
//    }


//}

public class PlayFabManager : MonoBehaviour
{
    public static PlayFabManager Instance;

    public InputField inputID, inputPassWord, inputEmail;
    public Text txtNotification;
    

    public UserData userData =  new UserData();
    private void Awake()
    {
        if(Instance==null) Instance = this;
      
        else  Destroy(this.gameObject);

        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        inputID.text = PlayerPrefs.GetString("ID");
        inputPassWord.text = PlayerPrefs.GetString("PassWord");       
    }
    private void OnValidate()
    {
        
    }
    //public UserData ReturnClass()
    //{
    //    //userData.userName = "JakeDiep";
    //    //int random = Random.Range((int)100000, (int)9999999);
    //    //userData.userID = random.ToString();
    //    //userData.money = Random.Range((int)10000000, (int)100000000);
    //    var random = UnityEngine.Random.Range((int)10000000, (int)100000000);
    //    return new UserData("jake",random);
    //}
    public void ShowInfo(UserData userData)
    {
        Debug.Log($"name is {userData.userName}, money is {userData.money}");
    }
   

    //save and Load data player
    public void SaveDatasUser()
    {
        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
            {               
                {"Player", JsonConvert.SerializeObject(userData)}
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
        Debug.Log($"Recieved Data from PlayFab ");
        if (obj.Data != null && obj.Data.ContainsKey("Player"))
        {
            userData = JsonConvert.DeserializeObject<UserData>(obj.Data["Player"].Value);
            if(userData.userName=="PlayerName" || userData.userName=="")
            {
                Debug.Log($"userData.userName is : {userData.userName}");
                userData.userName = inputID.text;
                SaveDatasUser();              
            }
            else
            {
                Debug.Log($"userName : {userData.userName} , money : {userData.money}");              
                SceneManager.LoadScene("Game");
            }         
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


    public void LoginGuest()
    {
        //string ID = UnityEngine.Random.Range(1, 1000).ToString();
        var request = new LoginWithPlayFabRequest
        {
            Username = "GUESTS",
            Password = "GUESTS",
        };
        PlayFabClientAPI.LoginWithPlayFab(request, OnLoginSuccess, OnLoginError);       
    }
    //login
    public void Login()
    {       
        if (inputID.text.Length<6 || inputID.text.Length > 10 || inputPassWord.text.Length<6 || inputPassWord.text.Length > 10)
        {          
            ShowNotification("User name or password must be 6 - 10 character!");
        }
        else
        {
            var request = new LoginWithPlayFabRequest
            {
                Username = inputID.text,
                Password = inputPassWord.text,
            };
            PlayFabClientAPI.LoginWithPlayFab(request, OnLoginSuccess, OnLoginError);
        }      
        
    }

    private void OnLoginError(PlayFabError obj)
    {
        Debug.Log($"Wrong Name or Password {obj.ErrorMessage}");
        ShowNotification("Wrong user name or password !");
    }

    private void OnLoginSuccess(LoginResult obj)
    {
        PlayerPrefs.SetString("ID", inputID.text);
        PlayerPrefs.SetString("PassWord", inputPassWord.text);
        Debug.Log($"Login successful with ID {obj.PlayFabId}");
        ShowNotification("Please wait...!");
        LoadDataUser();
    }



    //Register
    public void Register()
    {
        if(inputID.text.Length<6 || inputID.text.Length >10 || inputPassWord.text.Length<6 || inputPassWord.text.Length >10)
        {
            ShowNotification("User Name or Password must be 6-10 character.Please try again !!!");
        }
        else
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
    }
    private void RegisterError(PlayFabError obj)
    {      
        Debug.Log($"Register Error {obj.ErrorMessage}");
        ShowNotification("Register failed please try again with another User name or Email!");
    }
    private void RegisterSuccess(RegisterPlayFabUserResult obj)
    {
        PlayerPrefs.SetString("ID", inputID.text);
        PlayerPrefs.SetString("PassWord", inputPassWord.text);
        Debug.Log($"Register Success with userName: {obj.Username}");
        ShowNotification("Register successful!");
        userData.userName = obj.Username;
        userData.money = 5000000;
        SaveDatasUser();
        //Invoke(nameof(LoadScene), 2f);
        LoadScene();
    }

    public void ShowNotification(string notification)
    {
        txtNotification.text = notification;       
    }
    public void LoadScene()
    {
        //SceneManager.LoadScene("Game");
        SceneManager.LoadScene("Game");
    }
}
