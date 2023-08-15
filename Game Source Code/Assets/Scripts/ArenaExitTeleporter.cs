using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class ArenaExitTeleporter : MonoBehaviour
{
    public string SceneToLoad;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "PlayerNetworked")
        {

            NetworkManager.singleton.StopClient();
        }
    }

}