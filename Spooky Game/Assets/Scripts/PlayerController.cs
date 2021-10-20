using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rb;
    Animator anim;
    float xInput;

    public float speed = 10f, speedUp = 75f;
    float currentSpeed;

    public float dashForce;
    bool dashing;

    [HideInInspector]
    public bool isGrounded;
    bool jump;
    bool holding;
    float jumpBuffer;
    public Transform groundCheck;
    public float radius = 0.2f;
    public LayerMask groundMask;
    public float jumpHeight = 3f;

    int slashNr = 1;
    bool slash;
    float slashBuffer;
    float resetSlashNr;
    public float slashForce;
    public GameObject slashCol;
    public AudioSource slashSource;
    public AudioClip slashSound;

    [HideInInspector]
    public bool clamp = true, canMove = true;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, radius, groundMask);
        Jump();
        Dash();

        xInput = Input.GetAxisRaw("Horizontal");

        Rotate(xInput);

        Attack();

        if (jump) jumpBuffer -= Time.deltaTime;
        if (jumpBuffer <= 0) jump = false;
        if (slash) slashBuffer -= Time.deltaTime;
        if (slashBuffer <= 0) slash = false;

        resetSlashNr -= Time.deltaTime;
        if (resetSlashNr <= 0) slashNr = 1;

        anim.SetBool("IsGrounded", isGrounded);
    }
    private void FixedUpdate()
    {
        Move(xInput);

        if (clamp) ClampVel(xInput);
    }
    void Move(float x)
    {
        if (canMove) rb.AddForce(Vector2.right * x * speedUp);
    }
    void ClampVel(float x)
    {
        if(!dashing)
        {
            Vector2 velocity = new Vector2(rb.velocity.x, 0);
            Vector2 vel = Vector2.ClampMagnitude(velocity, speed);
            rb.velocity = new Vector2(vel.x, rb.velocity.y);

            if (isGrounded)
            {
                if (x == 0 || x * rb.velocity.x < 0) rb.velocity = new Vector2(0, rb.velocity.y);
            }
            else
            {
                if (x == 0) rb.velocity *= new Vector2(0.95f, 1);

                if (rb.velocity.y > 0 && !holding) rb.velocity *= new Vector2(1, 0.9f);
            }
        }
    }

    void Jump()
    {
        if (Input.GetButtonDown("Jump"))
        {
            jump = true;
            jumpBuffer = 0.1f;
        }

        if (isGrounded)
        {
            if (jump && canMove)
            {
                rb.velocity = new Vector2(rb.velocity.x, 0);

                Vector2 jumpforce = (Vector3.up * Mathf.Sqrt(jumpHeight * -2.0f * Physics2D.gravity.y * rb.gravityScale));
                rb.velocity = jumpforce + Vector2.right * rb.velocity.x;

                jump = false;
                holding = true;
            }
        }
        else
        {
            if (Input.GetButtonUp("Jump") && holding)
            {
                holding = false;
            }
        }
    }

    void Dash()
    {
        if(Input.GetButtonDown("Dash") && canMove)
        {
            rb.velocity = Vector2.zero;
            rb.velocity = (transform.right * dashForce);
            canMove = false;
            dashing = true;
            anim.SetTrigger("Dash");
        }
    }

    void Rotate(float x)
    {
        if (canMove)
        {
            if (x > 0) transform.rotation = Quaternion.Euler(0, 0, 0);
            if (x < 0) transform.rotation = Quaternion.Euler(0, 180, 0);
        }
    }

    public void CantMove()
    {
        canMove = false;
    }
    public void CanMove()
    {
        canMove = true;
        dashing = false;
    }

    #region Attacks
    void Attack()
    {
        if (Input.GetButtonDown("Fire1")) 
        {
            slash = true;
            slashBuffer = 0.05f;
        }
        if(slash && isGrounded)
        {
            anim.SetTrigger("Slash");
            anim.SetInteger("SlashNr", slashNr);

            slash = false;
        }
    }
    public void StartSlash()
    {
        rb.velocity = new Vector2(0, rb.velocity.y);
    }
    public void Slash()
    {
        if (xInput > 0) transform.rotation = Quaternion.Euler(0, 0, 0);
        if (xInput < 0) transform.rotation = Quaternion.Euler(0, 180, 0);


        rb.AddForce(new Vector2(xInput * slashForce, 0));

        slashNr *= -1;
        resetSlashNr = 0.5f;

        StartCoroutine(SlashCol());

        slashSource.PlayOneShot(slashSound);
    }
    IEnumerator SlashCol()
    {
        slashCol.SetActive(true);
        yield return new WaitForSeconds(0.05f);
        slashCol.SetActive(false);
    }
    #endregion
}
