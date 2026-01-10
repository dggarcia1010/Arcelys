using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState
{
    walk,
    attack,
    interact
}

public class PlayerMovement : MonoBehaviour
{
    public PlayerState currentState;

    [Header("Movimiento normal")]
    public float speed = 5f;

    [Header("Movimiento en hielo")]
    public bool onIce = false;
    public float iceAcceleration = 10f;   // Qué rápido responde al input
    public float iceFriction = 2f;         // Qué lento se frena (más bajo = más resbala)
    public float maxIceSpeedMultiplier = 1f;

    private Vector2 currentVelocity;

    private Rigidbody2D myRigidbody;
    private Vector3 change;
    private Animator animator;

    // --- Fix Animator ---
    private float animatorResetTimer = 0f;
    private const float animatorResetInterval = 300f;

    void Start()
    {
        currentState = PlayerState.walk;
        animator = GetComponent<Animator>();
        myRigidbody = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        animatorResetTimer += Time.deltaTime;
        if (animatorResetTimer >= animatorResetInterval)
        {
            ResetAnimatorState();
            animatorResetTimer = 0f;
        }

        change = Vector3.zero;
        change.x = Input.GetAxisRaw("Horizontal");
        change.y = Input.GetAxisRaw("Vertical");

        if (Input.GetButtonDown("attack") && currentState != PlayerState.attack)
        {
            StartCoroutine(AttackCo());
        }
        else if (currentState == PlayerState.walk)
        {
            UpdateAnimationAndMove();
        }
    }

    private IEnumerator AttackCo()
    {
        animator.SetBool("attacking", true);
        currentState = PlayerState.attack;

        yield return null;

        animator.SetBool("attacking", false);
        yield return new WaitForSeconds(.10f);

        currentState = PlayerState.walk;
    }

    void UpdateAnimationAndMove()
    {
        if (onIce)
        {
            // En hielo SIEMPRE se mueve (aunque no haya input)
            MoveCharacter();

            if (change != Vector3.zero)
            {
                animator.SetFloat("moveX", change.x);
                animator.SetFloat("moveY", change.y);
            }

            animator.SetBool("moving", currentVelocity.magnitude > 0.05f);
            return;
        }

        // Movimiento normal
        if (change != Vector3.zero)
        {
            MoveCharacter();
            animator.SetFloat("moveX", change.x);
            animator.SetFloat("moveY", change.y);
            animator.SetBool("moving", true);
        }
        else
        {
            animator.SetBool("moving", false);
        }
    }

    void MoveCharacter()
    {
        if (!onIce)
        {
            // Movimiento clásico
            myRigidbody.MovePosition(
                myRigidbody.position + (Vector2)change * speed * Time.deltaTime
            );
            return;
        }

        // --- Movimiento en hielo ---
        Vector2 inputDir = new Vector2(change.x, change.y).normalized;
        float maxSpeed = speed * maxIceSpeedMultiplier;

        if (inputDir != Vector2.zero)
        {
            Vector2 targetVelocity = inputDir * maxSpeed;
            currentVelocity = Vector2.MoveTowards(
                currentVelocity,
                targetVelocity,
                iceAcceleration * Time.deltaTime
            );
        }
        else
        {
            // Deslizamiento al soltar tecla
            currentVelocity = Vector2.MoveTowards(
                currentVelocity,
                Vector2.zero,
                iceFriction * Time.deltaTime
            );
        }

        myRigidbody.MovePosition(
            myRigidbody.position + currentVelocity * Time.deltaTime
        );
    }

    private void ResetAnimatorState()
    {
        if (animator != null)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            animator.Play(stateInfo.shortNameHash, 0, 0f);
            animator.Update(0f);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ice"))
        {
            onIce = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Ice"))
        {
            onIce = false;
            currentVelocity = Vector2.zero;
        }
    }
}