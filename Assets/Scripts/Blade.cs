using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blade : MonoBehaviour
{
    public GameObject bladeTrailPrefab;
    GameObject currentBladeTrail;

    bool isCutting = false;

    Rigidbody rb;
    Camera cam;
    SphereCollider sphere;

    Vector3 previousPosition;

    public float minCuttingVelocity = 0.1f;

    void Start()
    {
        cam = Camera.main;
        rb = GetComponent<Rigidbody>();
        sphere = GetComponent<SphereCollider>();
    }

    void Update()
    {
        // Chỉ cho phép cắt khi game đã bắt đầu
        if (!GameManager.Instance.isGameStarted) return;

        if (Input.GetMouseButtonDown(0))
        {
            StartCutting();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            StopCutting();
        }

        if (isCutting)
        {
            UpdateCut();
        }
    }

    void UpdateCut()
    {
        Vector3 newPosition = cam.ScreenToWorldPoint(
            new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f)
        );

        rb.position = newPosition;

        float velocity = (newPosition - previousPosition).magnitude / Time.deltaTime;

        sphere.enabled = velocity > minCuttingVelocity;

        previousPosition = newPosition;
    }

    void StartCutting()
    {
        isCutting = true;

        currentBladeTrail = Instantiate(bladeTrailPrefab, transform);

        previousPosition = cam.ScreenToWorldPoint(
            new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f)
        );

        sphere.enabled = false;
    }

    void StopCutting()
    {
        isCutting = false;

        currentBladeTrail.transform.SetParent(null);
        Destroy(currentBladeTrail, 2f);

        sphere.enabled = false;
    }
}