using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PixelSnapping : MonoBehaviour
{
    public Transform visual;
    public float pixelsPerUnit = 8f;

    void LateUpdate()
    {
        Vector3 pos = transform.position;

        float roundedX = Mathf.Round(pos.x * pixelsPerUnit) / pixelsPerUnit;
        float roundedY = Mathf.Round(pos.y * pixelsPerUnit) / pixelsPerUnit;

        Vector3 worldPos = new Vector3(roundedX, roundedY, pos.z);

        visual.localPosition = worldPos - transform.position;
    }
}
