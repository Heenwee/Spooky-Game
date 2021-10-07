using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PumpkinheadBehaviour : MonoBehaviour
{
    Rigidbody2D rb;
    Animator anim;
    Hp hp;

    public GameObject slashCol;

    bool idling;
    float idleDur;
    public float idleTime, idleTimeMult = 1;
    bool patroling;
    float patrolDur;
    public float patrolTime, patrolTimeMult = 1;
    public float patrolSpeed, patrolSpeedUp;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        hp = GetComponent<Hp>();
        //InvokeRepeating(nameof(Attack), 2, 2);
        StartCoroutine(Idle());
    }

    // Update is called once per frame
    private void Update()
    {
        if(hp.hit)
        {
            StopAllCoroutines();
            idling = false;
            patroling = false;
            StartCoroutine(Idle());
        }
    }
    private void OnDisable()
    {
        StopAllCoroutines();
    }
    void FixedUpdate()
    {
        Move();
    }
    void Move()
    {
        if(patroling)
        {
            rb.AddForce(transform.right * patrolSpeedUp);

            Vector2 velocity = new Vector2(rb.velocity.x, 0);
            Vector2 vel = Vector2.ClampMagnitude(velocity, patrolSpeed);
            rb.velocity = new Vector2(vel.x, rb.velocity.y);
        }
    }

    void Attack()
    {
        anim.SetTrigger("Attack");
    }
    void Slash()
    {
        StartCoroutine(SlashCol());
    }
    IEnumerator SlashCol()
    {
        slashCol.SetActive(true);
        yield return new WaitForSeconds(0.05f);
        slashCol.SetActive(false);
    }

    #region States
    public IEnumerator Idle()
    {
        idling = true;
        idleDur = Random.Range(idleTime * idleTimeMult, idleTime);
        while (idling)
        {
            if (idleDur <= 0) idling = false;
            idleDur -= Time.deltaTime;

            yield return null;
        }

        int dir = Random.Range(1, 3);
        if (dir == 1) transform.rotation = Quaternion.Euler(0, 0, 0);
        if (dir == 2) transform.rotation = Quaternion.Euler(0, 180, 0);

        StartCoroutine(Patrol());
    }
    IEnumerator Patrol()
    {
        patroling = true;
        patrolDur = Random.Range(patrolTime * patrolTimeMult, patrolTime);
        while (patroling)
        {
            if (patrolDur <= 0) patroling = false;
            patrolDur -= Time.deltaTime;

            yield return null;
        }

        rb.velocity = Vector2.zero;
        StartCoroutine(Idle());
    }
    void Active()
    {

    }
    #endregion
}
