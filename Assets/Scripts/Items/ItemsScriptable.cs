using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Items", menuName = "ScriptableObjects/Items")]
public class ItemsScriptable : ScriptableObject
{
    public ItemScriptable[] Items;
}
