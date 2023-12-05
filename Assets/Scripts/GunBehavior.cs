using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunBehavior : MonoBehaviour
{
    public FactoryGrid factoryGrid;
    public GameObject bulletPrefab;
    public Transform launchPoint;
    public float projectileSpeed;

    public void Fire()
    {
        Debug.Log("Firing");
        GameObject instance = Instantiate(bulletPrefab, launchPoint.position, transform.rotation);
        instance.GetComponent<ProjectileBehavior>().Setup(projectileSpeed);
    }
}
