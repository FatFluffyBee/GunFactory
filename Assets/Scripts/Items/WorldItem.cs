using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldItem : MonoBehaviour
{
    public bool isMoving = false;
    Vector3 targetPosition;
    public ItemSO itemSO;
    private int speed = 10; 
    PlacedObject itemHolder;


    private void Update()
    {
        if(isMoving)
        {
            Vector3 newPosition = Vector3.MoveTowards(transform.position, targetPosition, 10f * Time.deltaTime * speed);
            transform.position = newPosition;
        }

        if(targetPosition == transform.position)
        {
            isMoving = false;

            if (itemHolder.GetComponent<Bld_ConveyorBelt>())
                itemHolder.GetComponent<Bld_ConveyorBelt>().SetStateToReadyToSend();
            else if (itemHolder.GetComponent<Bld_Merger>())
                itemHolder.GetComponent<Bld_Merger>().SetStateToReadyToSend();
            else if (itemHolder.GetComponent<Bld_Splitter>())
                itemHolder.GetComponent<Bld_Splitter>().SetStateToReadyToSend();
        }
    }

    public void SetTargetPositionAndCurrentBelt(Vector3 targetPosition, PlacedObject itemHolder)
    {
        this.itemHolder = itemHolder;
        this.targetPosition = targetPosition;
        isMoving = true;
    }
}
