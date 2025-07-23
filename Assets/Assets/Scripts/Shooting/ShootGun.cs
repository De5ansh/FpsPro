using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootGun : MonoBehaviour
{
    // Start is called before the first frame update
    public Animator animator;
    public Transform firePoint;
    public float fireRange = 10f;
    public int damageAmt = 40;
    public float fireRate = 0.1f;
    private float nextFireTime = 0f;
    public bool isAuto = true;
    public int maxAmmo = 30;
    private int currentAmmo;
    public float reloadTime = 1.5f;
    private bool isReloading = false;
    public ParticleSystem muzzleFlash;
    public ParticleSystem bloodEffect;
    public AudioSource sound;
    public AudioClip shootClip;
    public AudioClip reloadClip;

    void Start()
    {
        currentAmmo = maxAmmo;
    }

    // Update is called once per frame
    void Update()
    {
        if (isReloading)
        {
            return;
        }
        if (isAuto)
        {
            if (Input.GetButton("Fire1") && Time.time >= nextFireTime)
            {
                nextFireTime = Time.time + 1f / fireRate;
                Shoot();
            } else if(Input.GetButtonUp("Fire1"))
            {
                animator.SetBool("Shoot", false);
            }
        }
        else
        {
            if (Input.GetButtonDown("Fire1") && Time.time >= nextFireTime)
            {
                nextFireTime = Time.time + 1f / fireRate;
                Shoot();
            } else if(Input.GetButtonUp("Fire1"))
            {
                animator.SetBool("Shoot", false);
            }
          
        }

        if (Input.GetKeyDown(KeyCode.R) && currentAmmo < maxAmmo)
        {
            Reload();
        }
    }

    private void Shoot()
    {
        if (currentAmmo > 0)
        {
            sound.PlayOneShot(shootClip);
            RaycastHit hit;

            if (Physics.Raycast(firePoint.position, firePoint.forward, out hit))
            {
                ZombieAI zomAI = hit.collider.GetComponent<ZombieAI>();
                if (zomAI != null)
                {
                    zomAI.TakeDamage(damageAmt);
                    ParticleSystem blood = Instantiate(bloodEffect, hit.point, Quaternion.LookRotation(hit.normal));
                    Destroy(blood.gameObject, blood.main.duration);
                }
                Debug.Log(hit.transform.name);
            }
            muzzleFlash.Play();
        }
        animator.SetBool("Shoot", true);
        currentAmmo--;

    }

    private void Reload()
    {
        if (!isReloading && currentAmmo < maxAmmo)
        {
            sound.PlayOneShot(reloadClip);
            isReloading = true;
            Invoke("FinishReload", reloadTime);
            animator.SetTrigger("Reload");
        }
    }

    private void FinishReload()
    {
        currentAmmo = maxAmmo;
        isReloading = false;
        animator.ResetTrigger("Reload");
    }
}
