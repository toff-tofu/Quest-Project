using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clickable : MonoBehaviour
{

    // This variable is marked as `static`, which means it is
    // accessible by any script, anywhere! They can just write
    // Clickable.Clicks to read/write to this value.
    //
    // See an example in `ClickTracker.cs`, which is on a UI
    // object titled `Tracker Text (TMP)`.
    public static int Clicks = 0;
    // private int amountClicked = 0;

    /// <summary>
    /// This function is called when the mouse button clicks
    /// on this object.
    /// </summary>
    private void OnMouseDown()
    {
        gameObject.GetComponent<AudioSource>().Play();
        gameObject.GetComponent<ParticleSystem>().Emit(10*Clicks);
        Clicks += 1;  // add one point
    }

}
