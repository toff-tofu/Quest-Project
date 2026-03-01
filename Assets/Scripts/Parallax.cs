using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Cinemachine;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    private UnityEngine.Vector3 startPos;
    public Transform camObject;
    private float length;
    [SerializeField] private float parallaxAmount;
    // Start is called before the first frame update
    void Start()
    {
        // camera = camObject.GetComponent<CameraManager>()._currentCamera;
        startPos = transform.position;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    // Update is called once per frame
    void Update()
    {
        float temp = (camObject.transform.position.x * (1 - parallaxAmount));
        float distanceX = camObject.position.x * parallaxAmount;
        float distanceY = camObject.position.y * parallaxAmount;
        transform.position = new UnityEngine.Vector3(startPos.x + distanceX, startPos.y + distanceY, transform.position.z);
        if (temp > startPos.x + length)
        {
            startPos.x += length;
        }
        else if (temp < startPos.x - length)
        {
            startPos.x -= length;
        }
    }
}
