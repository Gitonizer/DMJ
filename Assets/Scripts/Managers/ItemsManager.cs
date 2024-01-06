using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemsManager : MonoBehaviour
{
    [SerializeField] private ItemsScriptable _commonItems;
    [SerializeField] private ItemsScriptable _collectibleItems;
    [SerializeField] private ItemsScriptable _keyItems;

    private void OnEnable()
    {
        EventManager.OnCharacterDeath += SpawnItem;
    }
    private void OnDisable()
    {
        EventManager.OnCharacterDeath -= SpawnItem;
    }
    
    private void SpawnItem(Character character)
    {
        switch (character.CharacterType)
        {
            case CharacterType.Player:
                break;
            case CharacterType.Enemy:
                int randomItem = Random.Range(0, _commonItems.Items.Length);
                Vector3 characterPosition = new Vector3(character.transform.position.x, 1.4f, character.transform.position.z);
                WorldItem worldItem = Instantiate(_commonItems.Items[randomItem].WorldItem, characterPosition, Quaternion.identity, transform).GetComponentInChildren<WorldItem>();
                worldItem.ItemData = new ItemData()
                {
                    Name = _commonItems.Items[randomItem].Name,
                    Description = _commonItems.Items[randomItem].Description,
                    Item = _commonItems.Items[randomItem].Item,
                    ItemType = _commonItems.Items[randomItem].ItemType,
                    Value = _commonItems.Items[randomItem].Value,
                    InventoryTexture = _commonItems.Items[randomItem].InventoryTexture
                };

                break;
            case CharacterType.NPC:
                break;
            default:
                break;
        }
    }

    public IEnumerator SpawnItem(Vector3 position, Items item)
    {
        ItemScriptable pickedItem;

        pickedItem = FindItem(_collectibleItems.Items, item);
        if (pickedItem == null) pickedItem = FindItem(_commonItems.Items, item);
        if (pickedItem == null) pickedItem = FindItem(_keyItems.Items, item);

        if (pickedItem == null)
        {
            Debug.Log("Item to spawn not found");
            yield break;
        }

        //spawn item
        WorldItem worldItem = Instantiate(pickedItem.WorldItem, new Vector3(position.x, 1.4f, position.z), Quaternion.identity, transform).GetComponentInChildren<WorldItem>();
        worldItem.ItemData = new ItemData()
        {
            Name = pickedItem.Name,
            Description = pickedItem.Description,
            Item = pickedItem.Item,
            ItemType = pickedItem.ItemType,
            Value = pickedItem.Value,
            InventoryTexture = pickedItem.InventoryTexture
        };

        while (worldItem.transform.position.x != position.x && worldItem.transform.position.z != position.z)
        {
            transform.position = position;
            yield return null;
        }
    }
    private ItemScriptable FindItem(ItemScriptable[] items, Items targetItem)
    {
        foreach (var item in items)
        {
            if (item.Item == targetItem)
            {
                return item;
            }
        }

        return null;
    }
    public IEnumerator SpawnItem(Vector3 position)
    {
        int randomItem = Random.Range(0, _commonItems.Items.Length);
        ItemScriptable pickedItem = _commonItems.Items[randomItem];

        if (pickedItem == null)
        {
            Debug.Log("Item to spawn not found");
            yield break;
        }

        WorldItem worldItem = Instantiate(pickedItem.WorldItem, new Vector3(position.x, 1.4f, position.z), Quaternion.identity, transform).GetComponentInChildren<WorldItem>();
        worldItem.ItemData = new ItemData()
        {
            Name = pickedItem.Name,
            Description = pickedItem.Description,
            Item = pickedItem.Item,
            ItemType = pickedItem.ItemType,
            Value = pickedItem.Value,
            InventoryTexture = pickedItem.InventoryTexture
        };

        while(worldItem.transform.position.x != position.x || worldItem.transform.position.z != position.z)
        {
            transform.position = position;
            yield return null;
        }
    }
}
