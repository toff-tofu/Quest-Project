using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerGlobalAnimation : MonoBehaviour
{
    public Animator transition;
    public float startDelay;
    public float transitionTime;
    // Update is called once per frame
    void Update()
    {
    }

    public IEnumerator Death()
    {
        yield return new WaitForSeconds(startDelay);
        transition.SetTrigger("Die");
        yield return new WaitForSeconds(transitionTime);
        transition.SetTrigger("Respawn");
    }
}
