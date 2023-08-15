using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveScript : MonoBehaviour
{
    public static int _playerCharacterIndex = 0;
    public static string _walletAddress;

    void Start()
    {
        DontDestroyOnLoad(this);
    }

}
