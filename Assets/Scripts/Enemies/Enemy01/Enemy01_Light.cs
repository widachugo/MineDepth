using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.HighDefinition;

public class Enemy01_Light : MonoBehaviour
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

    private Vector3[] lastPositionLegs;

    private Vector3 dirTarget;
    private float distanceTarget;

    [HideInInspector] public bool regenBoss;

    [Header("Sound")]
    private AudioSource audioSource;
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
            audiosourceTarget.volume = 0.15f;
            audiosourceTarget.pitch = 2.05f;
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
            emissiveWeightSmooth = Mathf.Lerp(1, 0, Time.time);
            bodyRend.material.SetFloat("_EmissiveExposureWeight", emissiveWeightSmooth);

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

        if (travelPartsFinish && !regenBoss)
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

        enemiesHealth.canvas.SetActive(true);
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

        enemiesHealth.canvas.SetActive(false);

        regenBoss = false;

        firstDeath = true;

        if (firstDeath)
        {
            SpawnItems spawnItems = gameObject.GetComponent<SpawnItems>();
            spawnItems.OnDeath();
        }
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
