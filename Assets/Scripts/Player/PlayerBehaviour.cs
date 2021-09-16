using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerBehaviour : MonoBehaviour
{
    public float maxHealth;
    public float curHealth;
    public Image barHealth;

    private Transform respawnPoint;

    public GameObject deathPanelUI;
    public GameObject bodyAlive;
    public GameObject bodyDead;
    private GameObject bodyDeadGO;
    private bool OneTime;

    private CharacterController characterController;
    private PlayerMovement playerMovement;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        barHealth.fillAmount = Mathf.InverseLerp(0, maxHealth, curHealth);

        if (curHealth <= 0)
        {
            Death();
        }
    }

    private void Death()
    {
        deathPanelUI.SetActive(true);
        bodyAlive.SetActive(false);

        characterController.enabled = false;
        playerMovement.enabled = false;

        if (!OneTime)
        {
            bodyDeadGO = Instantiate(bodyDead, transform.position, transform.rotation);
            bodyDeadGO.transform.parent = transform;
            OneTime = true;
        }
    }

    public void Respawn()
    {
        transform.position = respawnPoint.position;
        curHealth = maxHealth;

        bodyAlive.SetActive(true);
        Destroy(bodyDeadGO);

        characterController.enabled = true;
        playerMovement.enabled = true;

        OneTime = false;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Respawn")
        {
            respawnPoint = other.gameObject.transform;
        }
    }

    public void GetDamage(int amount)
    {
        curHealth -= amount;
    }
}
