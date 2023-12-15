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
        uiItem.DestroyWorldItem();
        Destroy(uiItem.gameObject);
        EventManager.OnClose?.Invoke();
    }
}
