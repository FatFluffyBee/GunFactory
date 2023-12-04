using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bld_Splitter : PlacedObject
{
    private WorldItem worldItem;
    public int rotatingIndex = 0;

    public enum Bld_State { Empty, Moving, ReadyToSend}
    private Bld_State bldState;

    private List<Bld_ConveyorBelt> outputBelts = new List<Bld_ConveyorBelt>();
    private Bld_ConveyorBelt inputBelt;
    public Transform holdPoint;

    private void Update()
    {
        if (CanSendItem())
        {
            SendItemOnConveyor();
            RotateIndex();
        }          
    }

    private void SendItemOnConveyor()
    {
        ActualiseOutputBelts();

        if (outputBelts[rotatingIndex] == null)
            RotateIndex();

        if (outputBelts[rotatingIndex] == null)
            RotateIndex();
        
        if(outputBelts[rotatingIndex] == null)
            return;
        else
        {
            if (outputBelts[rotatingIndex].CanReceiveItem())
            {
                outputBelts[rotatingIndex].ReceiveItem(worldItem);
                worldItem = null;
                bldState = Bld_State.Empty;
            }
        }
    }

    public override bool CanReceiveSpecificItemFromBelt(Bld_ConveyorBelt belt) //called by conveyor interacting with it
    {
        if (bldState == Bld_State.Empty)
            return true;
        else
            return false;
    }

    public void ActualiseOutputBelts()
    {
        for (int i = 0; i < outputBelts.Count; i++)
        {
            if (outputBelts[i] == null)
            {
                Bld_ConveyorBelt tmp = FactoryGrid.instance.GetGridObject(outputAccessesPos[i]).GetPlacedObject()?.GetComponent<Bld_ConveyorBelt>();
                if (tmp != null)
                    if (tmp.IsPosOnInputPos(GetOrigin()))
                    {
                        outputBelts[i] = tmp;
                        Debug.Log(outputBelts[i]);
                    }
            }
        }
        
    }

    public void ActualiseInputBelt()
    {
        if (inputBelt == null)
        {
            Bld_ConveyorBelt tmp = FactoryGrid.instance.GetGridObject(outputAccessesPos[0]).GetPlacedObject()?.GetComponent<Bld_ConveyorBelt>();

            if (tmp != null)
                if (tmp.IsPosOnOutputPos(GetOrigin()))
                {
                    inputBelt = tmp;
                }
        }
    }

    public void RotateIndex()
    {
        switch (rotatingIndex)
        {
            case 0: rotatingIndex = 1; break;
            case 1: rotatingIndex = 2; break;
            case 2: rotatingIndex = 0; break;
        }
    }

    public bool CanSendItem()
    {
        if (bldState == Bld_State.ReadyToSend)
            return true;
        else
            return false;
    }

    public override void Setup()
    {
        base.Setup();

        outputBelts.Add(null);
        outputBelts.Add(null);
        outputBelts.Add(null);

        Debug.Log("On Setup Splitter");
    }

    public override void OnSupression()
    {
        Destroy(worldItem.gameObject);

        Debug.Log("On Suppresion Splitter");
    }

    public WorldItem GetHoldItem()
    {
        return worldItem;
    }

    public void SetWorldItemAndBeltStatus(WorldItem worldItem)
    {
        this.worldItem = worldItem;

        if (worldItem == null)
            bldState = Bld_State.Empty;
        else
            bldState = Bld_State.Moving;
    }

    public Vector3 GetHoldPointPosition()
    {
        return holdPoint.position;
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
