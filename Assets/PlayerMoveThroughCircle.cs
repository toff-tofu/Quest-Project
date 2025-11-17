using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveThroughCircle : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private GameObject player;
    private Rigidbody2D playerBody;
    private bool _insideCircle = false;
    private Vector2 startPosition;
    [SerializeField] private AreaEffector2D _areaEffector;
    private float _pxdrag;
    private float _pgravity;
    [SerializeField] private CircleCollider2D hitBox;
    [SerializeField] private LayerMask playerMask;
    void Start()
    {
        playerBody = player.GetComponent<Rigidbody2D>();
        _pxdrag = player.GetComponent<Movement>().xDrag;
        _pgravity = playerBody.gravityScale;
    }

    // Update is called once per frame
    void Update()
    {
        // if (_insideCircle) { preIn = true; } else { preIn = false; }
        CheckCollisions();

        if (_insideCircle)
        {
            _areaEffector.forceAngle = Mathf.Atan2(startPosition.y - transform.position.y, startPosition.x - transform.position.x) * Mathf.Rad2Deg;
            // transform.LookAt(new Vector2(startPosition.x, startPosition.y));
            if (Input.GetKey("c"))
            {
                _areaEffector.forceMagnitude = -200f;
                player.GetComponent<Movement>().xDrag = 0f;
                playerBody.gravityScale = 0f;
            }
            else
            {
                _areaEffector.forceMagnitude = 0f;
                player.GetComponent<Movement>().xDrag = _pxdrag;
                playerBody.gravityScale = _pgravity;
            }
        }
        else
        {
            _areaEffector.forceMagnitude = 0f;
            player.GetComponent<Movement>().xDrag = _pxdrag;
            playerBody.gravityScale = _pgravity;
        }
    }
    void CheckCollisions()
    {
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
        if (collision.tag == "Player")
        {
            startPosition = player.transform.position;
        }
    }
}
