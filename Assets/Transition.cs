using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transition : MonoBehaviour
{
    // Start is called before the first frame update
    private Camera mainCamera;
    public GameObject visual;
    void Start()
    {
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            mainCamera.transform.position = visual.transform.position + new Vector3(0, 0, -10);
            if (other.gameObject.GetComponent<Rigidbody2D>().velocity.x > 0)
            {
                other.gameObject.transform.position += new Vector3(1, 0, 0);
            }
            if (other.gameObject.GetComponent<Rigidbody2D>().velocity.x < 0)
            {
                other.gameObject.transform.position -= new Vector3(1, 0, 0);
            }
        }
    }
}
