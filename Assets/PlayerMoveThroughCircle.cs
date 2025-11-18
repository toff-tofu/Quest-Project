using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class PlayerMoveThroughCircle : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private GameObject player;
    private Rigidbody2D playerBody;
    private bool _insideCircle = false;
    private UnityEngine.Vector2 startPosition;
    [SerializeField] private AreaEffector2D _areaEffector;
    private float _pxdrag;
    private float _pgravity;
    [SerializeField] private CircleCollider2D hitBox;
    [SerializeField] private LayerMask playerMask;
    private Coroutine circleCoroutine;
    private float circleDuration = 0.3f;
    void Start()
    {
        playerBody = player.GetComponent<Rigidbody2D>();
        _pxdrag = player.GetComponent<Movement>().xDrag;
        _pgravity = playerBody.gravityScale;
    }

    // Update is called once per frame
    void ResetCoroutine()
    {
        if (circleCoroutine != null)
        {
            StopCoroutine(circleCoroutine);
        }
        circleCoroutine = StartCoroutine(CircleDash());
    }
    void Update()
    {
        // if (_insideCircle) { preIn = true; } else { preIn = false; }
        // CheckCollisions();

        if (_insideCircle)
        {

            // transform.LookAt(new Vector2(startPosition.x, startPosition.y));
            if (Input.GetKeyDown("c"))
            {
                circleCoroutine = StartCoroutine(CircleDash());
            }
            // if (!Input.GetKeyDown("c"))
            // {
            //     ResetCoroutine();
            // }
        }
        else
        {
            // _areaEffector.forceMagnitude = 0f;
            // player.GetComponent<Movement>().xDrag = _pxdrag;
            // playerBody.gravityScale = _pgravity;
        }
    }
    void CheckCollisions()
    {
        // if ((player.gameObject.transform.position.x - transform.position.x) * *2 + (player.gameObject.transform.position.y - transform.position.y) * *2 < (gameObject.GetComponent<CircleCollider2D>().radius * transform.localScale.x) * *2)
        // {
        //     _insideCircle = true;
        // }
        // else
        // {
        //     _insideCircle = false;
        // }
        Collider2D[] overlaps = Physics2D.OverlapAreaAll(hitBox.bounds.min, hitBox.bounds.max, playerMask);
        if (overlaps.Length == 0)
        {
            _insideCircle = false;
            // if (preIn == false)
            // {
            //     startPosition = player.transform.position;
            //     preIn = true;
            // }
        }
        else
        {
            _insideCircle = true;
        }
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            _insideCircle = true;
            // startPosition = player.transform.position;
        }
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            _insideCircle = false;
        }
    }
    public void beginCircle()
    {
        circleCoroutine = StartCoroutine(CircleDash());
    }
    private IEnumerator CircleDash()
    {
        UnityEngine.Vector2 end = FindEndPosition();
        UnityEngine.Vector2 start = playerBody.position;
        UnityEngine.Vector2 slope = FindSlope();
        float elapsedTime = 0f;
        while (elapsedTime < circleDuration)
        {
            elapsedTime += Time.deltaTime;
            playerBody.MovePosition(UnityEngine.Vector2.Lerp(start, end, elapsedTime / circleDuration));
            yield return null;
            if (circleDuration - elapsedTime < 0.05f)
            {
                playerBody.velocity = slope.normalized * gameObject.GetComponent<CircleCollider2D>().radius;
            }
        }
    }
    private UnityEngine.Vector2 FindSlope()
    {
        startPosition.x = player.transform.position.x;
        startPosition.y = player.transform.position.y;
        UnityEngine.Vector2 slopeVector = new UnityEngine.Vector2(startPosition.x - transform.position.x, startPosition.y - transform.position.y);
        slopeVector *= -1;
        slopeVector = slopeVector.normalized;
        slopeVector *= gameObject.GetComponent<CircleCollider2D>().radius * transform.localScale.x * 1.3f;
        return slopeVector;
    }
    private UnityEngine.Vector2 FindEndPosition()
    {
        UnityEngine.Vector2 findPoint = FindSlope() + new UnityEngine.Vector2(transform.position.x, transform.position.y);
        return findPoint;
    }
}
