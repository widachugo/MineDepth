using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBehaviour : MonoBehaviour
{
    private Animator anim;

    public bool regeneration;
    public float regenerationTime;
    private float smoothRadius;

    public GameObject regenParticleObject;

    private Vector4 vectorShader;

    public bool regenRadiusBegin;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (regeneration)
        {
            StartCoroutine(RegenCoroutine());
        }

        if (regenRadiusBegin)
        {
            smoothRadius += 0.4f * Time.deltaTime;
        }
        else
        {
            smoothRadius -= 0.6f * Time.deltaTime;
        }

        smoothRadius = Mathf.Clamp(smoothRadius, 0, 1);

        Shader.SetGlobalFloat("Vector1_664a6bc72abd4173a3abd9db52284ab1", Mathf.SmoothStep(0, 16, smoothRadius));
    }

    public void RegenBegin()
    {
        regenParticleObject.SetActive(true);

        Shader.SetGlobalFloat("Vector1_664a6bc72abd4173a3abd9db52284ab1", 0);

        vectorShader = new Vector4(transform.position.x, transform.position.y, transform.position.z, 0);
        Shader.SetGlobalColor("Color_722d554cf61f4f3bb45bba66ca824ba7", vectorShader);

        regenRadiusBegin = true;
    }

    public void RegenEnding()
    {
        regenRadiusBegin = false;
    }

    public void RegenIsFinished()
    {
        regeneration = false;
    }

    public IEnumerator RegenCoroutine()
    {
        anim.SetBool("Regen", true);

        yield return new WaitForSeconds(regenerationTime);

        anim.SetBool("Regen", false);
    }
}
