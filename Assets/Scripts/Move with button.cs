using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class MoveWithButton : MonoBehaviour
{
    public GameObject node1;
    public GameObject node2;
    public Rigidbody2D body;
    private GameObject nextNode;
    public float speed;
    public bool loop;
    public GameObject button;
    // Start is called before the first frame update
    void Start()
    {
        nextNode = node1;
        UnityEngine.Vector3 pos = transform.position;

        pos.x = Mathf.Round(pos.x / 8) * 8;
        pos.y = Mathf.Round(pos.y / 8) * 8;

        transform.position = pos;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (button.GetComponent<ButtonPress>().isPressed == true)
        {
            transform.position = UnityEngine.Vector3.MoveTowards(
           transform.position,
           nextNode.GetComponent<Transform>().position,
           speed * Time.fixedDeltaTime
       );

            if (UnityEngine.Vector3.Distance(transform.position, nextNode.GetComponent<Transform>().position) < 0.05f)
            {
                nextNode = (nextNode == node2) ? node1 : node2;
            }
        }

    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(transform);
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(null);
        }
    }

    void OnEnable()
    {
        GameEvents.OnPlayerDeath += ResetObject;
    }

    void OnDisable()
    {
        GameEvents.OnPlayerDeath -= ResetObject;
    }
    void ResetObject()
    {
        transform.position = node2.GetComponent<Transform>().position;
        button.GetComponent<ButtonPress>().isPressed = false;
    }
}
