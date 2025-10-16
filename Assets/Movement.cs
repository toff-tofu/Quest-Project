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
    public float groundDrag;
    public float airDrag;
//-------------------------------------------------------------------

    private bool grounded = false;
    private bool leftHanging = false;
    private bool rightHanging = false;
    private float walljumpXVel = 0;
    private float jumpSub = 1.2f;
    private Vector2 oldVel;
    private Vector3 oldPos;
    private float oldYVel;
    private Vector2 abilitySpeed = new Vector2(0,0);
//-------------------------------------------------------------------

    void Start()
    {
        oldYVel = body.velocity.y;
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
            print("You Hit The Ground");
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
        if (grounded)
        {
            leftHanging = false;
            rightHanging = false;
        }
    }

    
    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space)||Input.GetKeyDown(KeyCode.W)||Input.GetKeyDown(KeyCode.UpArrow)){
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
    void Die(){
        if (gameObject.GetComponent<Transform>().position.y < -5)
        {
            body.position = new Vector2(0,0);
        }
    }
    void Ability(){
        if (Input.GetKeyDown("x")){
            print("You Pressed Space");
            abilitySpeed = new Vector2(abilityPower*Input.GetAxisRaw("Horizontal"), abilityPower*Input.GetAxisRaw("Vertical"));
        }   
        
    }
    void CapSpeed(){
        if (body.velocity.x >= maxSpeed)
        {
            // float oldYVel = body.velocity.y;
            body.velocity = new Vector2(maxSpeed, oldYVel);
        }
        if (body.velocity.x <= -maxSpeed)
        {
            // float oldYVel = body.velocity.y;
            body.velocity = new Vector2(-maxSpeed, oldYVel);
        }
        
    }
    void Move(){
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
        body.AddForce(new Vector2((hVal + walljumpXVel)*acceleration, oldYVel));
        body.AddForce(abilitySpeed);
        walljumpXVel /= jumpSub;
        if (body.velocity != oldVel && grounded)
        {
            // gameObject.GetComponent<AudioSource>().Play();
            // gameObject.GetComponent<ParticleSystem>().Emit(1);
        }
        if (body.velocity == oldVel && grounded)
        {
            // gameObject.GetComponent<AudioSource>().Stop();
            // gameObject.GetComponent<ParticleSystem>().Emit(1);
        }
    }
    void ApplyForces(){
        abilitySpeed = new Vector2(abilitySpeed.x/1.15f, abilitySpeed.y/1.15f);
        if (abilitySpeed.x < 0.5f && abilitySpeed.x > -0.5f){
            abilitySpeed = new Vector2(0,abilitySpeed.y);
        }
        if (abilitySpeed.y < 0.5f && abilitySpeed.y > -0.5f){
            abilitySpeed = new Vector2(abilitySpeed.x,0);
        }
        if (grounded&&abilitySpeed.x==0){
            body.drag = groundDrag;
        } else {
            body.drag = airDrag;
        }
    }
}
