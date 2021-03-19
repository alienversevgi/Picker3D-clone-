using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<GameManager>();
            }

            return instance;
        }
    }
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public const float OBJECT_FORCE_VALUE = 2000;

    [SerializeField] private LevelManager levelManager;
    [SerializeField] private PickerController pickerController;
    [SerializeField] private UIManager uiManager;
    private event Action onReachedToCheckPoint;

    private int levelIndex;

    private void Start()
    {
        Initialize();
    }

    public void StartGame()
    {
        pickerController.EnableMoving();
    }

    public void Initialize()
    {
        GameEventManager.Instance.OnReachedToCheckPoint.Register(() => OnReachedToCheckPoint());
        uiManager.SetupLevelInfo(levelIndex);
        levelManager.Initialize(levelIndex);
    }

    private void OnReachedToCheckPoint()
    {
        pickerController.StopMovingAndForcePoolObjects();
        StartCoroutine(WaitAndCallAction(3.0f, () => CheckLevel()));
    }

    public void ReachedToCheckPoint()
    {
        onReachedToCheckPoint?.Invoke();
    }

    private void CheckLevel()
    {
        if (levelManager.GetPlatformTargetScoreSuccessful())
        {
            if (levelManager.HasNextPlatform())
            {
                levelManager.IncreasePlatformIndex();
                pickerController.EnableMoving();
            }
            else
            {
                levelIndex++;
                uiManager.ShowNextLevelButton();
            }
        }
        else
        {
            GameOver();
        }
    }

    private void GameOver()
    {
        uiManager.ShowRestartButton();
    }

    private IEnumerator WaitAndCallAction(float waitTime, Action action)
    {
        yield return new WaitForSecondsRealtime(waitTime);
        action();
    }

    public void CheckPlatformClear(int levelCurrentScore, int levelEndScore)
    {
        if (levelCurrentScore >= levelEndScore)
        {
            GameEventManager.Instance.OnSuccesfulyPlatformCleared.Raise();
        }
        else
        {
            uiManager.ShowRestartButton();
        }
    }
}
