using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIItem : MonoBehaviour
{
    public string Name;
    public string Description;
    public Items Item;
    public ItemType ItemType;
    public int Value;
    public Texture2D InventoryTexture { get; set; }
    public Image Image;

    [Header("Info")]
    public GameObject Info;
    public Text InfoTitle;
    public Text InfoDescription;

    [Header("Info")]
    public GameObject Use;

    private WorldItem _worldItem;

    private Transform _parentWhileDragging;
    private Transform _lastParent;

    public void Initialize(ItemData itemData, Transform inventoryTransform)
    {
        _lastParent = transform.parent;
        _parentWhileDragging = inventoryTransform;

        InfoTitle.text = Name = itemData.Name;
        InfoDescription.text = Description = itemData.Description;
        Item = itemData.Item;
        ItemType = itemData.ItemType;
        Value = itemData.Value;
        InventoryTexture = itemData.InventoryTexture;

        Image.sprite =
                Sprite.Create(InventoryTexture, new Rect(0, 0, InventoryTexture.width, InventoryTexture.height),
                Vector2.zero);
    }
    public void Initialize(WorldItem worldItem, Transform inventoryTransform)
    {
        _worldItem = worldItem;
        _lastParent = transform.parent;
        _parentWhileDragging = inventoryTransform;

        InfoTitle.text = Name = worldItem.ItemData.Name;
        InfoDescription.text = Description = worldItem.ItemData.Description;
        Item = worldItem.ItemData.Item;
        ItemType = worldItem.ItemData.ItemType;
        Value = worldItem.ItemData.Value;
        InventoryTexture = worldItem.ItemData.InventoryTexture;

        Image.sprite =
                Sprite.Create(InventoryTexture, new Rect(0, 0, InventoryTexture.width, InventoryTexture.height),
                Vector2.zero);
    }

    public void SetNewParent(Transform trans)
    {
        _lastParent.GetComponent<Cell>().HasItem = false;
        _lastParent = trans;
        transform.SetParent(trans);
    }

    public void OnDragPiece()
    {
        transform.SetParent(_parentWhileDragging);
        transform.SetAsLastSibling();
        Image.raycastTarget = false;
        transform.position = Input.mousePosition;
        Cell lastCell = _lastParent.GetComponent<Cell>();
        lastCell.HasItem = false;
        lastCell.UnassignItem();
    }

    public void OnDrop()
    {
        Image.raycastTarget = true;
        transform.SetParent(_lastParent);
        transform.localPosition = Vector3.zero;

        _lastParent.GetComponent<Cell>().HasItem = true;
    }

    public void OnClick()
    {
        ShowInfo(false);
        ShowUse(true);
    }

    public void OnEnter()
    {
        ShowInfo(true);
    }

    public void OnExit()
    {
        ShowInfo(false);
    }

    public void OnLeaveUse()
    {
        ShowUse(false);
    }

    public void OnUse()
    {
        switch (ItemType)
        {
            case ItemType.Collectible:
                break;
            case ItemType.Health:
                EventManager.OnHeal?.Invoke(Value);
                DestroyWorldItem();
                Destroy(gameObject);
                break;
            case ItemType.Mana:
                print("Used Mana Potion (not really)");
                DestroyWorldItem();
                Destroy(gameObject);
                break;
            default:
                break;
        }
    }

    public void DestroyWorldItem()
    {
        if (_worldItem != null)
            Destroy(_worldItem.transform.parent.gameObject);
    }

    private void ShowInfo(bool value)
    {
        Info.SetActive(value);
    }
    private void ShowUse(bool value)
    {
        if (ItemType != ItemType.Collectible) Use.SetActive(value); // improve this verification later

        switch (ItemType)
        {
            case ItemType.Health:
            case ItemType.Mana:
                Use.SetActive(value);
                break;
            default:
                break;
        }
    }

    private void OnDestroy()
    {
        if (_lastParent != null && _lastParent.GetComponent<Cell>())
            _lastParent.GetComponent<Cell>().HasItem = false;
    }
}
