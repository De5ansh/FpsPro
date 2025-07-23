using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieAI : MonoBehaviour
{
    // Start is called before the first frame update
    public NavMeshAgent navMeshAgent;
    public CapsuleCollider capsuleCollider;
    public enum zombieState { idle, chase, attack, dead };
    public zombieState curState = zombieState.idle;
    public Transform player;
    public Animator animator;
    public int health = 100;
    private int damageAmt = 10;
    public float chaseDistance = 10f;
    public float attackDistance = 2f;
    public float attackDelay = 1.5f;
    public float attackCooldown = 2f;
    private bool isAttacking;
    private float lastAttackTime;
    public GameObject bloodScreenEffect;
    private GameObject instantiatedObject;
    void Start()
    {
        capsuleCollider = GetComponent<CapsuleCollider>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        lastAttackTime = -attackCooldown;
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (curState)
        {
            case zombieState.idle:
                animator.SetBool("isWalking", false);
                animator.SetBool("isAttacking", false);
                if (Vector3.Distance(transform.position, player.position) <= chaseDistance)
                {
                    curState = zombieState.chase;
                }
                break;
            case zombieState.chase:
                animator.SetBool("isWalking", true);
                animator.SetBool("isAttacking", false);
                navMeshAgent.SetDestination(player.position);
                if (Vector3.Distance(transform.position, player.position) <= attackDistance)
                {
                    curState = zombieState.attack;
                }
                break;
            case zombieState.attack:
                animator.SetBool("isAttacking", true);
                navMeshAgent.SetDestination(transform.position);
                if (!isAttacking && Time.time - lastAttackTime >= attackCooldown)
                {
                    StartCoroutine(AttackDelay());
                    StartCoroutine(ActivateBloodScreen());
                }
                if (Vector3.Distance(transform.position, player.position) > attackDistance)
                {
                    curState = zombieState.chase;
                }
                break;
            case zombieState.dead:
                animator.SetBool("isWalking", false);
                animator.SetBool("isAttacking", false);
                animator.SetBool("isDead", true);
                navMeshAgent.enabled=false;
                capsuleCollider.enabled = false;
                enabled = false;
                break;
        }
    }

    private void Attack()
    {

    }

    public void TakeDamage(int damageAmt)
    {
        if (curState == zombieState.dead)
        {
            return;
        }
        health -= damageAmt;
        if (health <= 0)
        {
            health = 0;
            Die();
        }
    }

    private void Die()
    {
        curState = zombieState.dead;
    }

    private IEnumerator AttackDelay()
    {
        isAttacking = true;

        // Check if the player has a Movement component to apply damage
        Movement move = player.GetComponent<Movement>();
        if (move != null)
        {
            move.TakeDamage(damageAmt); // Apply damage to the player
            StartCoroutine(ActivateBloodScreen()); // Trigger blood screen effect
        }

        yield return new WaitForSeconds(attackDelay);

        isAttacking = false;
        lastAttackTime = Time.time;
    }

    private IEnumerator ActivateBloodScreen()
    {
        if (bloodScreenEffect == null)
        {
            Debug.LogError("Blood screen effect is not assigned!");
            yield break;
        }

        // Instantiate the blood screen effect
        if (instantiatedObject == null)
        {
            instantiatedObject = Instantiate(bloodScreenEffect);
        }

        // Wait for a short duration
        yield return new WaitForSeconds(attackDelay / 2f);

        // Destroy the blood screen effect
        Destroy(instantiatedObject);
        instantiatedObject = null;
    }

    void InitializeObject()
    {
        instantiatedObject = Instantiate(bloodScreenEffect);
    }

    void DestroyObject() { 
        if (instantiatedObject != null)
        {
            Destroy(instantiatedObject);
            instantiatedObject = null;  
        }
    }
}
