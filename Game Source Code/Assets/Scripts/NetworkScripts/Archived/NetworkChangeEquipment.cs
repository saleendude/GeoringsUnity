using Mirror;
using UnityEngine;
using System.Collections;

public enum WeaponTypes : byte
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

public class NetworkChangeEquipment : NetworkBehaviour
{
    [SerializeField] Weapon weapon = null;
    [SerializeField] Transform handTransform = null;
    [SerializeField] GameObject weaponHolder = null;
    [SerializeField] Animator animator = null;

    [SyncVar(hook = nameof(OnChangeWeapon))]
    public WeaponTypes weaponType;

    void OnChangeWeapon(WeaponTypes oldWeapon, WeaponTypes newWeapon)
    {
        StartCoroutine(ChangeWeapon(newWeapon));
    }

    IEnumerator ChangeWeapon(WeaponTypes newWeapon)
    {
        while (weaponHolder.transform.childCount > 0)
        {
            Destroy(weaponHolder.transform.GetChild(0).gameObject);
            yield return null;
        }

        switch (newWeapon)
        {
            case WeaponTypes.axe:
                SpawnWeapon();
                break;
        }
    }

    void Update()
    {
        if (!isLocalPlayer) return;

        if (Input.GetKeyDown(KeyCode.Z) && weaponType != WeaponTypes.nothing)
            CmdChangeEquippedWeapon(WeaponTypes.nothing);

        if (Input.GetKeyDown(KeyCode.X) && weaponType != WeaponTypes.axe)
            CmdChangeEquippedWeapon(WeaponTypes.axe);

    }

    [Command]
    void CmdChangeEquippedWeapon(WeaponTypes selectedWeapon)
    {
        weaponType = selectedWeapon;
    }

    private void SpawnWeapon()
    {
        if (weapon == null) return;

        {
            AnimatorOverrideController weaponAnimOverride = weapon.weaponAnimOverride;
            weapon.Spawn(handTransform, animator);
        }
    }
}
