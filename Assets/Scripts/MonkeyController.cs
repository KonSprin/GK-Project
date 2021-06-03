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
    public Transform cam;

    [SerializeField] private float jumpHeight;
    [SerializeField] private int jumpLimit;
    [SerializeField] private int punchLimit = 8;
    [SerializeField] private bool hasCollide = false;

    [SerializeField] private CapsuleCollider monkey;
    [SerializeField] private bool isColliding = false;

    public HealthBar healthBar;
    public int maxHeath = 10;
    public static int currentHealth;
    public GameObject finishText;
    public static Vector3 respawnPoint;
    private GameObject gameObject;
    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;

    private CharacterController controller;
    private Animator animator;

    private bool walkingKeys = false;
    private bool speedKey = false;
    private bool jumpKey = false;
    public static bool alive = true;
    private bool hasDied = false;
    private GameObject death;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        currentHealth = maxHeath;
        healthBar.SetMaxHealth(maxHeath);
        monkey.enabled = false;
        gameObject = GameObject.FindGameObjectWithTag("Player");
        death = GameObject.FindGameObjectWithTag("Death");
        death.SetActive(false);
        respawnPoint = gameObject.transform.position;
    }

    private void Update()
    {
        Move();  
        
    }

    private void FixedUpdate()
    {
        isGrounded = Physics.CheckSphere(transform.position, groundCheckDistance, groundMask);
        /*if (walkingKeys)
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
        }*/
        if (currentHealth <= 0 && !hasDied)
        {
            alive = false;
            hasDied = true;
            Die();
        }

        
    }

    private void Move()
    {

       
        float moveZ = Input.GetAxis("Vertical");
        float moveX = Input.GetAxis("Horizontal");
        moveDirection = new Vector3(moveX, 0, moveZ).normalized;
        //moveDirection = transform.TransformDirection(moveDirection);

        if (moveDirection != Vector3.zero && !Input.GetKey(KeyCode.LeftShift) && alive)
        {
            //walkingKeys = true;
            Walk();
        }
        else if (moveDirection != Vector3.zero && Input.GetKey(KeyCode.LeftShift) && alive)
        {
            //speedKey = true;
            Run();
        }
        else if (moveDirection == Vector3.zero)
        {
            Idle();
        }
        //moveDirection *= moveSpeed;

        if (isGrounded)
        {
            jumpLimit = 1;
        }

        if (Input.GetKeyDown(KeyCode.Space) && jumpLimit > 0 && alive)
        {
            //jumpKey = true;
            Jump();
            jumpLimit--;
            //jumpKey = false;
        }

        if (Input.GetMouseButtonDown(0) && alive)
        {
           ActivateEnemy();
        }

        if (Input.GetMouseButtonUp(0))
        {
            DeactivateEnemy();
        }
       
        if (!alive)
        {
            moveSpeed = 0;
        }

        if(moveDirection.magnitude >= 0.1f)
        {
            float targetAngle = cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);


            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * moveDirection;
            // moveDir *= moveSpeed;
            controller.Move(moveDir.normalized * moveSpeed * Time.deltaTime);
        }

        velocity.y += gravity + Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);


    }

    private void Idle()
    {
        animator.SetFloat("Speed", 0, 0.1f, Time.deltaTime);
    }

    private void Walk()
    {
        moveSpeed = walkSpeed;
        animator.SetFloat("Speed", 0.5f, 0.1f, Time.deltaTime);
    }

    private void Run()
    {
        moveSpeed = runSpeed;
        animator.SetFloat("Speed", 1.0f, 0.1f, Time.deltaTime);
    }

    private void Jump()
    {
        animator.SetTrigger("Jump");
        velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Enemy" && !hasCollide)
        {
            hasCollide = true;
            currentHealth -= 1;
            if(currentHealth > 0) {
                animator.SetTrigger("Hit");
            }
            healthBar.SetHealth(currentHealth);

            Invoke("Collided", 2);
            //StartCoroutine(Wait(2f));
            //hasCollide = false;
        }
        if (other.gameObject.tag == "Enemy2" && !hasCollide)
        {
            hasCollide = true;
            currentHealth -= 2;
            if (currentHealth > 0)
            {
                animator.SetTrigger("Hit");
            }
            healthBar.SetHealth(currentHealth);

            Invoke("Collided", 2);
            //StartCoroutine(Wait(2f));
            //hasCollide = false;
        }
        if (other.gameObject.tag == "GameOver" && !hasCollide)
        {
            finishText.GetComponent<Text>().text = "YOU WIN ! ! !";
            Application.Quit();
        }
    }

    private void Die()
    {
        
        print(respawnPoint);
        animator.SetTrigger("Die");
        print("I DIED");
        //Invoke("Respawn", 5);
        Respawn();
    }

    private void Respawn()
    {
        death.SetActive(true);
        
        animator.SetTrigger("Respawn");
        print("IM ALIVE");
        gameObject.transform.position = respawnPoint;
        currentHealth = maxHeath;
        healthBar.SetHealth(currentHealth);
        alive = true;
        hasDied = false;
        //Invoke("Clear", 3); 
        Clear();
    }

    private void Clear()
    {
        death.SetActive(false);
    }
    private void Collided()
    {
        hasCollide = false;
    }

    public void ActivateEnemy()
    {
        animator.SetTrigger("Attack");
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
