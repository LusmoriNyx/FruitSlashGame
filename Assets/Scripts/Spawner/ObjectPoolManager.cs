using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour
{
    public static ObjectPoolManager Instance { get; private set; }
    [SerializeField] private Transform poolRoot;
    /// <summary>
    /// Thiết lập singleton cho ObjectPoolManager.
    /// </summary>
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
    /// <summary>
    /// Khởi tạo thêm object vào pool theo loại trái cây.
    /// </summary>
    public void IniPool(FruitsType type, Fruit fruitQueue)
    {
        if (!fruitsPoolObject.TryGetValue(type, out Queue<GameObject> fruitsQueue))
        {
            fruitsQueue = new Queue<GameObject>();
            fruitsPoolObject[type] = fruitsQueue;
        }
        if (type != FruitsType.Bomb && !slicedFruitsPoolObject.TryGetValue(type, out Queue<GameObject> slicedQueue))
        {
            slicedQueue = new Queue<GameObject>();
            slicedFruitsPoolObject[type] = slicedQueue;
        }
        GameObject spawnedFruit = Instantiate(fruitQueue.FruitPrefab);
        if (poolRoot != null) spawnedFruit.transform.SetParent(poolRoot);
        spawnedFruit.SetActive(false);

        FruitBehaviour fruitBehaviour = spawnedFruit.GetComponent<FruitBehaviour>();
        if (fruitBehaviour != null)
            fruitBehaviour.fruit = fruitQueue;

        fruitsQueue.Enqueue(spawnedFruit);
        GameObject spawnedSlicedFruit;
        if (type != FruitsType.Bomb)
        {
            spawnedSlicedFruit = Instantiate(fruitQueue.SlicedFruitPrefab);
            if (poolRoot != null) spawnedSlicedFruit.transform.SetParent(poolRoot);
            slicedFruitsPoolObject[type].Enqueue(spawnedSlicedFruit);
            spawnedSlicedFruit.SetActive(false);
        }
    }
    /// <summary>
    /// Lấy object trái cây từ pool, tạo thêm nếu pool rỗng.
    /// </summary>
    public GameObject GetObjectFruit(Fruit fruit)
    {
        if (!GameManager.Instance.isGameStarted) return null;
        if (!fruitsPoolObject.TryGetValue(fruit.Type, out Queue<GameObject> queue) || queue.Count == 0)
        {
            IniPool(fruit.Type, fruit);
            queue = fruitsPoolObject[fruit.Type];
        }
        GameObject obj = queue.Dequeue();
        obj.SetActive(true);
        return obj ;
    }
    /// <summary>
    /// Lấy object trái cây đã cắt từ pool, tạo thêm khi cần.
    /// </summary>
    public GameObject GetObjectSlicedFruit(Fruit slice)
    {
        if (!GameManager.Instance.isGameStarted) return null;
        if (!slicedFruitsPoolObject.TryGetValue(slice.Type, out Queue<GameObject> queue) || queue.Count == 0)
        {
            IniPool(slice.Type, slice);
            queue = slicedFruitsPoolObject[slice.Type];
        }
        GameObject obj = queue.Dequeue();
        //obj.SetActive(true);
        return obj;
    }
    /// <summary>
    /// Trả object trái cây về pool và tắt object.
    /// </summary>
    public void ReturnObjectFruit(Fruit fruit, GameObject obj)
    {
        obj.SetActive(false);
        fruitsPoolObject[fruit.Type].Enqueue(obj);
    }
    /// <summary>
    /// Trả object trái cây đã cắt về pool và tắt object.
    /// </summary>
    public void ReturnObjectSlicedFruit(Fruit slice, GameObject obj)
    {
        obj.SetActive(false);
        slicedFruitsPoolObject[slice.Type].Enqueue(obj);
    }
}
