using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PumpkinheadBehaviour : MonoBehaviour
{
    Rigidbody2D rb;
    Animator anim;
    Hp hp;

    public GameObject slashCol;
    Transform target;

    [Header("Active")]
    public float speed;
    public float speedUp;
    public float sightRange;
    public LayerMask sightMask;
    bool activate, deactivate;
    public float slashRange;
    public float fireRate, fireRateMult = 0.75f;
    float rateTime;
    bool slashing, withinDistance;

    [Header("Idle")]
    public float idleTime;
    public float idleTimeMult = 1;
    bool idling;
    float idleDur;
    [Header("Patroling")]
    public float patrolTime;
    public float patrolTimeMult = 1;
    bool patroling;
    float patrolDur;
    public float patrolSpeed, patrolSpeedUp;

    public Transform edgeDetection;
    public float edgeDropRange;
    public float forwardRange;
    LayerMask layerMask;
    bool isGrounded, atEdge;

    // Start is called before the first frame update
    void Start()
    {
        activate = false;
        deactivate = false;

        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        hp = GetComponent<Hp>();
        //InvokeRepeating(nameof(Attack), 2, 2);
        StartCoroutine(Idle());

        target = GameObject.Find("Player").transform;

        layerMask |= (1 << LayerMask.NameToLayer("Ground"));
    }

    // Update is called once per frame
    private void Update()
    {
        if (hp.hit)
        {
            StopAllCoroutines();
            idling = false;
            patroling = false;
            StartCoroutine(Idle());
        }

        if(patroling)
        {
            RaycastHit2D hitForward = Physics2D.Raycast(transform.position, transform.right, Mathf.Infinity, layerMask);
            RaycastHit2D hitDown = Physics2D.Raycast(edgeDetection.position, Vector2.down, Mathf.Infinity, layerMask);

            if ((hitForward.distance < forwardRange) || (hitDown.distance > edgeDropRange && isGrounded))
            {
                rb.velocity = Vector3.zero;
                StopAllCoroutines();
                DisableAllBools();
                StartCoroutine(Idle());
                atEdge = true;
                //transform.Rotate(new Vector3(0, 180, 0));
            }
        }

        if (!slashing) slashCol.SetActive(false);
        anim.SetBool("Slashing", slashing);

        rateTime -= Time.deltaTime;
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
        if (patroling)
        {
            rb.AddForce(transform.right * patrolSpeedUp);

            Vector2 velocity = new Vector2(rb.velocity.x, 0);
            Vector2 vel = Vector2.ClampMagnitude(velocity, patrolSpeed);
            rb.velocity = new Vector2(vel.x, rb.velocity.y);
        }

        RaycastHit2D hit = Physics2D.Raycast(transform.position, (target.position - transform.position), sightRange, sightMask);
        if (hit.collider == null) 
        {
            Debug.LogWarning("raycast not working");
            Deactivate();
        }
        if(hit.transform.CompareTag("Player"))
        {
            Active();
            Activate();
        }
        else
        {
            Deactivate();
        }
    }

    IEnumerator Attack()
    {
        rateTime = Random.Range(fireRate * fireRateMult, fireRate);

        slashing = true;
        anim.SetTrigger("Attack");
        yield return new WaitForSeconds(2);
        slashing = false;
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
        if(!atEdge)
        {
            if (dir == 1) transform.rotation = Quaternion.Euler(0, 0, 0);
            if (dir == 2) transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        else
        {
            transform.Rotate(new Vector3(0, 180, 0));
            atEdge = false;
        }

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
        if(!slashing)
        {
            if (Vector2.Dot(transform.right, target.position - transform.position) < 0)
            {
                transform.Rotate(new Vector3(0, 180, 0));
            }
        }

        float clampedX = Mathf.Clamp(rb.velocity.x, -speed, speed);
        rb.velocity = new Vector2(clampedX, rb.velocity.y);

        if (!slashing && !withinDistance) rb.AddForce(transform.right * speedUp);
        else rb.velocity = Vector2.zero;

        withinDistance = Vector2.Distance(transform.position, target.position) <= slashRange;

        if (withinDistance)
        {
            if(rateTime <= 0)
            {
                if (!slashing) StartCoroutine(Attack());
            }
        }
    }
    void Activate()
    {
        if (!activate)
        {
            StopAllCoroutines();
            DisableAllBools();

            deactivate = false;
            activate = true;
        }
    }
    void Deactivate()
    {
        if(!deactivate)
        {
            rb.velocity = Vector2.zero;

            StopAllCoroutines();
            DisableAllBools();
            StartCoroutine(Idle());

            activate = false;
            deactivate = true;
        }
    }
    #endregion

    void DisableAllBools()
    {
        idling = false;
        patroling = false;
        slashing = false;
    }

    private void OnCollisionEnter2D(Collision2D col) { if (col.gameObject.CompareTag("Ground")) isGrounded = true; }
    private void OnCollisionExit2D(Collision2D col) { if (col.gameObject.CompareTag("Ground")) isGrounded = false; }
}
