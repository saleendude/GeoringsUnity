using Mirror;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Events;

public class NetworkChangeEquipment3 : NetworkBehaviour
{
    //public static NetworkChangeEquipment3 Instance;

    [SerializeField] Weapon[] weaponScriptableObjects = null;
    [SerializeField] Transform weaponHolderTransform = null;
    [SerializeField] GameObject weaponHolder = null;
    [SerializeField] Animator animator = null;

    public static UnityEvent OnLocalPlayerStart = new UnityEvent();

    [SyncVar(hook = nameof(OnChangeWeaponType))]
    public WeaponType weaponType;

    private void OnEnable()
    {
        WeaponSelectionScreen.OnWeaponButtonDisabled.AddListener(DestroyWeaponOnServer);
    }
    private void OnDisable()
    {
        WeaponSelectionScreen.OnWeaponButtonDisabled.RemoveListener(DestroyWeaponOnServer);
    }

    void OnChangeWeaponType(WeaponType oldWeaponType, WeaponType newWeaponType)
    {
        StartCoroutine(ChangeWeapon(newWeaponType));
    }


    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        OnLocalPlayerStart.Invoke();

        #region Set Weapon ScriptableObjects to false
        foreach (Weapon weaponScriptableObject in weaponScriptableObjects)
        {
            weaponScriptableObject.allowed = false;
        }
        #endregion

        #region Set Weapon ScriptableObjects using PlayerMain.allowedWeaponsDict
        //foreach (KeyValuePair<string, bool> p in PlayerMain.allowedWeaponsDict)
        //{
        //    if (p.Value == true)
        //    {
        //        foreach (Weapon weapon in weaponScriptableObjects)
        //        {
        //            if (weapon.weaponType.ToString() == p.Key)
        //            {
        //                weapon.allowed = true;
        //            }
        //        }
        //    }
        //}
        #endregion
    }

    #region Enable Weapon Functions
    public void EnableWoodenSwordButton()
    {
        if (!isLocalPlayer)
            return;
        CmdSetWeaponType(WeaponType.swordWooden);
    }

    public void EnableSteelSwordButton()
    {
        if (!isLocalPlayer)
            return;
        CmdSetWeaponType(WeaponType.swordSteel);
    }

    public void EnableRubySwordButton()
    {
        if (!isLocalPlayer)
            return;
        CmdSetWeaponType(WeaponType.swordRuby);
    }

    public void EnableEmeraldSwordButton()
    {
        if (!isLocalPlayer)
            return;
        CmdSetWeaponType(WeaponType.swordEmerald);
    }

    public void EnableScytheButton()
    {
        if (!isLocalPlayer)
            return;
        CmdSetWeaponType(WeaponType.scythe);
    }

    public void EnableStaffButton()
    {
        if (!isLocalPlayer)
            return;
        CmdSetWeaponType(WeaponType.staff);
    }

    public void EnableHammerButton()
    {
        if (!isLocalPlayer)
            return;
        CmdSetWeaponType(WeaponType.hammer);
    }

    public void EnableAxeButton()
    {
        if (!isLocalPlayer)
            return;
        CmdSetWeaponType(WeaponType.axe);
    }

    public void EnableDaggerButton()
    {
        if (!isLocalPlayer)
            return;
        CmdSetWeaponType(WeaponType.dagger);
    }

    #endregion

    IEnumerator ChangeWeapon(WeaponType newWeaponType)
    {
        while (weaponHolder.transform.childCount > 0)
        {
            Destroy(weaponHolder.transform.GetChild(0).gameObject); // change this to cmd
            yield return null;
        }

        switch (newWeaponType)
        {
            case WeaponType.axe:
                CmdSpawnWeapon(WeaponType.axe);
                break;
            case WeaponType.dagger:
                CmdSpawnWeapon(WeaponType.dagger);
                break;
            case WeaponType.hammer:
                CmdSpawnWeapon(WeaponType.hammer);
                break;
            case WeaponType.scythe:
                CmdSpawnWeapon(WeaponType.scythe);
                break;
            case WeaponType.swordSteel:
                CmdSpawnWeapon(WeaponType.swordSteel);
                break;
            case WeaponType.swordWooden:
                CmdSpawnWeapon(WeaponType.swordWooden);
                break;
            case WeaponType.swordRuby:
                CmdSpawnWeapon(WeaponType.swordRuby);
                break;
            case WeaponType.swordEmerald:
                CmdSpawnWeapon(WeaponType.swordEmerald);
                break;
            case WeaponType.staff:
                CmdSpawnWeapon(WeaponType.staff);
                break;
        }
    }

    [Command]
    void CmdSetWeaponType(WeaponType wt)
    {
        weaponType = wt;
    }

    [Command]
    void CmdSpawnWeapon(WeaponType wt)
    {
        if (weaponScriptableObjects == null) return;

        if (weaponHolder.transform.childCount > 0)
        {
            NetworkServer.Destroy(weaponHolder.transform.GetChild(0).gameObject);
        }

        foreach (Weapon weaponScriptableObject in weaponScriptableObjects)
        {
            //if(weaponScriptableObject.weaponType == wt && weaponScriptableObject.allowed) 
            if (weaponScriptableObject.weaponType == wt)
            {
                animator.runtimeAnimatorController = weaponScriptableObject.weaponAnimOverride;
                GameObject instantiatedWeapon = Instantiate(weaponScriptableObject.weaponPrefab, weaponHolderTransform);

                NetworkServer.Spawn(instantiatedWeapon, connectionToClient);
                RpcSpawnWeapon(weaponScriptableObject.weaponType);
            }
        }
    }

    [ClientRpc]
    void RpcSpawnWeapon(WeaponType CmdWt)
    {
        foreach (Weapon weaponScriptableObject in weaponScriptableObjects)
        {
            if (weaponScriptableObject.weaponType == CmdWt)
            {
                animator.runtimeAnimatorController = weaponScriptableObject.weaponAnimOverride;
                GameObject instantiatedWeapon = Instantiate(weaponScriptableObject.weaponPrefab, weaponHolderTransform);
                instantiatedWeapon.transform.SetParent(weaponHolderTransform);
            }
        }
    }

    void DestroyWeaponOnServer()
    {
        if (!isLocalPlayer)
            return;

        CmdDestroyWeapon();
    }

    [Command]
    public void CmdDestroyWeapon()
    {
        if (weaponHolder.transform.childCount > 0)
        {
            NetworkServer.Destroy(weaponHolder.transform.GetChild(0).gameObject);
            RpcDestroyWeaponOnClient();
        }
    }

    [ClientRpc]
    void RpcDestroyWeaponOnClient()
    {
        if (weaponHolder.transform.childCount > 0)
        {
            Destroy(weaponHolder.transform.GetChild(0).gameObject);
        }
    }

}