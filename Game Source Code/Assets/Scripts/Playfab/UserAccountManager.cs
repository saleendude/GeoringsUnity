using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using PlayFab;
using PlayFab.ClientModels;

// !!! THIS SCRIPT IS ONLY FOR COMMUNICATION WITH PLAYFAB. DOES NOT CONTAIN ANY UI SPECIFIC CODE. CHECK UIScripts FOLDER FOR THOSE !!!

public class UserAccountManager : MonoBehaviour
{
    public static UserAccountManager Instance; // making UserAccountManager class a singleton to make it easier to access

    #region Events
    // ==== Login Events ====
    public static UnityEvent OnLoginSuccess = new UnityEvent();
    public static UnityEvent<string> OnLoginFailed = new UnityEvent<string>();

    // ==== Account Creation Events ====
    public static UnityEvent OnCreateAccountSuccess = new UnityEvent();
    public static UnityEvent<string> OnCreateAccountFailed = new UnityEvent<string>();

    // ==== Get/Set Player Preferences Events ====
    public static UnityEvent OnUserAccountSetSuccess = new UnityEvent();
    public static UnityEvent<string> OnUserAccountSetFailure = new UnityEvent<string>();
    public static UnityEvent OnUserAccountGetSuccess = new UnityEvent();
    public static UnityEvent<string> OnUserAccountGetFailure = new UnityEvent<string>();

    // ==== Get/Set Player Weapons Data Events ====
    public static UnityEvent OnWeaponsDataSetSuccess = new UnityEvent();
    public static UnityEvent OnWeaponsDataSetFailure = new UnityEvent();
    public static UnityEvent OnWeaponsDataGetSuccess = new UnityEvent();
    public static UnityEvent OnWeaponsDataGetFailure = new UnityEvent();

    #endregion

    void Awake()
    {
        Instance = this; 
    }

    public void CreateAccount(string username, string password)
    {
        PlayFabClientAPI.RegisterPlayFabUser(new RegisterPlayFabUserRequest() 
        { Username = username, Password = password, RequireBothUsernameAndEmail=false }, 
        response => {
            //Debug.Log($"<color=green>Account created for wallet: {username}</color>");
            OnCreateAccountSuccess.Invoke(); // send out OnCreateAccountSuccess event if account successfully created
            Login(username, password);
        }, 
        error => {
            Debug.Log($"<color=red>Account creation for wallet failed: {username}\nError: {error.ErrorMessage}</color>");
            foreach (KeyValuePair<string, List<string>> kvp in error.ErrorDetails) {
                Debug.Log($"{kvp.Key}: ");
                foreach (var x in kvp.Value)
                {
                    Debug.Log(x.ToString());
                }
            }
            OnCreateAccountFailed.Invoke(error.ErrorMessage); // send out OnCreateAccountFailed event if account creation failed
        }
        );
    }

    public void Login(string username, string password)
    {
        PlayFabClientAPI.LoginWithPlayFab(new LoginWithPlayFabRequest()
        {
            Username = username,
            Password = password
        }, 
        response => {
            //Debug.Log($"<color=green>Wallet login successful: {username}</color>");
            OnLoginSuccess.Invoke(); // send out OnLoginSuccess event if account successfully logged in
            PlayerMain.username = username; // set current session's username in playermain (used for display tag only)
        }, 
        error => {
            Debug.Log($"<color=red>Wallet login failed: {username}\nError: {error.ErrorMessage}</color>");
            OnLoginFailed.Invoke(error.ErrorMessage); // send out OnLoginFailed event if account login failed
            foreach (KeyValuePair<string, List<string>> kvp in error.ErrorDetails)
            {
                Debug.Log($"{kvp.Key}: ");
                foreach (var x in kvp.Value)
                {
                    Debug.Log(x.ToString());
                }
            }
        });
    }


    public void SetCharacterPreference(int chosenCharacterIndex)
    {
        int chosenCharType = chosenCharacterIndex;
        string characterType = "demon";

        if (chosenCharType == 0)
            characterType = "demon";
        else if (chosenCharType == 1)
            characterType = "samurai";
        else if (chosenCharType == 2)
            characterType = "warrior";
        else if (chosenCharType == 3)
            characterType = "wizard";

        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
            {
                {"characterType", characterType},
                { "characterIndex", chosenCharacterIndex.ToString()}
            }
        };

        PlayFabClientAPI.UpdateUserData(request, 
            response => {
                //Debug.Log($"<color=green>User character preferences uploaded. Character type: {characterType}</color>");
                OnUserAccountSetSuccess.Invoke();
            }, 
            error => 
            {
                Debug.Log($"<color=red>Error with uploading character preferences. Character type: {characterType}\nError: {error.ErrorMessage}</color>");
                OnUserAccountSetFailure.Invoke(error.ErrorMessage);
            });
    }

    public void GetCharacterPreference()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest() { }, 
            response => {
                if (response.Data != null && response.Data.ContainsKey("characterType") && response.Data.ContainsKey("characterIndex"))
                {
                    PlayerSpawn.playerCharacterIndex = int.Parse(response.Data["characterIndex"].Value);
                    PlayerMain.characterIndex = int.Parse(response.Data["characterIndex"].Value);
                    PlayerMain.characterType = response.Data["characterType"].Value;
                    Debug.Log($"PlayerMain characterIndex: {PlayerMain.characterIndex}");
                    Debug.Log($"PlayerMain characterType: {PlayerMain.characterType}");
                }
                //Debug.Log($"<color=green>User character preferences downloaded from Playfab.</color>");
                OnUserAccountGetSuccess.Invoke();
            }, 
            error => {
                Debug.Log($"<color=red>User character preferences download unssuccessful | Error:{error.ErrorMessage}</color>");
                OnUserAccountGetFailure.Invoke(error.ErrorMessage);
            });
    }

    public void SetCharacterAllowedWeapons()
    {

    }
}
