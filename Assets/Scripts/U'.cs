using System;
using TMPro;
using UnityEngine;

public class ScoreUI : MonoBehaviour
{
    [SerializeField] private Board board;
    [SerializeField] private Piece block;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text linesText;

    private void Start()
    {
        board.OnLinesScoreChanged += GameBoard_OnLinesScoreChanged;
        block.OnScoreChanged += Block_OnScoreChanged;

        GameBoard_OnLinesScoreChanged(0);
        Block_OnScoreChanged(0);
    }

    private void Block_OnScoreChanged(int score)
    {
        scoreText.text = "Score:" + score.ToString();
    }

    private void GameBoard_OnLinesScoreChanged(int linesScore)
    {
        linesText.text = "Lines:" + linesScore.ToString();
    }

    private void OnDestroy()
    {
        board.OnLinesScoreChanged -= GameBoard_OnLinesScoreChanged;
        block.OnScoreChanged -= Block_OnScoreChanged;
    }
}