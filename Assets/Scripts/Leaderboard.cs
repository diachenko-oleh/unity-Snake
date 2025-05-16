using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Leaderboard : MonoBehaviour
{
    public static Leaderboard Instance;
    private List<int> leaderboard = new List<int> { 0,0,0,0,0};
    public TextMeshProUGUI leaderboardText;

    private void Awake()
    {
        Instance = this;
        transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
        Hide();
    }
    public void AddScore(int score)
    {
        leaderboard.Add(score);
        leaderboard.Sort((a, b) => b.CompareTo(a));
    }
    public void PrintLeaderboard()
    {
        leaderboardText.text = ""; 

        for (int i = 0; i < leaderboard.Count && i < 5; i++)
        {
            string line = (i + 1) + ") " + leaderboard[i];
            leaderboardText.text += line + "\n";                      
        }
    }
    public void CleanLeaderboard()
    {
        leaderboard.Clear();

        for (int i = 0; i < 5; i++)
        {
            leaderboard.Add(0);
            PlayerPrefs.SetInt("Leaderboard" + i, 0);
        }

        PlayerPrefs.Save();

        leaderboardText.text = "";
        for (int i = 0; i < 5; i++)
        {
            string line = (i + 1) + ") " + leaderboard[i];
            leaderboardText.text += line + "\n";
        }
    }
    public void LoadLeaderboard()
    {
        leaderboard = new List<int>();
        for (int i = 0; i < 5; i++)
        {
            leaderboard.Add(PlayerPrefs.GetInt("Leaderboard" + i, 0));
        }
    }
    public void SaveLeaderboard()
    {
        for (int i = 0; i < leaderboard.Count; i++)
        {
            PlayerPrefs.SetInt("Leaderboard" + i, leaderboard[i]);
        }
        PlayerPrefs.Save();
    }
    private void Show()
    {
        gameObject.SetActive(true);
        LoadLeaderboard();
        PrintLeaderboard();
    }
    private void Hide()
    {
        gameObject.SetActive(false);
    }
    public static void _Show()
    {
        Instance.Show();
    }
    public static void _Hide()
    {
        Instance.Hide();
    }
}
