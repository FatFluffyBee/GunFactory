using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bld_Merger : PlacedObject
{
    WorldItem worldItem;
    int rotatingIndex = 0;

    public enum Bld_State { Ready, Sending, }
    Bld_State mergerState;

    List<Bld_ConveyorBelt> inputBelts = new List<Bld_ConveyorBelt>();
    Bld_ConveyorBelt outputBelt;
    public Transform holdPoint;
    public int nTickToReachEnd = 16;

    private void Start()
    {
        TickManager.instance.OnTick.AddListener(MergerOnTick);
    }

    public void MergerOnTick() //merger ne fonctionne pas de la façon voulue, problème sera résolu avec le système de tick
    {
        Debug.Log("Merger On Tick");
        if (mergerState == Bld_State.Sending)
        {
            if(timerCount > timer)
            {
                SendItemOnConveyor();
            }

            timerCount += Time.deltaTime;
        }

        if (mergerState == Bld_State.Ready)
            RotateIndex();
    }

    private void SendItemOnConveyor()
    {
        if (outputBelt == null)
            ActualiseOutputBelt();

        if (outputBelt != null) //maybe remplacer par fonction plus génerale
        {
            if (outputBelt.GetHoldItem() == null)
            {
                outputBelt.SetHoldItem(worldItem);
                worldItem.SetTargetPosition(outputBelt.GetHoldPointPosition());
                worldItem = null;
                mergerState = Bld_State.Ready;

            }
        }
    }

    public override bool CheckIfCanSendItem(Bld_ConveyorBelt belt) //called by conveyor interacting with it
    {
        if (mergerState == Bld_State.Ready)
        {
            ActualiseInputBelts();

            if (belt == inputBelts[rotatingIndex])
            {
                mergerState = Bld_State.Sending;
                timerCount = 0;
                //feedback cause item will be sent (might need it's own function later)
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

        timer = 2f;
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

    public void SetWorldItem(WorldItem worldItem)
    {
        this.worldItem = worldItem;
    }

    public Vector3 GetHoldPointPosition()
    {
        return holdPoint.position;
    }
}
