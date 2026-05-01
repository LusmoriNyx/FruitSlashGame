using System.Collections;
using UnityEngine;

public class FruitBehaviour : MonoBehaviour
{
    public Fruit fruit;

    private bool isSliced;

    Rigidbody rb;
    public float startForce;
    [SerializeField] private GameObject explode_VFX;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // Nếu spawnPoint quay lên trên, thì lực sẽ được áp dụng theo hướng lên trên
     
        rb.AddForce(transform.up * startForce, ForceMode.Impulse);


        isSliced = false;

        Invoke(nameof(FruitsDropped), 5f);
    }

    public void LaunchWithDirection(Vector3 direction, float forceMultiplier = 1f)
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        Vector3 launchDirection = direction.sqrMagnitude > 0f ? direction.normalized : Vector3.up;
        rb.AddForce(launchDirection * startForce * forceMultiplier, ForceMode.Impulse);
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Blade"))
        {
            CancelInvoke(nameof(FruitsDropped));
            isSliced = true;

            GameObject slicedFruit = null;

            if (fruit != null && fruit.Type == FruitsType.Bomb)
            {
                AudioManager.Instance.ExplosionPlaySound();
                ObjectPoolManager.Instance.ReturnObjectFruit(fruit, gameObject);
                LifeManager lifeManager = FindAnyObjectByType<LifeManager>();
                if (lifeManager != null)
                {
                    lifeManager.UpdateLives();
                }
                if (explode_VFX != null)
                {
                    GameObject vfx = Instantiate(explode_VFX, transform.position, Quaternion.identity);
                    Destroy(vfx, 2f);
                }
            }
            else
            {
                if (LifeManager.Instance != null)
                    LifeManager.Instance.AddScore(fruit.PointValue);

                slicedFruit = ObjectPoolManager.Instance.GetObjectSlicedFruit(fruit);

                slicedFruit.transform.position = transform.position;
                slicedFruit.transform.rotation = Quaternion.identity;
                slicedFruit.SetActive(true);
                AudioManager.Instance.SlashPlaySound();

                StartCoroutine(ReturnSlicedAfterTime(slicedFruit, fruit, 2f));

                ObjectPoolManager.Instance.ReturnObjectFruit(fruit, gameObject);
            }
        }
    }

    private void FruitsDropped()
    {
        if (!isSliced && fruit != null)
        {
            ObjectPoolManager.Instance.ReturnObjectFruit(fruit, gameObject);
        }
    }

    IEnumerator ReturnSlicedAfterTime(GameObject obj, Fruit fruit, float time)
    {
        yield return new WaitForSeconds(time);

        if (obj.activeInHierarchy)
        {
            ObjectPoolManager.Instance.ReturnObjectSlicedFruit(fruit, obj);
        }
    }
   
}