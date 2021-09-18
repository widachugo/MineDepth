using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossMovement : MonoBehaviour
{
    private NavMeshAgent agent;
    private GameObject target;

    private Animator anim;

    public bool isActivate;
    public bool attack;

    public bool move;

    private BossBehaviour bossBehaviour;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        bossBehaviour = GetComponent<BossBehaviour>();

        target = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        if (isActivate)
        {
            anim.SetBool("BossActive", true);

            if (!attack && !bossBehaviour.regeneration)
            {
                agent.destination = target.transform.position;
            }
            else
            {
                agent.destination = transform.position;
            }

            if (agent.velocity.magnitude > 0 && transform.position != agent.destination)
            {
                anim.SetBool("Move", true);
                move = true;
            }
            else
            {
                anim.SetBool("Move", false);
                move = false;
            }
        }
        else
        {
            anim.SetBool("BossActive", false);
        }
    }
}
