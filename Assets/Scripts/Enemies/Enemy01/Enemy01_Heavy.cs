using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.HighDefinition;

public class Enemy01_Heavy : MonoBehaviour
{
    [Header("Characteristics")]
    public float health;
    public float shield;
    public GameObject healthEnemiesGo;
    private EnemiesHealth enemiesHealth;

    private bool dead;
    private bool firstDeath;

    private Renderer bodyRend;
    private float emissiveWeightSmooth;
    private Color colorBlend;

    private DitzelGames.FastIK.FastIKFabric[] fastIKFabrics;
    private Transform[] targets;
    private Vector3[] offsetsTarget;

    private Rigidbody[] rbChildren;
    private PartPosition[] partPositions;

    private NavMeshAgent navMeshAgent;

    public GameObject body;
    public GameObject bodyParent;

    [Header("Attack")]
    public GameObject enemyTarget;

    private bool attack;
    public float loadingAttack;
    private float loadingAttackT;
    public float durationAttack;
    private float durationAttackT;
    public float cooldownAttack;
    private float cooldownAttackT;

    public GameObject lightAoe;
    public GameObject lightAoeGo;
    public GameObject particleAoe;
    public GameObject particleAoeGo;
    private bool spawnOnce;
    private bool spawnOnce2;

    private Vector3[] lastPositionLegs;

    private Vector3 dirTarget;
    private float distanceTarget;

    [Header("Sound")]
    private AudioSource audioSource;
    public AudioClip shockwave;
    public AudioClip loading;
    private bool loadingOnce;
    public AudioClip[] steps;
    private bool[] stepsOnce;

    private void Start()
    {
        firstDeath = true;

        bodyRend = body.GetComponent<Renderer>();
        colorBlend = bodyRend.material.GetColor("_EmissiveColor");

        rbChildren = gameObject.GetComponentsInChildren<Rigidbody>();
        partPositions = new PartPosition[rbChildren.Length];

        for (int i = 0; i < rbChildren.Length; i++)
        {
            partPositions[i] = rbChildren[i].gameObject.AddComponent<PartPosition>();
            partPositions[i].UpdateLastPosition();
        }

        fastIKFabrics = gameObject.GetComponentsInChildren<DitzelGames.FastIK.FastIKFabric>();
        targets = new Transform[fastIKFabrics.Length];
        offsetsTarget = new Vector3[targets.Length];
        stepsOnce = new bool[targets.Length];
        lastPositionLegs = new Vector3[targets.Length];

        for (int i = 0; i < targets.Length; i++)
        {
            targets[i] = fastIKFabrics[i].Target;
            targets[i].gameObject.AddComponent<SphereCollider>();
            targets[i].gameObject.AddComponent<Rigidbody>().freezeRotation = true;

            targets[i].gameObject.AddComponent<AudioSource>();
            AudioSource audiosourceTarget = targets[i].GetComponent<AudioSource>();
            audiosourceTarget.playOnAwake = false;
            audiosourceTarget.volume = 0.5f;
            audiosourceTarget.pitch = 0.2f;
            audiosourceTarget.spatialBlend = 1.0f;
            audiosourceTarget.rolloffMode = AudioRolloffMode.Linear;
            audiosourceTarget.minDistance = 5;
            audiosourceTarget.maxDistance = 100;

            offsetsTarget[i] = targets[i].transform.position - transform.position;

            lastPositionLegs[i] = targets[i].transform.position;
        }

        navMeshAgent = gameObject.GetComponent<NavMeshAgent>();

        GameObject healthbar = Instantiate(healthEnemiesGo);
        healthbar.transform.parent = transform;
        enemiesHealth = healthbar.GetComponent<EnemiesHealth>();
        enemiesHealth.maxHealth = health;
        enemiesHealth.maxShield = shield;

        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (enemiesHealth.currentHealth <= 0)
        {
            dead = true;
        }
        else
        {
            dead = false;
        }

        //Behaviours
        if (dead)
        {
            Death();
        }
        else
        {
            Alive();
            Attack();
            Movements();
        }

        //Visual
        if (dead)
        {
            emissiveWeightSmooth = Mathf.Lerp(0, 1, Time.time);
            bodyRend.material.SetFloat("_EmissiveExposureWeight", emissiveWeightSmooth);
        }
        else
        {
            if (attack)
            {
                emissiveWeightSmooth = Mathf.Lerp(0.99f, 0f, loadingAttackT);
            }
            else
            {
                emissiveWeightSmooth = Mathf.Lerp(0.99f, 0f, Time.deltaTime);
            }

            bodyRend.material.SetFloat("_EmissiveExposureWeight", emissiveWeightSmooth);
            bodyRend.material.SetColor("_EmissiveColor", colorBlend);
        }

        enemiesHealth.gameObject.transform.position = new Vector3(transform.position.x, transform.position.y + 3, transform.position.z);
        enemiesHealth.gameObject.transform.localScale = transform.localScale / 3;
    }

    private void Movements()
    {
        dirTarget = transform.position - enemyTarget.transform.position;
        distanceTarget = dirTarget.magnitude;

        //Legs positions
        for (int i = 0; i < targets.Length; i++)
        {
            targets[i] = fastIKFabrics[i].Target;

            float newPosX = body.transform.position.x + offsetsTarget[i].x;
            float newPosZ = body.transform.position.z + offsetsTarget[i].z;
            newPosX = Mathf.Round(newPosX * 0.5f) / 0.5f;
            newPosZ = Mathf.Round(newPosZ * 0.5f) / 0.5f;

            targets[i].transform.position = new Vector3(newPosX, targets[i].transform.position.y, newPosZ);

            if (targets[i].transform.position != lastPositionLegs[i])
            {
                targets[i].GetComponent<AudioSource>().clip = steps[Random.Range(0, steps.Length)];
                targets[i].GetComponent<AudioSource>().Play();
            }

            lastPositionLegs[i] = targets[i].transform.position;
        }

        //AI Target
        var travelPartsFinish = CheckPositionsParts();

        if (travelPartsFinish)
        {
            Vector2 newPoint;
            newPoint = Random.insideUnitCircle * 5;

            if (!attack)
            {
                navMeshAgent.destination = new Vector3(enemyTarget.transform.position.x + newPoint.x, transform.position.y, enemyTarget.transform.position.z + newPoint.y);
            }
            else
            {
                navMeshAgent.destination = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            }

            if (!attack)
            {
                Vector3 lookAtPos = new Vector3(enemyTarget.transform.position.x, enemyTarget.transform.position.y, enemyTarget.transform.position.z);
                bodyParent.transform.LookAt(lookAtPos);
            }
        }
    }

    private void Attack()
    {
        cooldownAttackT += Time.deltaTime;

        if (loadingAttackT <= 0)
        {
            navMeshAgent.baseOffset = Mathf.Lerp(navMeshAgent.baseOffset, 3, Time.deltaTime * 10);
        }

        //Attack
        if (cooldownAttackT >= cooldownAttack)
        {
            attack = true;

            //Loading Attack
            if (!loadingOnce)
            {
                audioSource.clip = loading;
                audioSource.Play();

                loadingOnce = true;
            }

            loadingAttackT += Time.deltaTime;

            navMeshAgent.baseOffset = Mathf.Lerp(3, 4f, loadingAttackT);

            if (!spawnOnce)
            {
                GameObject goLight = Instantiate(lightAoe);
                goLight.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
                lightAoeGo = goLight;
                spawnOnce = true;
            }

            lightAoeGo.GetComponent<HDAdditionalLightData>().SetSpotAngle(Mathf.Lerp(0, 130, loadingAttackT));
            lightAoeGo.GetComponent<HDAdditionalLightData>().innerSpotPercent = 100;

            if (loadingAttackT >= loadingAttack)
            {
                //Duration Attack
                audioSource.clip = shockwave;
                audioSource.Play();

                durationAttackT += Time.deltaTime;

                navMeshAgent.baseOffset = Mathf.Lerp(navMeshAgent.baseOffset, 1, durationAttackT * 15);

                if (!spawnOnce2)
                {
                    GameObject goParticle = Instantiate(particleAoe);
                    goParticle.transform.position = new Vector3(transform.position.x, transform.position.y - 5.5f, transform.position.z);
                    particleAoeGo = goParticle;

                    spawnOnce2 = true;
                }

                if (durationAttackT >= durationAttack)
                {
                    spawnOnce = false;
                    spawnOnce2 = false;
                    loadingOnce = false;
                    loadingAttackT = 0f;
                    durationAttackT = 0f;
                    cooldownAttackT = 0f;
                    Destroy(lightAoeGo);
                    attack = false;
                }
            }
        }
    }

    private void Alive()
    {
        navMeshAgent.isStopped = false;

        BoxCollider[] boxCollidersChildren;
        boxCollidersChildren = gameObject.GetComponentsInChildren<BoxCollider>();

        Animator animBody = body.GetComponent<Animator>();
        animBody.enabled = true;

        for (int i = 0; i < rbChildren.Length; i++)
        {
            if (firstDeath)
            {
                rbChildren[i].gameObject.GetComponent<PartPosition>().ResetPosition();
            }

            rbChildren[i].isKinematic = true;
            rbChildren[i].useGravity = false;
        }

        for (int i = 0; i < boxCollidersChildren.Length; i++)
        {
            boxCollidersChildren[i].isTrigger = true;
        }

        for (int i = 0; i < fastIKFabrics.Length; i++)
        {
            fastIKFabrics[i].Target.gameObject.SetActive(true);
            fastIKFabrics[i].enabled = true;
        }

        enemiesHealth.gameObject.SetActive(true);
    }

    private void Death()
    {
        navMeshAgent.isStopped = true;

        BoxCollider[] boxCollidersChildren;
        boxCollidersChildren = gameObject.GetComponentsInChildren<BoxCollider>();

        Animator animBody = body.GetComponent<Animator>();
        animBody.enabled = false;

        for (int i = 0; i < rbChildren.Length; i++)
        {
            rbChildren[i].isKinematic = false;
            rbChildren[i].useGravity = true;
        }

        for (int i = 0; i < boxCollidersChildren.Length; i++)
        {
            boxCollidersChildren[i].isTrigger = false;
        }

        for (int i = 0; i < fastIKFabrics.Length; i++)
        {
            fastIKFabrics[i].Target.gameObject.SetActive(false);
            fastIKFabrics[i].enabled = false;
        }

        enemiesHealth.gameObject.SetActive(false);

        firstDeath = true;

        if (firstDeath)
        {
            SpawnItems spawnItems = gameObject.GetComponent<SpawnItems>();
            spawnItems.OnDeath();
        }

        if (particleAoeGo != null)
        {
            Destroy(particleAoeGo);
        }
        if (lightAoeGo != null)
        {
            Destroy(lightAoeGo);
        }
        spawnOnce = false;
        spawnOnce2 = false;
        loadingOnce = false;
        loadingAttackT = 0f;
        durationAttackT = 0f;
        cooldownAttackT = 0f;
        attack = false;
    }

    private bool CheckPositionsParts()
    {
        for (int i = 0; i < partPositions.Length; ++i)
        {
            if (partPositions[i].travelFinish == false)
            {
                return false;
            }
        }

        return true;
    }
}

