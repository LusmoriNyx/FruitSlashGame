using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FruitSpawner : MonoBehaviour
{
    public static FruitSpawner Instance { get; private set; } // Singleton để dễ dàng truy cập từ các script khác

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
    private float timer = 0f;
    private bool isIncSpeedSpawn = true;

    void Start()
    {
        InitializedFunTionSpawnFruits();
        StartSpawnFruits();
    }
    void Update()
    {
        if (maxDelay - minDelay <= 0.01f)
        {
            isIncSpeedSpawn = false;
        }
        timer += Time.deltaTime;
        if (timer >= 5f && isIncSpeedSpawn)
        {
            Debug.Log("Inc speed spawn");
            minDelay += 0.2f;
            maxDelay -= 0.2f;
            timer = 0f;
        }
    }
    public void StartSpawnFruits()
    {
        StartCoroutine(SpawnFruits());
    }
    IEnumerator SpawnFruits()
    {
        while (true)
        {
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
    private void InitializedFunTionSpawnFruits()
    {
        foreach(var fruit in fruits)
        {
            for(int i = 0; i < 5; i++)
            {
                ObjectPoolManager.Instance.IniPool(fruit.Type, fruit);
            }
        }
    }
}
