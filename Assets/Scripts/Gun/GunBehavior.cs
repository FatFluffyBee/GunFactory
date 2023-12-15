using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunBehavior : MonoBehaviour
{
    public FactoryGrid factoryGrid;
    public GameObject bulletPrefab;
    public Transform launchPoint;
    public float projectileSpeed;

    float timer = 1;
    float timerCount;

    public void Update()
    {
        timerCount += Time.deltaTime;

        if(timerCount > timer)
        {
            Fire();
            timerCount = 0;
        }
    }
    public void Fire()
    {
        GameObject instance = Instantiate(bulletPrefab, launchPoint.position, transform.rotation);
        instance.GetComponent<ProjectileBehavior>().Setup(projectileSpeed);
    }
}
