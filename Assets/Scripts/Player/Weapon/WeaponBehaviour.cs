using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBehaviour : MonoBehaviour
{
    private WeaponStats weaponStats;

    private float damageBullet;
    private float damageCac;
    private float fireRate;
    private float accuracy;
    private float numberBullet;

    private float cooldown;

    public GameObject bullet;
    public Transform muzzlePoint;

    private AudioSource audioSource;
    public AudioClip shoot;

    private Vector3 iniPos;
    private Animator anim;

    private void Start()
    {
        weaponStats = GetComponent<WeaponStats>();
        audioSource = GetComponent<AudioSource>();
        anim = GetComponent<Animator>();

        iniPos = transform.localPosition;
    }

    private void Update()
    {
        damageBullet = weaponStats.damageBullet;
        damageCac = weaponStats.damageCac;
        fireRate = weaponStats.fireRate;
        accuracy = weaponStats.accuracy;
        numberBullet = weaponStats.numberBullet;

        cooldown += Time.deltaTime;

        if (cooldown > fireRate && Input.GetButton("Fire1"))
        {
            Fire();

            cooldown = 0f;
        }

        //Post recoil
        transform.localPosition = Vector3.Lerp(transform.localPosition, iniPos, cooldown);
    }

    private void Fire()
    {
        //Recoil
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z - 0.1f);

        for (int i = 0; i < numberBullet; i++)
        {
            Quaternion dirWithAccuracy = Quaternion.Euler(muzzlePoint.rotation.x + Random.insideUnitSphere.x * accuracy,
                                                          muzzlePoint.rotation.y + Random.insideUnitSphere.y * accuracy,
                                                          muzzlePoint.rotation.z + Random.insideUnitSphere.z * accuracy);

            GameObject bulletGO = Instantiate(bullet, muzzlePoint.position, muzzlePoint.rotation * dirWithAccuracy);
            bulletGO.AddComponent<Bullet>().damage = damageBullet;
        }

        audioSource.clip = shoot;
        audioSource.Play();
    }
}
