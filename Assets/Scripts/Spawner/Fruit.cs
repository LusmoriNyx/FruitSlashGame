using UnityEngine;

[CreateAssetMenu(fileName = "Fruit", menuName = "Scriptable Objects/Fruit")]
public class Fruit : ScriptableObject
{
    [SerializeField] private GameObject fruit;
    public GameObject FruitPrefab => fruit;
    [SerializeField] private GameObject slicedFruit;
    public GameObject SlicedFruitPrefab => slicedFruit;
    [SerializeField] FruitsType type;
    public FruitsType Type => type;
    [SerializeField] private float spawnRate;
    public float SpawnRate => spawnRate;
}
    