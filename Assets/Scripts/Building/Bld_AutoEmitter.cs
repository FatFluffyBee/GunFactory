using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bld_AutoEmitter : PlacedObject
{
    public override void Setup()
    {
        base.Setup();

        Debug.Log("Setup AutoEmitter Building");
    }

    public void Fire(ItemSO item)
    {
        FactoryGrid.instance.GunBehavior.Fire();
    }

    public override void ReceiveItem(WorldItem item)
    {
        Fire(item.itemSO);
        Destroy(item.gameObject); 
    }

    public override bool CanReceiveSpecificItemFromBelt(Bld_ConveyorBelt belt)
    {
        return true;
    }
}
