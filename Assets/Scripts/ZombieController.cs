using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieController : MonoBehaviour
{
    NavMeshAgent nm;
    public Transform target;
    private Animator animator;
    public CharacterController cc;

    [SerializeField] private enum AIState { idle, chasing }
    [SerializeField] private AIState aiState = AIState.idle;
    [SerializeField] private float distanceTreshold = 15f;
    [SerializeField] private CapsuleCollider enemy;
    [SerializeField] private bool isColliding = false;
    [SerializeField] private float hitTime = 3f;
    [SerializeField] private float currTime = 0f;

    [SerializeField] private int punchLimit = 3;
    [SerializeField] private bool hasCollide = false;
    private Vector3 respawnPoint;


    void Start()
    {
        nm = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
        StartCoroutine(Think());
        respawnPoint = gameObject.transform.position;
    }

    void Update()
    {
        currTime += Time.deltaTime;
    }

    private void FixedUpdate()
    {
        CheckAttack();
        if (!MonkeyController.alive)
        {
            Invoke("ResetPosition", 5);
        }
    }

    IEnumerator Think()
    {
        while (true)
        {
            switch (aiState)
            {
                case AIState.idle:
                    animator.SetFloat("MoveSpeed", 0, 0.1f, Time.deltaTime);
                    float dist = Vector3.Distance(target.position, transform.position);
                    if (dist < distanceTreshold)
                    {
                        aiState = AIState.chasing;
                    }
                    if (nm != null)
                    {
                        nm.SetDestination(transform.position);
                    }
                    break;

                case AIState.chasing:
                    animator.SetFloat("MoveSpeed", 1, 0.1f, Time.deltaTime);
                    dist = Vector3.Distance(target.position, transform.position);
                    if (dist > distanceTreshold)
                    {
                        aiState = AIState.idle;
                    }
                    if (nm != null)
                    {
                        nm.SetDestination(target.position);
                    }
                    break;

                default:
                    break;
            }

            yield return new WaitForSeconds(1f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "MonkeyHit" && !hasCollide)
        {
            animator.SetTrigger("Hit");
            hasCollide = true;
            punchLimit--;
            hasCollide = false;
        }

        if (punchLimit == 0)
        {
            Die();
        }
    }

    private void Die()
    {
        animator.SetTrigger("Dead");
        //Destroy(nm);
        nm.enabled = false;
        enemy.enabled = false;
        cc.enabled = true;
        //Destroy(gameObject);
        DeactivateEnemy();
    }

    private void CheckAttack()
    {

        //isColliding = true;
        float dist = Vector3.Distance(target.position, transform.position);
        if (dist < 2f)
        {
            if (currTime >= hitTime)
            {
                animator.SetTrigger("Attack");
                ActivateEnemy();
                isColliding = true;
            }
            else
            {
                DeactivateEnemy();
                isColliding = false;
            }
        }
        else
        {
            DeactivateEnemy();
            isColliding = false;
        }
    }

    public void ActivateEnemy()
    {
        enemy.enabled = true;
        currTime = 0f;
    }

    public void DeactivateEnemy()
    {
        enemy.enabled = false;
    }

    public void ResetPosition()
    {

        nm.enabled = true;
        cc.enabled = false;
        punchLimit = 3;
        animator.SetTrigger("Resurrect");
        aiState = AIState.idle;
        gameObject.transform.position = respawnPoint;

    }
}
