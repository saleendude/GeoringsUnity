using UnityEngine;

public class PlayerMain_Solo : MonoBehaviour
{

    // character selection variables
    public static int characterIndex;
    public static string characterType;

    // weapon variables
    public GameObject[] allWeapons;
    public int weaponChoice = 0;
    public bool weaponChange = true;

    // character specs variables
    public float healthAmt;


    void Start()
    {
        for (int i = 0; i < allWeapons.Length; i++) // disabling all weapons at start of the game.
        {
            allWeapons[i].SetActive(false);
        }
    }

    void Update()
    {
        if (weaponChange)
        {
            weaponChange = false;
            for (int i = 0; i < allWeapons.Length; i++) // disabling all weapons to prevent double carry.
            {
                allWeapons[i].SetActive(false);
            }

            allWeapons[weaponChoice].SetActive(true);
        }
    }
}
