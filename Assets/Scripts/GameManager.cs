using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] public Snake snake;
    private Grid grid;
    
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI fianlScore;
    public int score = 0;
    public GameObject resultText;
    public GameObject retryButton;
    public GameObject menuButton;

    public GameObject background;
    public GameObject backgroundMusic;

    private void Awake()
    {
        Instance = this;
        if (Leaderboard.Instance != null)
        {
            Leaderboard.Instance.LoadLeaderboard();
        }
    }
    void Start()
    {
        Time.timeScale = 1;
        Debug.Log("Start");
        grid = new Grid(100, 100);
        snake.Setup(grid);
        grid.Setup(snake);
        retryButton.SetActive(false);
        menuButton.SetActive(false);
        background.SetActive(false);
        fianlScore.text = "";
        resultText.SetActive(false);
        
    }
    public void EndGame()
    {
        backgroundMusic.SetActive(false);
        retryButton.SetActive(true);
        menuButton.SetActive(true);
        background.SetActive(true);
        resultText.SetActive(true);
        fianlScore.text = "Final " + scoreText.text;

        Leaderboard.Instance.AddScore(score);
        Leaderboard.Instance.SaveLeaderboard();

        scoreText.text = "";
        score = 0;
    }
    public void StartNewGame()
    {
        SceneManager.LoadScene(1);
        retryButton.SetActive(false);
    }
    public void MainMenu()
    {
       
        menuButton.SetActive(false);
        SceneManager.LoadScene(0);
    }
    public void BackToGame()
    {
        Pause._Hide();
        Time.timeScale = 1;
    }
    public void PauseGame()
    {
        Pause._Show();
        Time.timeScale = 0;
    }

    

}
