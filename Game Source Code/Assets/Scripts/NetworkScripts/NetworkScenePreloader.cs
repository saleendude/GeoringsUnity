using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkScenePreloader : MonoBehaviour
{
    string[] args = System.Environment.GetCommandLineArgs();

    private void Awake()
    {
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == "-launch-as-client")
                OnClientClick();
            if (args[i] == "-launch-as-server")
                OnServerClick();
            if (args[i] == "-launch-as-host")
                OnHostClick();
        }

    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("OnSceneLoaded: " + scene.name);
        Debug.Log(mode);
        NetworkManager.singleton.StartServer();
    }

    public void OnClientClick()
    {
        Debug.Log("Starting as client!");
        NetworkManager.singleton.StartClient();
        SceneManager.LoadScene("MultiplayerTesting");
    }
    public void OnServerClick()
    {
        Debug.Log("Starting as Server!");
        NetworkManager.singleton.StartServer();
        SceneManager.LoadScene("MultiplayerTesting");
    }
    public void OnHostClick()
    {
        Debug.Log("Starting as Host!");
        NetworkManager.singleton.StartHost();
        SceneManager.LoadScene("MultiplayerTesting");
    }
}
