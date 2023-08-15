using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetworkWeaponSelection2 : NetworkBehaviour
{

    // ==== Weapon Variables ====
    public GameObject[] allWeapons;
    public List<GameObject> allowedWeapons;

    [SyncVar(hook = nameof(OnChangeWeapon))]
    public int weaponChoice = 0;

    public void OnChangeWeapon(int oldWeaponChoice, int newWeaponChoice)
    {
        for (int i = 0; i < allowedWeapons.Count; i++)
        {
            if (i == newWeaponChoice)
            {
                allowedWeapons[i].SetActive(true);
                Debug.Log($"Setting weapon choice: {newWeaponChoice}");
            }
            else
                allowedWeapons[i].SetActive(false);
        }
        Debug.Log($"Current weapon choice: {newWeaponChoice}");
    }

    public override void OnStartLocalPlayer()
    {
        if (isLocalPlayer)
        {
            CmdInitializeAvailableWeapons(); // To check PlayerMain to get list of weapons unlocked by player
            CmdSelectWeapon(weaponChoice); // To select within list of unlocked weapons
        }
    }

    void Update()
    {
        if (!hasAuthority && !isLocalPlayer)
            return;

        int previousWeapon = weaponChoice;

        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            if (weaponChoice >= allowedWeapons.Count - 1)
                weaponChoice = 0;
            else
                weaponChoice++;
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            if (weaponChoice <= 0)
                weaponChoice = allowedWeapons.Count - 1;
            else
                weaponChoice--;
        }
        if (previousWeapon != weaponChoice)
            CmdSelectWeapon(weaponChoice);
    }

    [Command]
    void CmdSelectWeapon(int selectedWeapon)
    {
        weaponChoice = selectedWeapon;
    }


    [Command]
    void CmdInitializeAvailableWeapons()
    {
        RpcInitializeAvailableWeapons();
    }

    [ClientRpc]
    void RpcInitializeAvailableWeapons()
    {
    }
}
