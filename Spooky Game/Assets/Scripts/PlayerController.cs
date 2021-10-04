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

    bool isGrounded;
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

        xInput = Input.GetAxisRaw("Horizontal");

        Rotate(xInput);

        Attack();

        if (jump) jumpBuffer -= Time.deltaTime;
        if (jumpBuffer <= 0) jump = false;
        if (slash) slashBuffer -= Time.deltaTime;
        if (slashBuffer <= 0) slash = false;

        resetSlashNr -= Time.deltaTime;
        if (resetSlashNr <= 0) slashNr = 1;
    }
    private void FixedUpdate()
    {
        Move(xInput);

        if (clamp) ClampVel(xInput);
    }
    void Move(float x)
    {
        if (canMove) rb.AddForce(Vector2.right * x * speedUp);
        else rb.velocity = new Vector2(0, rb.velocity.y);
    }
    void ClampVel(float x)
    {
        Vector2 velocity = new Vector2(rb.velocity.x, 0);
        Vector2 vel = Vector2.ClampMagnitude(velocity, speed);
        rb.velocity = new Vector2(vel.x, rb.velocity.y);

        if(isGrounded)
        {
            if(x == 0) rb.velocity = new Vector2(0, rb.velocity.y);
        }
        else
        {
            if (x == 0) rb.velocity *= new Vector2(0.95f, 1);

            if (rb.velocity.y > 0 && !holding) rb.velocity *= new Vector2(1, 0.9f);
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
            if (jump)
            {
                rb.velocity = new Vector2(rb.velocity.x, 0);

                Vector2 jumpforce = (Vector3.up * Mathf.Sqrt(jumpHeight * -2.0f * Physics2D.gravity.y * rb.gravityScale));
                rb.velocity += jumpforce;

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
    public void Slash()
    {
        slashNr *= -1;
        resetSlashNr = 0.5f;
    }
    #endregion
}
