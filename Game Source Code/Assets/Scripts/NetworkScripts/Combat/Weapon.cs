using UnityEngine;

[CreateAssetMenu(fileName ="Weapon", menuName = "Weapon/New Weapon", order = 0)]
public class Weapon : ScriptableObject
{
    public AnimatorOverrideController weaponAnimOverride = null;
    public GameObject weaponPrefab = null;
    public float weaponWeight = 0;
    public float weaponDamage = 0;
    public WeaponType weaponType;
    public bool allowed = false;

    public void Spawn(Transform handTransform, Animator animator)
    {
        Instantiate(weaponPrefab, handTransform);
        Debug.Log($"Weapon spawned: {weaponPrefab.name}");
        animator.runtimeAnimatorController = weaponAnimOverride;
    }
}
