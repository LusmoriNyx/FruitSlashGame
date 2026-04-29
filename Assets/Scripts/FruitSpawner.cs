using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FruitSpawner : MonoBehaviour
{
    public static FruitSpawner Instance { get; private set; } // Singleton để dễ dàng truy cập từ các script khác
    private static readonly WaitForSeconds IdleSpawnWait = new WaitForSeconds(0.1f);

    [SerializeField] private List<Fruit> fruits = new List<Fruit>();
    [SerializeField] private Transform[] spawnPoints;
    [Header("Side Spawn Points")]
    [SerializeField] private Transform leftSpawnPoint;
    [SerializeField] private Transform rightSpawnPoint;
    [SerializeField, Range(0f, 1f)] private float sideSpawnRate = 0.4f;

    [Header("Side Throw Force")]
    [SerializeField] private Transform centerTarget;
    [SerializeField] private float sideHorizontalBias = 2f;
    [SerializeField] private float sideUpwardBias = 1f;
    [SerializeField] private float sideForceMultiplier = 1f;
    private float minDelay = 0.1f;
    private float maxDelay = 3f;
    private float speedRampTimer = 0f;
    private bool isIncSpeedSpawn = true;

    /// <summary>
    /// Khởi tạo object pool ban đầu và bắt đầu vòng spawn.
    /// </summary>
    void Start()
    {
        InitializedFunTionSpawnFruits();
        StartSpawnFruits();
    }
    /// <summary>
    /// Tăng dần tốc độ spawn theo thời gian để tăng độ khó.
    /// </summary>
    void Update()
    {
        if (!isIncSpeedSpawn) return;

        if (maxDelay - minDelay <= 0.01f)
        {
            isIncSpeedSpawn = false;
            return;
        }
        speedRampTimer += Time.deltaTime;
        if (speedRampTimer >= 5f)
        {
#if UNITY_EDITOR
            Debug.Log("Inc speed spawn");
#endif
            minDelay = Mathf.Min(minDelay + 0.2f, maxDelay - 0.01f);
            maxDelay = Mathf.Max(maxDelay - 0.2f, minDelay + 0.01f);
            speedRampTimer -= 5f;
        }
    }
    /// <summary>
    /// Khởi chạy coroutine spawn trái cây.
    /// </summary>
    public void StartSpawnFruits()
    {
        StartCoroutine(SpawnFruits());
    }
    /// <summary>
    /// Vòng lặp spawn trái cây liên tục khi game đang chạy.
    /// </summary>
    IEnumerator SpawnFruits()
    {
        while (true)
        {
            if (GameManager.Instance == null || !GameManager.Instance.isGameStarted)
            {
                yield return IdleSpawnWait;
                continue;
            }

            float delay = Random.Range(minDelay, maxDelay);
            yield return new WaitForSeconds(delay);
            bool isSideSpawn;
            Transform spawnPoint = GetSpawnPoint(out isSideSpawn);
            if (spawnPoint == null)
            {
                continue;
            }
            Fruit selectedFruit = fruits[Random.Range(0, fruits.Count)];
            GameObject objectFruit = null;
            if (selectedFruit != null && selectedFruit.Type == FruitsType.Bomb)
            {
                if (Random.Range(0, 1f) < selectedFruit.SpawnRate)
                {
                    objectFruit = ObjectPoolManager.Instance.GetObjectFruit(selectedFruit);
                }
            }
            else
            {
                objectFruit = ObjectPoolManager.Instance.GetObjectFruit(selectedFruit);
            }
            if (objectFruit != null)
            {
                objectFruit.transform.position = spawnPoint.position;

                if (isSideSpawn)
                {
                    FruitBehaviour fruitBehaviour = objectFruit.GetComponent<FruitBehaviour>();
                    if (fruitBehaviour != null)
                    {
                        Vector3 sideDirection = CalculateSideDirection(spawnPoint.position);
                        fruitBehaviour.LaunchWithDirection(sideDirection, sideForceMultiplier);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Chọn ngẫu nhiên điểm spawn thường hoặc điểm spawn bên hông.
    /// </summary>
    private Transform GetSpawnPoint(out bool isSideSpawn)
    {
        bool canUseSide = leftSpawnPoint != null && rightSpawnPoint != null;
        if (canUseSide && Random.value <= sideSpawnRate)
        {
            isSideSpawn = true;
            return Random.value < 0.5f ? leftSpawnPoint : rightSpawnPoint;
        }

        isSideSpawn = false;
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            return null;
        }

        int spawnIndex = Random.Range(0, spawnPoints.Length);
        return spawnPoints[spawnIndex];
    }

    /// <summary>
    /// Tính hướng ném từ cạnh vào trung tâm với bias ngang và dọc.
    /// </summary>
    private Vector3 CalculateSideDirection(Vector3 spawnPosition)
    {
        Vector3 centerPosition = GetCenterPosition(spawnPosition);
        Vector3 toCenter = centerPosition - spawnPosition;
        Vector3 horizontal = new Vector3(toCenter.x, 0f, toCenter.z);

        if (horizontal.sqrMagnitude < 0.0001f)
        {
            horizontal = spawnPosition.x <= centerPosition.x ? Vector3.right : Vector3.left;
        }

        Vector3 launchDirection = (horizontal.normalized * sideHorizontalBias) + (Vector3.up * sideUpwardBias);
        return launchDirection.normalized;
    }

    /// <summary>
    /// Lấy vị trí trung tâm mục tiêu để tính hướng ném trái cây.
    /// </summary>
    private Vector3 GetCenterPosition(Vector3 spawnPosition)
    {
        if (centerTarget != null)
        {
            return centerTarget.position;
        }

        Camera mainCam = Camera.main;
        if (mainCam == null)
        {
            return transform.position;
        }

        float depthFromCamera = Mathf.Abs(spawnPosition.z - mainCam.transform.position.z);
        return mainCam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, depthFromCamera));
    }
    /// <summary>
    /// Tạo sẵn một số object trong pool cho từng loại trái cây.
    /// </summary>
    private void InitializedFunTionSpawnFruits()
    {
        if (ObjectPoolManager.Instance == null) return;

        foreach(var fruit in fruits)
        {
            for(int i = 0; i < 5; i++)
            {
                ObjectPoolManager.Instance.IniPool(fruit.Type, fruit);
            }
        }
    }
}
