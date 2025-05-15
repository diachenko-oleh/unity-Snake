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
            foodPosition = new Vector2Int(Random.Range(0, width / 5) * 5, Random.Range(0, height / 5) * 5);
        } while (snake.GetFullSnakePosition().IndexOf(foodPosition)!=-1);

            //Vector2Int.Distance(snake.GetFullSnakePosition().IndexOf(foodPosition), foodPosition) < 10);

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
        if (gridPosition.x < -width)
        {
            gridPosition.x = width - 1;
        }
        else if (gridPosition.y < -height)
        {
            gridPosition.y = height - 1;
        }
        else if (gridPosition.x > width)
        {
            gridPosition.x = -width;
        }
        else if (gridPosition.y > height)
        {
            gridPosition.y = -height;
        }
        return gridPosition;
    }

}
