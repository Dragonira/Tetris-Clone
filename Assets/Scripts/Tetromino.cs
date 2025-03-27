using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum Tetromino
{
    Letter_I,
    Letter_O,
    Letter_T,
    Letter_J,
    Letter_L,
    Letter_S,
    Letter_Z
}

[Serializable]
public struct TetrominoData
{
    public Tetromino tetromino;
    public Tile tile;
    public Vector2Int[] Cells { get; private set; }
    public Vector2Int[,] WallKicks { get; private set; }

    public void Initialize()
    {
        Cells = data.Cells[tetromino];
        WallKicks = data.WallKicks[tetromino];
    }
}
