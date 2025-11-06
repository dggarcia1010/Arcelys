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

    public float speed = 5f;

    private Rigidbody2D myRigidbody;

    private Vector3 change;

    private Animator animator;


    // --- Nuevo: control del tiempo del Animator para evitar el bug ---

    private float animatorResetTimer = 0f;

    private const float animatorResetInterval = 300f; // cada 5 minutos (300 s)


    void Start()

    {

        currentState = PlayerState.walk;

        animator = GetComponent<Animator>();

        myRigidbody = GetComponent<Rigidbody2D>();

    }


    void Update()

    {

        // --- Reinicia el Animator cada cierto tiempo para evitar el overflow ---

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

        myRigidbody.MovePosition(

            transform.position + change * speed * Time.deltaTime

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

} 
