using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public enum MovementType
    {
        RigidbodyVelocity,
        RigidbodyAddForce,
        VectorMoveToward,
        TransformTranslate,
        DirectPositionChange
    }
    [SerializeField]
    private MovementType movementType = MovementType.RigidbodyVelocity;

    [SerializeField]
    private float moveSpeed = 1f;
    // public float collisionOffset = 0.05f;
    public MagicAttack magicAttack;
    Vector2 movementInput;
    Rigidbody2D rb;
    Animator animator;
    SpriteRenderer spriteRenderer;

    public ContactFilter2D interactionFilter;
    List<RaycastHit2D> castCollisions = new List<RaycastHit2D>();
    // bool canMove = true;
    // bool isAttacking = false;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public float interactionDistance;

    // Update is called once per frame
    void Update()
    {
        movementInput.x = Input.GetAxisRaw("Horizontal");
        movementInput.y = Input.GetAxisRaw("Vertical");


        if(movementType == MovementType.VectorMoveToward)
        {
            VectorMoveTowards();
        }

        if(movementType == MovementType.TransformTranslate)
        {
            TransformTranslate();
        }

        if(movementType == MovementType.DirectPositionChange)
        {
            PositionChange();
        }

        if (Input.GetKeyDown(KeyCode.Space) == true) {
            animator.SetTrigger("magicAttack");
        }
        
        int count = rb.Cast(
                 movementInput,
                 interactionFilter,
                 castCollisions,
                 interactionDistance
             );

        if (Input.GetKeyDown(KeyCode.Space))
        {
            OnInteractInput();
        }
    }

    public void OnInteractInput()
    {
        for (int i = 0; i < castCollisions.Count; i++)
        {
            var x = castCollisions[i].transform.gameObject.GetComponent<Interactable>();
            if (x != null)
            {
                x.OnInteract(gameObject);
            }
        }
    }

    private void VectorMoveTowards()
    {
        transform.position = Vector2.MoveTowards(transform.position,
            transform.position + (Vector3)movementInput, moveSpeed * Time.deltaTime);
    }


    private void TransformTranslate()
    {
        transform.Translate(movementInput * moveSpeed * Time.deltaTime);
    }


    private void PositionChange()
    {
        transform.position += (Vector3)movementInput * Time.deltaTime * moveSpeed;
    }
    private void FixedUpdate()
    {

        if(movementType == MovementType.RigidbodyVelocity)
        {
            RigidbodyVelocity();
        }

        if(movementType == MovementType.RigidbodyAddForce)
        {
            RigidbodyAddForce();
        }
        // if(canMove){
        //     if(movementInput != Vector2.zero) {
        //     bool success = TryMove(movementInput);

        //     if (!success && movementInput.x != 0) {
        //         success = TryMove(new Vector2(movementInput.x, 0));

        //         if (!success && movementInput.y != 0) {
        //             success = TryMove(new Vector2(0, movementInput.y));
        //         }
        //     }

        //     animator.SetBool("isMoving", success);
            
        // } else {
        //     animator.SetBool("isMoving", false);
        // }

        // if (movementInput.x < 0) {
        //     spriteRenderer.flipX = true;
        // } else if (movementInput.x > 0) {
        //     spriteRenderer.flipX = false;
        // }
        // }

    }

    private void RigidbodyVelocity()
    {
        rb.velocity = movementInput.normalized * moveSpeed;
    }


    private void RigidbodyAddForce()
    {
        rb.AddForce(movementInput * moveSpeed, ForceMode2D.Impulse);
    }

    // private bool TryMove(Vector2 direction) {
    //     int count = rb.Cast(
    //             direction,
    //             movementFilter,
    //             castCollisions,
    //             moveSpeed * Time.fixedDeltaTime + collisionOffset
    //         );
    //         if (count == 0) {
    //             rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);
    //             return true;
    //         } else {
    //             return false;
    //         }
    // }

    // void OnMove(InputValue movementValue) {
    //     movementInput = movementValue.Get<Vector2>();
    // }

    // void OnFire() {
    //     if (isAttacking == false) {
    //         animator.SetTrigger("magicAttack");
    //     } 
    // }

    // public void LockMovement(){
    //     canMove = false;
    // }
    
    // public void UnlockMovement(){
    //     canMove = true;
    // }

    // public void Attack(){
    //     isAttacking = !isAttacking;
    // }

    // public void MagicAttack() {
    //     LockMovement();
    //     if(spriteRenderer.flipX == true) {
    //         magicAttack.AttackLeft();
    //     } else {
    //         magicAttack.AttackRight();
    //     }
    // }

    // public void StopAttack() {
    //     UnlockMovement();
    //     magicAttack.StopAttack();
    // }


}
