using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

/// <summary>
/// LifeManager tối ưu hiệu suất: quản lý số mạng, điểm số, timer, xử lý game over mượt hơn.
/// </summary>
public class LifeManager : MonoBehaviour
{
    public static LifeManager Instance;

    [SerializeField] private List<GameObject> lives = new List<GameObject>();
    [SerializeField] private int lifeCount = 3;

    [Header("Score")]
    public TMP_Text scoreText;
    private int currentScore = 0;

    [Header("Game Over Panel")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject excellentImg;
    [SerializeField] private GameObject modestImg;
    [SerializeField] private TMP_Text rankText;

    [SerializeField] private UnityEngine.UI.Button restartButton;
    [SerializeField] private UnityEngine.UI.Button continueButton;

    [Header("Timer UI")]
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private float totalTime = 60f;
    private float currentTime;
    private bool isTimerRunning;
    private bool isTimeGameOver;
    private int lastDisplayedSeconds = -1;

    private Coroutine timerGameOverCoroutine;

    private readonly WaitForSeconds waitDelay = new WaitForSeconds(2f);

    /// <summary>
    /// Khởi tạo singleton để các script khác truy cập LifeManager.
    /// </summary>
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    /// <summary>
    /// Khởi tạo UI điểm, UI game over, timer và đăng ký sự kiện nút.
    /// </summary>
    private void Start()
    {
        UpdateScoreUI();
        if (gameOverPanel) gameOverPanel.SetActive(false);
        if (excellentImg) excellentImg.SetActive(false);
        if (modestImg) modestImg.SetActive(false);

        if (restartButton) restartButton.onClick.AddListener(OnRestartClicked);
        if (continueButton) continueButton.onClick.AddListener(OnContinueClicked);

        currentTime = totalTime;
        isTimerRunning = false;
        isTimeGameOver = false;
        UpdateTimerUI();
    }

    /// <summary>
    /// Điều khiển timer theo trạng thái game và xử lý hết giờ.
    /// </summary>
    private void Update()
    {
        // Bộ flag, tránh truy cập lặp GameManager và update timer nếu không cần
        var gm = GameManager.Instance;
        if (gm != null && !isTimeGameOver)
        {
            if (gm.isGameStarted)
            {
                if (!isTimerRunning)
                {
                    StartTimer();
                }
            }
            else if (isTimerRunning)
            {
                StopTimer();
            }
        }

        if (!isTimerRunning) return;

        currentTime -= Time.deltaTime;
        if (currentTime <= 0f)
        {
            if (currentTime < 0f) currentTime = 0f;

            isTimerRunning = false;
            isTimeGameOver = true;

            UpdateTimerUI();

            if (gm != null) gm.isGameStarted = false;

            if (timerGameOverCoroutine == null)
                timerGameOverCoroutine = StartCoroutine(ShowGameOverAfterDelay());
        }
        else
        {
            UpdateTimerUI();
        }
    }

    // Không cần truyền delay vào coroutine, tối ưu giữ WaitForSeconds instance
    /// <summary>
    /// Chờ một khoảng ngắn trước khi hiển thị panel game over do hết giờ.
    /// </summary>
    private IEnumerator ShowGameOverAfterDelay()
    {
        yield return waitDelay;
        ShowGameOverPanel();
        timerGameOverCoroutine = null;
    }

    /// <summary>
    /// Bắt đầu chạy timer nếu game đang ở trạng thái hợp lệ.
    /// </summary>
    private void StartTimer()
    {
        if (isTimeGameOver) return;
        if (currentTime <= 0f || currentTime > totalTime)
            currentTime = totalTime;
        isTimerRunning = true;
        UpdateTimerUI();
    }

    /// <summary>
    /// Reset timer về giá trị ban đầu và hủy coroutine game over đang chờ (nếu có).
    /// </summary>
    private void ResetTimer()
    {
        currentTime = totalTime;
        isTimeGameOver = false;

        isTimerRunning = (GameManager.Instance != null && GameManager.Instance.isGameStarted);

        UpdateTimerUI();

        if (timerGameOverCoroutine != null)
        {
            StopCoroutine(timerGameOverCoroutine);
            timerGameOverCoroutine = null;
        }
    }

    /// <summary>
    /// Tạm dừng timer.
    /// </summary>
    private void StopTimer()
    {
        isTimerRunning = false;
    }

    /// <summary>
    /// Cập nhật text timer, chỉ đổi khi số giây thay đổi.
    /// </summary>
    private void UpdateTimerUI()
    {
        if (!timerText) return;
        int seconds = Mathf.Max(0, Mathf.CeilToInt(currentTime));
        if (seconds == lastDisplayedSeconds) return;
        lastDisplayedSeconds = seconds;
        timerText.text = $"00:{seconds:D2}";
    }

    /// <summary>
    /// Trừ một mạng, cập nhật icon mạng và xử lý game over khi hết mạng.
    /// </summary>
    public void UpdateLives()
    {
        if (lifeCount <= 0 || isTimeGameOver) return;

        int index = lifeCount - 1;
        if (index >= 0 && index < lives.Count && lives[index]) // tránh lỗi out of range/null
            lives[index].SetActive(false);

        lifeCount--;
        if (lifeCount == 0)
        {
            var gm = GameManager.Instance;
            if (gm != null)
                gm.isGameStarted = false;
            ShowGameOverPanel();
            StopTimer();
        }
    }

    /// <summary>
    /// Hiển thị panel game over và xếp loại theo điểm hiện tại.
    /// </summary>
    private void ShowGameOverPanel()
    {
        if (!gameOverPanel || !excellentImg || !modestImg || !rankText) return;

        gameOverPanel.SetActive(true);

        bool isExcellent = currentScore >= 100;
        excellentImg.SetActive(isExcellent);
        modestImg.SetActive(!isExcellent);
        rankText.text = isExcellent ? "Xếp loại: Xuất sắc" : "Xếp loại: Khiêm tốn";
    }

    /// <summary>
    /// Xử lý nút Restart: tải lại scene hiện tại.
    /// </summary>
    private void OnRestartClicked()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// Xử lý nút Continue: reset trạng thái và chạy lại luồng bắt đầu game.
    /// </summary>
    private void OnContinueClicked()
    {
        if (gameOverPanel) gameOverPanel.SetActive(false);
        if (excellentImg) excellentImg.SetActive(false);
        if (modestImg) modestImg.SetActive(false);

        lifeCount = lives.Count;
        for (int i = 0; i < lives.Count; ++i)
            if (lives[i]) lives[i].SetActive(true);

        ResetScore();
        ResetTimer();

        var gm = GameManager.Instance;
        if (gm != null)
        {
            if (gm.gameUI) gm.gameUI.SetActive(false);
            if (gm.startButton) gm.startButton.SetActive(false);

            gm.isGameStarted = false;
            gm.OnClickStart();
        }
    }

    /// <summary>
    /// Cộng điểm cho người chơi và cập nhật UI điểm.
    /// </summary>
    public void AddScore(int points)
    {
        currentScore += points;
        UpdateScoreUI();
#if UNITY_EDITOR
        Debug.Log($"+ {points} điểm. Tổng: {currentScore}");
#endif
    }

    /// <summary>
    /// Đưa điểm về 0 và làm mới UI điểm.
    /// </summary>
    public void ResetScore()
    {
        currentScore = 0;
        UpdateScoreUI();
    }

    /// <summary>
    /// Hiển thị điểm hiện tại lên text UI.
    /// </summary>
    private void UpdateScoreUI()
    {
        if (scoreText)
            scoreText.text = currentScore.ToString();
    }
}