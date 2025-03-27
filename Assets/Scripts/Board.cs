using System;

using UnityEngine;

using UnityEngine.Tilemaps;

public class Board : MonoBehaviour
{
    [SerializeField]
    public Tilemap tilemap;
    [SerializeField] public Piece activePiece;

    private int linesScore;
    public event Action OnGameOver;
    public event Action<int> OnLinesScoreChanged;
    public bool canPlay = true;


    public Vector3Int spawnPosition;
    public Vector3Int NextPosition;
    public Vector2Int boardSize = new Vector2Int(10, 20);

    public RectInt Bounds
    {
        get
        {
            Vector2Int min = new Vector2Int(-this.boardSize.x / 2, -this.boardSize.y / 2);
            return new RectInt(min, this.boardSize);
        }
    }

    public TetrominoData[] Tetrominoes;

    private void Awake()
    {
        tilemap = GetComponentInChildren<Tilemap>();
        activePiece = GetComponentInChildren<Piece>();
     
        for (int i = 0; i < Tetrominoes.Length; i++)
        {
            Tetrominoes[i].Initialize();
        }

    }

    public void Start()
    {
        SpawnTetromino();
      

    }

    public void SpawnTetromino()
    {
        if (!canPlay) { return; }
        int random = UnityEngine.Random.Range(0, Tetrominoes.Length);
        TetrominoData data = Tetrominoes[random];

        activePiece.Initialize(this, spawnPosition, data);

        if (IsValidPosition(this.activePiece, this.spawnPosition))
        {
            Set(this.activePiece);
        }
        else
        {
            GameOver();
        }
    }

    public void GameOver()
    {
        
        this.tilemap.ClearAllTiles();
        FinishGame();

       
        OnGameOver?.Invoke();
    }
    public void Set(Piece piece)
    {
        if (!canPlay) { return; }

        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.Position;
            tilemap.SetTile(tilePosition, piece.Data.tile);
        }
    }
    public bool IsValidPosition(Piece piece, Vector3Int postion)
    {
        RectInt bounds = this.Bounds;
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + postion;

            if (!bounds.Contains((Vector2Int)tilePosition))
            {
                return false;
            }
            if (this.tilemap.HasTile(tilePosition))
            {
                return false;
            }
        }
        return true;
    }

    public void Clear(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.Position;
            this.tilemap.SetTile(tilePosition, null);
        }
    }

    public void ClearLines()
    {
        RectInt bounds = this.Bounds;
        int row = bounds.yMin;

        while (row < bounds.yMax)
        {
            if (IsLineComplete(row))
            {
                LineClear(row);
                activePiece.SetScore(100);
                SetLinesScore(1);
            }
            else
            {
                row++;
            }
        }
    }
    private bool IsLineComplete(int row)
        {
            RectInt bounds = this.Bounds;
            for (int x = bounds.xMin; x < bounds.xMax; x++)
            {
                Vector3Int tilePosition = new Vector3Int(x, row, 0);
                if (!this.tilemap.HasTile(tilePosition))
                {
                    return false;
                }
            }
            return true;
        }
    private void LineClear(int row)
    {
       
        RectInt bounds = this.Bounds;
        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            Vector3Int tilePosition = new Vector3Int(x,  row , 0);
            this.tilemap.SetTile(tilePosition, null);
        }
        
        while(row < bounds.yMax)
        {
            for (int x = bounds.xMin; x < bounds.xMax; x++)
            {
                Vector3Int tilePosition = new Vector3Int(x, row + 1, 0);
                TileBase tile = this.tilemap.GetTile(tilePosition);
                tilePosition = new Vector3Int(x, row, 0 );
                this.tilemap.SetTile(tilePosition, tile);
            }
            row++;
        }
    }

    private void SetLinesScore(int amount)
    {
        linesScore += amount;
        OnLinesScoreChanged?.Invoke(linesScore);
    }

    private void ResetLinesScore()
    {
        linesScore = 0;
        OnLinesScoreChanged?.Invoke(linesScore);
    }

    private void FinishGame()
    {
        canPlay = false;
        this.enabled = false;
        activePiece.enabled = false;
    }

    public void RestartGame()
    {
        canPlay = true;
        this.enabled = true;
        activePiece.enabled = true;

        ResetLinesScore();
        activePiece.ResetScore();
    }

}



