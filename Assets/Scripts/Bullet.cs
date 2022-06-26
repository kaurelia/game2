using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] float bulletSpeed = 10f;

    PlayerMovement player;
    Rigidbody2D myRigidbody;
    float xSpeed;

    void Awake()
    {
        player = FindObjectOfType<PlayerMovement>();
    }

    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        xSpeed = player.transform.localScale.x * bulletSpeed;
    }


    void Update()
    {
        Vector2 bulletVelocity = new Vector2(xSpeed, 0f);
        myRigidbody.velocity = bulletVelocity;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Enemy")
        {
            Destroy(other.gameObject);
        }

        Destroy(gameObject);
    }    
}
