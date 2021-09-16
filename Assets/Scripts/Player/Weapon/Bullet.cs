using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [HideInInspector] public float damage;
    public float speed = 600;

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        Destroy(gameObject, 10f);
    }

    private void Update()
    {
        rb.AddForce(transform.forward * speed * Time.deltaTime, ForceMode.Impulse);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            other.gameObject.GetComponentInChildren<EnemiesHealth>().TakeDamage(damage);
            Destroy(gameObject);
        }
        else if (other.gameObject.tag != "Player" && !other.GetComponent<Bullet>())
        {
            Destroy(gameObject);
        }
    }
}
