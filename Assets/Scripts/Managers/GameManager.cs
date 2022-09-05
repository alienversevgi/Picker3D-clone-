using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public const float OBJECT_FORCE_VALUE = 5000;

    [SerializeField] private LevelManager levelManager;
    [SerializeField] private PoolManager poolManager;
    [SerializeField] private PickerController pickerController;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private List<LevelData> levels;

    private int currentLevelIndex 
    {
        get 
        {
            return PlayerPrefs.GetInt("CURRENT_LEVEL_INDEX", 0);
        }
        set 
        {
            PlayerPrefs.SetInt("CURRENT_LEVEL_INDEX", value);
        }
    } 

    private bool areAllLevelCompleted 
    {
        get 
        {
            return PlayerPrefs.GetInt("Are_All_Level_Completed", 0) == 1 ? true : false;
        }
        set 
        {
            PlayerPrefs.SetInt("Are_All_Level_Completed", value == true ? 1 : 0);
        }
    }

    private void Start()
    {
        Initialize();
    }

    public void StartGame()
    {
        pickerController.EnableMoving();
        levelManager.ActivateCurrentLevel();
    }

    public void Initialize()
    {
        if (areAllLevelCompleted)
            currentLevelIndex = Random.Range(0, levels.Count);

        GameEventManager.Instance.OnReachedToCheckPoint.Register(() => OnReachedToCheckPoint());
        GameEventManager.Instance.OnFirstInputDetected.Register(() => StartGame());
        poolManager.Initialize();
        uiManager.Initialize(() => NextLevel(), () => ResetLevel());
        uiManager.SetupLevelInfo(currentLevelIndex);
        levelManager.Initialize(currentLevelIndex, levels);
        levelManager.SetupLevel(currentLevelIndex);
    }

    private void OnReachedToCheckPoint()
    {
        pickerController.StopMovingAndForcePoolObjects();
        StartCoroutine(WaitAndCallAction(3.0f, () => CheckLevel()));
    }

    private void CheckLevel()
    {
        if (levelManager.GetPlatformTargetScoreSuccessful())
        {
            if (levelManager.HasNextPlatform())
            {
                levelManager.RunNextPlatform();
                pickerController.EnableMoving();
            }
            else
            {
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

    public void NextLevel()
    {
        currentLevelIndex++;

        if (areAllLevelCompleted)
        {
            currentLevelIndex = Random.Range(0, levels.Count);
        }
        else if (currentLevelIndex == levels.Count - 1)
        {
            areAllLevelCompleted = true;
        }

        ResetLevel();
    }

    public void ResetLevel()
    {
        levelManager.ResetLevel();
        uiManager.SetupLevelInfo(currentLevelIndex);
        levelManager.SetupLevel(currentLevelIndex);
        pickerController.ResetPosition();
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