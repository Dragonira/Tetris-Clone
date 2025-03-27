using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GridBrushBase;

public class Piece : MonoBehaviour
{
    public Board Board { get; private set; }
    public TetrominoData Data { get; private set; }
    public Vector3Int Position { get; private set; }


    public event Action<int> OnScoreChanged;

    [SerializeField] public TextMeshProUGUI scoreText;
    [SerializeField] public TextMeshProUGUI LinesText;
    public int score = 0;
    public int lines = 0;
    public int Points = 10;
   

    public float stepDelay = 1f;
    public float lockDelay = 0.5f;
    private float stepTime;
    private float lockTime;
    public int Rotation { get; private set; }
    public Vector3Int[] cells { get; private set; }

    public void Start()
    {
        scoreText.text = "Score: " + score;
        LinesText.text = "Lines: " + lines;
    }
    public void Initialize(Board board , Vector3Int position,TetrominoData data)
    {
        this.Board = board;
        this.Position = position;
        this.Data = data;
        this.Rotation = 0;
        this.stepTime = Time.time + stepDelay;
        this.lockTime = 0f;

        if (this.cells == null)
        {
           this.cells = new Vector3Int[this.Data.Cells.Length];
        }

        for (int i = 0; i < this.Data.Cells.Length; i++)
        {
            this.cells[i] = (Vector3Int)Data.Cells[i];
        }
    }
    public void Update()
    {
        this.Board.Clear(this);

        this.lockTime += Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Rotate(1);
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            Rotate(-1);
        }
        

        if (Input.GetKeyDown(KeyCode.A))
        {
            Move(Vector2Int.left);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            Move(Vector2Int.right);
        }
        if(Input.GetKeyDown(KeyCode.S))
        {
            Move(Vector2Int.down);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            HardDrop();
        }
        if(Time.time >= this.stepTime)
        {
            Step(); 
        }
        this.Board.Set(this);

    }

    private void Step()
    {
      
        this.stepTime = Time.time + stepDelay;

        Move(Vector2Int.down);

        if (this.lockTime >= this.lockDelay)
        {
            Lock();
        }
    }
    private void Lock()
    {
        
        this.Board.Set(this);
       this.Board.ClearLines();
       this.Board.SpawnTetromino();
        SetScore(10);
    }
    private void HardDrop()
    {
        while(Move(Vector2Int.down))
            continue;
        Lock();
    }
    public bool Move(Vector2Int direction)
    {
        Vector3Int newPosition = this.Position;
        newPosition.x += direction.x;
        newPosition.y += direction.y;

        bool valid = this.Board.IsValidPosition(this, newPosition);

        if (valid)
        {
            this.Position = newPosition;
            this.lockTime = 0f;

        }
        return valid;
    }

    private void Rotate(int direction)
    {
        int originRotation = this.Rotation;
        this.Rotation += Warp(this.Rotation + direction,0,4) ;
        ApplyRotationMatrix(direction);
        if (!TestWallKicks(this.Rotation, direction))
        {
            this.Rotation = originRotation;
            ApplyRotationMatrix(-direction);
        }

    }
    private void ApplyRotationMatrix(int direction)
    {
        for (int i = 0; i < this.cells.Length; i++)
        {
            Vector3 cells = this.cells[i];
            int x, y;
            switch (this.Data.tetromino)
            {
                case Tetromino.Letter_I:
                case Tetromino.Letter_O:
                    // "I" and "O" are rotated from an offset center point
                    cells.x -= 0.5f;
                    cells.y -= 0.5f;
                    x = Mathf.CeilToInt((cells.x * data.RotationMatrix[0] * direction) + (cells.y * data.RotationMatrix[1] * direction));
                    y = Mathf.CeilToInt((cells.x * data.RotationMatrix[2] * direction) + (cells.y * data.RotationMatrix[3] * direction));
                    break;

                default:
                    x = Mathf.RoundToInt((cells.x * data.RotationMatrix[0] * direction) + (cells.y * data.RotationMatrix[1] * direction));
                    y = Mathf.RoundToInt((cells.x * data.RotationMatrix[2] * direction) + (cells.y * data.RotationMatrix[3] * direction));
                    break;
            }

            this.cells[i] = new Vector3Int(x, y, 0);
        }
    }

    private bool TestWallKicks(int rotationIndex, int rotationDirection)
    {
        int TetWallKicksIndex = TestWallKicksIndex(rotationIndex, rotationDirection);
        
        for (int i = 0 ; i < this.Data.WallKicks.GetLength(1) ; i++)
        {
            Vector2Int wallKick = this.Data.WallKicks[TetWallKicksIndex, i];
            if (Move(wallKick))
            {
                return true;
            }
        }

        return false;

    }

    private int TestWallKicksIndex(int rotationIndex, int rotationDirection)
    {
        int wallKicksIndex = rotationIndex * 2;
        if (rotationDirection < 0)
        {
            wallKicksIndex--;
        }
        return Warp(wallKicksIndex, 0, this.Data.WallKicks.GetLength(0));
    }
    private int Warp(int input, int min, int max)
    {
        if (input < min)
        {
            return max+(min - input) %(max - min);
        }
        else 
        {
            return min + (input - min) % (max - min);
        }
        
    }
    public void SetScore(int amount)
    {
        score += amount;
        OnScoreChanged?.Invoke(score);
    }

    public void ResetScore()
    {
        score = 0;
        OnScoreChanged?.Invoke(score);
    }
}

