using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// THIS SCRIPT HANDLES UI INPUTS AND SENDS THEM TO UserAccountManager TO LOGIN PLAYFAB PROFILE
public class UILogin : MonoBehaviour
{
    [SerializeField] Text walletAddress;
    [SerializeField] TextMeshProUGUI errorText;
    string password;

    [SerializeField] private Image accountLoginSuccessBanner;
    [SerializeField] private Image accountLoginFailureBanner;
    [SerializeField] private Text LoadingNewSceneText;

    public void UpdateUsername(string _username) // Fn to get username from walletAddress text item. To be injected by react in build.
    {
        walletAddress.text = _username;
    }

    public void UpdatePassword(string _password) // Fn to get password from PasswordInput inputfield with OnValueChanged call in TMP component.
    {
        password = _password;
    }

    public void Login()
    {
        UserAccountManager.Instance.Login(walletAddress.text, password); // send wallet address and password to UserAccountManager to use in playfab account login
    }

    #region EventNotificationFunctions

    void OnEnable()
    {
        UserAccountManager.OnLoginFailed.AddListener(AccountLoginFailedNotification); // start listening to events being sent out from account login failures
        UserAccountManager.OnLoginSuccess.AddListener(AccountLoginSuccessNotification); // start listening to events being sent out from account login successes
    }
    void OnDisable()
    {
        UserAccountManager.OnLoginFailed.RemoveListener(AccountLoginFailedNotification); // stop listening to events being sent out from account login failures
        UserAccountManager.OnLoginSuccess.RemoveListener(AccountLoginSuccessNotification); // stop listening to events being sent out from account login successes
    }

    void AccountLoginFailedNotification(string error)
    {
        accountLoginSuccessBanner.gameObject.SetActive(false);
        accountLoginFailureBanner.gameObject.SetActive(true);
        errorText.gameObject.SetActive(true);
        errorText.text = error;
    }
    void AccountLoginSuccessNotification()
    {
        accountLoginFailureBanner.gameObject.SetActive(false);
        errorText.gameObject.SetActive(false);
        accountLoginSuccessBanner.gameObject.SetActive(true);

        errorText.gameObject.SetActive(false);
        UserAccountManager.Instance.GetCharacterPreference();
        StartCoroutine(LoadNextScene());
    }


    IEnumerator LoadNextScene()
    {
        LoadingNewSceneText.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("TestScene");
    }

    #endregion
}
