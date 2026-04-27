using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour
{
    public static ObjectPoolManager Instance { get; private set; }
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    //Biến để lưu trữ các pool đối tượng. Gồm loại quả và loại quả bị cắt tương ứng.
    //Dùng Enum FruitsType đã được đinhj nghĩa để phân loại quả.
    private Dictionary<FruitsType, Queue<GameObject>> fruitsPoolObject = 
        new Dictionary<FruitsType, Queue<GameObject>>();
    private Dictionary<FruitsType, Queue<GameObject>> slicedFruitsPoolObject = 
        new Dictionary<FruitsType, Queue<GameObject>>();

    //Hàm khởi tạo pool đối tượng cho một loại quả cụ thể. Nếu pool chưa tồn tại, nó sẽ được tạo mới.
    public void IniPool(FruitsType type, Fruit fruitQueue)
    {
        if (!fruitsPoolObject.ContainsKey(type))
        {
            fruitsPoolObject[type] = new Queue<GameObject>();
        }
        if (!slicedFruitsPoolObject.ContainsKey(type) && type != FruitsType.Bomb)
        {
            slicedFruitsPoolObject[type] = new Queue<GameObject>();
        }
        GameObject spawnedFruit = Instantiate(fruitQueue.FruitPrefab);
        spawnedFruit.SetActive(false);

        spawnedFruit.GetComponent<FruitBehaviour>().fruit = fruitQueue;
        //spawnedSlicedFruit.GetComponent<FruitSliceBehaviour>().sliceFruit = fruitQueue;

        fruitsPoolObject[type].Enqueue(spawnedFruit);
        GameObject spawnedSlicedFruit;
        if (type != FruitsType.Bomb)
        {
            spawnedSlicedFruit = Instantiate(fruitQueue.SlicedFruitPrefab);
            slicedFruitsPoolObject[type].Enqueue(spawnedSlicedFruit);
            spawnedSlicedFruit.SetActive(false);
        }
    }
    public GameObject GetObjectFruit(Fruit fruit)
    {
        if (!GameManager.Instance.isGameStarted) return null;
        if (!fruitsPoolObject.ContainsKey(fruit.Type) || fruitsPoolObject[fruit.Type].Count == 0)
        {
            IniPool(fruit.Type, fruit);
        }
        GameObject obj = fruitsPoolObject[fruit.Type].Dequeue();
        obj.SetActive(true);
        return obj ;
    }
    public GameObject GetObjectSlicedFruit(Fruit slice)
    {
        if (!GameManager.Instance.isGameStarted) return null;
        if (!slicedFruitsPoolObject.ContainsKey(slice.Type) || slicedFruitsPoolObject[slice.Type].Count == 0)
        {
            IniPool(slice.Type, slice);
        }
        GameObject obj = slicedFruitsPoolObject[slice.Type].Dequeue();
        //obj.SetActive(true);
        return obj;
    }
    public void ReturnObjectFruit(Fruit fruit, GameObject obj)
    {
        obj.SetActive(false);
        fruitsPoolObject[fruit.Type].Enqueue(obj);
    }
    public void ReturnObjectSlicedFruit(Fruit slice, GameObject obj)
    {
        obj.SetActive(false);
        slicedFruitsPoolObject[slice.Type].Enqueue(obj);
    }
}
