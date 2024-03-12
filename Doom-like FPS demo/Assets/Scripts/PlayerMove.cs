using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float speed = 5.0f;
    public float gravity = -9.81f;
    public float jumpHeight = 3.0f;
    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;

    public GameObject bullet;
    public Transform firePosition;
    public Transform myCameraHead;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Make sure character grounded
        }

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 moveDirection = transform.right * horizontal + transform.forward * vertical;
        controller.Move(moveDirection * speed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);


        Shoot();
    }

    private void Shoot()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;

            if(Physics.Raycast(myCameraHead.position, myCameraHead.forward, out hit, 100f))
            {
                if(Vector3.Distance(myCameraHead.position, hit.point) > 2f)
                {
                    firePosition.LookAt(hit.point);
                }
                else
                {
                    firePosition.LookAt(myCameraHead.position + (myCameraHead.forward * 50f));
                }
                Debug.Log("Hit " + hit.transform.name + ", Nice Shoot!");
            }

            Instantiate(bullet, firePosition.position, firePosition.rotation);
        }
    }
}
