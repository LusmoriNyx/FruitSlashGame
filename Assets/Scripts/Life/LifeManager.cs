using System.Collections.Generic;
using UnityEngine;

public class LifeManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> lives = new List<GameObject>();
    [SerializeField] private int lifeCount = 3;

    public void UpdateLives()
    { 
        lives[lifeCount - 1].gameObject.SetActive(false);
        lifeCount--;
    }
}
