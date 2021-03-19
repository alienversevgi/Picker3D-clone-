using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu()]
public class GameEvent : ScriptableObject
{
    public event Action EventListeners;

    public void Raise()
    {
        EventListeners.Invoke();
    }

    public void Register(Action listener)
    {
        EventListeners += listener;
    }

    public void UnregisterListener(Action listener)
    {
        EventListeners -= listener;
    }
}
