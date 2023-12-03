using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Bld_ConveyorBelt : PlacedObject
{
    public WorldItem worldItem;
    public Transform holdPoint;
    public int nTickToReachEnd = 16;
    public override void Setup()
    {
        base.Setup();

        bool setupValid = ConveyorSystem.instance.AddConveyorToBeltPath(this);
        if(!setupValid)
        {
            Destroy(this.gameObject);
        }
        Debug.Log("Setup Conveyor Belt");
    }

    public override void OnSupression()
    {
        ConveyorSystem.instance.RemoveConveyorFromBelthPath(this);
        Destroy(worldItem.gameObject);

        Debug.Log("On Suppresion Conveyor Belt");
    }

    public WorldItem GetHoldItem()
    {
        return worldItem;
    }
    public void SetHoldItem(WorldItem worldItem)
    {
        this.worldItem = worldItem;
    }

    public Vector3 GetHoldPointPosition()
    {
        return holdPoint.position;
    }
}
