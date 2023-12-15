using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Cell : MonoBehaviour, IDropHandler
{
    public CellType CellType { get { return _cellType; } set { _cellType = value; } }
    public bool HasItem { get { return _hasItem; } set { _hasItem = value; } }
    public bool IsBox { get { return _isBox; } set { _isBox = value; } }

    [SerializeField] private bool _hasItem = false;
    [SerializeField] private bool _isBox = true;
    [SerializeField] private CellType _cellType;
    public void OnDrop(PointerEventData data)
    {
        if (!_hasItem && IsBox)
        {
            GameObject dropped = data.pointerDrag;
            UIItem uiItem = dropped.GetComponent<UIItem>();
            uiItem.Image.raycastTarget = true;
            uiItem.SetNewParent(transform);
            uiItem.transform.SetAsLastSibling();
            uiItem.transform.localPosition = Vector3.zero;
            _hasItem = true;

            if (CellType == CellType.Player)
            {
                uiItem.DestroyWorldItem();
            }
        }
    }
}
