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
    }

    // Update is called once per frame
    void Update()
    {
        if (button.GetComponent<ButtonPress>().isPressed == true)
        {
            UnityEngine.Vector2 direction = new UnityEngine.Vector2(nextNode.transform.position.x - transform.position.x,
                                        nextNode.transform.position.y - transform.position.y);
            direction = direction.normalized;
            body.velocity = direction * speed;
            if (UnityEngine.Vector2.Distance(transform.position, nextNode.transform.position) < 0.1f)
            {
                transform.position = nextNode.transform.position;
                body.velocity = new UnityEngine.Vector2(0, 0);
                if (loop)
                {
                    if (nextNode == node1)
                    {
                        nextNode = node2;
                    }
                    else
                    {
                        nextNode = node1;
                    }
                }

            }
        }

    }
}
