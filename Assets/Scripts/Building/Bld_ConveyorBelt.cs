using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Bld_ConveyorBelt : PlacedObject
{
    public enum Bld_State {Empty, Moving, ReadyToSend}
    public Bld_State bldState = Bld_State.Empty;
    public WorldItem worldItem;
    public Transform holdPoint;
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
    public void SetWorldItemAndBeltStatus(WorldItem worldItem)
    {
        this.worldItem = worldItem;

        if(worldItem == null)
            bldState = Bld_State.Empty;
        else
            bldState = Bld_State.Moving;
    }

    public Vector3 GetHoldPointPosition()
    {
        return holdPoint.position;
    }

    public bool CanSendItem()
    {
        if(bldState == Bld_State.ReadyToSend)
            return true;
        else
            return false;
    }

    public bool CanReceiveItem()
    {
        if (bldState == Bld_State.Empty)
            return true;
        else
            return false;
    }

    public void SetStateToReadyToSend()
    {
        bldState = Bld_State.ReadyToSend;
    }

    public override void ReceiveItem(WorldItem item)
    {
        SetWorldItemAndBeltStatus(item);
        item.SetTargetPositionAndCurrentBelt(GetHoldPointPosition(), this);
    }
}
