using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class Move : MonoBehaviour
{
    public GameObject node1;
    public GameObject node2;
    public Rigidbody2D body;
    private GameObject nextNode;
    public float speed;
    public bool loop;
    // Start is called before the first frame update
    void Start()
    {
        nextNode = node1;
    }

    // Update is called once per frame
    void Update()
    {
        // body.velocity = new Vector2((transform.position.x - nextNode.transform.position.x) / speed,
        //                             (transform.position.y - nextNode.transform.position.y) / speed);
        UnityEngine.Vector2 direction = new UnityEngine.Vector2(nextNode.transform.position.x - transform.position.x,
                                        nextNode.transform.position.y - transform.position.y);
        direction = direction.normalized;
        body.velocity = direction * speed;
        // body.position -= new Vector2((transform.position.x - nextNode.transform.position.x) / speed,
        //                              (transform.position.y - nextNode.transform.position.y) / speed) / 500;
        if (UnityEngine.Vector2.Distance(transform.position, nextNode.transform.position) < 0.1f)
        {
            body.MovePosition(nextNode.transform.position);
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
