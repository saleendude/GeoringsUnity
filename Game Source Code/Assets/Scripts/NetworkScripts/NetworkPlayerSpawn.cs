using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkPlayerSpawn : NetworkBehaviour
{
    [SerializeField] GameObject[] characterPrefabs;


    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        if (!isLocalPlayer) { return; }
        if (NetworkClient.ready)
            CmdReplacePlayer();
    }

    [Command]
    void CmdReplacePlayer()
    {
        GameObject go = Instantiate(characterPrefabs[PlayerMain.characterIndex], transform.position, transform.rotation);
        NetworkServer.Spawn(go);
        Debug.Log($"Current Prefab: {connectionToClient.identity.gameObject.name}");

        if (NetworkServer.ReplacePlayerForConnection(connectionToClient, go, true))
        {
            Debug.Log("Replaced!");
            NetworkServer.Destroy(gameObject);
        }
    }
}
