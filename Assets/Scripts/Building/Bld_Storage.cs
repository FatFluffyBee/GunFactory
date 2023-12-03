using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlacedObject;

public class Bld_Storage : PlacedObject
{
    private Bld_ConveyorBelt conveyorBelt;
    RecipeSO.RecipeItem recipeItem;

    public override void Setup()
    {

        Debug.Log("Setup Storage Building");
    }

    private void Update() // Refaire en plus géneral et concis (work with other things)
    {
        /*if (conveyorBelt == null)
            conveyorBelt = GetConveyorBeltAdjacent();

        if (conveyorBelt != null && recipeItem[0].GetQuantity() > 0) //maybe remplacer par fonction plus génerale // passe un item à son voisin
        {
            if (conveyorBelt.GetHoldItem() == null)
            {
                itemStored[0].AddQuantity(-1);
                WorldItem itemInstance = Instantiate(itemStored[0].GetItemSO().worldItemPrefab, conveyorBelt.GetHoldPointPosition(), transform.rotation).GetComponent<WorldItem>();
                conveyorBelt.SetHoldItem(itemInstance);

            }
        }*/
    }
}
