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
        GenerateField();
        SpawnFood();
    }
    public void GenerateField()
    {
        GameObject parent = new GameObject("BackgroundTiles");

        int tileSize = 16;
        int start = -48;
        int end = 48;
        Color light = new Color(86f / 255f, 186f / 255f, 86f / 255f);
        Color dark = new Color(46f / 255f, 146f / 255f, 46f / 255f);

        for (int x = start; x <= end; x += tileSize)
        {
            for (int y = start; y <= end; y += tileSize)
            {
                GameObject tile = new GameObject($"Tile_{x}_{y}");
                tile.transform.position = new Vector3(x, y, 0);
                tile.transform.parent = parent.transform;
                tile.transform.localScale = new Vector3(tileSize, tileSize, 0);

                SpriteRenderer renderer = tile.AddComponent<SpriteRenderer>();
                renderer.sprite = AssetsHandler.Instance.background;
                bool isEven = ((x - start) / tileSize + (y - start) / tileSize) % 2 == 0;
                renderer.color = isEven ? light:dark ;
                renderer.sortingLayerName = "Background";
            }
        }
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
            gridPosition.x = width / 2;
        }
        else if (gridPosition.y < -height / 2)
        {
            gridPosition.y = height / 2;
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
