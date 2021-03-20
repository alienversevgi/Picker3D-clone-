using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Button NextLevelButton;
    [SerializeField] private Button RestartLevelButton;
    [SerializeField] private Text CurrentLevelText;
    [SerializeField] private Text TabToPlayText;

    private Action nextLevelAction;
    private Action restartAction;
    private bool isFirstInputDetected;

    public event Action onFirstInputDetected;

    private void Update()
    {
        if (!isFirstInputDetected && Input.GetMouseButtonDown(0))
        {
            isFirstInputDetected = true;
            TabToPlayText.gameObject.SetActive(false);
            onFirstInputDetected?.Invoke();
        }
    }

    public void Initialize(Action nextLevelAction, Action restartAction)
    {
        this.nextLevelAction = nextLevelAction;
        this.restartAction = restartAction;
    }

    public void ShowRestartButton()
    {
        RestartLevelButton.gameObject.SetActive(true);
    }

    public void ShowNextLevelButton()
    {
        NextLevelButton.gameObject.SetActive(true);
    }

    public void SetupLevelInfo(int currentLevel)
    {
        TabToPlayText.gameObject.SetActive(true);
        isFirstInputDetected = false;
        CurrentLevelText.text = (currentLevel + 1).ToString();
    }

    public void OnRestartButtonClicked()
    {
        RestartLevelButton.gameObject.SetActive(false);
        restartAction();
    }

    public void OnNextLevelButtonClicked()
    {
        NextLevelButton.gameObject.SetActive(false);
        nextLevelAction();
    }
}
