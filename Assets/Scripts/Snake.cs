using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Snake : MonoBehaviour
{
    private enum Direction { Left,Right,Up,Down }
    private enum State {Dead,Alive}         //convert to bool
    private State state;
    private Direction gridMoveDirection;
    private Vector2Int gridPosition;
    private float gridMoveTimer;
    private float gridMoveTimerMax;
    private Grid levelGrid;
    private int snakeSize;
    private int startSnakeSize;
    private List<SnakeMovePosition> snakeMovePositionList;
    private List<SnakeBodyPart> snakeBodyPartList;
    public void Setup(Grid grid)
    {
        this.levelGrid = grid;
    }
    private void Start()
    {
        gridPosition = new Vector2Int(0, 0);
        gridMoveTimerMax = 0.2f;
        gridMoveTimer = gridMoveTimerMax;
        gridMoveDirection = Direction.Right;

        snakeMovePositionList = new List<SnakeMovePosition>();
        snakeBodyPartList = new List<SnakeBodyPart>();

        startSnakeSize = 3;
        for (int i = 0; i < startSnakeSize; i++)
        {
            CreateSnakeBody();
            snakeMovePositionList.Add(new SnakeMovePosition(gridPosition - new Vector2Int(5 * i, 0), null, Direction.Right));
        }
        snakeSize += startSnakeSize;
        state = State.Alive;
        UpdateScore(0);
       
    }
    private void Update()
    {
        switch (state)
        {
            case State.Alive:
                HandleInput();
                HandelGridMovement();
                break;
            case State.Dead:
                break;
        }
    }
    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.W)) // вверх
        {
            if (gridMoveDirection!= Direction.Down)
            {
                gridMoveDirection = Direction.Up;
            }
        }
        if (Input.GetKeyDown(KeyCode.S)) // вниз
        {
            if (gridMoveDirection != Direction.Up)
            {
                gridMoveDirection = Direction.Down;
            }
        }
        if (Input.GetKeyDown(KeyCode.A)) // влево
        {
            if (gridMoveDirection!= Direction.Right)
            {
                gridMoveDirection = Direction.Left;
            }
        }
        if (Input.GetKeyDown(KeyCode.D)) // вправо
        {
            if (gridMoveDirection!= Direction.Left)
            {
                gridMoveDirection = Direction.Right;
            }
        }
    }
    private void HandelGridMovement()
    {
        gridMoveTimer += Time.deltaTime;
        if (gridMoveTimer > gridMoveTimerMax)
        {
            gridMoveTimer -= gridMoveTimerMax;
            SnakeMovePosition previousSnakeMovePosition = null;
            if ( snakeMovePositionList.Count>0)
            {
                previousSnakeMovePosition = snakeMovePositionList[0];
            }
            SnakeMovePosition snakeMovePosition = new SnakeMovePosition(gridPosition, previousSnakeMovePosition,gridMoveDirection);
            snakeMovePositionList.Insert(0, snakeMovePosition);

            Vector2Int gridMoveDirectionVector;
            switch (gridMoveDirection)
            {
                default:
                case Direction.Right:
                    gridMoveDirectionVector = new Vector2Int(5,0);
                    break;
                case Direction.Left:
                    gridMoveDirectionVector = new Vector2Int(-5, 0);
                    break;
                case Direction.Up:
                    gridMoveDirectionVector = new Vector2Int(0, 5);
                    break;
                case Direction.Down:
                    gridMoveDirectionVector = new Vector2Int(0, -5);
                    break;
            }
            gridPosition += gridMoveDirectionVector;
            gridPosition = levelGrid.ValidateGridPosition(gridPosition);
            bool isFoodEaten = levelGrid.SnakeEatFood(gridPosition);
            if (isFoodEaten)
            {
                snakeSize++;
                Debug.Log("snake size: " + snakeSize);
                UpdateScore(snakeSize-startSnakeSize);
                CreateSnakeBody();
            }
            
            if (snakeMovePositionList.Count >= snakeSize + 1)
            {
                snakeMovePositionList.RemoveAt(snakeMovePositionList.Count - 1);
            }

           
            transform.position = new Vector3(gridPosition.x, gridPosition.y);
            transform.eulerAngles = new Vector3(0, 0, GetAngleFromVector(gridMoveDirectionVector) - 270);

            UpdateSnakeBody();
            foreach (SnakeBodyPart snakeBodyPart in snakeBodyPartList)
            {
                Vector2Int snakeBidyParGridPosition = snakeBodyPart.GetGridPosition();
                if (gridPosition == snakeBidyParGridPosition)
                {
                    Debug.Log("GAME OVER!");
                    state = State.Dead;
                    GameManager.Instance.retryButton.SetActive(true);
                }
            }
        }
    }
    private void CreateSnakeBody()
    {
        snakeBodyPartList.Add(new SnakeBodyPart(snakeMovePositionList.Count));
    }
    private void UpdateSnakeBody()
    {
        for (int i = 0; i < snakeBodyPartList.Count; i++)
        {
            snakeBodyPartList[i].SetSnakeMovePosition(snakeMovePositionList[i],i);
        }
    }
    private float GetAngleFromVector(Vector2Int dir)
    {
        float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (n < 0) n += 360;
        return n;
    }
    public Vector2Int GetGridPosition()
    {
        return gridPosition;
    }
    public List<Vector2Int> GetFullSnakePosition() 
    {
        List<Vector2Int> gridPositionList = new List<Vector2Int>() { gridPosition};
        foreach (SnakeMovePosition position in snakeMovePositionList)
        {
            gridPositionList.Add(position.GetGridPosition());
        }
        return gridPositionList;
    }
    private void UpdateScore(int score)
    {
        GameManager.Instance.scoreText.text = $"Score: {score}";
    }


    private class SnakeBodyPart 
    {
        private SnakeMovePosition snakeMovePosition;
        private Transform transform;
        private GameObject snakeBody;

        public SnakeBodyPart(int index)
        {
            snakeBody = new GameObject("SnakeBody", typeof(SpriteRenderer));
            snakeBody.GetComponent<SpriteRenderer>().sprite = AssetsHandler.Instance.snakeBodySprite;
            snakeBody.GetComponent<SpriteRenderer>().sortingOrder = -index;
            snakeBody.transform.localScale = new Vector3(20, 20, 1);
            transform = snakeBody.transform;
        }
        public void SetSnakeMovePosition(SnakeMovePosition snakeMovePosition, int index)
        {
            snakeBody.GetComponent<SpriteRenderer>().sprite = AssetsHandler.Instance.snakeBodySprite;
            this.snakeMovePosition = snakeMovePosition;
            transform.position = new Vector3(snakeMovePosition.GetGridPosition().x, snakeMovePosition.GetGridPosition().y);
            float angle = 0f;
            switch (snakeMovePosition.GetDirection())
            {
                default:
                    // from left/right to up
                case Direction.Up:
                    switch (snakeMovePosition.GetPreviousDirection())
                    {
                        default:
                            angle = 0;
                            break;
                        case Direction.Left:
                            if (index != GameManager.Instance.snake.snakeBodyPartList.Count-1)
                            {
                                snakeBody.GetComponent<SpriteRenderer>().sprite = AssetsHandler.Instance.snakeCornerSprite;
                                transform.position = new Vector3(snakeMovePosition.GetGridPosition().x + 1, snakeMovePosition.GetGridPosition().y + 1);
                                angle = 180f;
                            }
                            break;
                        case Direction.Right:
                            if (index != GameManager.Instance.snake.snakeBodyPartList.Count - 1)
                            {
                                snakeBody.GetComponent<SpriteRenderer>().sprite = AssetsHandler.Instance.snakeCornerSprite;
                                transform.position = new Vector3(snakeMovePosition.GetGridPosition().x - 1, snakeMovePosition.GetGridPosition().y + 1);
                                angle = 270f;
                            }
                            break;
                    }
                    break;
                    // from left/right to down
                case Direction.Down:
                    switch (snakeMovePosition.GetPreviousDirection())
                    {
                        default:
                            angle = 180f;
                            break;
                        case Direction.Left:
                            if (index != GameManager.Instance.snake.snakeBodyPartList.Count - 1)
                            {
                                snakeBody.GetComponent<SpriteRenderer>().sprite = AssetsHandler.Instance.snakeCornerSprite;
                                transform.position = new Vector3(snakeMovePosition.GetGridPosition().x + 1, snakeMovePosition.GetGridPosition().y - 1);
                                angle = 90f;
                            }
                            else
                            {
                                angle = -180;
                            }
                            break;
                        case Direction.Right:
                            if (index != GameManager.Instance.snake.snakeBodyPartList.Count - 1)
                            {
                                snakeBody.GetComponent<SpriteRenderer>().sprite = AssetsHandler.Instance.snakeCornerSprite;
                                transform.position = new Vector3(snakeMovePosition.GetGridPosition().x - 1, snakeMovePosition.GetGridPosition().y - 1);
                                angle = 0f;
                            }
                            else
                            {
                                angle = -180;
                            }
                            break;
                    }
                    break;
                    //from down/up to left
                case Direction.Left:
                    switch (snakeMovePosition.GetPreviousDirection())
                    {
                        default:
                            angle = 90;
                            break;
                        case Direction.Down:
                            if (index != GameManager.Instance.snake.snakeBodyPartList.Count - 1)
                            {
                                snakeBody.GetComponent<SpriteRenderer>().sprite = AssetsHandler.Instance.snakeCornerSprite;
                                transform.position = new Vector3(snakeMovePosition.GetGridPosition().x - 1, snakeMovePosition.GetGridPosition().y + 1);
                                angle = 270f;
                            }
                            else
                            {
                                angle = 90;
                            }
                            break;
                        case Direction.Up:
                            if (index != GameManager.Instance.snake.snakeBodyPartList.Count - 1)
                            {
                                snakeBody.GetComponent<SpriteRenderer>().sprite = AssetsHandler.Instance.snakeCornerSprite;
                                transform.position = new Vector3(snakeMovePosition.GetGridPosition().x - 1, snakeMovePosition.GetGridPosition().y - 1);
                                angle = 0f;
                            }
                            else
                            {
                                angle = 90;
                            }
                            break;
                    }
                    break;
                    //from down/up to right
                case Direction.Right:
                    switch (snakeMovePosition.GetPreviousDirection())
                    {
                        default: 
                            angle = -90;
                            break;
                        case Direction.Down:
                            if (index != GameManager.Instance.snake.snakeBodyPartList.Count - 1)
                            {
                                snakeBody.GetComponent<SpriteRenderer>().sprite = AssetsHandler.Instance.snakeCornerSprite;
                                transform.position = new Vector3(snakeMovePosition.GetGridPosition().x + 1, snakeMovePosition.GetGridPosition().y + 1);
                                angle = 180f;
                            }
                            else
                            {
                                angle = -90;
                            }
                            break;
                        case Direction.Up:
                            if (index != GameManager.Instance.snake.snakeBodyPartList.Count - 1)
                            {
                                snakeBody.GetComponent<SpriteRenderer>().sprite = AssetsHandler.Instance.snakeCornerSprite;
                                transform.position = new Vector3(snakeMovePosition.GetGridPosition().x + 1, snakeMovePosition.GetGridPosition().y - 1);
                                angle = 90f;
                            }
                            else
                            {
                                angle = -90;
                            }
                            break;
                    }
                    break;
                
            }
            transform.eulerAngles = new Vector3(0, 0, angle);
        }
        public Vector2Int GetGridPosition()
        {
            return snakeMovePosition.GetGridPosition();
        }
    }


    private class SnakeMovePosition 
    {
        private SnakeMovePosition previousSnakeMovePosition;
        private Vector2Int gridPosition;
        private Direction direction;

        public SnakeMovePosition(Vector2Int gridPosition, SnakeMovePosition previousSnakeMovePosition, Direction direction) 
        { 
            this.gridPosition = gridPosition;
            this.direction = direction;
            this.previousSnakeMovePosition = previousSnakeMovePosition;
        }
        public Vector2Int GetGridPosition()
        {
            return gridPosition;
        }
        public Direction GetDirection()
        {
            return direction;
        }
        public Direction GetPreviousDirection()
        {
            if (previousSnakeMovePosition == null)
            {
                return Direction.Right;
            }
            return previousSnakeMovePosition.direction;
        }
    }
    
}
