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

    private void Awake()
    {
        _inventoryCanvas = GetComponent<Canvas>();

        _playerInventoryCells = _playerInventory.GetComponentsInChildren<Cell>();
        _traderInventoryCells = _traderInventory.GetComponentsInChildren<Cell>();
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
        _inventoryCanvas.enabled = !_inventoryCanvas.enabled;

        Cursor.lockState = _inventoryCanvas.enabled ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = _inventoryCanvas.enabled;

        _playerInventory.SetActive(true);
        _traderInventory.SetActive(false);

        _isUp = _inventoryCanvas.enabled;
    }

    private void OnClose()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        _inventoryCanvas.enabled = false;

        foreach (var cell in _traderInventoryCells)
        {
            if (cell.HasItem)
            {
                Destroy(cell.GetComponentInChildren<UIItem>().gameObject);
                cell.HasItem = false;
            }
        }

        _playerInventory.SetActive(false);
        _traderInventory.SetActive(false);

        _isUp = _inventoryCanvas.enabled;
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

        _inventoryCanvas.enabled = !_inventoryCanvas.enabled;

        Cursor.lockState = _inventoryCanvas.enabled ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = _inventoryCanvas.enabled;

        _playerInventory.SetActive(_inventoryCanvas.enabled);
        _traderInventory.SetActive(_inventoryCanvas.enabled);

        _isUp = _inventoryCanvas.enabled;
    }
}
