using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerChooseSignup : MonoBehaviour
{

    // ==== Account Creation Variables ====
    [Header("Account Creation Variables")]
    [SerializeField] Text walletAddress;
    [SerializeField] TextMeshProUGUI errorText;
    string password;

    [SerializeField] private Image accountCreationSuccessBanner;
    [SerializeField] private Image accountCreationFailureBanner;
    [SerializeField] private Text LoadingNewSceneText;

    // ==== Player Choice Variables
    private int _playerChoice = 0; // current player choice
    [Header("Player Choice Variables")]
    public GameObject[] _characterModels; // array to store character models. Note order: [CLERIC, ROGUE, WARRIOR, WIZARD]
    public GameObject[] _characterTypes; // array to store character type text. Note order: [CLERIC, ROGUE, WARRIOR, WIZARD]
    public GameObject[] _characterDescriptions; // array to store character descriptions. Note order: [CLERIC, ROGUE, WARRIOR, WIZARD]

    void Start()
    {
        walletAddress.text = SaveScript._walletAddress;
    }


    #region UserAccountCreation

    public void UpdateUsername(string _username) // Fn to get username from walletAddress text item. To be injected by react in build.
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

    // start listening to events from UserAccountManager
    void OnEnable()
    {
        UserAccountManager.OnCreateAccountFailed.AddListener(AccountCreationFailedNotification);
        UserAccountManager.OnCreateAccountSuccess.AddListener(AccountCreationSuccessNotification);

        UserAccountManager.OnLoginSuccess.AddListener(AccountLoginSuccessNotification);
        UserAccountManager.OnLoginFailed.AddListener(AccountLoginFailedNotification);

        UserAccountManager.OnUserAccountGetSuccess.AddListener(AccountGetSuccess);
        UserAccountManager.OnUserAccountGetFailure.AddListener(AccountGetFailure);

        UserAccountManager.OnUserAccountSetSuccess.AddListener(AccountSetSuccess);
        UserAccountManager.OnUserAccountSetFailure.AddListener(AccountSetFailure);
    }

    // stop listening to events from UserAccountManager
    void OnDisable()
    {
        UserAccountManager.OnCreateAccountFailed.RemoveListener(AccountCreationFailedNotification);
        UserAccountManager.OnCreateAccountSuccess.RemoveListener(AccountCreationSuccessNotification);

        UserAccountManager.OnLoginSuccess.RemoveListener(AccountLoginSuccessNotification);
        UserAccountManager.OnLoginFailed.RemoveListener(AccountLoginFailedNotification);

        UserAccountManager.OnUserAccountGetSuccess.RemoveListener(AccountGetSuccess);
        UserAccountManager.OnUserAccountGetFailure.RemoveListener(AccountGetFailure);

        UserAccountManager.OnUserAccountSetSuccess.RemoveListener(AccountSetSuccess);
        UserAccountManager.OnUserAccountSetFailure.RemoveListener(AccountSetFailure);
    }

    void AccountCreationFailedNotification(string error)
    {
        LoadingNewSceneText.gameObject.SetActive(false);
        accountCreationSuccessBanner.gameObject.SetActive(false);
        accountCreationFailureBanner.gameObject.SetActive(true);
        errorText.gameObject.SetActive(true);
        errorText.text = error;
    }
    void AccountCreationSuccessNotification()
    {
        LoadingNewSceneText.gameObject.SetActive(false);
        accountCreationFailureBanner.gameObject.SetActive(false);
        errorText.gameObject.SetActive(false);
        accountCreationSuccessBanner.gameObject.SetActive(true);
    }

    void AccountLoginFailedNotification(string error)
    {
        LoadingNewSceneText.gameObject.SetActive(false);
        errorText.gameObject.SetActive(true);
        errorText.text = error;
    }
    void AccountLoginSuccessNotification()
    {
        errorText.gameObject.SetActive(false);
        UserAccountManager.Instance.SetCharacterPreference(_playerChoice);
    }

    void AccountSetSuccess()
    {
        Debug.Log($"<color=green>Event:: Account details set for player: {walletAddress.text}</color>");
        UserAccountManager.Instance.GetCharacterPreference();
    }
    void AccountSetFailure(string error)
    {
        Debug.Log($"<color=red>Event:: Account details not set for player: {walletAddress.text}\nError:{error}</color>");
    }
    void AccountGetSuccess()
    {
        Debug.Log($"<color=green>Event:: Account details downloaded for player: {walletAddress.text}. Starting next scene.</color>");
        StartCoroutine(LoadNextScene());
    }
    void AccountGetFailure(string error)
    {
        Debug.Log($"<color=red>Event:: Unable to get account details for player: {walletAddress.text}.\nError:{error}</color>");
    }

    IEnumerator LoadNextScene()
    {
        LoadingNewSceneText.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("TestScene");
    }

    #endregion

    #endregion

    #region UserPlayerSelection

    public void NextCharacterButton()
    {
        if (_playerChoice < _characterModels.Length - 1)
        {
            _characterModels[_playerChoice].SetActive(false); // initially turn off current character model
            _characterTypes[_playerChoice].SetActive(false); // initially turn off current character type text
            _characterDescriptions[_playerChoice].SetActive(false); // initially turn off current character description


            _playerChoice++; // move player choice to the next model in line by incrementing by 1

            _characterModels[_playerChoice].SetActive(true); // turn on next character type model
            _characterTypes[_playerChoice].SetActive(true); // turn on next character type text
            _characterDescriptions[_playerChoice].SetActive(true); // turn on next character type description

        }
    }
    public void PreviousCharacterButton()
    {
        if (_playerChoice > 0)
        {
            _characterModels[_playerChoice].SetActive(false); // initially turn off current character model
            _characterTypes[_playerChoice].SetActive(false); // initially turn off current character type text
            _characterDescriptions[_playerChoice].SetActive(false); // initially turn off current character description

            _playerChoice--; // move player choice to the next model in line by incrementing by 1

            _characterModels[_playerChoice].SetActive(true); // turn on prior character type model
            _characterTypes[_playerChoice].SetActive(true); // turn on prior character type text
            _characterDescriptions[_playerChoice].SetActive(true); // turn on prior character type description
        }
    }


    public void Proceed()
    {
        /* CreateAccount uses username and pw sent to UserAccountManager to create a new account,
        then logs in using that account, send the player character preference to playfab, then loads
        TestScene after successfully grabbing player's character preference and using it to instantiate 
        player model in PlayerSpawn script.

        CreateAccount()-> Successful Account Creation event -> Login() -> Successful Login Event -> Set player preference -> Get player preference -> 
        Instantiate Player model in Test scene using player preference.
         */
        CreateAccount();

    }

    public void Back()
    {
        SceneManager.LoadScene("MainMenu");
    }
    #endregion
}
