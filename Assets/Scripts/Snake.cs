using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Snake : MonoBehaviour
{
    private enum Direction { Left,Right,Up,Down }
    private bool IsAliveState;    
    private Direction MoveDirection;
    private Vector2Int Position;
    private float MoveTimer;
    private float MoveTimerMax;
    private Grid Grid;
    private int snakeSize;
    private int startSnakeSize;
    private List<SnakeMovePosition> snakeMovePositionList;
    private List<SnakeBodyPart> snakeBodyPartList;
    public void Setup(Grid grid)
    {
        this.Grid = grid;
    }
    public Vector2Int GetGridPosition()
    {
        return Position;
    }
    private void Start()
    {
        Position = new Vector2Int(0, 0);
        MoveTimerMax = 0.2f;
        MoveTimer = MoveTimerMax;
        MoveDirection = Direction.Right;

        snakeMovePositionList = new List<SnakeMovePosition>();
        snakeBodyPartList = new List<SnakeBodyPart>();

        startSnakeSize = 3;
        for (int i = 0; i < startSnakeSize; i++)
        {
            CreateSnakeBody();
            snakeMovePositionList.Add(new SnakeMovePosition(Position - new Vector2Int(5 * i, 0), null, Direction.Right));
        }
        snakeSize += startSnakeSize;
        IsAliveState = true;
        UpdateScore(0);
       
    }
    private void Update()
    {
        if (IsAliveState)
        {
            InputDirection();
            Move();
        }
    }
    #region Base Functionality
    private void InputDirection()
    {
        if (Input.GetKeyDown(KeyCode.W)) //up
        {
            if (MoveDirection!= Direction.Down)
            {
                MoveDirection = Direction.Up;
            }
        }
        if (Input.GetKeyDown(KeyCode.S)) //down
        {
            if (MoveDirection != Direction.Up)
            {
                MoveDirection = Direction.Down;
            }
        }
        if (Input.GetKeyDown(KeyCode.A)) //left
        {
            if (MoveDirection!= Direction.Right)
            {
                MoveDirection = Direction.Left;
            }
        }
        if (Input.GetKeyDown(KeyCode.D)) //right
        {
            if (MoveDirection!= Direction.Left)
            {
                MoveDirection = Direction.Right;
            }
        }
    }
   
    private void Move()
    {
        MoveTimer += Time.deltaTime;
        if (MoveTimer > MoveTimerMax)
        {
            MoveTimer -= MoveTimerMax;
            AddMovePosition();
            Vector2Int MoveDirectionVector = GetGridMoveDirectionVector();
            HandleMovement(MoveDirectionVector);    //move snake
            HandleFood();
            TrimLastPart();
            UpdateTransform(MoveDirectionVector);   //update snake's position and angle
            UpdateSnakeBody();                          //update snake body parts' position
            CheckSelfCollision();                       
        }
    }
    private Vector2Int GetGridMoveDirectionVector()
    {
        switch (MoveDirection)
        {
            default:
            case Direction.Right:
                return new Vector2Int(5, 0);
            case Direction.Left:
                return new Vector2Int(-5, 0);
            case Direction.Up:
                return new Vector2Int(0, 5);
            case Direction.Down:
                return new Vector2Int(0, -5);
        }
    }
    private void AddMovePosition()
    {
        SnakeMovePosition previousSnakeMovePosition = snakeMovePositionList.Count > 0 ? snakeMovePositionList[0] : null;
        SnakeMovePosition snakeMovePosition = new SnakeMovePosition(Position, previousSnakeMovePosition, MoveDirection);
        snakeMovePositionList.Insert(0, snakeMovePosition);
    }
    private void HandleMovement(Vector2Int moveVector)
    {
        Position += moveVector;
        Position = Grid.ValidateGridPosition(Position);
    }
    private void HandleFood()
    {
        bool isFoodEaten = Grid.SnakeEatFood(Position);
        if (isFoodEaten)
        {
            snakeSize++;
                    //Debug.Log("Snake size: " + snakeSize);
            UpdateScore(snakeSize - startSnakeSize);
            CreateSnakeBody();
            AudioSource.PlayClipAtPoint(AssetsHandler.Instance.foodEaten, new Vector3(0, 0, 0), 1.0f);
            
        }
    }

    private void TrimLastPart()
    {
        if (snakeMovePositionList.Count >= snakeSize + 1)
        {
            snakeMovePositionList.RemoveAt(snakeMovePositionList.Count - 1);
        }
    }
    private void UpdateTransform(Vector2Int gridMoveDirectionVector)
    {
        transform.position = new Vector3(Position.x, Position.y);
        transform.eulerAngles = new Vector3(0, 0, GetAngleFromVector(gridMoveDirectionVector) - 270);
    }
    private void CheckSelfCollision()
    {
        foreach (SnakeBodyPart snakeBodyPart in snakeBodyPartList)
        {
            if (Position == snakeBodyPart.GetGridPosition())
            {
                Debug.Log("GAME OVER!");
                IsAliveState = false;
                GameManager.Instance.EndGame();
                AudioSource.PlayClipAtPoint(AssetsHandler.Instance.snakeDeath, new Vector3(0, 0, 0), 0.5f);
            }
        }
    }
    #endregion
    #region Additional Functionality
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
    public List<Vector2Int> GetFullSnakePosition() 
    {
        List<Vector2Int> gridPositionList = new List<Vector2Int>() { Position};
        foreach (SnakeMovePosition position in snakeMovePositionList)
        {
            gridPositionList.Add(position.GetGridPosition());
        }
        return gridPositionList;
    }
    private void UpdateScore(int score)
    {
        GameManager.Instance.scoreText.text = $"Score: {score}";
        GameManager.Instance.score = score;
    }
    #endregion

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
        private Vector2Int position;
        private Direction direction;

        public SnakeMovePosition(Vector2Int Position, SnakeMovePosition previousSnakeMovePosition, Direction direction) 
        { 
            this.position = Position;
            this.direction = direction;
            this.previousSnakeMovePosition = previousSnakeMovePosition;
        }
        public Vector2Int GetGridPosition()
        {
            return position;
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
