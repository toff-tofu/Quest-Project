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
    private bool _moving = false;
    void Start()
    {
        playerBody = player.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_insideCircle)
        {
            if (Input.GetKey("c"))
            {
                _moving = true;
            }
            if (_moving == true)
            {
                Vector2 toCenter = new Vector2(transform.position.x - startPosition.x,
                                               transform.position.y - startPosition.y).normalized;
                playerBody.AddForce(toCenter * GetComponent<CircleCollider2D>().radius * 50);
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            _insideCircle = true;
            startPosition = playerBody.transform.position;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            _insideCircle = false;
            _moving = false;
            // startPosition = Vector2.zero;
        }
    }
}
