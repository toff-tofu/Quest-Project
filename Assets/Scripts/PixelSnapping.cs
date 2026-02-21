using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PixelSnapping : MonoBehaviour
{
    public Transform visual;
    public Transform parentTransform;
    public float pixelsPerUnit = 8f;
    void LateUpdate()
    {
        Vector3 pos = parentTransform.position;

        float roundedX = Mathf.Round(pos.x * pixelsPerUnit) / pixelsPerUnit;
        float roundedY = Mathf.Round(pos.y * pixelsPerUnit) / pixelsPerUnit;

        visual.position = new Vector3(roundedX, roundedY, pos.z);
    }
}
