using UnityEngine;

using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Board board;
    [SerializeField] private Button gameOverButton;

    private CanvasGroup canvasGroup;
    private float fadeDuration = 1f;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        board.OnGameOver += GameBoard_OnGameOver;

        Hide();

        gameOverButton.onClick.AddListener(() =>
        {
            Hide();
            board.RestartGame();
           
        });
    }

    private void GameBoard_OnGameOver()
    {
        Show();
    }

    private void Show()
    {
        gameOverButton.gameObject.SetActive(true);
   
    }

    private void Hide()
    {
    
            gameOverButton.gameObject.SetActive(false);
   
    }

    private void OnDisable()
    {
        board.OnGameOver -= GameBoard_OnGameOver;
    }
}