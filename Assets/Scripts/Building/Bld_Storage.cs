using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlacedObject;

public class Bld_Storage : PlacedObject
{
    private Bld_ConveyorBelt conveyorBelt;
    [SerializeField] private List<ItemSO> storedItem = new List<ItemSO>();
    [SerializeField] private List<int> itemQuantity = new List<int>();
    private int maxStack = 2;

    public override void Setup()
    {
        base.Setup();

        Debug.Log("Setup Storage Building");
    }

    private void Update() // Refaire en plus géneral et concis (work with other things)
    {
        if (conveyorBelt == null)
            conveyorBelt = GetConveyorBeltAdjacent();

        if (conveyorBelt != null)
            if(conveyorBelt.CanReceiveItem() && storedItem.Count > 0)
            {
                if (conveyorBelt.GetHoldItem() == null)
                {
                    itemQuantity[0]--;
                    WorldItem itemInstance = Instantiate(storedItem[0].worldItemPrefab, conveyorBelt.GetHoldPointPosition(), transform.rotation).GetComponent<WorldItem>();
                    conveyorBelt.ReceiveItem(itemInstance);
                }
            }
    }

    public override void ReceiveItem(WorldItem item)
    {
        AddItemToStorage(item);
        Destroy(item.gameObject);
    }

    private void AddItemToStorage(WorldItem item)
    {
        Debug.Log(storedItem.Count);
        if (storedItem.Count == 0)
        {
            storedItem.Add(item.itemSO);
            itemQuantity.Add(1);
        }
        else
        {
            for (int i = 0; i < storedItem.Count; i++)
            {
                if (item.itemSO == storedItem[i])
                {
                    itemQuantity[i]++;
                    return;
                }
            }

            if (storedItem.Count < maxStack) //innnaceesible si l'item etait dans le tableau car return 
            {
                storedItem.Add(item.itemSO);
                itemQuantity.Add(1);
            }
        }
    }

    public override bool CanReceiveSpecificItemFromBelt(Bld_ConveyorBelt belt)
    {
        if (storedItem.Count < maxStack)
            return true;

        ItemSO itemToCheck = belt.GetHoldItem().itemSO;

        foreach (ItemSO itemSO in storedItem)
            if (itemSO == itemToCheck)
                return true;


        return false;
    }
}
