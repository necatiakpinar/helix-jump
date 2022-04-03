using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class Manager_UI : MonoBehaviour
{
    [Header("Text")]
    public TMP_Text textLevel;
    public TMP_Text textCurrentScore;
    public TMP_Text textHighScore;
    public int currentScore = 0;

    public int currentLevel = 1;
    private static Manager_UI Instance;
    public static Manager_UI GetInstance()
    {
        return Instance;
    }
    private void Awake() 
    {
        Instance = this;
    }
    void Start()
    {
        InitializeData();
    }

    private void InitializeData()
    {
        if (PlayerPrefs.HasKey("HighScore")) textHighScore.text = PlayerPrefs.GetInt("HighScore").ToString();
        else textHighScore.text = "0";

        if (PlayerPrefs.HasKey("Level")) currentLevel = PlayerPrefs.GetInt("Level");
        else currentLevel = 1;

        textLevel.text = "Level: " + currentLevel.ToString();
        textCurrentScore.text = "0";
    }

    public void IncreaseCurrentScore()
    {
        currentScore += currentLevel;
        textCurrentScore.text = currentScore.ToString();
    }
    public void CheckHighScore()
    {
        if (currentScore > PlayerPrefs.GetInt("HighScore"))
        {
            PlayerPrefs.SetInt("HighScore",currentScore);
            textCurrentScore.text = currentScore.ToString();
        }
        else return;
        
    }
    public void PassToNextLevel()
    {
        currentLevel++;
        PlayerPrefs.SetInt("Level",currentLevel);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}


