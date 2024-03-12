using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BulletController : MonoBehaviour
{
    // Start is called before the first frame update
    public float speed, bulletLife;

    public Rigidbody myRigidbody;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        BulletFly();

        bulletLife -= Time.deltaTime; 

        if(bulletLife < 0)
        {
            Destroy(gameObject);
        }
    }

    private void BulletFly()
    {
        myRigidbody.velocity = transform.forward * speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject);
    }
}
