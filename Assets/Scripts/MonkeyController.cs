using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonkeyController : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;

    private Vector3 moveDirection;
    private Vector3 velocity;

    [SerializeField] private bool isGrounded;
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float gravity;

    [SerializeField] private float jumpHeight;
    [SerializeField] private int jumpLimit;
    [SerializeField] private int punchLimit = 8;
    [SerializeField] private bool hasCollide = false;

    [SerializeField] private CapsuleCollider monkey;
    [SerializeField] private bool isColliding = false;

    public HealthBar healthBar;
    public int maxHeath = 10;
    public static int currentHealth;

    public static Vector3 respawnPoint;
    private GameObject gameObject;

    private CharacterController controller;

    private bool walkingKeys = false;
    private bool speedKey = false;
    private bool jumpKey = false;


    private void Start()
    {
        controller = GetComponent<CharacterController>();
        currentHealth = maxHeath;
        healthBar.SetMaxHealth(maxHeath);
        monkey.enabled = false;
        gameObject = GameObject.FindGameObjectWithTag("Player");
        respawnPoint = gameObject.transform.position;
    }

    private void Update()
    {
        Move();
    }

    private void FixedUpdate()
    {
        if (walkingKeys)
        {
            Walk();
            walkingKeys = false;
        }

        if (speedKey)
        {
            Run();
            speedKey = false;
        }

        if (jumpKey)
        {
            Jump();
            jumpKey = false;
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Move()
    {
        isGrounded = Physics.CheckSphere(transform.position, groundCheckDistance, groundMask);

        float moveZ = Input.GetAxis("Vertical");
        float moveX = Input.GetAxis("Horizontal");
        moveDirection = new Vector3(moveX, 0, moveZ);
        moveDirection = transform.TransformDirection(moveDirection);

        if (moveDirection != Vector3.zero && !Input.GetKey(KeyCode.LeftShift))
        {
            walkingKeys = true;
            //Walk();
        }
        else if (moveDirection != Vector3.zero && Input.GetKey(KeyCode.LeftShift))
        {
            speedKey = true;
            //Run();
        }
        else if (moveDirection == Vector3.zero)
        {
            Idle();
        }
        moveDirection *= moveSpeed;

        if (isGrounded)
        {
            jumpLimit = 2;
        }

        if (Input.GetKeyDown(KeyCode.Space) && jumpLimit > 0)
        {
            jumpKey = true;
            //Jump();
            jumpLimit--;
        }

        if (Input.GetMouseButtonDown(0))
        {
            ActivateEnemy();
        }

        if (Input.GetMouseButtonUp(0))
        {
            DeactivateEnemy();
        }

        controller.Move(moveDirection * Time.deltaTime);

        velocity.y += gravity + Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private void Idle()
    {

    }

    private void Walk()
    {
        moveSpeed = walkSpeed;
    }

    private void Run()
    {
        moveSpeed = runSpeed;
    }

    private void Jump()
    {
        velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Enemy" && !hasCollide)
        {
            hasCollide = true;
            currentHealth -= 1;
            healthBar.SetHealth(currentHealth);

            Invoke("Collided", 2);
            //StartCoroutine(Wait(2f));
            //hasCollide = false;
        }
    }

    private void Die()
    {
        print(respawnPoint);
        gameObject.transform.position = respawnPoint;
        currentHealth = maxHeath;
        healthBar.SetHealth(currentHealth);
    }

    private void Collided()
    {
        hasCollide = false;
    }

    private void Attack()
    {
        //isColliding = true;
        ActivateEnemy();
        DeactivateEnemy();
    }

    public void ActivateEnemy()
    {
        // Attack animation
        isColliding = true;
        monkey.enabled = true; // hand object
    }

    public void DeactivateEnemy()
    {
        monkey.enabled = false; //hand object
        isColliding = false;
    }

    /*private IEnumerator Wait(float duration)
    {
        yield return new WaitForSeconds(duration);
    }*/

}
