using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class Grid
{
    private Vector2Int foodPosition;
    private GameObject foodObject;
    private int width;
    private int height;
    private Snake snake;
    public Grid(int width,int height)
    {
        this.width = width;
        this.height = height;
    }
    public void Setup(Snake snake)
    {
        this.snake = snake;
        SpawnFood();
    }
    private void SpawnFood()
    {
        do
        {
            foodPosition = new Vector2Int(Random.Range(-10, 11) * 5, Random.Range(-10, 11) * 5);
        } while (snake.GetFullSnakePosition().IndexOf(foodPosition)!=-1);
        foodObject = new GameObject("Food", typeof(SpriteRenderer));
        foodObject.GetComponent<SpriteRenderer>().sprite = AssetsHandler.Instance.foodSprite;
        foodObject.transform.position = new Vector3(foodPosition.x, foodPosition.y);
        foodObject.transform.localScale = new Vector3(7,7,1);
    }
    public bool SnakeEatFood(Vector2Int snakePosition)
    {
        if (snakePosition == foodPosition)
        {
           Object.Destroy(foodObject);
            SpawnFood();
           return true;
        }
        else {return false; }
    }
    public Vector2Int ValidateGridPosition(Vector2Int gridPosition)
    {
        if (gridPosition.x < -width/2)
        {
            gridPosition.x = width / 2 - 1;
        }
        else if (gridPosition.y < -height / 2)
        {
            gridPosition.y = height / 2 - 1;
        }
        else if (gridPosition.x > width / 2)
        {
            gridPosition.x = -width / 2;
        }
        else if (gridPosition.y > height / 2)
        {
            gridPosition.y = -height / 2;
        }
        return gridPosition;
    }

}
