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
    [SerializeField] private Text NextLevelText;
    [SerializeField] private Text TabToPlayText;
    private Action nextLevelAction;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TabToPlayText.gameObject.SetActive(false);
            GameManager.Instance.StartGame();
        }
    }

    public void Initialize(Action nextLevelAction)
    {
        this.nextLevelAction = nextLevelAction;
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
        CurrentLevelText.text = currentLevel.ToString();
        NextLevelText.text = (currentLevel + 1).ToString();
    }

    public void OnRestartButtonClicked()
    {
        SceneManager.LoadScene(0);
    }

    public void OnNextLevelButtonClicked()
    {
        nextLevelAction();
    }
}
