using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Building", menuName = "ScriptableObjects/CreateNewItem", order = 2)]

public class ItemSO : ScriptableObject
{
    public int ItemID;
    public string itemName;
    public GameObject worldItemPrefab;
    public Sprite itemSprite;
}
