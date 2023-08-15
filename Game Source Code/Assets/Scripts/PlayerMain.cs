using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class PlayerMain : NetworkBehaviour
{
    // character selection variables
    public static int characterIndex;
    public static string characterType;

    // character specs variables
    [SyncVar] public float maxHealth = 100;
    [SyncVar] public float currentHealthAmt;

    // character health bar variables
    [SerializeField] private HealthBarUpdate healthBar;
    [SerializeField] private Canvas healthBarCanvas;

    // character display tag variables

    public static string username = "walletAddress";
    public static string connectedWalletAddress = "connectedWalletAddress"; // contains full wallet address, instead of shortened address used for playfab username string
    [SerializeField] private DisplayTagFaceCamera playerDisplayTag;


    public override void OnStartLocalPlayer()
    {
        // initializing health bar
        currentHealthAmt = maxHealth;
        healthBarCanvas.gameObject.SetActive(true);
        healthBar.UpdateHealthBar(maxHealth, currentHealthAmt);

        // initializing player displaytag
        //playerDisplayTag.SetDisplayName(username);
        //CmdSetPlayerDisplayTag(username);
    }

    [Command]
    void CmdSetPlayerDisplayTag(string username)
    {
        RpcSetPlayerDisplayTag(username);
    }

    [ClientRpc]
    void RpcSetPlayerDisplayTag(string username)
    {
        playerDisplayTag.SetDisplayName(username);
    }

    void Update()
    {
        if (!isLocalPlayer) return;

        if (Input.GetKeyDown(KeyCode.X))
        {
            Debug.Log($"Attempting Damage!");
            TakeDamage(10);

            if (healthBarCanvas.gameObject)
                healthBar.UpdateHealthBar(maxHealth, currentHealthAmt);
        }
    }

    void TakeDamage(int damage)
    {
        currentHealthAmt -= damage;
    }

}
