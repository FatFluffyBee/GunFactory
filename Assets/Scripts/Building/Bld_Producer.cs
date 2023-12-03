using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Bld_Producer;

public class Bld_Producer : PlacedObject
{
    public RecipeSO recipeTest;
    private Bld_ConveyorBelt conveyorBelt;

    bool isProducing = false;

    public override void Setup()
    {
        base.Setup();

        ChangeRecipe(TESTSCRIPT.instance.recipes[0]);
        recipeTest = currentRecipe;
       
        Debug.Log("Setup Producer Building");
    }

    private void Update() // Refaire en plus géneral et concis (work with other things)
    {
        recipeTest = currentRecipe;

        if(isProducing) 
        {
            timerCount += Time.deltaTime;

            if (timerCount > timer)
            {
                ItemIsProduced();
            }
        }
        else
        {
            if(ChechCanCraftRecipe())
            {
                StartItemProduction();
            }
        }

        SendItemOnConveyor();
    }

    private void StartItemProduction()
    {
        RemoveItemIngredients();
        isProducing = true;
    }

    private void ItemIsProduced()
    {
        producedItemsQty[0] += currentRecipe.producedItems[0].amount;
        gridObject.SetValue(producedItemsQty[0]);
        isProducing = false;
        timerCount = 0;
    }

    public void CancelItemProduction()
    {
        isProducing = false;
        timerCount = 0;
    }

    private void SendItemOnConveyor()
    {
        if (conveyorBelt == null)
            conveyorBelt = GetConveyorBeltAdjacent();

        if (conveyorBelt != null && producedItemsQty[0] > 0) //maybe remplacer par fonction plus génerale
        {
            if (conveyorBelt.GetHoldItem() == null)
            {
                producedItemsQty[0]--;
                WorldItem itemInstance = Instantiate(currentRecipe.producedItems[0].item.worldItemPrefab, conveyorBelt.GetHoldPointPosition(), transform.rotation).GetComponent<WorldItem>();
                conveyorBelt.SetHoldItem(itemInstance);

            }
        }
    }
}
