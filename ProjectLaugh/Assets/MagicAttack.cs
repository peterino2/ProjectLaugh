using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicAttack : MonoBehaviour
{
    // Start is called before the first frame update
    Vector2 rightAttackOffset;
    Collider2D swordCollider;
    
    void Start()
    {
        swordCollider = GetComponent<Collider2D>();
        rightAttackOffset = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    public void AttackRight() {
        swordCollider.enabled = true;
        transform.position = rightAttackOffset;
    }

    public void AttackLeft() {
        swordCollider.enabled = true;
        transform.position = new Vector3(rightAttackOffset.x * -1, rightAttackOffset.y);
    }

    public void StopAttack() {
        swordCollider.enabled = false;
    }
}
