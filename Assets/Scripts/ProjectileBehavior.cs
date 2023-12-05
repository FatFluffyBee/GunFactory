using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBehavior : MonoBehaviour
{
    public int timeToDie;
    private float speed;

    private void Update()
    {
        transform.position += Vector3.forward * speed * Time.deltaTime;
    }

    public void Setup(float speed)
    {
        this.speed = speed;
        Destroy(gameObject, timeToDie);
    }

}
