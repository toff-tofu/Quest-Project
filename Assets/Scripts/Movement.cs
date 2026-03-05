using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.VisualScripting;
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
    public LayerMask movingBlockMask;
    public LayerMask blockMask;
    public LayerMask buttonMask;
    public LayerMask hazardMask;
    public bool facingRight = true;
    public Animator animator;

    //-------------------------------------------------------------------

    private bool grounded = false;
    private bool leftHanging = false;
    private bool rightHanging = false;
    private float walljumpXVel = 0;
    private float jumpSub = 1.2f;
    private Vector2 oldVel;
    private Vector3 oldPos;
    private float oldYVel;
    public Vector2 resPos;
    private Vector2 abilitySpeed = new Vector2(0, 0);
    public bool canDash = false;
    // private GameObject onMovingBlock = null;
    private Vector2 blockVel = new Vector2(0, 0);
    private float dashStartY;
    private float carrySpeed = 0;
    private float hVal;
    private float coyoteTime = 0.2f;
    private float coyoteTimeCounter = 0;
    private float jumpBufferTime = 0.2f;
    private float jumpBufferCounter = 0;
    private bool canTurn = false;
    private float deathDuration = 0.84f;
    // private bool falling = false;
    // private bool jumpRise = false;
    public Vector2 pPos = new Vector3();
    private CameraFollowObject _CameraFollowObject;
    [Header("Camera Follow Object")]
    [SerializeField] private GameObject cameraFollow;
    private float _fallSpeedYDampingChangeThreshold;

    [SerializeField] private float dashDuration = 0.2f;
    private Coroutine dashCoroutine;
    public bool usingCircle = false;
    public bool dashing = false;
    private CinemachineImpulseSource impulseSource;
    private Coroutine deathCoroutine;
    private bool invulnerable = false;
    private bool canMove = false;
    private float dashCooldown = 0.2f;
    private float dashCooldownTime = 0.2f;

    //-------------------------------------------------------------------

    void Start()
    {
        oldYVel = body.velocity.y;
        resPos = body.position;
        _CameraFollowObject = cameraFollow.GetComponent<CameraFollowObject>();
        _fallSpeedYDampingChangeThreshold = CameraManager.instance.FallSpeedYDampingThreshold;
        animator = GetComponentInChildren<Animator>();
        impulseSource = GetComponent<CinemachineImpulseSource>();
        canMove = true;
    }
    void Update()
    {
        if (!invulnerable)
        {
            CheckGrounded();
            TouchButton();
            CheckHanging();
            Jump();
            Ability();
        }
        if (body.velocity.y < _fallSpeedYDampingChangeThreshold && !CameraManager.instance.isLerpingY && !CameraManager.instance.lerpedFromPlayerFalling)
        {
            CameraManager.instance.LerpYDamping(true);
        }
        else if (body.velocity.y >= 0f && !CameraManager.instance.isLerpingY && CameraManager.instance.lerpedFromPlayerFalling)
        {
            CameraManager.instance.lerpedFromPlayerFalling = false;
            CameraManager.instance.LerpYDamping(false);
        }
    }
    void FixedUpdate()
    {
        pPos = gameObject.GetComponent<Transform>().position;
        if (canMove)
        {
            Move();
        }
        ApplyForces();
        CornerCorrect();
        CapSpeed();

        if (gameObject.GetComponent<Transform>().position.y < -30 && !invulnerable)
        {
            Die();
        }
        Collider2D[] overlaps2 = Physics2D.OverlapAreaAll(hurtBox.bounds.min, hurtBox.bounds.max, hazardMask);
        if (overlaps2.Length > 0 && !invulnerable)
        {
            Die();
        }
        if (Math.Abs(body.velocity.x) > 0.1f)
        {
            TurnCheck();
        }
        SetAnimators();
        dashCooldown -= Time.deltaTime;
        if (dashCooldown <= 0)
        {
            dashCooldown = 0f;
        }
        // body.position = new Vector2(RoundEigth(body.position.x), RoundEigth(body.position.y));
    }
    void TurnCheck()
    {
        if (Input.GetAxisRaw("Horizontal") > 0 && !facingRight)
        {
            Turn();
        }
        else if (Input.GetAxisRaw("Horizontal") < 0 && facingRight)
        {
            Turn();
        }
    }
    void Turn()
    {
        if (canTurn)
        {
            if (facingRight)
            {
                Vector3 rotator = new Vector3(transform.rotation.x, 180f, transform.rotation.z);
                transform.rotation = Quaternion.Euler(rotator);
                facingRight = !facingRight;

                _CameraFollowObject.CallTurn();
            }
            else
            {
                Vector3 rotator = new Vector3(transform.rotation.x, 0, transform.rotation.z);
                transform.rotation = Quaternion.Euler(rotator);
                facingRight = !facingRight;
                _CameraFollowObject.CallTurn();
            }
        }

    }
    private void CheckGrounded()
    {
        RaycastHit2D[] leftCols = Physics2D.RaycastAll(hitBox.bounds.center - new Vector3(hitBox.bounds.size.x / 2, 0, 0),
                                                        Vector2.down, 0.6f, LayerMask.GetMask("Block"));
        RaycastHit2D[] rightCols = Physics2D.RaycastAll(hitBox.bounds.center + new Vector3(hitBox.bounds.size.x / 2, 0, 0),
                                                        Vector2.down, 0.6f, LayerMask.GetMask("Block"));
        // grounded = leftCols.Length > 0 || rightCols.Length > 0;
        if (transform.parent != null)
        {
            grounded = true;

            // onMovingBlock =transform.parent.get
        }
        else if (leftCols.Length > 0 || rightCols.Length > 0)
        {
            grounded = true;
            // onMovingBlock = null;
        }
        else
        {
            grounded = false;
        }
        if (grounded)
        {
            canDash = true;
        }
        // Collider2D[] overlaps = Physics2D.OverlapAreaAll(hitBox.bounds.min, hitBox.bounds.max, movingBlockMask);
        // if (overlaps.Length == 0)
        // {
        //     onMovingBlock = null;
        // }
        // else
        // {
        //     foreach (Collider2D col in overlaps)
        //     {
        //         if (col.gameObject.layer == 11) // Layer 11 is "Moving Block"
        //         {
        //             grounded = true;
        //             onMovingBlock = col.gameObject;
        //             break;
        //         }
        //         else
        //         {
        //             onMovingBlock = null;
        //         }
        //     }
        // }
        // RaycastHit2D[] mLeftCols = Physics2D.RaycastAll(gameObject.GetComponent<Transform>().position - new Vector3(0.4f, 0, 0),
        //                                                 Vector2.down, 0.8f, LayerMask.GetMask("Moving Block"));
        // RaycastHit2D[] mRightCols = Physics2D.RaycastAll(gameObject.GetComponent<Transform>().position + new Vector3(0.4f, 0, 0),
        //                                                 Vector2.down, 0.8f, LayerMask.GetMask("Moving Block"));
        // onMovingBlock = mLeftCols.Length > 0 || mRightCols.Length > 0;


    }
    private void CheckHanging()
    {
        RaycastHit2D[] leftColsTop = Physics2D.RaycastAll(hitBox.bounds.center + new Vector3(0, hitBox.bounds.size.y / 2, 0),
                                                        Vector2.left, 0.55f, LayerMask.GetMask("Block"));
        RaycastHit2D[] leftColsBottom = Physics2D.RaycastAll(hitBox.bounds.center - new Vector3(0, hitBox.bounds.size.y / 2, 0),
                                                        Vector2.left, 0.55f, LayerMask.GetMask("Block"));
        RaycastHit2D[] rightColsTop = Physics2D.RaycastAll(hitBox.bounds.center + new Vector3(0, hitBox.bounds.size.y / 2, 0),
                                                        Vector2.right, 0.55f, LayerMask.GetMask("Block"));
        RaycastHit2D[] rightColsBottom = Physics2D.RaycastAll(hitBox.bounds.center - new Vector3(0, hitBox.bounds.size.y / 2, 0),
                                                        Vector2.right, 0.55f, LayerMask.GetMask("Block"));
        leftHanging = leftColsTop.Length > 0 && !facingRight || leftColsBottom.Length > 0 && !facingRight;
        rightHanging = rightColsTop.Length > 0 && facingRight || rightColsBottom.Length > 0 && facingRight;
        if (leftColsTop.Length > 0 && rightColsBottom.Length <= 0 && leftColsBottom.Length <= 0 || rightColsTop.Length > 0 && rightColsBottom.Length <= 0 && leftColsBottom.Length <= 0)
        {
            animator.SetBool("Wall Hanging", true);
        }
        else
        {
            animator.SetBool("Wall Hanging", false);
        }
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
            animator.SetBool("Falling", false);
            animator.SetBool("Falling Forward", false);
            animator.SetBool("Rising", false);
        }
        else
        {

            coyoteTimeCounter -= Time.deltaTime;
            if (body.velocity.y > 0)
            {
                animator.SetBool("Rising", true);
                animator.SetBool("Falling Forward", false);
                animator.SetBool("Falling", false);
            }
            else
            {
                animator.SetBool("Rising", false);
                if (hVal != 0)
                {
                    animator.SetBool("Falling Forward", true);
                    animator.SetBool("Falling", false);
                }
                else
                {
                    animator.SetBool("Falling", true);
                    animator.SetBool("Falling Forward", false);
                }
            }
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
            animator.SetTrigger("Jumped");
            body.velocity = new Vector2(body.velocity.x * 1.05f, JumpHeight);
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

        if (jumpBufferCounter > 0f && (leftHanging || rightHanging))
        {
            if (leftHanging)
            {
                // gameObject.GetComponent<ParticleSystem>().Emit(5);
                float oldXVel = body.velocity.x;
                body.velocity = new Vector3(oldXVel, JumpHeight, 0);
                walljumpXVel = MoveSpeed;
            }
            if (rightHanging)
            {
                // gameObject.GetComponent<ParticleSystem>().Emit(5);
                float oldXVel = body.velocity.x;
                body.velocity = new Vector3(oldXVel, JumpHeight, 0);
                walljumpXVel = -MoveSpeed;
            }
            animator.SetTrigger("Wall Jumped");
            Turn();
            canTurn = false;
            jumpBufferCounter = 0f;
        }
    }
    void Die()
    {
        invulnerable = true;
        GameEvents.PlayerDied();
        StopAllCoroutines();
        animator.SetTrigger("Die");
        deathCoroutine = StartCoroutine(Death());
        impulseSource.GenerateImpulse();
        dashing = false;
    }
    void Ability()
    {
        if (Input.GetKeyDown("x") && canDash && abilitySpeed.x == 0 && dashCooldown <= 0f)
        {
            beginDash();
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
        hVal = 0;
        if (walljumpXVel < 3 && walljumpXVel >= 0 || walljumpXVel > -3 && walljumpXVel <= 0)
        {
            canTurn = true;
            hVal = Input.GetAxisRaw("Horizontal") * MoveSpeed;
        }//Continue if holding direction away from wall
        else if (Input.GetAxisRaw("Horizontal") * MoveSpeed > walljumpXVel && facingRight)
        {
            hVal = Input.GetAxisRaw("Horizontal") * MoveSpeed;
            animator.SetTrigger("Wall Jump Not Turned");
            // animator.SetBool("Wall Jump Away", true);
            // animator.SetBool("Wall Jump Towards", false);
        }
        else if (Input.GetAxisRaw("Horizontal") * MoveSpeed < walljumpXVel && !facingRight)
        {
            hVal = Input.GetAxisRaw("Horizontal") * MoveSpeed;
            animator.SetTrigger("Wall Jump Not Turned");
            // animator.SetBool("Wall Jump Away", true);
            // animator.SetBool("Wall Jump Towards", false);
        }//Trigger animation if holding back to the wall
        else if (Input.GetAxisRaw("Horizontal") * MoveSpeed < walljumpXVel && facingRight && walljumpXVel != 0)
        {
            animator.SetTrigger("Wall Jump Turned");
            // animator.SetBool("Wall Jump Away", false);
            // animator.SetBool("Wall Jump Towards", true);
        }
        else if (Input.GetAxisRaw("Horizontal") * MoveSpeed > walljumpXVel && !facingRight && walljumpXVel != 0)
        {
            animator.SetTrigger("Wall Jump Turned");
            // animator.SetBool("Wall Jump Away", false);
            // animator.SetBool("Wall Jump Towards", true);
        }
        else if (Input.GetAxisRaw("Horizontal") * MoveSpeed == 0 && walljumpXVel != 0)
        {
            animator.SetTrigger("Wall Jump Neutral");
        }
        oldYVel = body.velocity.y;
        oldVel = body.velocity;
        oldPos = gameObject.GetComponent<Transform>().position;
        walljumpXVel /= jumpSub;
    }
    void ApplyForces()
    {
        // abilitySpeed = new Vector2(abilitySpeed.x / abilityDrag * xDrag, 0);
        // if (abilitySpeed.x < abilityControl && abilitySpeed.x > -abilityControl)
        // {
        //     abilitySpeed = new Vector2(0, 0);
        //     GetComponent<TrailRenderer>().emitting = false;
        // }
        // else
        // {
        //     // body.AddForce(new Vector2(0, -body.gravityScale * body.mass));
        //     GetComponent<TrailRenderer>().emitting = true;
        //     if (transform.position.y != dashStartY)
        //     {
        //         transform.position = new Vector3(transform.position.x, dashStartY, transform.position.z);
        //     }
        // }

        body.velocity = new Vector2(body.velocity.x / xDrag, body.velocity.y);
        if (body.velocity.y < -terminalVel)
        {
            body.velocity = new Vector2(body.velocity.x, -terminalVel);
        }
        // if (onMovingBlock != null)
        // {
        //     // blockVel = onMovingBlock.GetComponent<Rigidbody2D>().velocity;
        //     grounded = true;
        //     canDash = true;
        // }
        // else
        // {
        //     // blockVel = new Vector2(0, 0);
        // }

        body.AddForce(new Vector2((hVal + walljumpXVel) * acceleration, body.velocity.y));
        // body.velocity += blockVel / 2;
        body.AddForce(abilitySpeed);
        body.AddForce(new Vector2(carrySpeed, 0));
        carrySpeed /= 1.08f;
        oldVel = body.velocity;
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
    public void beginDash()
    {
        dashCoroutine = StartCoroutine(Dash());
        // GetComponent<TrailRenderer>().emitting = true;
        dashing = true;
    }
    private IEnumerator Dash()
    {
        float dashDirection = DetermineDashDirection();
        float startX = transform.position.x;
        float elapsedTime = 0f;
        float y = transform.position.y + 0.125f;
        bool shouldBeGrounded = grounded;
        impulseSource.GenerateImpulse();
        dashCooldown = dashCooldownTime;
        gameObject.transform.parent = null;
        while (elapsedTime < dashDuration)
        {
            elapsedTime += Time.deltaTime;
            float x = Mathf.Lerp(startX, dashDirection, elapsedTime / dashDuration);
            body.MovePosition(new Vector2(x, y));
            yield return null;
            if (dashDuration - elapsedTime < 0.2f && Input.GetKeyDown(KeyCode.UpArrow) && (leftHanging || rightHanging))
            {
                elapsedTime = dashDuration;
                // body.velocity = new Vector2(facingRight ? abilityPower : -abilityPower, 0);
                // GetComponent<TrailRenderer>().emitting = false;
                dashing = false;
                yield break;
            }
            if (dashDuration - elapsedTime < 0.2f && Input.GetKeyDown(KeyCode.UpArrow))
            {
                elapsedTime = dashDuration;
                body.velocity = new Vector2(facingRight ? 20 * abilityPower : 20 * -abilityPower, 8);
                // body.velocity = new Vector2(facingRight ? abilityPower : -abilityPower, 0);
                // GetComponent<TrailRenderer>().emitting = false;
                dashing = false;
                yield break;
            }
            if (dashDuration - elapsedTime < 0.2f && Input.GetKeyDown(KeyCode.DownArrow))
            {
                elapsedTime = dashDuration;
                body.velocity = new Vector2(facingRight ? 20 * abilityPower : 20 * -abilityPower, -8);
                // body.velocity = new Vector2(facingRight ? abilityPower : -abilityPower, 0);
                // GetComponent<TrailRenderer>().emitting = false;
                dashing = false;
                yield break;
            }
            if (dashDuration - elapsedTime < 0.2f && Input.GetKeyDown(KeyCode.RightArrow))
            {
                elapsedTime = dashDuration;
                body.velocity = new Vector2(50, 0);
                // body.velocity = new Vector2(facingRight ? abilityPower : -abilityPower, 0);
                // GetComponent<TrailRenderer>().emitting = false;
                dashing = false;
                yield break;
            }
            if (dashDuration - elapsedTime < 0.2f && Input.GetKeyDown(KeyCode.LeftArrow))
            {
                elapsedTime = dashDuration;
                body.velocity = new Vector2(-50, 0);
                // body.velocity = new Vector2(facingRight ? abilityPower : -abilityPower, 0);
                // GetComponent<TrailRenderer>().emitting = false;
                dashing = false;
                yield break;
            }
            // if (dashDuration - elapsedTime < 0.05f)
            // {

            // }
        }
        if (shouldBeGrounded && dashing)
        {
            body.velocity = new Vector2(facingRight ? abilityPower : -abilityPower, 0);
        }
        else
        {
            body.velocity = new Vector2(facingRight ? 10 * abilityPower : 10 * -abilityPower, 0);
        }

        // GetComponent<TrailRenderer>().emitting = false;
        dashing = false;
        dashCooldown = dashCooldownTime;
    }
    private IEnumerator Death()
    {
        // float elapsedTime = 0f;
        // float startX = transform.position.x;
        // float startY = transform.position.y;
        invulnerable = true;
        canMove = false;
        hitBox.enabled = false;
        hurtBox.enabled = false;
        GetComponentInChildren<SpriteRenderer>().sortingOrder = 50;
        StartCoroutine(GetComponent<TriggerGlobalAnimation>().Death());
        // float x = Mathf.Lerp(startX, resPos.x, elapsedTime / deathDuration);
        // float y = Mathf.Lerp(startY, resPos.y, elapsedTime / deathDuration);


        for (int i = 0; i < 20; i++)
        {
            Vector3 targetDirection = new Vector3(body.position.x - resPos.x, body.position.y - resPos.y, 0).normalized;
            body.MovePosition(new Vector2(body.position.x - targetDirection.x, body.position.y - targetDirection.y));
            yield return null;
        }
        yield return new WaitForSeconds(GetComponent<TriggerGlobalAnimation>().transitionTime);
        body.MovePosition(resPos);
        transform.position = new Vector3(resPos.x, resPos.y, 0);
        GetComponentInChildren<SpriteRenderer>().sortingOrder = 50;
        invulnerable = false;
        canMove = true;
        hitBox.enabled = true;
        hurtBox.enabled = true;
        body.velocity = new Vector2(0, 0);
    }
    private float DetermineDashDirection()
    {
        if (facingRight)
        {
            return transform.position.x + abilityPower;
        }
        else
        {
            return transform.position.x - abilityPower;
        }
    }
    void SetAnimators()
    {
        if (hVal != 0)
        {
            animator.SetBool("Moving", true);
        }
        else
        {
            animator.SetBool("Moving", false);
        }
        if (dashing)
        {
            animator.SetBool("Dashing", true);
        }
        else
        {
            animator.SetBool("Dashing", false);
        }
        if (leftHanging || rightHanging)
        {
            animator.SetBool("Wall Clinging", true);
        }
        else
        {
            animator.SetBool("Wall Clinging", false);
        }
    }
    float RoundEigth(float num)
    {
        return Mathf.Round(num * 8f) / 8f;
    }
    void CornerCorrect()
    {
        float length = 0.125f;
        float rayLength = Mathf.Abs(body.velocity.y * Time.fixedDeltaTime) + 0.02f;
        if (body.velocity.y > 0)
        {
            if (!Physics2D.Raycast(new Vector2(hitBox.bounds.min.x, hitBox.bounds.max.y) + Vector2.right * length, Vector2.up, rayLength, blockMask) && Physics2D.Raycast(new Vector3(hitBox.bounds.min.x, hitBox.bounds.max.y), Vector2.up, rayLength, blockMask))
            {
                // body.MovePosition(new Vector2(body.position.x, body.position.y + oldVel.y) + Vector2.right * 0.125f);
                body.MovePosition(body.position + Vector2.right * length);
                return;
            }
            if (!Physics2D.Raycast(new Vector2(hitBox.bounds.max.x, hitBox.bounds.max.y) + Vector2.left * length, Vector2.up, rayLength, blockMask) && Physics2D.Raycast(new Vector3(hitBox.bounds.max.x, hitBox.bounds.max.y), Vector2.up, rayLength, blockMask))
            {
                body.MovePosition(body.position + Vector2.left * length);
                // body.MovePosition(new Vector2(body.position.x, body.position.y + oldVel.y) + Vector2.left * 0.125f);
                return;
            }
        }
        Debug.DrawRay(new Vector2(hitBox.bounds.min.x, hitBox.bounds.max.y), Vector2.up * rayLength, Color.red);
        Debug.DrawRay(new Vector2(hitBox.bounds.max.x, hitBox.bounds.max.y), Vector2.up * rayLength, Color.blue);
    }


}