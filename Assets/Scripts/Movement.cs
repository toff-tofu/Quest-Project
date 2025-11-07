using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShortcutManagement;
using UnityEngine;


public class Movement : MonoBehaviour
{
    public float maxSpeed;
    public float MoveSpeed;
    public float JumpHeight;
    public Rigidbody2D body;
    public float abilityPower;
    public float acceleration;
    public float abilityDrag;
    public float abilityControl;
    public float xDrag;
    public float terminalVel;
    public Collider2D hitBox;
    public Collider2D hurtBox;
    public LayerMask groundMask;
    public LayerMask buttonMask;
    public LayerMask hazardMask;
    //-------------------------------------------------------------------

    private bool grounded = false;
    private bool leftHanging = false;
    private bool rightHanging = false;
    private float walljumpXVel = 0;
    private float jumpSub = 1.2f;
    private Vector2 oldVel;
    private Vector3 oldPos;
    private float oldYVel;
    private Vector2 resPos;
    private Vector2 abilitySpeed = new Vector2(0, 0);
    private bool canDash = false;
    private GameObject onMovingBlock = null;
    private Vector2 blockVel = new Vector2(0, 0);
    private float dashStartY;
    private float carrySpeed = 0;
    private float hVal;
    private float coyoteTime = 0.2f;
    private float coyoteTimeCounter = 0;
    private float jumpBufferTime = 0.2f;
    private float jumpBufferCounter = 0;
    public Vector2 pPos = new Vector3();
    //-------------------------------------------------------------------

    void Start()
    {
        oldYVel = body.velocity.y;
        resPos = body.position;
    }
    void Update()
    {

        CheckGrounded();
        TouchButton();
        CheckHanging();
        Jump();
        Ability();

    }
    void FixedUpdate()
    {
        pPos = gameObject.GetComponent<Transform>().position;
        Move();
        ApplyForces();
        CapSpeed();
        Die();
    }
    private void CheckGrounded()
    {
        RaycastHit2D[] leftCols = Physics2D.RaycastAll(gameObject.GetComponent<Transform>().position - new Vector3(0.4f, 0, 0),
                                                        Vector2.down, 0.6f, LayerMask.GetMask("Block"));
        RaycastHit2D[] rightCols = Physics2D.RaycastAll(gameObject.GetComponent<Transform>().position + new Vector3(0.4f, 0, 0),
                                                        Vector2.down, 0.6f, LayerMask.GetMask("Block"));
        grounded = leftCols.Length > 0 || rightCols.Length > 0;
        if (grounded)
        {
            canDash = true;
        }
        Collider2D[] overlaps = Physics2D.OverlapAreaAll(hitBox.bounds.min, hitBox.bounds.max, groundMask);
        if (overlaps.Length == 0)
        {
            onMovingBlock = null;
        }
        else
        {
            foreach (Collider2D col in overlaps)
            {
                if (col.gameObject.layer == 11) // Layer 11 is "Moving Block"
                {
                    grounded = true;
                    onMovingBlock = col.gameObject;
                    break;
                }
                else
                {
                    onMovingBlock = null;
                }
            }
        }
        // RaycastHit2D[] mLeftCols = Physics2D.RaycastAll(gameObject.GetComponent<Transform>().position - new Vector3(0.4f, 0, 0),
        //                                                 Vector2.down, 0.8f, LayerMask.GetMask("Moving Block"));
        // RaycastHit2D[] mRightCols = Physics2D.RaycastAll(gameObject.GetComponent<Transform>().position + new Vector3(0.4f, 0, 0),
        //                                                 Vector2.down, 0.8f, LayerMask.GetMask("Moving Block"));
        // onMovingBlock = mLeftCols.Length > 0 || mRightCols.Length > 0;


    }
    private void CheckHanging()
    {
        RaycastHit2D[] leftColsTop = Physics2D.RaycastAll(gameObject.GetComponent<Transform>().position + new Vector3(0, 0.5f, 0),
                                                        Vector2.left, 0.6f, LayerMask.GetMask("Block"));
        RaycastHit2D[] leftColsBottom = Physics2D.RaycastAll(gameObject.GetComponent<Transform>().position - new Vector3(0, 0.5f, 0),
                                                        Vector2.left, 0.6f, LayerMask.GetMask("Block"));
        RaycastHit2D[] rightColsTop = Physics2D.RaycastAll(gameObject.GetComponent<Transform>().position + new Vector3(0, 0.5f, 0),
                                                        Vector2.right, 0.6f, LayerMask.GetMask("Block"));
        RaycastHit2D[] rightColsBottom = Physics2D.RaycastAll(gameObject.GetComponent<Transform>().position - new Vector3(0, 0.5f, 0),
                                                        Vector2.right, 0.6f, LayerMask.GetMask("Block"));
        leftHanging = leftColsTop.Length > 0 || leftColsBottom.Length > 0;
        rightHanging = rightColsTop.Length > 0 || rightColsBottom.Length > 0;
        if (leftHanging || rightHanging)
        {
            canDash = true;
        }
        if (grounded)
        {
            leftHanging = false;
            rightHanging = false;
        }
    }
    void Jump()
    {
        if (grounded)
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        if (jumpBufferCounter > 0f && coyoteTimeCounter > 0f)
        {
            body.velocity = new Vector2(body.velocity.x, JumpHeight);
            if (Math.Abs(abilitySpeed.x) > abilityControl)
            {
                carrySpeed = abilitySpeed.x / 2;
                abilitySpeed = new Vector2(0, 0);
            }

            jumpBufferCounter = 0f;
        }
        if (Input.GetKeyUp(KeyCode.UpArrow) && body.velocity.y > 0f)
        {
            body.velocity = new Vector2(body.velocity.x, body.velocity.y * 0.5f);

            coyoteTimeCounter = 0f;
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (leftHanging)
            {
                gameObject.GetComponent<ParticleSystem>().Emit(5);
                float oldXVel = body.velocity.x;
                body.velocity = new Vector3(oldXVel, JumpHeight, 0);
                walljumpXVel = MoveSpeed;
            }
            if (rightHanging)
            {
                gameObject.GetComponent<ParticleSystem>().Emit(5);
                float oldXVel = body.velocity.x;
                body.velocity = new Vector3(oldXVel, JumpHeight, 0);
                walljumpXVel = -MoveSpeed;
            }
        }
    }
    void Die()
    {
        if (gameObject.GetComponent<Transform>().position.y < -30)
        {
            body.position = resPos;
        }
        Collider2D[] overlaps2 = Physics2D.OverlapAreaAll(hitBox.bounds.min, hitBox.bounds.max, hazardMask);
        if (overlaps2.Length > 0)
        {
            body.position = resPos;
        }
    }
    void Ability()
    {
        if (Input.GetKeyDown("x") && canDash)
        {
            body.velocity = new Vector2(0, 0);
            abilitySpeed = new Vector2(abilityPower * Input.GetAxisRaw("Horizontal"), 0);
            canDash = false;
            dashStartY = transform.position.y;
        }

    }
    void CapSpeed()
    {
        // if (body.velocity.x >= maxSpeed)
        // {
        //     // float oldYVel = body.velocity.y;
        //     body.velocity = new Vector2(maxSpeed, oldYVel);
        // }
        // if (body.velocity.x <= -maxSpeed)
        // {
        //     // float oldYVel = body.velocity.y;
        //     body.velocity = new Vector2(-maxSpeed, oldYVel);
        // }

    }
    void Move()
    {
        hVal = 0;
        if (walljumpXVel < 3 && walljumpXVel >= 0 || walljumpXVel > -3 && walljumpXVel <= 0)
        {
            hVal = Input.GetAxisRaw("Horizontal") * MoveSpeed;
        }
        oldYVel = body.velocity.y;
        oldVel = body.velocity;
        oldPos = gameObject.GetComponent<Transform>().position;
        walljumpXVel /= jumpSub;
    }
    void ApplyForces()
    {
        abilitySpeed = new Vector2(abilitySpeed.x / abilityDrag * xDrag, 0);
        if (abilitySpeed.x < abilityControl && abilitySpeed.x > -abilityControl)
        {
            abilitySpeed = new Vector2(0, 0);
            GetComponent<TrailRenderer>().emitting = false;
        }
        else
        {
            // body.AddForce(new Vector2(0, -body.gravityScale * body.mass));
            GetComponent<TrailRenderer>().emitting = true;
            if (transform.position.y != dashStartY)
            {
                transform.position = new Vector3(transform.position.x, dashStartY, transform.position.z);
            }
        }

        body.velocity = new Vector2(body.velocity.x / xDrag, body.velocity.y);
        if (body.velocity.y < -terminalVel)
        {
            body.velocity = new Vector2(body.velocity.x, -terminalVel);
        }
        if (onMovingBlock != null)
        {
            blockVel = onMovingBlock.GetComponent<Rigidbody2D>().velocity;
            grounded = true;
            canDash = true;
            print(blockVel);
        }
        else
        {
            blockVel = new Vector2(0, 0);
        }
        body.AddForce(new Vector2((hVal + walljumpXVel) * acceleration, body.velocity.y));
        body.velocity += blockVel / 2;
        body.AddForce(abilitySpeed);
        body.AddForce(new Vector2(carrySpeed, 0));
        carrySpeed /= 1.08f;
    }
    void TouchButton()
    {
        Collider2D[] overlaps1 = Physics2D.OverlapAreaAll(hitBox.bounds.min, hitBox.bounds.max, buttonMask);
        foreach (Collider2D col in overlaps1)
        {
            print("Your touching smth");
            if (col.gameObject.tag == "Moving Block Button")
            {
                print("You have found the button");
                col.gameObject.GetComponent<ButtonPress>().isPressed = true;
                Destroy(col.gameObject.GetComponent<Collider2D>());
            }
        }
    }
}
