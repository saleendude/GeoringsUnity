using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class GeoringsNetworkManager : NetworkManager
{
    [SerializeField] GameObject[] characterPrefabs;
    [SerializeField] GameObject[] spawnPoints;
    //public int characterIndex;

    public struct CharacterMessage : NetworkMessage
    {
        public int characterIndex;
        public string characterType;
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        Debug.Log("<color=green>Server Started</color>");
        NetworkServer.RegisterHandler<CharacterMessage>(OnMessageReceived);
    }

    public override void OnClientConnect()
    {
        base.OnClientConnect();

        CharacterMessage characterMessage = new CharacterMessage
        {
            characterIndex = PlayerMain.characterIndex,
            characterType = PlayerMain.characterType
        };

        NetworkClient.Send(characterMessage);
    }

    void OnMessageReceived(NetworkConnectionToClient conn, CharacterMessage message)
    {
        int selectedClass = message.characterIndex;
        Debug.Log("Spawning with class:  " + selectedClass);

        GameObject player;
        Transform startPos = GetStartPosition();

        if (startPos != null)
        {
            player = Instantiate(characterPrefabs[selectedClass], startPos.position, startPos.rotation) as GameObject;
        }
        else
        {
            player = Instantiate(characterPrefabs[selectedClass], Vector3.zero, Quaternion.identity) as GameObject;
        }

        NetworkServer.AddPlayerForConnection(conn, player);
    }
}

