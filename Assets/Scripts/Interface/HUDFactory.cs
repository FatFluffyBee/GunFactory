using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDFactory : MonoBehaviour
{
    PlacedObject selectedObject;

    public List<CaseNumbered> caseIngredients = new List<CaseNumbered>();
    public List<CaseNumbered> caseProductions = new List<CaseNumbered>();
    public HealthBar productionBar;
    public TextMeshProUGUI productionTimeRemaining;
    RecipeSO recipe;

    private void Update()
    {
        if (recipe != selectedObject.GetCurrentRecipe())
            SetHUDSelectedObject(FactoryGrid.instance.GetSelectedObject());

        for(int i = 0; i < selectedObject.GetIngredientsQty().Count; i++)
        {
            caseIngredients[i].UpdateCurrentValue(selectedObject.GetIngredientsQty()[i]);
        }

        for(int i = 0; i < selectedObject.GetProducedQty().Count; i++)
        {
            caseProductions[i].UpdateCurrentValue(selectedObject.GetProducedQty()[i]);
        }

        productionBar.SetHealthBar(selectedObject.GetTimeRatio());
        productionTimeRemaining.text = selectedObject.GetTimeRemaining().ToString();
    }

    public void SetHUDSelectedObject(PlacedObject newSelectedObject)
    {
        this.selectedObject = newSelectedObject;
        recipe = selectedObject.GetCurrentRecipe();

        foreach (CaseNumbered e in caseIngredients)
            e.Reset();

        foreach (CaseNumbered e in caseProductions)
            e.Reset();

        for (int i = 0; i < recipe.ingredientsItems.Count; i++)
        {
            caseIngredients[i].InitializeImageAndValues(recipe.ingredientsItems[i].item.itemSprite, recipe.ingredientsItems[i].amount, 0);
        }

        for (int i = 0; i < recipe.producedItems.Count; i++)
        {
            caseProductions[i].InitializeImageAndValues(recipe.producedItems[i].item.itemSprite, recipe.producedItems[i].amount, 0);
        }
    }
}
