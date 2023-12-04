using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bld_Merger : PlacedObject
{
    private WorldItem worldItem;
    public int rotatingIndex = 0;

    public enum Bld_State { Empty, Moving, ReadyToSend }
    public  Bld_State bldState;

    private List<Bld_ConveyorBelt> inputBelts = new List<Bld_ConveyorBelt>();
    private Bld_ConveyorBelt outputBelt;
    public Transform holdPoint;

    private void Update() //merger ne fonctionne pas de la fa�on voulue, probl�me sera r�solu avec le syst�me de tick
    {
        if (CanSendItem())
        {
            SendItemOnConveyor();
        }

        if(bldState == Bld_State.Empty) 
        {
            RotateIndex();
        }
    }

    public bool CanSendItem()
    {
        if (bldState == Bld_State.ReadyToSend)
            return true;
        else
            return false;
    }

    private void SendItemOnConveyor()
    {
        if (outputBelt == null)
            ActualiseOutputBelt();

        if (outputBelt != null)
            if (outputBelt.CanReceiveItem())
            {
                outputBelt.ReceiveItem(worldItem);
                worldItem = null;
                bldState = Bld_State.Empty;
            }
    }

    public override bool CanReceiveSpecificItemFromBelt(Bld_ConveyorBelt belt) //called by conveyor interacting with it
    {
        if (bldState == Bld_State.Empty)
        {
            ActualiseInputBelts();

            if (belt == inputBelts[rotatingIndex])
            {
                return true;
            }
        }
        return false;
    }

    public void ActualiseInputBelts()
    {
        for(int i = 0; i < inputBelts.Count; i++)
        {
            if (inputBelts[i] == null)
            {
                Bld_ConveyorBelt tmp = FactoryGrid.instance.GetGridObject(inputAccessesPos[i]).GetPlacedObject()?.GetComponent<Bld_ConveyorBelt>();
                if(tmp != null)
                    if(tmp.IsPosOnOutputPos(GetOrigin()))
                    {
                        inputBelts[i] = tmp;
                    }
            }
        }
    }

    public void ActualiseOutputBelt()
    {
        if(outputBelt == null)
        {
            Bld_ConveyorBelt tmp = FactoryGrid.instance.GetGridObject(outputAccessesPos[0]).GetPlacedObject()?.GetComponent<Bld_ConveyorBelt>();

            if(tmp != null)
                if(tmp.IsPosOnInputPos(GetOrigin()))
                {
                    outputBelt = tmp;
                }
        }
    }

    public void RotateIndex()
    {
        switch(rotatingIndex)
        {
            case 0: rotatingIndex = 1; break;
            case 1: rotatingIndex = 2; break;
            case 2: rotatingIndex = 0; break;
        }
    }

    public override void Setup()
    {
        base.Setup();

        inputBelts.Add(null);
        inputBelts.Add(null);
        inputBelts.Add(null);

         Debug.Log("On Merger Setup");
    }

    public override void OnSupression()
    {
        Destroy(worldItem.gameObject);

        Debug.Log("On Suppresion Merger");
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
