using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    // Player movement attributes
    public float speed = 5.0f;
    public float gravity = -9.81f;
    public float jumpHeight = 3.0f;
    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;

    // Animator for controlling player animations
    public Animator myAnimation;

    // Shooting attributes
    public GameObject bullet;
    public Transform firePosition;
    public Transform myCameraHead;
    public GameObject muzzleFlash, bulletHole;

    // Sliding movement attributes
    private bool isSliding = false;
    public float slideSpeed = 10.0f;
    public float currentSliderTimer, maxSliderTimer = 2f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        CheckGroundStatus();
        Move();
        Jump();
        Slide();
        Shoot();
    }

    // Checks if the player is grounded and resets the vertical velocity if so
    private void CheckGroundStatus()
    {
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Ensure player stays grounded
        }
    }

    // Handles horizontal player movement based on input
    private void Move()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 moveDirection = transform.right * horizontal + transform.forward * vertical;
        controller.Move(moveDirection * speed * Time.deltaTime);

        // Update animation speed parameter
        myAnimation.SetFloat("Player_Speed", moveDirection.magnitude);
    }

    // Processes player jumping
    private void Jump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            // Apply initial velocity for the jump (based on jump height and gravity)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    // Initiates and manages the sliding mechanic
    private void Slide()
    {
        if (Input.GetKeyDown(KeyCode.C) && isGrounded)
        {
            isSliding = true;
            currentSliderTimer = maxSliderTimer; // Reset sliding timer
        }

        if (isSliding)
        {
            SlideMovement();
        }
    }

    // Controls the sliding movement and timer
    private void SlideMovement()
    {
        if (currentSliderTimer > 0)
        {
            velocity = Vector3.ProjectOnPlane(myCameraHead.transform.forward, Vector3.up).normalized * slideSpeed;
            currentSliderTimer -= Time.deltaTime;
        }
        else
        {
            isSliding = false;
        }
        controller.Move(velocity * Time.deltaTime);
    }

    // Handles shooting mechanics, including bullet instantiation and raycasting
    private void Shoot()
    {
        if (Input.GetMouseButtonDown(0)) // On left mouse click
        {
            RaycastHit hit;

            // Perform raycast from camera position forward
            if (Physics.Raycast(myCameraHead.position, myCameraHead.forward, out hit, 100f))
            {
                ProcessHit(hit); // Process hit object
            }

            // Instantiate muzzle flash and bullet
            Instantiate(muzzleFlash, firePosition.position, firePosition.rotation, firePosition);
            Instantiate(bullet, firePosition.position, firePosition.rotation);
        }
    }

    // Processes the hit object from shooting, including placing bullet holes and destroying enemies
    private void ProcessHit(RaycastHit hit)
    {
        if (Vector3.Distance(myCameraHead.position, hit.point) > 2f)
        {
            firePosition.LookAt(hit.point); // Aim the fire position at hit point
            if (hit.collider.tag == "Shootable")
            {
                Instantiate(bulletHole, hit.point, Quaternion.LookRotation(hit.normal)); // Place bullet hole
            }
            if (hit.collider.CompareTag("Enemy"))
            {
                Destroy(hit.collider.gameObject); // Destroy enemy
            }
        }
        else
        {
            firePosition.LookAt(myCameraHead.position + (myCameraHead.forward * 50f)); // Default aim direction
        }
        Debug.Log("Hit " + hit.transform.name + ", Nice Shoot!");
    }
}
