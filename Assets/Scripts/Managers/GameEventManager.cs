using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEventManager : MonoBehaviour
{
    #region Singelaton

    private static GameEventManager instance;

    public static GameEventManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<GameEventManager>();
            }

            return instance;
        }
    }
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    #endregion

    public GameEvent OnReachedToCheckPoint;
    public GameEvent OnSuccesfulyPlatformCleared; //Checkpointe ulaştım ama animsyon oynamadı
    public GameEvent OnPlatformAnimationFinished;
    public GameEvent OnPlatformFinished;
    public GameEvent OnFirstInputDetected;
}
