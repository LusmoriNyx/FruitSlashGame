using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FruitBehaviour : MonoBehaviour
{
    public Fruit fruit;
    private bool isSliced;
    Rigidbody2D rb;
    public float startForce;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.AddForce(transform.up * startForce, ForceMode2D.Impulse);
    }
    private void OnEnable()
    {
        isSliced = false;
        Invoke("FruitsDroped", 5f);
    }
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Blade")
        {
            CancelInvoke("FruitsDroped");
            isSliced = true;
            Vector3 direction = (col.transform.position - transform.position).normalized;
            Quaternion rotation = Quaternion.LookRotation(direction);
            GameObject slicedFruit = null;
            if (fruit != null && fruit.Type == FruitsType.Bomb)
            {
                //Nổ tung, -1 life
                ObjectPoolManager.Instance.ReturnObjectFruit(fruit, gameObject);
            }
            else
            {
                slicedFruit = ObjectPoolManager.Instance.GetObjectSlicedFruit(fruit);
                slicedFruit.GetComponent<FruitSliceBehaviour>().sliceFruit = fruit;
                ObjectPoolManager.Instance.ReturnObjectFruit(fruit, gameObject);
            }
            if (slicedFruit != null)
            {
                slicedFruit.transform.position = transform.position;
            }
        }
    }
    private void FruitsDroped()
    {
        if(isSliced == false)
        {
            ObjectPoolManager.Instance.ReturnObjectFruit(fruit, gameObject);
        }
    }
}
