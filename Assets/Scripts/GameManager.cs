using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    public Snake snake;
    private Grid grid;
    public static GameManager Instance;
    public TextMeshProUGUI scoreText;
    public GameObject retryButton;
    public TextMeshProUGUI resultText;
    public TextMeshProUGUI fianlScore;
    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        Debug.Log("Start");
        grid = new Grid(50, 50);
        snake.Setup(grid);
        grid.Setup(snake);
        retryButton.SetActive(false);
        fianlScore.text = "";
        resultText.text = "";
    }
    public void EndGame(bool win)
    {
        scoreText.text = "";
        retryButton.SetActive(true);
        fianlScore.text = "Final" + scoreText;
        resultText.text = win?"You Win" : "You Lose";

    }
    public void StartNewGame(int index)
    {
        retryButton.SetActive(false);
        SceneManager.LoadScene(0);
    }
}
