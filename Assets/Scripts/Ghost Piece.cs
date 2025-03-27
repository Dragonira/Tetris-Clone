
using UnityEngine;
using UnityEngine.Tilemaps;

public class GhostPiece : MonoBehaviour
{
 public Board board;
 public Piece piece;

 public Tile tile;
    public Vector3Int[] cells { get; private set; }
    public Tilemap tilemap { get; private set; }

    public Vector3Int position { get; private set; }

    private void Awake()
    {
       this.tilemap = GetComponentInChildren<Tilemap>();
        this.cells = new Vector3Int[4];

    }

    private void Clear()
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = cells[i] + position;
            tilemap.SetTile(tilePosition, null);
        }
    }
    private void LateUpdate()
    {
        Clear();
        Copy();
        Drop();
        Set();
    }
    private void Copy()
    {
        for (int i = 0; i < cells.Length; i++)
        {
            this.cells[i] = this.piece.cells[i];
        }
    }
    private void Drop()
    {
        Vector3Int position = this.piece.Position;
        int current = position.y;
        int bottom = -this.board.boardSize.y / 2 - 1;

        this.board.Clear(this.piece);

        for (int row = current; row>=bottom; row--)
        {
            position.y = row;
            if(this.board.IsValidPosition(this.piece, position))
            {
               this.position = position;
            }
            else
            {
                break;
            }
        }
        this.board.Set(this.piece);
    }
    private void Set()
    {
        for (int i = 0; i < cells.Length; i++)
        {
            Vector3Int tilePosition = cells[i] + position;
            tilemap.SetTile(tilePosition, tile);
        }
    }

}
