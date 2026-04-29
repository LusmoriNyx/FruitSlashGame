using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// Quản lý trạng thái game, UI và luồng bắt đầu game.
/// </summary>
public class GameManager : MonoBehaviour
{
    private static readonly WaitForSeconds CountdownTick = new WaitForSeconds(1f);

    // Singleton instance để truy cập toàn cục
    public static GameManager Instance;

    [Header("UI")]
    // Nút bắt đầu game
    public GameObject startButton;
    // Panel hiển thị đếm ngược trước khi vào game
    public GameObject countdownPanel;
    // Text dùng để hiển thị số đếm ngược
    public TMP_Text countdownText;
    // Giao diện game khi đang chơi
    public GameObject gameUI;

    // Cờ kiểm tra xem game đã bắt đầu chưa
    public bool isGameStarted = false;

    [SerializeField] private Button startButtonComponent;

    // Thời gian hiệu ứng scale cho nút Start
    public float scaleDuration = 0.3f;
    // Giá trị scale tối đa nút Start
    public float targetScale = 1.2f;
    private bool isStartSequenceRunning;

    /// <summary>
    /// Thiết lập singleton khi Awake
    /// </summary>
    private void Awake()
    {
        Instance = this;
        if (startButton != null && startButtonComponent == null)
            startButtonComponent = startButton.GetComponent<Button>();
    }

    /// <summary>
    /// Khởi tạo trạng thái UI khi bắt đầu game
    /// </summary>
    /// <summary>
    /// Thiết lập trạng thái UI ban đầu khi vào scene.
    /// </summary>
    void Start()
    {
        gameUI.SetActive(false);           // Ẩn UI game chính
        startButton.SetActive(true);       // Hiện nút Start
        countdownPanel.SetActive(false);   // Ẩn panel đếm ngược
        isGameStarted = false;             // Gán trạng thái game chưa bắt đầu
    }

    /// <summary>
    /// Hàm public xử lý sự kiện khi bấm nút Start
    /// </summary>
    public void OnClickStart()
    {
        if (isStartSequenceRunning) return;

        Debug.Log("Nút Start đã được nhấn!");
        // Disable tương tác để tránh bấm nhiều lần liên tiếp
        if (startButtonComponent != null)
            startButtonComponent.interactable = false;
        // Bắt đầu hiệu ứng scale cho nút Start
        isStartSequenceRunning = true;
        StartCoroutine(PlayButtonScaleEffect());
    }

    /// <summary>
    /// Hiệu ứng scale cho nút Start khi bấm, sau đó ẩn nút và hiển thị panel đếm ngược
    /// </summary>
    /// <summary>
    /// Chạy animation phóng to/thu nhỏ nút Start rồi chuyển sang đếm ngược.
    /// </summary>
    IEnumerator PlayButtonScaleEffect()
    {
        Transform btnTransform = startButton.transform;
        Vector3 originalScale = btnTransform.localScale;

        // Scale up: Phóng to nút Start
        float elapsed = 0;
        while (elapsed < scaleDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / scaleDuration;
            float scale = Mathf.Lerp(1f, targetScale, t); // Scale từ 1 lên targetScale
            btnTransform.localScale = originalScale * scale;
            yield return null;
        }
        btnTransform.localScale = originalScale * targetScale;

        // Scale down: Thu nhỏ nút Start về 0
        elapsed = 0;
        while (elapsed < scaleDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / scaleDuration;
            float scale = Mathf.Lerp(targetScale, 0f, t); // Scale từ targetScale về 0
            btnTransform.localScale = originalScale * scale;
            yield return null;
        }

        // Ẩn nút Start và trả về scale ban đầu để tránh bug cho lần sau (nếu có dùng lại)
        startButton.SetActive(false);
        btnTransform.localScale = originalScale;

        // Đặt giá trị ban đầu để không bị nháy số cũ trước khi vòng for chạy.
        if (countdownText != null)
            countdownText.text = "3";

        // Hiện panel đếm ngược, bắt đầu count down
        if (countdownPanel != null)
            countdownPanel.SetActive(true);
        StartCoroutine(CountDownRoutine());
    }

    /// <summary>
    /// Đếm ngược 3 đến 1, hiển thị lên màn hình, gọi StartGame khi xong
    /// </summary>
    /// <summary>
    /// Hiển thị đếm ngược 3-2-1 trước khi bắt đầu gameplay.
    /// </summary>
    IEnumerator CountDownRoutine()
    {
        for (int i = 3; i > 0; i--)
        {
            if (countdownText != null)
                countdownText.text = i.ToString();
            yield return CountdownTick; // Đợi 1 giây mỗi số
        }

        // Trước khi tắt panel, reset lại số hiển thị về 3
        if (countdownText != null)
            countdownText.text = "3";

        // Ẩn panel đếm ngược, sau đó bắt đầu game
        countdownPanel.SetActive(false);

        StartGame();
    }

    /// <summary>
    /// Kích hoạt UI game chính, reset điểm số và thay đổi trạng thái game
    /// </summary>
    /// <summary>
    /// Bật UI gameplay và đánh dấu game đã bắt đầu.
    /// </summary>
    void StartGame()
    {
        if (gameUI != null)
            gameUI.SetActive(true);        // Hiện UI game
        isGameStarted = true;          // Cập nhật trạng thái game đã bắt đầu
        // Reset điểm số nếu có LifeManager Instance
        if (LifeManager.Instance != null)
            LifeManager.Instance.ResetScore();
        Debug.Log("Game Start!");
        isStartSequenceRunning = false;
    }
}