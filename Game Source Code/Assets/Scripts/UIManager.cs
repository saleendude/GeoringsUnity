using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Canvas weaponSelectionOfflineCanvas;
    public static bool uiTurnedOn = true;

    void Start()
    {
        weaponSelectionOfflineCanvas.gameObject.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (weaponSelectionOfflineCanvas.gameObject.activeSelf)
            {
                Debug.Log("Inventory screen open. Closing..");
                weaponSelectionOfflineCanvas.gameObject.SetActive(false);
                uiTurnedOn = false;
            }
            else
            {
                Debug.Log("Inventory screen closed. Opening..");
                weaponSelectionOfflineCanvas.gameObject.SetActive(true);
                uiTurnedOn = true;
            }
        }

        if (!weaponSelectionOfflineCanvas.gameObject.activeSelf)
        {
            uiTurnedOn = false;
        }
    }
}
