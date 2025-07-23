using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("Player Helath")]
    public int maxHealth = 100;
    public int curHealth;

    [Header("Player Movement")]
    public float spd = 5f;
    public float jumpForce = 2f;
    private CharacterController charController;
    public float gravity = -9.81f;
    public Transform gravCheck;
    public LayerMask groundMask;
    private bool isGrounded;
    public float groundDistance = 0.4f;
    private Vector3 velocity;

    [Header("Foot Steps")]
    public AudioSource leftAudioSource;
    public AudioSource rightAudioSource;
    public AudioClip[] footStepSounds;
    public float footStepInterval = 0.5f;
    private float nextFootStepTime;
    private bool isLeftFootStep = true;

    void Start()
    {
        curHealth = maxHealth;
        charController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics.CheckSphere(gravCheck.position, groundDistance, groundMask);
        if (isGrounded && velocity.y < 0) 
        {
            velocity.y = -2f;
        }
        HnadleMovement();
        HandleGravity();

        if (isGrounded && charController.velocity.magnitude>0.1f && Time.time >= nextFootStepTime)
        {
            PlayerFootStepSound();
            nextFootStepTime = Time.time + footStepInterval;
        }
        
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(gravity * -2 * jumpForce);
        }
        charController.Move(velocity * Time.deltaTime);
    }

    void HnadleMovement()
    {
        float horzontal = Input.GetAxis("Horizontal");
        float vertial = Input.GetAxis("Vertical");
        Vector3 movement = transform.right * horzontal + transform.forward * vertial;
        movement.y = 0;
        charController.Move(movement * spd * Time.deltaTime);
    }

    void HandleGravity()
    {
        velocity.y += gravity * Time.deltaTime;
    }

    void PlayerFootStepSound()
    {
        AudioClip footStepClip = footStepSounds[Random.Range(0, footStepSounds.Length)];
        if (isLeftFootStep)
        {
            leftAudioSource.PlayOneShot(footStepClip);
        } else
        {
            rightAudioSource.PlayOneShot(footStepClip);
        }
        isLeftFootStep = !isLeftFootStep;
    }

    public void TakeDamage(int damageAmount)
    {
        curHealth -= damageAmount;
        if (curHealth <= 0)
        {
            curHealth = 0;
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Dead");
    }
}
