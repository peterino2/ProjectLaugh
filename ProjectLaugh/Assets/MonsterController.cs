using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterController : MonoBehaviour
{

    Rigidbody2D rb;
    Animator animator;
    SpriteRenderer spriteRenderer;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator.SetBool("isDead", false);
        animator.SetBool("isRunning", false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T) == true) {
            attackOne();
        }
        if (Input.GetKeyDown(KeyCode.Y) == true) {
            attackTwo();
        }
        if (Input.GetKeyDown(KeyCode.U) == true) {
            run();
        }
        if (Input.GetKeyDown(KeyCode.I) == true) {
            stopRun();
        }
        if (Input.GetKeyDown(KeyCode.O) == true) {
            die();
        }
        
    }

    public void attackOne(){
        animator.SetTrigger("atk_one");
    }
    public void attackTwo(){
        animator.SetTrigger("atk_two");
    }

    public void run(){
        animator.SetBool("isRunning", true);
    }
    public void stopRun(){
        animator.SetBool("isRunning", false);
    }
    public void die(){
        animator.SetBool("isDead", true);
    }
}
