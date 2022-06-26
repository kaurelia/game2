using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float jumpSpeed = 5f;
    [SerializeField] float climbSpeed = 5f;
    [SerializeField] Vector2 deathKick = new Vector2(10f, 10f);
    [SerializeField] GameObject bullet;
    [SerializeField] Transform gun;

    Animator animator;
    Rigidbody2D myRigidbody;
    CapsuleCollider2D myBodyCollider;
    BoxCollider2D myFeetCollider;
    Vector2 moveInput;

    float gravityScaleAtStart;
    bool isAlive = true;

    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        myBodyCollider = GetComponent<CapsuleCollider2D>();
        myFeetCollider = GetComponent<BoxCollider2D>();

        gravityScaleAtStart = myRigidbody.gravityScale;
    }

    void Update()
    {
        if (!isAlive) { return; }

        Run();
        FlipStrite();
        ClimbLadder();
        Die();
    }

    void OnMove(InputValue value)
    {
        if (!isAlive) { return; }
        moveInput = value.Get<Vector2>();
    }

    void OnJump(InputValue value)
    {
        if (!isAlive) { return; }

        int layerMask = LayerMask.GetMask("Ground");

        if (!myFeetCollider.IsTouchingLayers(layerMask)) { return; }

        if (value.isPressed)
        {
            myRigidbody.velocity += new Vector2(0f, jumpSpeed);
        }
    }

    void OnFire(InputValue value)
    {
        if (!isAlive) { return; }
        Instantiate(bullet, gun.position, gun.rotation);
    }

    void Run()
    {
        bool playerHasHorizontalSpeed = Mathf.Abs(myRigidbody.velocity.x) > Mathf.Epsilon;
        Vector2 playerVelocity = new Vector2(
            moveInput.x * moveSpeed,
            myRigidbody.velocity.y);

        myRigidbody.velocity = playerVelocity;
        animator.SetBool("isRunning", playerHasHorizontalSpeed);
    }

    void FlipStrite()
    {
        bool playerHasHorizontalSpeed = Mathf.Abs(myRigidbody.velocity.x) > Mathf.Epsilon;

        if (playerHasHorizontalSpeed)
        {
            transform.localScale = new Vector2(
                Mathf.Sign(myRigidbody.velocity.x), 1f
            );
        }
    }

    void ClimbLadder()
    {
        bool playerHasVerticalSpeed = Mathf.Abs(myRigidbody.velocity.y) > Mathf.Epsilon;
        int layerMask = LayerMask.GetMask("Climbing");

        if (!myFeetCollider.IsTouchingLayers(layerMask))
        {
            animator.SetBool("isClimbing", false);
            myRigidbody.gravityScale = gravityScaleAtStart;
            return;
        }

        Vector2 climbVelocity = new Vector2(
            myRigidbody.velocity.x,
            moveInput.y * climbSpeed);

        myRigidbody.gravityScale = 0;
        myRigidbody.velocity = climbVelocity;
        animator.SetBool("isClimbing", playerHasVerticalSpeed);
    }

    void Die()
    {
        int layerMask = LayerMask.GetMask("Enemy", "Hazards");

        if (myBodyCollider.IsTouchingLayers(layerMask) ||
            myFeetCollider.IsTouchingLayers(layerMask))
        {
            isAlive = false;
            animator.SetTrigger("Dying");
            myRigidbody.velocity = deathKick;

            FindObjectOfType<GameSession>().ProcessPlayerDeath();
        }
    }
}
