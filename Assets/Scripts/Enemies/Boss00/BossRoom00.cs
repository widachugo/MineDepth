using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRoom00 : MonoBehaviour
{
    public GameObject boss;
    private BossBehaviour bossBehaviour;

    public bool roomActive;

    public float radiusMaxValue = 100;

    private void Start()
    {
        Shader.SetGlobalVector("Vector3_8fcac79a3a65402e8ebbe9dad556074e", transform.position);

        bossBehaviour = boss.GetComponent<BossBehaviour>();
        bossBehaviour.enabled = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            roomActive = true;
        }

        if (roomActive)
        {
            bossBehaviour.enabled = true;

            if (!bossBehaviour.regeneration)
            {
                bossBehaviour.OnLights();
            }
            bossBehaviour.isActivate = true;

            if (bossBehaviour.activationFinished)
            {
                Shader.SetGlobalFloat("Vector1_309aa2b5690441338787b55d6c51aeda", radiusMaxValue);
            }
        }
        else
        {
            Shader.SetGlobalFloat("Vector1_309aa2b5690441338787b55d6c51aeda", 0);
        }
    }
}
