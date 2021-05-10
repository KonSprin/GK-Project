using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieController : MonoBehaviour
{
    NavMeshAgent nm;
    public Transform target;
    [SerializeField] private enum AIState { idle,chasing }
    [SerializeField] private AIState aiState = AIState.idle;
    [SerializeField] private float distanceTreshold = 10f;
    [SerializeField] private CapsuleCollider enemy;
    [SerializeField] private bool isColliding = false;
    [SerializeField] private float hitTime = 3f;
    [SerializeField] private float currTime = 0f;

    [SerializeField] private int punchLimit = 8;
    [SerializeField] private bool hasCollide = false;


    void Start()
    {
        nm = GetComponent<NavMeshAgent>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
        StartCoroutine(Think());

    }

    void Update()
    {
        currTime += Time.deltaTime;
    }

    IEnumerator Think()
    {
        while(true)
        {
            switch(aiState)
            {
                case AIState.idle:

                    float dist = Vector3.Distance(target.position, transform.position);
                    if (dist < distanceTreshold)
                    {
                        aiState = AIState.chasing;
                    }
                    nm.SetDestination(transform.position);
                    break;

                case AIState.chasing:

                    dist = Vector3.Distance(target.position, transform.position);
                    if (dist > distanceTreshold)
                    {
                        aiState = AIState.idle;
                    }
                    nm.SetDestination(target.position);
                    CheckAttack();
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
        //Die animation
        Destroy(gameObject);
    }

    private void CheckAttack()
    {

        //isColliding = true;
        float dist = Vector3.Distance(target.position, transform.position);
        if (dist < 2f)
        {
            if (currTime >= hitTime)
            {
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

}
