using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "ScriptableObjects/Item")]
public class ItemScriptable : ScriptableObject
{
    public string Name;
    [TextArea] public string Description;
    public Items Item;
    public ItemType ItemType;
    public int Value;
    public Texture2D InventoryTexture;

    public ItemParent WorldItem;
}
