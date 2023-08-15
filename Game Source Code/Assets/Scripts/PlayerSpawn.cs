using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawn : MonoBehaviour
{
    public GameObject[] _characterPrefabs;
    public Texture2D[] _characterSkins;
    public Transform _playerSpawnPoint;
    public static int playerCharacterIndex;


    GameObject spawnedModel;

    void Start()
    {

        // get index of player's selected character from PlayFab account, use it to instantiate prefab of same model from characterPrefabs array.
        spawnedModel = Instantiate(_characterPrefabs[playerCharacterIndex], _playerSpawnPoint.position, _playerSpawnPoint.rotation);

    }

}
