using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehavior : MonoBehaviour
{
    public Transform target;

    public float speed;

    void Start()
    {
        if (target == null)
            target = GameObject.Find("Player").transform;
    }

    void Update()
    {
        Vector2 newpos = Vector2.MoveTowards(transform.position, target.position, Time.deltaTime * speed);
        transform.position = newpos;
    }
}
