using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Transition : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Camera mainCamera;
    public CompositeCollider2D room;
    public GameObject player;
    void Start()
    {
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        // Smoothly follow the player when far enough
        float distance = Vector3.Distance(mainCamera.transform.position, player.transform.position);
        if (distance > 0.01f)
        {
            float x1 = Mathf.Lerp(mainCamera.transform.position.x, player.transform.position.x, 0.1f);
            float y1 = Mathf.Lerp(mainCamera.transform.position.y, player.transform.position.y, 0.1f);
            mainCamera.transform.position = new Vector3(x1, y1, mainCamera.transform.position.z);
        }

        float x = Mathf.Clamp(mainCamera.transform.position.x, room.bounds.min.x + mainCamera.orthographicSize * mainCamera.aspect, room.bounds.max.x - mainCamera.orthographicSize * mainCamera.aspect);
        float y = Mathf.Clamp(mainCamera.transform.position.y, room.bounds.min.y + mainCamera.orthographicSize * mainCamera.aspect, room.bounds.max.y - mainCamera.orthographicSize * mainCamera.aspect);
        mainCamera.transform.position = new Vector3(x, y, mainCamera.transform.position.z);
    }
}
