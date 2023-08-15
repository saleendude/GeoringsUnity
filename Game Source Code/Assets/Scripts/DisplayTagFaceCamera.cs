using UnityEngine;
using UnityEngine.UI;

public class DisplayTagFaceCamera : MonoBehaviour
{
    private Camera cam;
    [SerializeField] private Text playerDisplayTagText;


    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        transform.rotation = Quaternion.LookRotation(transform.position - cam.transform.position);
    }

    public void SetDisplayName(string displayName)
    {
        playerDisplayTagText.text = displayName;
    }
}
