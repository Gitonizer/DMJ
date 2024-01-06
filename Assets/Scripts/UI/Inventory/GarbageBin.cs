using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GarbageBin : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData data)
    {
        GameObject dropped = data.pointerDrag;
        UIItem uiItem = dropped.GetComponent<UIItem>();

        switch (uiItem.ItemType)
        {
            case ItemType.Key:
            case ItemType.Collectible: // maybe remove this later
                print("can't discard key or collectible items");
                break;
            case ItemType.Health:
            case ItemType.Mana:
                uiItem.DestroyWorldItem();
                Destroy(uiItem.gameObject);
                EventManager.OnClose?.Invoke();
                break;
            default:
                break;
        }
    }
}
