using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AudioFade : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<AudioEchoFilter>().enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.GetComponent<AudioLowPassFilter>().cutoffFrequency = Clickable.Clicks * 300 + 500;
        if (Clickable.Clicks >= 10)
        {
            gameObject.GetComponent<AudioEchoFilter>().delay = (15 - Clickable.Clicks - 1) * 100;
        }
        if (Clickable.Clicks == 14)
        {
            gameObject.GetComponent<AudioEchoFilter>().enabled = false;
        }
    }
}
