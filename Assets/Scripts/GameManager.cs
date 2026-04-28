using System.Collections;
using UnityEngine;
using TMPro;

/// <summary>
/// Quản lý trạng thái game, UI và luồng bắt đầu game.
/// </summary>
public class GameManager : MonoBehaviour
{
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

    // Thời gian hiệu ứng scale cho nút Start
    public float scaleDuration = 0.3f;
    // Giá trị scale tối đa nút Start
    public float targetScale = 1.2f;

    /// <summary>
    /// Thiết lập singleton khi Awake
    /// </summary>
    private void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// Khởi tạo trạng thái UI khi bắt đầu game
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
        Debug.Log("Nút Start đã được nhấn!");
        // Disable tương tác để tránh bấm nhiều lần liên tiếp
        startButton.GetComponent<UnityEngine.UI.Button>().interactable = false;
        // Bắt đầu hiệu ứng scale cho nút Start
        StartCoroutine(PlayButtonScaleEffect());
    }

    /// <summary>
    /// Hiệu ứng scale cho nút Start khi bấm, sau đó ẩn nút và hiển thị panel đếm ngược
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

        // Hiện panel đếm ngược, bắt đầu count down
        countdownPanel.SetActive(true);
        StartCoroutine(CountDownRoutine());
    }

    /// <summary>
    /// Đếm ngược 3 đến 1, hiển thị lên màn hình, gọi StartGame khi xong
    /// </summary>
    IEnumerator CountDownRoutine()
    {
        for (int i = 3; i > 0; i--)
        {
            countdownText.text = i.ToString();
            yield return new WaitForSeconds(1f); // Đợi 1 giây mỗi số
        }

        // Ẩn panel đếm ngược, bắt đầu game
        countdownPanel.SetActive(false);
        StartGame();
    }

    /// <summary>
    /// Kích hoạt UI game chính, reset điểm số và thay đổi trạng thái game
    /// </summary>
    void StartGame()
    {
        gameUI.SetActive(true);        // Hiện UI game
        isGameStarted = true;          // Cập nhật trạng thái game đã bắt đầu
        // Reset điểm số nếu có LifeManager Instance
        if (LifeManager.Instance != null)
            LifeManager.Instance.ResetScore();
        Debug.Log("Game Start!");
    }
}