using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemiesHealth : MonoBehaviour
{
    public float maxHealth;
    public float currentHealth;

    public float maxShield;
    public float currentShield;

    public Image healthBar;
    public Image shieldBar;

    public GameObject shieldParent;

    private GameObject cam;

    private void Start()
    {
        cam = GameObject.FindGameObjectWithTag("MainCamera");

        currentHealth = maxHealth;
        currentShield = maxShield;
    }

    private void Update()
    {
        float healthBarInvLerp = Mathf.InverseLerp(0, maxHealth, currentHealth);
        healthBar.fillAmount = Mathf.Lerp(0, 1, healthBarInvLerp);

        float shieldBarInvLerp = Mathf.InverseLerp(0, maxShield, currentShield);
        shieldBar.fillAmount = Mathf.Lerp(0, 1, shieldBarInvLerp);

        transform.LookAt(cam.transform.position);

        if (currentShield <= 0)
        {
            shieldParent.SetActive(false);
        }
        else
        {
            shieldParent.SetActive(true);
        }
    }

    public void TakeDamage(float amount)
    {
        if (currentShield >= 1)
        {
            currentShield -= amount;
        }
        else
        {
            currentHealth -= amount;
        }
    }
}
