using TMPro;
using UnityEngine;

public class Pause : MonoBehaviour
{
    private static Pause instance;

    private void Awake()
    {
        instance = this;
        transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(-2000,0);
        Hide();
    }
    private void Show()
    {
        gameObject.SetActive(true);
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }
    public static void _Show()
    {
        instance.Show();
    } 
    public static void _Hide()
    {
        instance.Hide();
    }
}
