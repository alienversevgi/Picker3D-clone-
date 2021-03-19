using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameEventListener : MonoBehaviour
{
    public GameEvent GameEvent;
    public event Action Response;

    public void Initialize(GameEvent gameEvent, Action response)
    {
        this.GameEvent = gameEvent;
        this.Response = response;
    }

    //private void OnEnable()
    //{
    //    GameEvent.RegisterListener(this);
    //}

    //private void OnDisable()
    //{
    //    GameEvent.UnregisterListener(this);
    //}

    public void OnEventRaised()
    {
        Response.Invoke();
    }
}