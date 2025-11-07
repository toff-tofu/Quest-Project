using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowObject : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float flipTime = 0.5f;
    private Coroutine turnCoroutine;
    private Movement player;
    private bool facingRight = true;
    // Start is called before the first frame update
    void Awake()
    {
        player = playerTransform.gameObject.GetComponent<Movement>();
        facingRight = player.facingRight;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = playerTransform.position;
    }
    public void CallTurn()
    {
        turnCoroutine = StartCoroutine(FlipYLerp());
    }
    private IEnumerator FlipYLerp()
    {
        float startRotation = transform.eulerAngles.y;
        float endRotationAmount = DetermineEndRotation();
        float yRotation = 0f;
        float elapsedTime = 0f;
        while (elapsedTime < flipTime)
        {
            elapsedTime += Time.deltaTime;
            yRotation = Mathf.Lerp(startRotation, endRotationAmount, elapsedTime / flipTime);
            transform.rotation = Quaternion.Euler(0f, yRotation, 0f);
            yield return null;
        }
    }
    private float DetermineEndRotation()
    {
        facingRight = !facingRight;
        if (facingRight)
        {
            return 0f;
        }
        else
        {
            return 180f;
        }
    }
}
