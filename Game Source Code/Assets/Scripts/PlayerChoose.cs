using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerChoose : MonoBehaviour
{
    private int _playerChoice = 0; // current player choice
    private string _walletAddress;
    public GameObject[] _characterModels; // array to store character models. Note order: [CLERIC, ROGUE, WARRIOR, WIZARD]
    public GameObject[] _characterTypes; // array to store character type text. Note order: [CLERIC, ROGUE, WARRIOR, WIZARD]
    public GameObject[] _characterDescriptions; // array to store character descriptions. Note order: [CLERIC, ROGUE, WARRIOR, WIZARD]

    private void Start()
    {
        _walletAddress = GameObject.FindGameObjectWithTag("walletAddress").GetComponent<Text>().text;
    }

    public void NextCharacterButton()
    {
        if (_playerChoice < _characterModels.Length - 1) // check if current player choice number is less than length of character array
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
        if (_playerChoice > 0) // check if current player choice number is less than length of character array
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
        SaveScript._playerCharacterIndex = _playerChoice; // send selected player choice to saveScript
        SaveScript._walletAddress = _walletAddress;
        SceneManager.LoadScene("TestScene"); // load next scene using data sent to saveScript.
    }

    public void Back()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
