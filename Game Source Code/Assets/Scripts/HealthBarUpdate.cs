using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUpdate : MonoBehaviour
{
    // healthbar variables
    [SerializeField] private Image healthBarSprite;
    [SerializeField] private float healthReductionDisplaySpeed = 1;
    private float target;
    private Camera cam;

    public void UpdateHealthBar(float maxHealth, float currentHealth)
    {
        target = currentHealth / maxHealth;
    }

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        transform.rotation = Quaternion.LookRotation(transform.position - cam.transform.position);
        healthBarSprite.fillAmount = Mathf.MoveTowards(healthBarSprite.fillAmount, target, healthReductionDisplaySpeed * Time.deltaTime);
    }
}
