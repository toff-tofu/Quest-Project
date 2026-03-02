using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class GameEvents
{
    public static Action OnPlayerDeath;

    public static void PlayerDied()
    {
        OnPlayerDeath?.Invoke();
    }
}
