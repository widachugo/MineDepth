using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldMoney : MonoBehaviour
{
    public float distanceTargetPlayer;

    private GameObject player;
    private float distance;

    private Rigidbody rb;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        distance = Vector3.Distance(transform.position, player.transform.position);

        if (distance <= distanceTargetPlayer)
        {
            transform.position = Vector3.MoveTowards(transform.position, player.transform.position + new Vector3(0, 2, 0), 10 * Time.deltaTime);
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Destroy(gameObject);
        }
    }
}
