using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transition : MonoBehaviour
{
    // Start is called before the first frame update
    private Camera mainCamera;
    public GameObject visual;
    public GameObject player;
    public float size = 0.5f;
    void Start()
    {
        mainCamera = Camera.main;
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (player.GetComponent<Rigidbody2D>().velocity.x > 0 &&
        player.GetComponent<Movement>().pPos.x < transform.position.x - (size / 2) &&
        player.transform.position.x >= transform.position.x - (size / 2))
        {
            mainCamera.transform.position = visual.transform.position + new Vector3(0, 0, -10);
            player.transform.position += new Vector3(1, 0, 0);
        }
        if (player.GetComponent<Rigidbody2D>().velocity.x < 0 &&
        player.GetComponent<Movement>().pPos.x > transform.position.x + (size / 2) &&
        player.transform.position.x <= transform.position.x + (size / 2))
        {
            mainCamera.transform.position = visual.transform.position + new Vector3(0, 0, -10);
            player.transform.position -= new Vector3(1, 0, 0);
        }
    }
}
