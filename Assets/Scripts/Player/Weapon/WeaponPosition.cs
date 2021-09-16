using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPosition : MonoBehaviour
{
    public bool weaponActive = true;
    public GameObject weaponGlobal;

    public GameObject ankleLeftHand;
    public GameObject ankleRightHand;

    private GameObject playerTargetLeftHand;
    private GameObject playerTargetRightHand;

    public DitzelGames.FastIK.FastIKFabric playerLeftHand;
    public DitzelGames.FastIK.FastIKFabric playerRightHand;

    private void Start()
    {
        playerTargetLeftHand = GameObject.FindGameObjectWithTag("TargetArmL");
        playerTargetRightHand = GameObject.FindGameObjectWithTag("TargetArmR");
    }

    private void Update()
    {
        playerTargetLeftHand.transform.position = ankleLeftHand.transform.position;
        playerTargetRightHand.transform.position = ankleRightHand.transform.position;

        if (weaponActive)
        {
            weaponGlobal.SetActive(true);
            playerLeftHand.enabled = true;
            playerRightHand.enabled = true;
        }
        else
        {
            weaponGlobal.SetActive(false);
            playerLeftHand.enabled = false;
            playerRightHand.enabled = false;
        }
    }
}
