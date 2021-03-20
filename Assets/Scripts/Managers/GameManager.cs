using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public const float OBJECT_FORCE_VALUE = 5000;

    [SerializeField] private GameData gameData;

    [SerializeField] private LevelManager levelManager;
    [SerializeField] private PoolManager poolManager;
    [SerializeField] private PickerController pickerController;
    [SerializeField] private UIManager uiManager;
    private event Action onReachedToCheckPoint;

    private int levelIndex = 1;

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
        if (gameData.AreAllLevelCompleted)
        {
            levelIndex = Random.Range(0, gameData.Levels.Count );
            gameData.CurrentLevelIndex = levelIndex;
        }
        else
        {
            levelIndex = gameData.CurrentLevelIndex;
        }

        GameEventManager.Instance.OnReachedToCheckPoint.Register(() => OnReachedToCheckPoint());
        poolManager.Initialize();
        uiManager.Initialize(() => NextLevel(), () => ResetLevel());
        uiManager.SetupLevelInfo(levelIndex);
        levelManager.Initialize(levelIndex, gameData.Levels);
        levelManager.SetupLevel(levelIndex);
        uiManager.onFirstInputDetected += UiManager_onFirstInputDetected;
    }

    private void UiManager_onFirstInputDetected()
    {
        StartGame();
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
        levelIndex++;
        gameData.CurrentLevelIndex = levelIndex;

        if (gameData.AreAllLevelCompleted)
        {
            levelIndex = Random.Range(0, gameData.Levels.Count);
            gameData.CurrentLevelIndex = levelIndex;
        }
        else if (gameData.CurrentLevelIndex == gameData.Levels.Count - 1)
        {
            gameData.AreAllLevelCompleted = true;
        }

        ResetLevel();
    }

    public void ResetLevel()
    {
        levelManager.ResetLevel();
        uiManager.SetupLevelInfo(levelIndex);
        levelManager.SetupLevel(levelIndex);
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
