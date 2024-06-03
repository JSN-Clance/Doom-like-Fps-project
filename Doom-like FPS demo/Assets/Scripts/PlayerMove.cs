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
    public GameObject bulletPrefab;
    public Transform firePosition;
    public Transform myCameraHead;
    public GameObject muzzleFlash, bulletHole;

    // Shooting control attributes
    public float fireRate = 0.1f; // Time between shots
    public bool isFullAuto = true; // Full-auto mode
    private float nextFireTime = 0f;

    // Sliding movement attributes
    private bool isSliding = false;
    public float slideSpeed = 10.0f;

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

    private void CheckGroundStatus()
    {
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Ensure player stays grounded
        }
    }

    private void Move()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 moveDirection = transform.right * horizontal + transform.forward * vertical;
        if (!isSliding)
        {
            controller.Move(moveDirection * speed * Time.deltaTime);
        }

        // Update animation speed parameter
        myAnimation.SetFloat("Player_Speed", moveDirection.magnitude);
    }

    private void Jump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private void Slide()
    {
        if (Input.GetKey(KeyCode.C) && isGrounded)
        {
            isSliding = true;
            SlideMovement();
        }
        else
        {
            isSliding = false;
        }
    }

    private void SlideMovement()
    {
        Vector3 slideDirection = Vector3.ProjectOnPlane(myCameraHead.transform.forward, Vector3.up).normalized * slideSpeed;
        controller.Move(slideDirection * Time.deltaTime);
    }

    private void Shoot()
    {
        if (isFullAuto)
        {
            if (Input.GetMouseButton(0) && Time.time >= nextFireTime) // On left mouse hold for full-auto
            {
                nextFireTime = Time.time + fireRate;
                FireBullet();
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0) && Time.time >= nextFireTime) // On left mouse click for semi-auto
            {
                nextFireTime = Time.time + fireRate;
                FireBullet();
            }
        }
    }

    private void FireBullet()
    {
        RaycastHit hit;
        Vector3 targetPoint;

        // Perform raycast from camera position forward
        if (Physics.Raycast(myCameraHead.position, myCameraHead.forward, out hit, 100f))
        {
            targetPoint = hit.point;
            ProcessHit(hit); // Process hit object
        }
        else
        {
            targetPoint = myCameraHead.position + myCameraHead.forward * 100f;
        }

        // Ensure the firePosition looks at the target point
        firePosition.LookAt(targetPoint);

        // Instantiate muzzle flash and bullet
        Instantiate(muzzleFlash, firePosition.position, firePosition.rotation, firePosition);

        // Instantiate bullet and set its initial direction
        GameObject bullet = Instantiate(bulletPrefab, firePosition.position, firePosition.rotation);
        bullet.GetComponent<Rigidbody>().velocity = firePosition.forward * bullet.GetComponent<BulletController>().speed;
    }

    private void ProcessHit(RaycastHit hit)
    {
        if (Vector3.Distance(myCameraHead.position, hit.point) > 2f)
        {
            if (hit.collider.tag == "Shootable")
            {
                Instantiate(bulletHole, hit.point, Quaternion.LookRotation(hit.normal)); // Place bullet hole
            }
            if (hit.collider.CompareTag("Enemy"))
            {
                Destroy(hit.collider.gameObject); // Destroy enemy
            }
        }
        Debug.Log("Hit " + hit.transform.name + ", Nice Shoot!");
    }
}
