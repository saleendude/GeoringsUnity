using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

// THIS SCRIPT HANDLES UI INPUTS AND SENDS THEM TO UserAccountManager TO CREATE PLAYFAB PROFILE

public class UICreateAccount : MonoBehaviour
{
    [SerializeField] Text walletAddress;
    [SerializeField] TextMeshProUGUI errorText;
    string password;

    [SerializeField] private Image accountCreationSuccessBanner;
    [SerializeField] private Image accountCreationFailureBanner;

    public void UpdateUsername( string _username) // Fn to get username from walletAddress text item. To be injected by react in build.
    {
        walletAddress.text = _username;
    }

    public void UpdatePassword(string _password) // Fn to get password from PasswordInput inputfield with OnValueChanged call in TMP component.
    {
        password = _password;
    }

    public void CreateAccount()
    {
        UserAccountManager.Instance.CreateAccount(walletAddress.text, password); // send wallet address and password to UserAccountManager to use in playfab account creation
    }

    #region EventNotificationFunctions

    void OnEnable() 
    {
        UserAccountManager.OnCreateAccountFailed.AddListener(AccountCreationFailedNotification); // start listening to events being sent out from account creation failures
        UserAccountManager.OnCreateAccountSuccess.AddListener(AccountCreationSuccessNotification); // start listening to events being sent out from account creation failures
    }
    void OnDisable() 
    {
        UserAccountManager.OnCreateAccountFailed.RemoveListener(AccountCreationFailedNotification); // stop listening to events being sent out from account creation successes
        UserAccountManager.OnCreateAccountSuccess.RemoveListener(AccountCreationSuccessNotification); // stop listening to events being sent out from account creation successes
    }

    void AccountCreationFailedNotification(string error)
    {
        accountCreationSuccessBanner.gameObject.SetActive(false);
        accountCreationFailureBanner.gameObject.SetActive(true);
        errorText.gameObject.SetActive(true);
        errorText.text = error;
    }
    void AccountCreationSuccessNotification()
    {
        accountCreationFailureBanner.gameObject.SetActive(false);
        errorText.gameObject.SetActive(false);
        accountCreationSuccessBanner.gameObject.SetActive(true);
    }

    public void LoadCustomizationScreen()
    {
        SaveScript._walletAddress = walletAddress.text;
        SceneManager.LoadScene("PlayerChoose");
    }

    #endregion
}
