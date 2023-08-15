using Mirror;
using UnityEngine;

public class NetworkConnectionStats : MonoBehaviour
{

    void OnEnable()
    {
        NetworkDiagnostics.InMessageEvent += MessageInEvent;
    }
    void OnDisable()
    {
        NetworkDiagnostics.InMessageEvent -= MessageInEvent;
    }

    private void MessageInEvent(NetworkDiagnostics.MessageInfo obj)
    {
        Debug.Log($"Message Bytes: {obj.bytes}");
        Debug.Log($"Message Channel: {obj.channel}");
        Debug.Log($"Message Count: {obj.count}");
        Debug.Log($"Message: {obj.message}");
    }
}
