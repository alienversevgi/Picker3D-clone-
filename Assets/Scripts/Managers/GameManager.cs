using Game.Data;
using Game.Level;
using Game.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game
{
    public class GameManager : MonoBehaviour
    {
        #region Fields

        public const float OBJECT_FORCE_VALUE = 5000;

        [SerializeField] private LevelManager levelManager;
        [SerializeField] private PoolManager poolManager;
        [SerializeField] private PickerController pickerController;
        [SerializeField] private UIManager uiManager;

        #endregion

        #region Unity Methods

        private void Start()
        {
            Initialize();
        }

        #endregion

        #region Private Methods

        private void StartGame()
        {
            pickerController.EnableMoving();
            levelManager.ActivateCurrentLevel();
        }

        private void Initialize()
        {
            GameEventManager.Instance.OnReachedToCheckPoint.Register(() => OnReachedToCheckPoint());
            GameEventManager.Instance.OnFirstInputDetected.Register(() => StartGame());
            poolManager.Initialize();
            uiManager.Initialize(() => NextLevel(), () => ResetLevel());
            levelManager.Initialize();
            levelManager.LoadLevel();
            uiManager.SetupLevelInfo(levelManager.CurrentLevelIndex);
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

        private void NextLevel()
        {
            levelManager.IncrementLevel();
            ResetLevel();
        }

        private void ResetLevel()
        {
            levelManager.ResetLevel();
            levelManager.LoadLevel();
            uiManager.SetupLevelInfo(levelManager.CurrentLevelIndex);
            pickerController.ResetPosition();
        }

        private IEnumerator WaitAndCallAction(float waitTime, Action action)
        {
            yield return new WaitForSecondsRealtime(waitTime);
            action();
        }

        #endregion
    }
}