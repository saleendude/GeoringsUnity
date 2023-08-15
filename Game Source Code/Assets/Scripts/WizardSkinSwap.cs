using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WizardSkinSwap : MonoBehaviour
{
    private Renderer _wizardRenderer; // Mesh renderer for character

    public GameObject wizardModel; // Character model

    public Texture2D[] wizardTextures;


    void Start()
    {
        _wizardRenderer = wizardModel.GetComponent<Renderer>();
    }

    public void SwapTexture1()
    {
        _wizardRenderer.material.SetTexture("_BaseMap", wizardTextures[0]);

        Debug.Log($"New wizard skin selected | Skin number : {0}");
    }
    public void SwapTexture2()
    {
        _wizardRenderer.material.SetTexture("_BaseMap", wizardTextures[1]);

        Debug.Log($"New wizard skin selected | Skin number : {1}");
    }
    public void SwapTexture3()
    {
        _wizardRenderer.material.SetTexture("_BaseMap", wizardTextures[2]);

        Debug.Log($"New wizard skin selected | Skin number : {2}");
    }
    public void SwapTexture4()
    {
        _wizardRenderer.material.SetTexture("_BaseMap", wizardTextures[3]);

        Debug.Log($"New wizard skin selected | Skin number : {3}");
    }
}
