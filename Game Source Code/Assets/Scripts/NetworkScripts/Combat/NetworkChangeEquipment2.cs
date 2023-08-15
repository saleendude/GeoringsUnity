using Mirror;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum WeaponType : byte
{
    nothing,
    axe,
    staff,
    dagger,
    hammer,
    scythe,
    swordRuby,
    swordSteel,
    swordWooden,
    swordEmerald
}


public class NetworkChangeEquipment2 : NetworkBehaviour
{
    [SerializeField] Weapon[] weapons = null;
    [SerializeField] Transform handTransform = null;
    [SerializeField] GameObject weaponHolder = null;
    [SerializeField] Animator animator = null;

    [SyncVar(hook = nameof(OnChangeWeapon))]
    public WeaponType weaponType;

    [SyncVar(hook = nameof(OnChangeWeaponIndex))]
    public int weaponIndex = 0;

    [SyncVar]
    public uint enabledAssetId = 0;

    void OnChangeWeapon(WeaponType oldWeapon, WeaponType newWeapon)
    {
        StartCoroutine(ChangeWeapon(newWeapon));
    }

    void OnChangeWeaponIndex(int oldWeaponIndex, int newWeaponIndex)
    {
        if (!isLocalPlayer) return;
        switch (newWeaponIndex)
        {
            case 0:
                CmdUpdateWeaponType(WeaponType.nothing);
                break;
            case 1:
                CmdUpdateWeaponType(WeaponType.swordWooden);
                break;
            case 2:
                CmdUpdateWeaponType(WeaponType.axe);
                break;
            case 3:
                CmdUpdateWeaponType(WeaponType.dagger);
                break;
            case 4:
                CmdUpdateWeaponType(WeaponType.swordSteel);
                break;
            case 5:
                CmdUpdateWeaponType(WeaponType.swordRuby);
                break;
            case 6:
                CmdUpdateWeaponType(WeaponType.swordEmerald);
                break;
            case 7:
                CmdUpdateWeaponType(WeaponType.scythe);
                break;
            case 8:
                CmdUpdateWeaponType(WeaponType.staff);
                break;
            case 9:
                CmdUpdateWeaponType(WeaponType.hammer);
                break;
        }
    }

    [Command]
    void CmdUpdateWeaponType(WeaponType updatedWeaponType)
    {
        weaponType = updatedWeaponType;
    }

    IEnumerator ChangeWeapon(WeaponType newWeapon)
    {
        while (weaponHolder.transform.childCount > 0)
        {
            Destroy(weaponHolder.transform.GetChild(0).gameObject);
            yield return null;
        }

        switch (newWeapon)
        {
            case WeaponType.axe:
                SpawnWeapon();
                break;
            case WeaponType.dagger:
                SpawnWeapon();
                break;
            case WeaponType.hammer:
                SpawnWeapon();
                break;
            case WeaponType.scythe:
                SpawnWeapon();
                break;
            case WeaponType.swordSteel:
                SpawnWeapon();
                break;
            case WeaponType.swordWooden:
                SpawnWeapon();
                break;
            case WeaponType.swordRuby:
                SpawnWeapon();
                break;
            case WeaponType.swordEmerald:
                SpawnWeapon();
                break;
            case WeaponType.staff:
                SpawnWeapon();
                break;
        }
    }

    void Start()
    {
        if (!isLocalPlayer) return;
        CmdInitializeAvailableWeapons();
    }

    void Update()
    {
        if (!isLocalPlayer) return;

        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            CmdChangeWeaponIndexUp();
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            CmdChangeWeaponIndexDown();
        }

    }

    [Command]
    void CmdChangeWeaponIndexUp()
    {
        if (weaponIndex >= weapons.Length - 1)
            weaponIndex = 0;
        else
            weaponIndex++;
    }

    [Command]
    void CmdChangeWeaponIndexDown()
    {
        if (weaponIndex <= 0)
            weaponIndex = weapons.Length - 1;
        else
            weaponIndex--;
    }

    [Command]
    void CmdChangeEquippedWeapon(WeaponType selectedWeapon)
    {
        weaponType = selectedWeapon;
    }

    [Command]
    void CmdInitializeAvailableWeapons()
    {
        RpcInitializeAvaiableWeapons();
    }

    [ClientRpc]
    void RpcInitializeAvaiableWeapons()
    {
        foreach (Weapon weapon in weapons) // reset all so's allowed bool to false
        {
            weapon.allowed = false;
        }

        Debug.Log("=== Weapons initialized ===");
    }


    private void SpawnWeapon()
    {
        if (weapons == null) return;

        foreach (Weapon weapon in weapons)
        {
            if (weapon.weaponType == weaponType)
            {
                CmdSpawnWeapon(weapon.weaponType);
            }
        }
    }

    [Command]
    private void CmdSpawnWeapon(WeaponType wt)
    {

        if (weaponHolder.transform.childCount > 0)
        {
            Debug.Log($"Old weapon found on server: {weaponHolder.transform.GetChild(0).gameObject.name}. Destroying..");
            NetworkServer.Destroy(weaponHolder.transform.GetChild(0).gameObject);
        }
        Debug.Log($"wt from SpawnWeapon: {wt}");

        foreach (Weapon weapon in weapons)
        {
            Debug.Log($"{weapon.weaponType}: {weapon.allowed}");
            if (weapon.weaponType == wt && weapon.allowed)
            {
                Debug.Log($"Weapon From CmdSpawnWeapon: {weapon.weaponType}");
                animator.runtimeAnimatorController = weapon.weaponAnimOverride;
                AnimatorOverrideController weaponAnimOverride = weapon.weaponAnimOverride;
                GameObject instantiatedWeapon = Instantiate(weapon.weaponPrefab, handTransform);

                NetworkServer.Spawn(instantiatedWeapon, connectionToClient);
                RpcSpawnWeapon(weapon.weaponType);
            }
        }
    }

    [ClientRpc]
    void RpcSpawnWeapon(WeaponType wt)
    {
        Debug.Log(wt);
        Debug.Log("Spawn Weapon called from RpcSpawnWeapon!");

        foreach (Weapon weapon in weapons)
        {
            if (weapon.weaponType == wt && weapon.allowed)
            {
                animator.runtimeAnimatorController = weapon.weaponAnimOverride;
                GameObject instantiatedWeapon = Instantiate(weapon.weaponPrefab, handTransform);
                instantiatedWeapon.transform.SetParent(handTransform);
            }
        }
    }

    [Command]
    void CmdDestroyOldWeapon(GameObject obj)
    {
        if (!obj) return;

        NetworkServer.Destroy(obj);
    }

    IEnumerator DestroyOldWeapon(GameObject obj)
    {
        CmdDestroyOldWeapon(obj);
        yield return null;
    }
}
