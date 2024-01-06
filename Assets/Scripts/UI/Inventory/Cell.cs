using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Cell : MonoBehaviour, IDropHandler
{
    public CellType CellType { get { return _cellType; } set { _cellType = value; } }
    public bool HasItem { get { return _hasItem; } set { _hasItem = value; } }
    public bool IsBox { get { return _isBox; } set { _isBox = value; } }
    public UIItem UIItem { get { return _uiItem; } }

    [SerializeField] private bool _hasItem = false;
    [SerializeField] private bool _isBox = true;
    [SerializeField] private CellType _cellType;

    private UIItem _uiItem;

    public void UnassignItem()
    {
        _uiItem = null;
    }
    public void OnDrop(PointerEventData data)
    {
        if (!_hasItem && IsBox)
        {
            GameObject dropped = data.pointerDrag;
            _uiItem = dropped.GetComponent<UIItem>();
            _uiItem.Image.raycastTarget = true;
            _uiItem.SetNewParent(transform);
            _uiItem.transform.SetAsLastSibling();
            _uiItem.transform.localPosition = Vector3.zero;
            _hasItem = true;

            if (CellType == CellType.Player)
            {
                _uiItem.DestroyWorldItem();
            }
        }
    }
}
