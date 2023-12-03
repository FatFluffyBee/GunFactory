using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TESTSCRIPT : MonoBehaviour
{
    public static TESTSCRIPT instance;
    public List<RecipeSO> recipes;
    public List<ItemSO> items;

    private void Awake()
    {
        instance = this; 
    }
}
