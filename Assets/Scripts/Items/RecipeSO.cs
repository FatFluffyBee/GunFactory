using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Building", menuName = "ScriptableObjects/CreateNewRecipe", order = 3)]
public class RecipeSO : ScriptableObject
{
    public List<RecipeItem> ingredientsItems;
    public List<RecipeItem> producedItems;
    public int timeToComplete;

    [System.Serializable]
    public struct RecipeItem
    {
        public ItemSO item;
        public int amount;
    }
}
