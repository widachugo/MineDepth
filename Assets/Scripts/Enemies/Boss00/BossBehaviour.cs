using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBehaviour : MonoBehaviour
{
    public float health;

    private Animator anim;
    private GameObject player;
    private CharacterController controller;
    private LineRenderer[] linesRenderer;

    private float playerDist;

    public bool regeneration;
    public float regenerationTime;
    private float tregenerationTime;
    private float smoothRadius;


    public bool attack;
    //Var attack distance
    private float randomTimeAttack;
    private float tAttack;
    private bool attackOneTime;
    private int randomAttackAnim;
    private RaycastHit hitPlayer;
    private bool canImpulse;
    public LayerMask layerAttackDistance;
    //Var attack Cac
    private float cooldown = 3f;
    private float tCooldown;
    private bool attackCacOneTime;
    private int randomAttackCacAnim;

    [HideInInspector] public bool isActivate; //Manage statut of BossMovement script
    [HideInInspector] public bool activationFinished; //Manage statut of BossRoom00 script

    public GameObject lineRendererRegen;
    public GameObject regenParticleObject;
    private GameObject GOregenParticle;
    private bool regenRadiusBegin;
    private Vector4 vectorShader;
    private Light[] lightsObj;

    public GameObject[] enemiesInThisRoom;

    private void Start()
    {
        anim = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();

        lightsObj = GetComponentsInChildren<Light>();
        OffLights();

        player = GameObject.FindGameObjectWithTag("Player");

        randomTimeAttack = Random.Range(5, 15);

        linesRenderer = new LineRenderer[enemiesInThisRoom.Length];
        for (int i = 0; i < enemiesInThisRoom.Length; i++)
        {
            GameObject lineRenderer = Instantiate(lineRendererRegen, transform.position, transform.rotation);
            lineRenderer.gameObject.transform.parent = transform;
            linesRenderer[i] = lineRenderer.GetComponent<LineRenderer>();

            linesRenderer[i].enabled = false;
        }
    }

    private void Update()
    {
        playerDist = Vector3.Distance(transform.position, player.transform.position);

        //Regeneration
        if (regeneration)
        {
            anim.SetBool("Regen", true);

            tregenerationTime += Time.deltaTime;

            if (tregenerationTime >= regenerationTime)
            {
                anim.SetBool("Regen", false);
                tregenerationTime = 0f;
                regeneration = false;
            }

            for (int i = 0; i < enemiesInThisRoom.Length; i++)
            {
                Enemy01_Light enemy01_Light = enemiesInThisRoom[i].GetComponent<Enemy01_Light>();
                enemy01_Light.regenBoss = true;
                linesRenderer[i].enabled = true;
                linesRenderer[i].SetPosition(1, enemiesInThisRoom[i].transform.position);

                if (enemiesInThisRoom[i].GetComponentInChildren<EnemiesHealth>().currentHealth <= 0)
                {
                    linesRenderer[i].enabled = false;
                }
            }
        }
        //Attacks
        else if (!regeneration && !attack)
        {
            //Attack distance
            if (playerDist >= 15)
            {
                tCooldown = 0f;

                tAttack += Time.deltaTime;

                if (tAttack >= randomTimeAttack)
                {
                    randomAttackAnim = Random.Range(2, 4);
                    attackOneTime = false;
                    tAttack = 0f;
                    AttackDistance();
                }
            }
            //Attack cac
            else
            {
                tCooldown += Time.deltaTime;

                if (tCooldown >= cooldown)
                {
                    randomAttackCacAnim = 1;
                    attackCacOneTime = false;
                    tCooldown = 0f;
                    AttackCac();
                }
            }
        }

        if (!regeneration)
        {
            for (int i = 0; i < enemiesInThisRoom.Length; i++)
            {
                linesRenderer[i].enabled = false;
            }
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
        float radiusDist = Mathf.SmoothStep(0, 16, smoothRadius);
        if (GOregenParticle != null)
        {
            GOregenParticle.transform.localScale = new Vector3(Mathf.SmoothStep(0, 1.1f, smoothRadius), Mathf.SmoothStep(0, 1.1f, smoothRadius), 1);
        }

        for (int i = 0; i < enemiesInThisRoom.Length; i++)
        {
            Vector3 enemyDir = enemiesInThisRoom[i].transform.position + transform.position;
            Vector3 newPos = transform.position + enemyDir.normalized * radiusDist;
            linesRenderer[i].SetPosition(0, newPos);
        }

        var ray = new Vector3(player.transform.position.x, player.transform.position.y + 2, player.transform.position.z) 
                  - new Vector3(transform.position.x, transform.position.y + 10, transform.position.z);
        if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y + 10, transform.position.z), ray, out hitPlayer, layerAttackDistance))
        {
            if (hitPlayer.transform.gameObject == player)
            {
                canImpulse = true;
            }
            else
            {
                canImpulse = false;
            }
        }
    }

    public void ActivationFinished()
    {
        activationFinished = true;
    }

    //Call with StandToCrouch event anim
    public void RegenBegin()
    {
        GOregenParticle = Instantiate(regenParticleObject);
        GOregenParticle.transform.position = transform.position;

        OffLights();

        Shader.SetGlobalFloat("Vector1_664a6bc72abd4173a3abd9db52284ab1", 0);

        vectorShader = new Vector4(transform.position.x, transform.position.y, transform.position.z, 0);
        Shader.SetGlobalColor("Color_722d554cf61f4f3bb45bba66ca824ba7", vectorShader);

        regenRadiusBegin = true;

        for (int i = 0; i < enemiesInThisRoom.Length; i++)
        {
            Enemy01_Light enemy01_Light = enemiesInThisRoom[i].GetComponent<Enemy01_Light>();
            EnemiesHealth enemiesHealth = enemiesInThisRoom[i].GetComponentInChildren<EnemiesHealth>();
            enemiesHealth.currentHealth = enemy01_Light.health;
        }
    }

    //Call with CrouchToStand event anim
    public void RegenEnding()
    {
        regenRadiusBegin = false;
        OnLights();
    }

    public void OnLights()
    {
        for (int i = 0; i < lightsObj.Length; i++)
        {
            lightsObj[i].gameObject.SetActive(true);
        }
    }

    public void OffLights()
    {
        for (int i = 0; i < lightsObj.Length; i++)
        {
            lightsObj[i].gameObject.SetActive(false);
        }
    }

    public void AttackFinished()
    {
        attack = false;
        anim.SetBool("Attack", false);
    }

    public void AttackImpulsion()
    {
        Vector3 playerDir = player.transform.position - transform.position;

        StartCoroutine(ImpulsionCoroutine(playerDir));
    }

    private void AttackDistance()
    {
        if (!attackOneTime && canImpulse)
        {
            attack = true;

            anim.SetBool("Attack", true);
            anim.SetInteger("AttackID", randomAttackAnim);

            randomTimeAttack = Random.Range(5, 15);

            attackOneTime = true;
        }
    }

    private void AttackCac()
    {
        if (!attackCacOneTime)
        {
            attack = true;

            anim.SetBool("Attack", true);
            anim.SetInteger("AttackID", randomAttackCacAnim);

            attackCacOneTime = true;
        }
    }

    public IEnumerator ImpulsionCoroutine(Vector3 dir)
    {
        controller.Move(dir * 100 * Time.deltaTime);

        yield return new WaitForSeconds(10f);
    }
}
