using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class cameraControlTrigger : MonoBehaviour
{
    // Start is called before the first frame update
    public CustomInspectorObjects customInspectorObjects;
    private Collider2D _coll;
    private void Start()
    {
        _coll = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Vector2 exitDirection = (collision.transform.position - _coll.bounds.center).normalized;
            if (customInspectorObjects.cameraOnLeft != null && customInspectorObjects.cameraOnRight != null)
            {
                CameraManager.instance.SwapCamera(customInspectorObjects.cameraOnLeft, customInspectorObjects.cameraOnRight, customInspectorObjects.cameraOnTop, customInspectorObjects.cameraOnBottom, exitDirection);
            }
            if (customInspectorObjects.cameraOnTop != null && customInspectorObjects.cameraOnBottom != null)
            {
                CameraManager.instance.SwapCamera(customInspectorObjects.cameraOnLeft, customInspectorObjects.cameraOnRight, customInspectorObjects.cameraOnTop, customInspectorObjects.cameraOnBottom, exitDirection);
            }
        }
    }
}
[System.Serializable]
public class CustomInspectorObjects
{
    public CinemachineVirtualCamera cameraOnLeft;
    public CinemachineVirtualCamera cameraOnRight;
    public CinemachineVirtualCamera cameraOnTop;
    public CinemachineVirtualCamera cameraOnBottom;
}
