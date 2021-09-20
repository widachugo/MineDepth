using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepSound : MonoBehaviour
{
    private AudioSource audioSource;

    public AudioClip[] stepsSounds;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Ground")
        {
            audioSource.clip = stepsSounds[Random.Range(0, stepsSounds.Length)];
            audioSource.Play();
        }
    }
}
