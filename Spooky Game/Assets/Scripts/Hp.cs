using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hp : MonoBehaviour
{
    Animator anim;
    Rigidbody2D rb;

    public int hp;
    int currentHp;

    public GameObject hitEffect, deathEffect;

    LayerMask layerMask;
    public GameObject blood;

    [HideInInspector]
    public bool hit, death;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        currentHp = hp;
        layerMask |= (1 << LayerMask.NameToLayer("Ground"));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(int dmg, Transform colPos)
    {
        hit = true;

        rb.velocity = Vector2.zero;

        currentHp -= dmg;
        if (currentHp <= 0) Die();
        anim.SetTrigger("Hit");

        if (Vector2.Dot(transform.right, colPos.position - transform.position) < 0)
        {
            transform.Rotate(new Vector3(0, 180, 0));
        }
        Instantiate(hitEffect, transform.position, transform.rotation);

        SpawnBlood();
    }
    void SpawnBlood()
    {
        GameObject bloodSpawner = new GameObject();
        bloodSpawner.transform.parent = transform;
        bloodSpawner.transform.localEulerAngles = new Vector3(0, 0, -45);
        bloodSpawner.transform.Rotate(0, 0, Random.Range(-120, 0), Space.Self);

        RaycastHit2D hit = Physics2D.Raycast(transform.position, bloodSpawner.transform.right, 5, layerMask);
        Instantiate(blood, hit.point, Quaternion.Euler(0, 0, Random.Range(-180, 180)));

        Destroy(bloodSpawner);
    }
    void Die()
    {
        death = true;
        anim.SetBool("Dead", true);
        GetComponent<Rigidbody2D>().simulated = false;

        Instantiate(deathEffect, transform.position, Quaternion.identity);

        MonoBehaviour[] scripts = GetComponents<MonoBehaviour>();
        foreach(MonoBehaviour s in scripts)
        {
            if(s != this)
            {
                s.enabled = false;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if(col.CompareTag("Slash"))
        {
            Dmg dmg = col.GetComponent<Dmg>();
            TakeDamage(dmg.dmg, col.gameObject.transform);
        }
    }
}
