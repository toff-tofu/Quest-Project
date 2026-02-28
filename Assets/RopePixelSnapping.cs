using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RopePixelSnapping : MonoBehaviour
{
    [SerializeField] LineRenderer line;
    public float pixelsPerUnit;
    void LateUpdate()
    {
        int pointCount = line.positionCount;
        for (int i = 0; i < pointCount; i++)
        {
            Vector3 pos = line.GetPosition(i);
            float snappedX = Mathf.Round(pos.x * pixelsPerUnit) / pixelsPerUnit;
            float snappedY = Mathf.Round(pos.y * pixelsPerUnit) / pixelsPerUnit;
            line.SetPosition(i, new Vector3(snappedX, snappedY, pos.z));
        }
    }
}
