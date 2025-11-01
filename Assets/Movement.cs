using System.Collections;
using System.Collections.Generic;
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
    //-------------------------------------------------------------------

    private bool grounded = false;
    private bool leftHanging = false;
    private bool rightHanging = false;
    private float walljumpXVel = 0;
    private float jumpSub = 1.2f;
    private Vector2 oldVel;
    private Vector3 oldPos;
    private float oldYVel;
    private Vector2 abilitySpeed = new Vector2(0, 0);
    private bool canDash = false;
    private float onMovingBlock = false;
    // private float trail;
    //-------------------------------------------------------------------

    void Start()
    {
        oldYVel = body.velocity.y;
        // trail = GetComponent<TrailRenderer>().time;
        // trail = 0;
    }
    void Update()
    {
        CheckGrounded();
        CheckHanging();
        Jump();
        Ability();
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        Move();
        ApplyForces();
        CapSpeed();
        Die();
    }
    private void CheckGrounded()
    {
        RaycastHit2D[] leftCols = Physics2D.RaycastAll(gameObject.GetComponent<Transform>().position - new Vector3(0.4f, 0, 0),
                                                        Vector2.down, 0.8f, LayerMask.GetMask("Block"));
        RaycastHit2D[] rightCols = Physics2D.RaycastAll(gameObject.GetComponent<Transform>().position + new Vector3(0.4f, 0, 0),
                                                        Vector2.down, 0.8f, LayerMask.GetMask("Block"));
        grounded = leftCols.Length > 0 || rightCols.Length > 0;
        if (grounded)
        {
            canDash = true;
        }


        RaycastHit2D[] mLeftCols = Physics2D.RaycastAll(gameObject.GetComponent<Transform>().position - new Vector3(0.4f, 0, 0),
                                                        Vector2.down, 0.8f, LayerMask.GetMask("Moving Block"));
        RaycastHit2D[] mRightCols = Physics2D.RaycastAll(gameObject.GetComponent<Transform>().position + new Vector3(0.4f, 0, 0),
                                                        Vector2.down, 0.8f, LayerMask.GetMask("Moving Block"));
        onMovingBlock = mLeftCols.Length > 0 || mRightCols.Length > 0;
        if (onMovingBlock)
        {

            //NEED TO WRITE CODE TO FIND BLOCK THAT YOU ARE ON AND MATCH ITS VELOCITY
            body.velocity = new Vector2(body.velocity.x, 0);
            canDash = true;
        }
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
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (grounded)
            {
                gameObject.GetComponent<ParticleSystem>().Emit(10);
                float oldXVel = body.velocity.x;
                body.velocity = new Vector3(oldXVel, JumpHeight, 0);
                print("You Jumped Off The Ground");
            }
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
            body.position = new Vector2(0, 0);
        }
    }
    void Ability()
    {
        if (Input.GetKeyDown("x") && canDash)
        {
            body.velocity = new Vector2(0, 0);
            abilitySpeed = new Vector2(abilityPower * Input.GetAxisRaw("Horizontal"), 0);
            canDash = false;
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
        //acceleration based
        // float hVal = Input.GetAxisRaw("Horizontal") * MoveSpeed;
        // gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(hVal, 0));
        //direction based
        float hVal = 0;
        if (walljumpXVel < 3 && walljumpXVel >= 0 || walljumpXVel > -3 && walljumpXVel <= 0)
        {
            hVal = Input.GetAxisRaw("Horizontal") * MoveSpeed;
        }
        oldYVel = body.velocity.y;
        oldVel = body.velocity;
        oldPos = gameObject.GetComponent<Transform>().position;
        // if (abilitySpeed.x <= abilityControl && abilitySpeed.x >= -abilityControl && abilitySpeed.y <= abilityControl && abilitySpeed.y >= -abilityControl)
        // {
        body.AddForce(new Vector2((hVal + walljumpXVel) * acceleration, body.velocity.y));
        // }
        // else
        // {
        //     body.drag = abilityDrag;
        // }
        body.AddForce(abilitySpeed);
        walljumpXVel /= jumpSub;
    }
    void ApplyForces()
    {
        abilitySpeed = new Vector2(abilitySpeed.x / abilityDrag * xDrag, 0);
        if (abilitySpeed.x < abilityControl && abilitySpeed.x > -abilityControl)
        {
            abilitySpeed = new Vector2(0, body.velocity.y);
            GetComponent<TrailRenderer>().emitting = false;
        }
        else
        {
            GetComponent<TrailRenderer>().emitting = true;
        }

        body.velocity = new Vector2(body.velocity.x / xDrag, body.velocity.y);
        if (body.velocity.y < -terminalVel)
        {
            body.velocity = new Vector2(body.velocity.x, -terminalVel);
        }
        if (onMovingBlock)
        {
            body.velocity = new Vector2(body.velocity.x, 0);
        }
    }
}
