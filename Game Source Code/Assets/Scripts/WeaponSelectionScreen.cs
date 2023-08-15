using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using UnityEngine.Events;

public class WeaponSelectionScreen : MonoBehaviour
{
    [SerializeField] private Canvas weaponSelectionCanvas;
    [SerializeField] private Button[] weaponButtons;

    GameObject networkPlayerObject = null;

    [DllImport("__Internal")]
    private static extern void CheckWalletNFTs(string walletAddress);

    public static UnityEvent OnWeaponButtonDisabled = new UnityEvent();

    void OnEnable()
    {
        NetworkChangeEquipment3.OnLocalPlayerStart.AddListener(SetPlayerGameObject);
    }

    void OnDisable()
    {
        NetworkChangeEquipment3.OnLocalPlayerStart.RemoveListener(SetPlayerGameObject);
    }

    void SetPlayerGameObject()
    {
        networkPlayerObject = NetworkClient.localPlayer.gameObject;
    }

    void Start()
    {
        // disable all buttons initially
        foreach (Button weaponButton in weaponButtons)
        {
            weaponButton.interactable = false;
        }

        // send out event to react to check for available weapons on start, outside Editor
#if UNITY_EDITOR == false
        CheckWalletNFTs(PlayerMain.connectedWalletAddress);
#endif

#if UNITY_EDITOR == true
        EnableWeapon("swordWooden||swordRuby||swordSteel||swordEmerald||scythe||staff||dagger||hammer||axe");
#endif
    }

    public void ClickRefreshButton()
    {
        CheckWalletNFTs(PlayerMain.connectedWalletAddress);

    }

    public void EnableWeapon(string weaponNames)
    {
        string[] weaponsArray = weaponNames.Split("||");
        foreach (Button weaponButton in weaponButtons)
        {
            if (weaponsArray.Contains(weaponButton.name))
                weaponButton.interactable = true;
            else
            {
                weaponButton.interactable = false;
                OnWeaponButtonDisabled.Invoke();
            }
        }

    }

    public void ClickBackButton()
    {
        weaponSelectionCanvas.gameObject.SetActive(false);
    }

    #region Select Buttons

    public void SelectSwordWooden()
    {
        if (networkPlayerObject != null)
            networkPlayerObject.GetComponent<NetworkChangeEquipment3>().EnableWoodenSwordButton();
    }

    public void SelectSwordSteel()
    {
        if (networkPlayerObject != null)
            networkPlayerObject.GetComponent<NetworkChangeEquipment3>().EnableSteelSwordButton();
    }

    public void SelectSwordRuby()
    {
        if (networkPlayerObject != null)
            networkPlayerObject.GetComponent<NetworkChangeEquipment3>().EnableRubySwordButton();
    }

    public void SelectSwordEmerald()
    {
        if (networkPlayerObject != null)
            networkPlayerObject.GetComponent<NetworkChangeEquipment3>().EnableEmeraldSwordButton();
    }

    public void SelectScythe()
    {
        if (networkPlayerObject != null)
            networkPlayerObject.GetComponent<NetworkChangeEquipment3>().EnableScytheButton();
    }

    public void SelectStaff()
    {
        if (networkPlayerObject != null)
            networkPlayerObject.GetComponent<NetworkChangeEquipment3>().EnableStaffButton();
    }

    public void SelectAxe()
    {
        if (networkPlayerObject != null)
            networkPlayerObject.GetComponent<NetworkChangeEquipment3>().EnableAxeButton();
    }

    public void SelectDagger()
    {
        if (networkPlayerObject != null)
            networkPlayerObject.GetComponent<NetworkChangeEquipment3>().EnableDaggerButton();
    }

    public void SelectHammer()
    {
        if (networkPlayerObject != null)
            networkPlayerObject.GetComponent<NetworkChangeEquipment3>().EnableHammerButton();
    }
    #endregion
}
