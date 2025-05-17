using UnityEngine;

public class AssetsHandler : MonoBehaviour
{
    public static AssetsHandler Instance;
    private void Awake()
    {
        Instance = this;
    }
    public Sprite snakeHeadSprite;
    public Sprite snakeBodySprite;
    public Sprite snakeCornerSprite;
    public Sprite foodSprite;
    public Sprite background;
    public AudioClip foodEaten;
    public AudioClip snakeDeath;
}
