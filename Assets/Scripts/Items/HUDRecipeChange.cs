using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDRecipeChange : MonoBehaviour
{
    public int index;
    RecipeSO recipe;
    Image image;

    private void Start()
    {
        if(index < TESTSCRIPT.instance.recipes.Count)
        {
            image = GetComponent<Image>();

            recipe = TESTSCRIPT.instance.recipes[index];
            image.sprite = recipe.producedItems[0].item.itemSprite;
            transform.GetComponent<Button>().onClick.AddListener(ChangeRecipeToSelected);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
    public void ChangeRecipeToSelected()
    {
        PlacedObject placedObject = FactoryGrid.instance.GetSelectedObject();

        placedObject.ChangeRecipe(recipe);
    }
    
}
