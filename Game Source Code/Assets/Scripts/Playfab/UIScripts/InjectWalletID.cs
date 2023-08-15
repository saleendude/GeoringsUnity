using UnityEngine;
using UnityEngine.UI;

public class InjectWalletID : MonoBehaviour
{
    public Text _walletAddress;

    public void SetWalletAddress(string address)
    {
        PlayerMain.connectedWalletAddress = address;
        _walletAddress.text = address.Substring(24).ToLower();
    }
}