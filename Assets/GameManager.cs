using System.Collections;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI")]
    public GameObject startButton;
    public GameObject countdownPanel;
    public TMP_Text countdownText;

    public bool isGameStarted = false;

    // Thời gian cho hiệu ứng phóng to/nhỏ
    public float scaleDuration = 0.3f;
    public float targetScale = 1.2f;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        startButton.SetActive(true);
        countdownPanel.SetActive(false);
        isGameStarted = false;
    }

    public void OnClickStart()
    {
        Debug.Log("Nút Start đã được nhấn!");
        // Vô hiệu hóa nút để tránh spam click
        startButton.GetComponent<UnityEngine.UI.Button>().interactable = false;
        // Chạy hiệu ứng phóng to/nhỏ
        StartCoroutine(PlayButtonScaleEffect());
    }

    IEnumerator PlayButtonScaleEffect()
    {
        Transform btnTransform = startButton.transform;
        Vector3 originalScale = btnTransform.localScale;

        // Phóng to
        float elapsed = 0;
        while (elapsed < scaleDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / scaleDuration;
            float scale = Mathf.Lerp(1f, targetScale, t);
            btnTransform.localScale = originalScale * scale;
            yield return null;
        }
        btnTransform.localScale = originalScale * targetScale;

        // Thu nhỏ về 0 (hoặc về kích thước ban đầu) - thường dùng để ẩn
        elapsed = 0;
        while (elapsed < scaleDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / scaleDuration;
            float scale = Mathf.Lerp(targetScale, 0f, t);
            btnTransform.localScale = originalScale * scale;
            yield return null;
        }

        // Ẩn nút hoàn toàn
        startButton.SetActive(false);
        // Reset scale cho lần chơi sau
        btnTransform.localScale = originalScale;

        // Hiện panel đếm ngược
        countdownPanel.SetActive(true);
        StartCoroutine(CountDownRoutine());
    }

    IEnumerator CountDownRoutine()
    {
        for (int i = 3; i > 0; i--)
        {
            countdownText.text = i.ToString();
            yield return new WaitForSeconds(1f);
        }

        countdownPanel.SetActive(false);
        StartGame();
    }

    void StartGame()
    {
        isGameStarted = true;
        if (LifeManager.Instance != null)
            LifeManager.Instance.ResetScore();
        Debug.Log("Game Start!");
    }
}