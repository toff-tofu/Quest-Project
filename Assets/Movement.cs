using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    // Start is called before the first frame update
    public float maxSpeed;
    public float MoveSpeed;
    public float JumpHeight;
    private bool grounded = false;
    private bool leftHanging = false;
    private bool rightHanging = false;
    private float jumpXVel = 0;
    private float jumpSub = 1.2f;
    void Start()
    {
        // jumpSub = MoveSpeed / 7;
    }

    void Update()
    {
        CheckGrounded();
        CheckHanging();
        if (grounded)
        {
            leftHanging = false;
            rightHanging = false;
        }
        if (Input.GetKeyDown(KeyCode.Space)||Input.GetKeyDown(KeyCode.W)||Input.GetKeyDown(KeyCode.UpArrow))
        {
            Jump();
        }
        
        
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
    }

    void Jump()
    {
        if (grounded)
        {
            gameObject.GetComponent<ParticleSystem>().Emit(10);
            float oldXVel = gameObject.GetComponent<Rigidbody2D>().velocity.x;
            gameObject.GetComponent<Rigidbody2D>().velocity = new Vector3(oldXVel, JumpHeight, 0);
            print("You Jumped Off The Ground");
        }
        if (leftHanging)
        {
            gameObject.GetComponent<ParticleSystem>().Emit(5);
            float oldXVel = gameObject.GetComponent<Rigidbody2D>().velocity.x;
            gameObject.GetComponent<Rigidbody2D>().velocity = new Vector3(oldXVel, JumpHeight, 0);
            jumpXVel = MoveSpeed;
        }
        if (rightHanging)
        {
            gameObject.GetComponent<ParticleSystem>().Emit(5);
            float oldXVel = gameObject.GetComponent<Rigidbody2D>().velocity.x;
            gameObject.GetComponent<Rigidbody2D>().velocity = new Vector3(oldXVel, JumpHeight, 0);
            jumpXVel = -MoveSpeed;
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        //acceleration based
        // float hVal = Input.GetAxisRaw("Horizontal") * MoveSpeed;
        // gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(hVal, 0));
        //direction based
        float hVal = 0;
        if (jumpXVel < 3 && jumpXVel >= 0 || jumpXVel > -3 && jumpXVel <= 0)
        {
            hVal = Input.GetAxisRaw("Horizontal") * MoveSpeed;
        }
        float oldYVel = gameObject.GetComponent<Rigidbody2D>().velocity.y;
        Vector2 oldVel = gameObject.GetComponent<Rigidbody2D>().velocity;
        Vector3 oldPos = gameObject.GetComponent<Transform>().position;
        gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(hVal + jumpXVel, oldYVel);
        jumpXVel /= jumpSub;

        if (gameObject.GetComponent<Rigidbody2D>().velocity != oldVel && grounded)
        {
            // gameObject.GetComponent<AudioSource>().Play();
            // gameObject.GetComponent<ParticleSystem>().Emit(1);
        }
        if (gameObject.GetComponent<Rigidbody2D>().velocity == oldVel && grounded)
        {
            // gameObject.GetComponent<AudioSource>().Stop();
            // gameObject.GetComponent<ParticleSystem>().Emit(1);
        }
        if (gameObject.GetComponent<Rigidbody2D>().velocity.x >= maxSpeed)
        {
            // float oldYVel = gameObject.GetComponent<Rigidbody2D>().velocity.y;
            gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(maxSpeed, oldYVel);
        }
        if (gameObject.GetComponent<Rigidbody2D>().velocity.x <= -maxSpeed)
        {
            // float oldYVel = gameObject.GetComponent<Rigidbody2D>().velocity.y;
            gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(-maxSpeed, oldYVel);
        }
        if (gameObject.GetComponent<Transform>().position.y < -5)
        {
            gameObject.GetComponent<Rigidbody2D>().position = new Vector2(0,0);
        }
    }
}
