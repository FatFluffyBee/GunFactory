using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlacedObject : MonoBehaviour
{
    protected PlacableObjectSO.Dir dir;
    protected Vector2Int origin;
    protected FactoryGridObject gridObject;
    [SerializeField] List<GameObject> inputAccessesGO = new List<GameObject>();
    [SerializeField] List<GameObject> outputAccessesGO = new List<GameObject>();

    protected List<Vector2Int> inputAccessesPos = new List<Vector2Int>();
    protected List<Vector2Int> outputAccessesPos = new List<Vector2Int>();


    //maybe to move in a subClass
    [System.NonSerialized] protected RecipeSO currentRecipe;
    protected float timer;
    protected float timerCount;
    protected List<int> ingredientsItemsQty;
    protected List<int> producedItemsQty;

    private void Start()
    {
        Setup();
    }

    public void SetOriginAndDirection(Vector2Int origin, PlacableObjectSO.Dir dir, FactoryGridObject gridObject) //called from grid factory on initialization
    {
        this.dir = dir;
        this.origin = origin;
        this.gridObject = gridObject;
    }

    public void InitializeAccessesPos()
    {
        foreach(GameObject e in inputAccessesGO)
        {
            inputAccessesPos.Add(FactoryGrid.instance.GetXZ(e.transform.position));
            e.SetActive(false);
        }

        foreach (GameObject e in outputAccessesGO)
        {
            outputAccessesPos.Add(FactoryGrid.instance.GetXZ(e.transform.position));
            e.SetActive(false);
        }
    }

    virtual public void Setup()
    {
        InitializeAccessesPos();
    }

    virtual public void OnSupression()
    {

    }

    public Vector2Int GetOrigin()
    {
        return origin;
    }

    public PlacableObjectSO.Dir GetDir()
    {
        return dir;
    }

    public Vector2Int GetOriginAndDirForward()
    {
        return origin + PlacableObjectSO.GetDirForwardVector(dir);
    }

    public List<Vector2Int> GetOriginAndDirSideAndBack()
    {
        List<PlacableObjectSO.Dir> dirTab = new List<PlacableObjectSO.Dir>() { PlacableObjectSO.Dir.Up, PlacableObjectSO.Dir.Down, PlacableObjectSO.Dir.Left, PlacableObjectSO.Dir.Right };
        List<Vector2Int> listPos = new List<Vector2Int>();

        dirTab.Remove(dir);

        foreach (PlacableObjectSO.Dir e in dirTab)
        {
            listPos.Add(origin + PlacableObjectSO.GetDirForwardVector(e));
        }

        return listPos;
    }

    public Bld_ConveyorBelt GetConveyorBeltAdjacent() //to redo tooo
    {
        Bld_ConveyorBelt belt = FactoryGrid.instance.GetGridObject(outputAccessesPos[0]).GetPlacedObject()?.GetComponent<Bld_ConveyorBelt>();

        if (belt != null)
            if (belt.IsPosOnInputPos(FactoryGrid.instance.GetGridObject(origin).GetBuildingPos()))
                return belt;

        return null;
    }

    // compare la position avec la position des inputs de l'objet (fct géneral = objet check output pos for placed object, and check on this object for input pos. if true thn cionnection)
    public bool IsPosOnInputPos(List<Vector2Int> buiildingPoses) //pas besoin de prendre en compte la direction car l'objet preneur doit déja faire face à l'objet pour le récuperer
    {
        if (inputAccessesPos.Count == 0)
        {
            Debug.Log("Building has no inputs");
            return false;
        }
           
        foreach(Vector2Int buiildingPos in buiildingPoses)
            foreach(Vector2Int inputPos in inputAccessesPos)
            {
                //Debug.Log(inputPos + " " + buiildingPos);
                if (inputPos == buiildingPos)
                {
                    return true;
                }
            }

        Debug.Log("Not in the right position");
        return false;
    }

    public bool IsPosOnOutputPos(List<Vector2Int> buiildingPoses) //pas besoin de prendre en compte la direction car l'objet preneur doit déja faire face à l'objet pour le récuperer
    {
        Debug.Log("Checking");
        if (outputAccessesPos.Count == 0)
        {
            Debug.Log("Building has no output");
            return false;
        }

        foreach (Vector2Int buiildingPos in buiildingPoses)
            foreach (Vector2Int outputPos in outputAccessesPos)
            {
                //Debug.Log(outputPos + " " + buiildingPos);
                if (outputPos == buiildingPos)
                {
                    Debug.Log(this.gameObject.name);
                    return true;
                }
            }

        Debug.Log("Not in the right position");
        return false;
    }

    public bool IsPosOnOutputPos(Vector2Int placedPos)
    {
        List<Vector2Int> buildingPos = new List<Vector2Int> { placedPos };
        return IsPosOnOutputPos(buildingPos);
    }

    public bool IsPosOnInputPos(Vector2Int placedPos) //pas besoin de prendre en compte la direction car l'objet preneur doit déja faire face à l'objet pour le récuperer
    {
        List<Vector2Int> buildingPos = new List<Vector2Int> { placedPos };
        return IsPosOnInputPos(buildingPos);
    }

    public virtual bool CanReceiveSpecificItemFromBelt(Bld_ConveyorBelt belt)
    {
        ItemSO itemSO = belt.GetHoldItem().itemSO;
        foreach (RecipeSO.RecipeItem e in currentRecipe.ingredientsItems)
        {
            if (e.item == itemSO)
            {
                return true;
            }

        }
        return false;
    }

    public void AddItemToStorage(ItemSO itemSO)
    {
        for (int i = 0; i < currentRecipe.ingredientsItems.Count; i++)
        {
            if (currentRecipe.ingredientsItems[i].item == itemSO)
            {
                ingredientsItemsQty[i]++;
            }
        }
    }

    public void RecipeToString()
    {
        string debug;
        debug = currentRecipe.producedItems[0].amount + " " + currentRecipe.producedItems[0].item.itemName + " = ";

        foreach (RecipeSO.RecipeItem e in currentRecipe.ingredientsItems)
        {
            debug += e.amount + " " + e.item.itemName + " + ";
        }

        Debug.Log(debug);
    }

    public void ChangeRecipe(RecipeSO newRecipe)
    {
        //Debug.Log(newRecipe);

        currentRecipe = newRecipe;
        ingredientsItemsQty = new List<int>();
        producedItemsQty = new List<int>();

        for (int i = 0; i < newRecipe.ingredientsItems.Count; i++)
        {
            ingredientsItemsQty.Add(0);
        }

        for (int i = 0; i < newRecipe.producedItems.Count; i++)
        {
            producedItemsQty.Add(0);
        }

        timer = newRecipe.timeToComplete;

        GetComponent<Bld_Producer>()?.CancelItemProduction();
    }

    public bool ChechCanCraftRecipe()
    {
        if (currentRecipe.ingredientsItems.Count == 0)
            return true;

        for (int i = 0; i < currentRecipe.ingredientsItems.Count; i++)
        {
            if (currentRecipe.ingredientsItems[i].amount > ingredientsItemsQty[i])
                return false;
        }

        return true;
    }

    public void RemoveItemIngredients()
    {
        for (int i = 0; i < currentRecipe.ingredientsItems.Count; i++)
        {
            ingredientsItemsQty[i] -= currentRecipe.ingredientsItems[i].amount;
        }
    }

    public RecipeSO GetCurrentRecipe()
    {
        return currentRecipe;
    }

    public List<int> GetIngredientsQty()
    {
        return ingredientsItemsQty;
    }

    public List<int> GetProducedQty()
    {
        return producedItemsQty;
    }

    public float GetTimeRatio()
    {
        return timerCount / timer;
    }

    public float GetTimeRemaining()
    {
        return Mathf.Floor((timer - timerCount) * 10) / 10;
    }

    public virtual void ReceiveItem(WorldItem item) //placed object receive a world item
    {
        Debug.Log("WARNING : RECEIVEITEM NOT SETUP IN :" + transform.name);
    }
}
