using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    private Canvas _inventoryCanvas;
    private bool _isUp;

    [SerializeField] private GameObject _playerInventory;
    [SerializeField] private GameObject _traderInventory;

    [SerializeField] private Cell[] _playerInventoryCells;
    [SerializeField] private Cell[] _traderInventoryCells;

    [SerializeField] private UIItem UIItemPrefab;

    public bool IsUp { get { return _isUp; } }

    private Animator _animator;

    private void Awake()
    {
        _inventoryCanvas = GetComponent<Canvas>();

        _playerInventoryCells = _playerInventory.GetComponentsInChildren<Cell>();
        _traderInventoryCells = _traderInventory.GetComponentsInChildren<Cell>();

        _animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        EventManager.OnTrade += OnTrade;
        EventManager.OnClose += OnClose;
    }

    private void OnDisable()
    {
        EventManager.OnTrade -= OnTrade;
        EventManager.OnClose -= OnClose;
    }

    public void OnInventory()
    {
        if (_inventoryCanvas.enabled)
        {
            _animator.Play("Close");
            EnableMouse(false);
        }
        else
        {
            _animator.Play("Open");
            EnableMouse(true);
        }

        _playerInventory.SetActive(true);
        _traderInventory.SetActive(false);
    }

    public void OnClose()
    {
        _animator.Play("Close");
        EnableMouse(false);

        foreach (var cell in _traderInventoryCells)
        {
            if (cell.HasItem)
            {
                Destroy(cell.GetComponentInChildren<UIItem>().gameObject);
                cell.HasItem = false;
            }
        }
    }

    private void OnTrade(List<WorldItem> worldItems, bool isBox)
    {
        if (!_inventoryCanvas.enabled)
        {
            for (int i = 0; i < _traderInventoryCells.Length; i++)
            {
                _traderInventoryCells[i].IsBox = isBox;

                if (i < worldItems.Count)
                {
                    UIItem uiItem = Instantiate(UIItemPrefab, _traderInventoryCells[i].transform).GetComponent<UIItem>();
                    uiItem.Initialize(worldItems[i], transform);
                    _traderInventoryCells[i].HasItem = true;
                }
            }
        }
        else
        {
            foreach (var cell in _traderInventoryCells)
            {
                if (cell.HasItem)
                {
                    Destroy(cell.GetComponentInChildren<UIItem>().gameObject);
                    cell.HasItem = false;
                }
            }
        }

        if (_inventoryCanvas.enabled)
        {
            _animator.Play("Close");
            EnableMouse(false);
        }
        else
        {
            _animator.Play("Open");
            EnableMouse(true);

            _playerInventory.SetActive(true);
            _traderInventory.SetActive(true);
        }
    }

    public bool DeliverItem(Items item)
    {
        foreach (var cell in _playerInventoryCells)
        {
            if (cell.HasItem)
            {
                if (cell.UIItem.Item == item)
                {
                    cell.HasItem = false;
                    Destroy(cell.UIItem.gameObject);
                    return true;
                }
            }
        }

        return false;
    }

    private void EnableMouse(bool value)
    {
        Cursor.lockState = value ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = value;
    }

    // events for animation
    public void OnEnableInventory()
    {
        _inventoryCanvas.enabled = _isUp = true;
    }

    public void OnDisableInventory()
    {
        _inventoryCanvas.enabled = _isUp = false;

        _playerInventory.SetActive(false);
        _traderInventory.SetActive(false);
    }
}
