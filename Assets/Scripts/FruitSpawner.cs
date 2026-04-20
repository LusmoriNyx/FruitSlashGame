using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FruitSpawner : MonoBehaviour
{
    [SerializeField] private List<Fruit> fruits = new List<Fruit>();
    [SerializeField] private Transform[] spawnPoints;
    private float minDelay = 0.1f;
    private float maxDelay = 1f;
    private float timer = 0f;
    private bool isIncSpeedSpawn = true;

    void Start()
    {
        InitializedFunTionSpawnFruits();
    }
    public void StartSpawnFruits()
    {
        StartCoroutine(SpawnFruits());
    }
    IEnumerator SpawnFruits()
    {
        while (true)
        {
            if (minDelay > maxDelay)
            {
                isIncSpeedSpawn = false;
            }
            timer += Time.deltaTime;
            if(timer >= 5f && isIncSpeedSpawn)
            {
                minDelay += 0.1f;
                maxDelay -= 0.1f;
                timer = 0f;
            }
            
            float delay = Random.Range(minDelay, maxDelay);
            yield return new WaitForSeconds(delay);
            int spawnIndex = Random.Range(0, spawnPoints.Length);
            Transform spawnPoint = spawnPoints[spawnIndex];
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
            }
        }
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
