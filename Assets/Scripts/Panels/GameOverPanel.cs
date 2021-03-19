using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverPanel : MonoBehaviour
{
    [SerializeField] private Button NextLevelButton;
    [SerializeField] private Button RestartLevelButton;

    public void ShowRestartButton()
    {
        RestartLevelButton.gameObject.SetActive(true);
    }

    public void ShowNextLevelButton()
    {
        NextLevelButton.gameObject.SetActive(true);
    }

    public void OnRestartButtonClicked()
    {
        SceneManager.LoadScene(0);
    }
    
    public void OnNextLevelButtonClicked()
    {
        
    }
}
