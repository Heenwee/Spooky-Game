using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHp : MonoBehaviour
{
    Rigidbody2D rb;
    PlayerController pc;
    Animator anim;

    public int hp;
    int currentHp;
    public GameObject hitEffect;

    public Animator UIAnim;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        pc = GetComponent<PlayerController>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(int dmg, Transform colPos, float knockback)
    {
        UIAnim.SetTrigger("Hit");
        anim.SetTrigger("Hit");
        if (Vector2.Dot(transform.right, colPos.position - transform.position) < 0)
        {
            transform.Rotate(new Vector3(0, 180, 0));
        }
        pc.isGrounded = false;
        Vector2 knockbackVel = new Vector2(knockback, knockback / 2);
        rb.AddForce(-knockbackVel.x * transform.right + knockbackVel.y * transform.up);
        Instantiate(hitEffect, transform.position, transform.rotation);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if(col.CompareTag("Enemy_Attack"))
        {
            Dmg dmg = col.GetComponent<Dmg>();
            TakeDamage(dmg.dmg, col.gameObject.transform, dmg.knockback);
        }
    }
}
