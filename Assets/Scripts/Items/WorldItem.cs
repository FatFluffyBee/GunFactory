using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldItem : MonoBehaviour
{
    private bool isMoving = false;
    Vector3 targetPosition;
    public ItemSO itemSO;
    private int nTick = 16;

    private void Start()
    {
        TickManager.instance.OnTick.AddListener(OnTickMoveWorldItem);
    }

    private void OnTickMoveWorldItem()
    {
        if(isMoving)
        {
            Vector3 newPosition = Vector3.MoveTowards(transform.position, targetPosition, 10f / (float)nTick);
            transform.position = newPosition;
        }

        if(targetPosition == transform.position)
        {
            isMoving = false;
        }
    }

    public void SetTargetPosition(Vector3 targetPosition)
    {
        this.targetPosition = targetPosition;
        isMoving = true;
    }
}
