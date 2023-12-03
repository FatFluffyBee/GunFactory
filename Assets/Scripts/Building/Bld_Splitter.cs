using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bld_Splitter : PlacedObject
{
    WorldItem worldItem;
    int rotatingIndex = 0;

    public enum Bld_State { Ready, Sending, }
    Bld_State mergerState;

    List<Bld_ConveyorBelt> outputBelts = new List<Bld_ConveyorBelt>();
    Bld_ConveyorBelt inputBelt;
    public Transform holdPoint;
    public int nTickToReachEnd = 16;

    private void Start()
    {
        TickManager.instance.OnTick.AddListener(SplitterOnTick);
    }

    public void SplitterOnTick() 
    {
        Debug.Log("Splitter On Tick");
        if (mergerState == Bld_State.Sending)
        {
            if (timerCount > timer)
            {
                SendItemOnConveyor();
                RotateIndex();
            }
            timerCount += Time.deltaTime;
        }          
    }

    private void SendItemOnConveyor()
    {
        if (outputBelts[rotatingIndex] == null)
            ActualiseOutputBelts();

        if (outputBelts[rotatingIndex] != null) //maybe remplacer par fonction plus génerale
        {
            if (outputBelts[rotatingIndex].GetHoldItem() == null)
            {
                outputBelts[rotatingIndex].SetHoldItem(worldItem);
                worldItem.SetTargetPosition(outputBelts[rotatingIndex].GetHoldPointPosition());
                worldItem = null;
                mergerState = Bld_State.Ready;
            }
        }
    }

    public override bool CheckIfCanSendItem(Bld_ConveyorBelt belt) //called by conveyor interacting with it
    {
       if (mergerState == Bld_State.Ready)
        {
            mergerState = Bld_State.Sending;
            timerCount = 0;
            //feedback cause item will be sent (might need it's own function later)
            return true;
        }

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


    public override void Setup()
    {
        base.Setup();

        outputBelts.Add(null);
        outputBelts.Add(null);
        outputBelts.Add(null);

        timer = 2f;
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

    public void SetWorldItem(WorldItem worldItem)
    {
        this.worldItem = worldItem;
    }

    public Vector3 GetHoldPointPosition()
    {
        return holdPoint.position;
    }




}
