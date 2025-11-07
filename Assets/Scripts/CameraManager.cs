using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Unity.VisualScripting;

public class CameraManager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private CinemachineVirtualCamera[] virtualCameras;
    public static CameraManager instance;

    [Header("Controls for damping player y")]
    [SerializeField] private float _fallPanAmount = 0.25f;
    [SerializeField] private float _fallPanTime = 0.35f;
    public float FallSpeedYDampingThreshold = -15f;

    public bool isLerpingY { get; private set; }
    public bool lerpedFromPlayerFalling { get; set; }
    private Coroutine lerpYPanCoroutine;
    private CinemachineVirtualCamera _currentCamera;
    private CinemachineFramingTransposer _framingTransposer;
    private float _normYPanAmount;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        for (int i = 0; i < virtualCameras.Length; i++)
        {
            _currentCamera = virtualCameras[i];
            _framingTransposer = _currentCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        }
        _normYPanAmount = _framingTransposer.m_YDamping;
        _currentCamera = virtualCameras[0];
    }
    public void LerpYDamping(bool isPlayerFalling)
    {
        lerpYPanCoroutine = StartCoroutine(LerpYAction(isPlayerFalling));
    }
    private IEnumerator LerpYAction(bool isPlayerFalling)
    {
        isLerpingY = true;
        float startDamping = _framingTransposer.m_YDamping;
        float endDamping = 0f;
        if (isPlayerFalling)
        {
            endDamping = _fallPanAmount;
            lerpedFromPlayerFalling = true;
        }
        else
        {
            endDamping = _normYPanAmount;
        }
        float elapsedTime = 0f;
        while (elapsedTime < _fallPanTime)
        {
            elapsedTime += Time.deltaTime;
            float newDamping = Mathf.Lerp(startDamping, endDamping, elapsedTime / _fallPanTime);
            _framingTransposer.m_YDamping = newDamping;
            yield return null;
        }
        isLerpingY = false;
    }

    // public void SwapCamera(CinemachineVirtualCamera cameraLeft, CinemachineVirtualCamera cameraRight, CinemachineVirtualCamera cameraTop, CinemachineVirtualCamera cameraBotton, Vector2 exitDirection)
    // {
    //     if (_currentCamera == cameraLeft && exitDirection.x > 0f)
    //     {
    //         cameraRight.enabled = true;
    //         cameraLeft.enabled = false;
    //         _currentCamera = cameraRight;
    //         _framingTransposer = _currentCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
    //     }
    //     else if (_currentCamera == cameraRight && exitDirection.x < 0f)
    //     {
    //         cameraLeft.enabled = true;
    //         cameraRight.enabled = false;
    //         _currentCamera = cameraLeft;
    //         _framingTransposer = _currentCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
    //     }
    // }
    public void SwapCamera(CinemachineVirtualCamera cameraLeft, CinemachineVirtualCamera cameraRight, Vector2 exitDirection)
    {
        if (_currentCamera == cameraLeft && exitDirection.x > 0f)
        {
            cameraRight.enabled = true;
            cameraLeft.enabled = false;
            _currentCamera = cameraRight;
            _framingTransposer = _currentCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        }
        else if (_currentCamera == cameraRight && exitDirection.x < 0f)
        {
            cameraLeft.enabled = true;
            cameraRight.enabled = false;
            _currentCamera = cameraLeft;
            _framingTransposer = _currentCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        }
    }
}
