using UnityEngine;

public class FruitSliceBehaviour : MonoBehaviour
{
    public Fruit sliceFruit;

    private void OnEnable()
    {
        Debug.Log(sliceFruit);
        if(sliceFruit != null )
        {
            Invoke("ReturnSlicedFruit", 3f);
        }
    }
    private void ReturnSlicedFruit()
    {
        ObjectPoolManager.Instance.ReturnObjectSlicedFruit(sliceFruit, gameObject);
    }
}
