using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayfabController : MonoBehaviour
{
    public static PlayfabController PFC;

    private string userEmail;
    private string userPassword;
    private string userName;
    private string myID;
    public GameObject logInPannel;
    public GameObject addlogInPannel;
    public GameObject recoverButton;

    void DisplayPlayFabError(PlayFabError error) 
    { 
        Debug.Log(error.GenerateErrorReport()); 
    }
    void DisplayError(string error) 
    { 
        Debug.LogError(error); 
    }

    private void OnEnable()
    {
        if(PlayfabController.PFC == null)
        {
            PlayfabController.PFC = this;
        }
        else 
        {
            if (PlayfabController.PFC != this) 
            {
                Destroy(this.gameObject);
            }
        }
        DontDestroyOnLoad(this.gameObject);
    }

    public void Start()
    {
        //Note: Setting title Id here can be skipped if you have set the value in Editor Extensions already.
        if (string.IsNullOrEmpty(PlayFabSettings.TitleId))
        {
            PlayFabSettings.TitleId = "7B5AD"; // Please change this value to your own titleId from PlayFab Game Manager
        }
        // var request = new LoginWithCustomIDRequest { CustomId = "GettingStartedGuide", CreateAccount = true };
        // PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailure);

        if (PlayerPrefs.HasKey("EMAIL"))
        {
            userEmail = PlayerPrefs.GetString("EMAIL");
            userPassword = PlayerPrefs.GetString("PASSWORD");
        var request = new LoginWithEmailAddressRequest { Email = userEmail, Password = userPassword };
        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnLoginFailure);
        }
        else 
        {
#if UNITY_ANDROID
            var requestAndroid = new LoginWithAndroidDeviceIDRequest { AndroidDeviceId = ReturnMobileID(), CreateAccount = true };
            PlayFabClientAPI.LoginWithAndroidDeviceID(requestAndroid, OnLoginMobileSuccess, OnLoginMobileFailure);
#endif
#if UNITY_IOS
            var requestIOS = new LoginWithIOSDeviceIDRequest { DeviceId = ReturnMobileID(), CreateAccount = true };
            PlayFabClientAPI.LoginWithIOSDeviceID(requestIOS, OnLoginMobileSuccess, OnLoginMobileFailure);
#endif
        }

    }
    #region login
    private void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("Congratulations, you made your first successful API call!");
        PlayerPrefs.SetString("EMAIL", userEmail);
        PlayerPrefs.SetString("PASSWORD", userPassword);
        logInPannel.SetActive(false);
        recoverButton.SetActive(false);
        GetStatistics();

        myID = result.PlayFabId;

        GetPlayerData();

    }

    private void OnLoginMobileSuccess(LoginResult result)
    {
        Debug.Log("Congratulations, you made your first successful API call!");
        GetStatistics();
        logInPannel.SetActive(false);

        myID = result.PlayFabId;

        GetPlayerData();
    }

    private void OnRegisterSucsses(RegisterPlayFabUserResult result)
    {
        Debug.Log("Congratulations, you made your first successful API call!");
        PlayerPrefs.SetString("EMAIL", userEmail);
        PlayerPrefs.SetString("PASSWORD", userPassword);

        PlayFabClientAPI.UpdateUserTitleDisplayName(new UpdateUserTitleDisplayNameRequest { DisplayName = userName }, OnDisplayName, OnLoginMobileFailure);
        GetStatistics();
        logInPannel.SetActive(false);

        myID = result.PlayFabId;

        GetPlayerData();
    }

    void OnDisplayName(UpdateUserTitleDisplayNameResult result)
    {
        Debug.Log(result.DisplayName + " is your new display name");
    }

    private void OnLoginFailure(PlayFabError error)
    {
        var registerRequest = new RegisterPlayFabUserRequest { Email = userEmail, Password = userPassword, Username = userName };
        PlayFabClientAPI.RegisterPlayFabUser(registerRequest, OnRegisterSucsses, OnegisterFailure);
    }

    private void OnLoginMobileFailure(PlayFabError error)
    {
        Debug.Log(error.GenerateErrorReport());
    }

    private void OnegisterFailure(PlayFabError error)
    {
        Debug.LogError(error.GenerateErrorReport());
    }

    public void GetUserEmail(string emailIn)
    {
        userEmail = emailIn;
    }

    public void GetUserPasword(string passwordIn)
    {
        userPassword = passwordIn;
    }

    public void GetUserName(string userNameIn)
    {
        userName = userNameIn;
    }

    public void ONClickLogIn()
    {
        var request = new LoginWithEmailAddressRequest { Email = userEmail, Password = userPassword };
        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnLoginFailure);


    }

    public static string ReturnMobileID()
    {
        string deviceID = SystemInfo.deviceUniqueIdentifier;
        return deviceID;
    }

    public void OpenAddLogin()
    {
        addlogInPannel.SetActive(true);
    }

    public void OnClickAddLogin()
    {
        var addLoginRequest = new AddUsernamePasswordRequest { Email = userEmail, Password = userPassword, Username = userName };
        PlayFabClientAPI.AddUsernamePassword(addLoginRequest, OnAddLoginSucsses, OnegisterFailure);
    }

    private void OnAddLoginSucsses(AddUsernamePasswordResult result)
    {
        Debug.Log("Congratulations, you made your first successful API call!");
        PlayerPrefs.SetString("EMAIL", userEmail);
        PlayerPrefs.SetString("PASSWORD", userPassword);
        GetStatistics();
        addlogInPannel.SetActive(false);
    }
    #endregion Login

    public int playerLevel;
    public int gameLevel;

    public int playerHealth;
    public int playerDamage;

    public int playerHighscore;

    #region PlayerStats

    public void setStats()
    {
        PlayFabClientAPI.UpdatePlayerStatistics(new UpdatePlayerStatisticsRequest
        {
            // request.Statistics is a list, so multiple StatisticUpdate objects can be defined if required.
            Statistics = new List<StatisticUpdate> {
               new StatisticUpdate { StatisticName = "PlayerLevel", Value = playerLevel },
                new StatisticUpdate { StatisticName = "GameLevel", Value = gameLevel },
                new StatisticUpdate { StatisticName = "PlayerHealth", Value = playerHealth },
                new StatisticUpdate { StatisticName = "PlayerDamage", Value = playerDamage },
                new StatisticUpdate { StatisticName = "PlayerHighscore", Value = playerHighscore },
    }
        },
        result => { Debug.Log("User statistics updated"); },
        error => { Debug.LogError(error.GenerateErrorReport()); });


    }

    void GetStatistics()
    {
        PlayFabClientAPI.GetPlayerStatistics(
            new GetPlayerStatisticsRequest(),
            OnGetStatistics,
            error => Debug.LogError(error.GenerateErrorReport())
        );
    }

    void OnGetStatistics(GetPlayerStatisticsResult result)
    {
        Debug.Log("Received the following Statistics:");
        foreach (var eachStat in result.Statistics) 
        {
            Debug.Log("Statistic (" + eachStat.StatisticName + "): " + eachStat.Value);
            switch (eachStat.StatisticName) 
            {
                case "PlayerLevel":
                    playerLevel = eachStat.Value;
                    break;
                case "GameLevel":
                    gameLevel = eachStat.Value;
                    break;
                case "PlayerHealth":
                    playerHealth = eachStat.Value;
                    break;
                case "PlayerDamage":
                    playerDamage = eachStat.Value;
                    break;
                case "PlayerHighscore":
                    playerHighscore = eachStat.Value;
                    break;
            }
        }
    }

    // Build the request object and access the API
    public void StartCloudUpdatePlayerStats()
    {
        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
        {
            FunctionName = "UpdatePlayerStats", // Arbitrary function name (must exist in your uploaded cloud.js file)
            FunctionParameter = new { playerLevel = playerLevel, gameLevel =gameLevel, playerHealth = playerHealth, playerDamage = playerDamage, PlayerHighscore = playerHighscore }, // The parameter provided to your function
            GeneratePlayStreamEvent = true, // Optional - Shows this event in PlayStream
        }, OnCloudUpdateState, OnErrorShared);
    }
    // OnCloudHelloWorld defined in the next code block

    private static void OnCloudUpdateState(ExecuteCloudScriptResult result)
    {
        // CloudScript returns arbitrary results, so you have to evaluate them one step and one parameter at a time
        
        Debug.Log(PlayFab.Json.PlayFabSimpleJson.SerializeObject(result.FunctionResult));

        PlayFab.Json.JsonObject jsonResult = (PlayFab.Json.JsonObject)result.FunctionResult;
        object messageValue;
        jsonResult.TryGetValue("messageValue", out messageValue); // note how "messageValue" directly corresponds to the JSON values set in CloudScript
        Debug.Log((string)messageValue);
    }

    private static void OnErrorShared(PlayFabError error)
    {
        Debug.Log(error.GenerateErrorReport());
    }

    #endregion PlayerStats


    public GameObject LeaderBoardPannel;
    public GameObject ListingPrefb;
    public Transform ListingContiner;


    #region LeaderBoard

    public void GetleaderBoard()
    {
        var requestLeaderBoard = new GetLeaderboardRequest { StartPosition = 0, StatisticName = "PlayerHighscore", MaxResultsCount = 20 };
        PlayFabClientAPI.GetLeaderboard(requestLeaderBoard, OnGetLeaderBoard, OnErrorLeaderBoard);
    }

    void OnGetLeaderBoard(GetLeaderboardResult result)
    {
        LeaderBoardPannel.SetActive(true);
        //Debug.Log(result.Leaderboard[0].StatValue);
        foreach(PlayerLeaderboardEntry player in result.Leaderboard)
        {
            GameObject tempListing = Instantiate(ListingPrefb, ListingContiner);
            ListingPrefab LL = tempListing.GetComponent<ListingPrefab>();
            LL.playerNameText.text = player.DisplayName;
            LL.playerScoreText.text = player.StatValue.ToString();

            Debug.Log(player.DisplayName + ": " + player.StatValue);
        }
    }

    public void CloseLeaderBoardPannel()
    {
        LeaderBoardPannel.SetActive(false);
        for (int i = ListingContiner.childCount - 1; i >= 0; i--)
        {
            Destroy(ListingContiner.GetChild(i).gameObject);
        }
    }

    void OnErrorLeaderBoard(PlayFabError error)
    {
        Debug.LogError(error.GenerateErrorReport());
    }

    #endregion LeaderBoard

    #region PlayerData

    public void GetPlayerData()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest()
        {
            PlayFabId = myID,
            Keys = null
        }, UserDataSuccess, OnErrorLeaderBoard);

    }


    
    void UserDataSuccess (GetUserDataResult result)
    {
        DataClassTest ClassTest = new DataClassTest();
        //if (result.Data == null || !result.Data.ContainsKey("Skins"))
        //{
        //    Debug.Log("skins not set");
        //}
        //else
        //{
        //    PersistentData.PD.skinsStringToData(result.Data["Skins"].Value);
        //}
        if (result.Data == null || !result.Data.ContainsKey("DataClassTest"))
        {
            Debug.Log("data not set");
        }
        else
        {
            string temp = result.Data["DataClassTest"].Value;
            ClassTest = JsonUtility.FromJson<DataClassTest>(temp);
            Debug.Log("Recived Data");
        }


    }

    public void SetUserData()
    {
        DataClassTest ClassTest = new DataClassTest();
        string ClassAsJason =  JsonUtility.ToJson(ClassTest);
        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest()
        {
            Data = new Dictionary<string, string>()
            {
             { "DataClassTest",  ClassAsJason }
               // { "Skins", SkinsData}
            }

        }, SetDataSuccess, OnErrorLeaderBoard);
    }

    void SetDataSuccess(UpdateUserDataResult result)
    {
        Debug.Log(result.DataVersion);
    }

    #endregion PlayerData

    #region Friends

    [SerializeField]
    Transform friendScrollView;
    List<FriendInfo> myFriends;

    void DisplayFriends(List<FriendInfo> friendsCache)
    {
        friendsCache.ForEach(f => Debug.Log(f.FriendPlayFabId));
        foreach(FriendInfo f in friendsCache)
        {
            bool isFound = false;
            if (myFriends != null)
            {
                foreach (FriendInfo g in myFriends)
                {
                    if (f.FriendPlayFabId == g.FriendPlayFabId)
                    {
                        isFound = true;
                    }
                }
            }
            if(isFound == false)
            {
            GameObject listing = Instantiate(ListingPrefb, friendScrollView);
            ListingPrefab tempListing = listing.GetComponent<ListingPrefab>();
            tempListing.playerNameText.text = f.TitleDisplayName;
            }
        }
        myFriends = friendsCache;
    }

    IEnumerator waitForFriend()
    {
        yield return new WaitForSeconds(2f);
        GetFriends();
    }

    public void RunWaitFunction()
    {
        StartCoroutine(waitForFriend());
    }

    List<FriendInfo> _friends = null;

    public void GetFriends()
    {
        PlayFabClientAPI.GetFriendsList(new GetFriendsListRequest
        {
            IncludeSteamFriends = false,
            IncludeFacebookFriends = false,
            XboxToken = null
        }, result => {
            _friends = result.Friends;
            DisplayFriends(_friends); // triggers your UI
        }, DisplayPlayFabError);
    }

    enum FriendIdType { PlayFabId, Username, Email, DisplayName };

    void AddFriend(FriendIdType idType, string friendId)
    {
        var request = new AddFriendRequest();
        switch (idType)
        {
            case FriendIdType.PlayFabId:
                request.FriendPlayFabId = friendId;
                break;
            case FriendIdType.Username:
                request.FriendUsername = friendId;
                break;
            case FriendIdType.Email:
                request.FriendEmail = friendId;
                break;
            case FriendIdType.DisplayName:
                request.FriendTitleDisplayName = friendId;
                break;
        }
        // Execute request and update friends when we are done
        PlayFabClientAPI.AddFriend(request, result => {
            Debug.Log("Friend added successfully!");
        }, DisplayPlayFabError);
    }
    string friendSearch;
    [SerializeField]
    GameObject friendPanel;
    private object jason;

    public void InputFriendid(string idin)
    {
        friendSearch = idin;
    }

    public void SubmitFriendRequest()
    {
        AddFriend(FriendIdType.PlayFabId, friendSearch);
    }

    public void OpenCloseFriends()
    {
        friendPanel.SetActive(!friendPanel.activeInHierarchy);
    }


    #endregion Friends
}
[Serializable]
public class DataClassTest 
{
    public int testInt = 1;
    public float TestFloat = 2.35f;
    public int[] IntArrayTest = new int[3];
    public string StringTest = "string text";

}
