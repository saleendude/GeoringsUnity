using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetworkWeaponSelection : NetworkBehaviour
{
    // ==== Weapon Variables ====

    public GameObject[] allWeapons;
    public List<GameObject> allowedWeapons;
    [SyncVar] public int weaponChoice = 0;


    public override void OnStartLocalPlayer()
    {
        if (isLocalPlayer)
        {
            CmdInitializeAvailableWeapons(); // To check PlayerMain to get list of weapons unlocked by player
            CmdSelectWeapon(); // To select within list of unlocked weapons
        }
    }

    void HandleWeaponScroll()
    {
        int previousSelectedWeapon = weaponChoice;

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

        if (previousSelectedWeapon != weaponChoice)
            CmdSelectWeapon();
    }

    void Update()
    {
        if (!isLocalPlayer)
            return;
        HandleWeaponScroll();
    }

    [Command]
    void CmdInitializeAvailableWeapons()
    {
        RpcInitializeAvailableWeapons();
    }

    [Command]
    void CmdSelectWeapon()
    {
        RpcSelectWeapon();
    }

    [ClientRpc]
    void RpcInitializeAvailableWeapons()
    {
        // Disable all weapons to false initially
        for (int i = 0; i < allWeapons.Length; i++)
        {
            allWeapons[i].SetActive(false);
        }

    }

    [ClientRpc]
    void RpcSelectWeapon()
    {
        int i = 0;
        foreach (GameObject weapon in allowedWeapons)
        {
            if (i == weaponChoice)
                weapon.SetActive(true);
            else
                weapon.SetActive(false);
            i++;
        }
    }

}
