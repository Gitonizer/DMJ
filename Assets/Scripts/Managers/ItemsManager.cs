using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemsManager : MonoBehaviour
{
    [SerializeField] private ItemsScriptable _gameItems;
    [SerializeField] private int _numbItems;

    private int _groundLimit;

    private void Awake()
    {
        _groundLimit = 40;
    }
    private void OnEnable()
    {
        EventManager.OnCharacterDeath += SpawnItem;
    }
    private void OnDisable()
    {
        EventManager.OnCharacterDeath -= SpawnItem;
    }
    private void Start()
    {
        //change this logic when procgen is implemented
        SpawnItems();
    }

    private void SpawnItems()
    {
        for (int i = 0; i < _numbItems; i++)
        {
            int randomItem = Random.Range(0, _gameItems.Items.Length);
            Vector3 randomPosition = new Vector3(Random.Range(-_groundLimit, _groundLimit), 1.4f, Random.Range(-_groundLimit, _groundLimit));

            WorldItem worldItem = Instantiate(_gameItems.Items[randomItem].WorldItem, randomPosition, Quaternion.identity, transform).GetComponentInChildren<WorldItem>();
            worldItem.ItemData = new ItemData()
            {
                Name = _gameItems.Items[randomItem].Name,
                Description = _gameItems.Items[randomItem].Description,
                ItemType = _gameItems.Items[randomItem].ItemType,
                Value = _gameItems.Items[randomItem].Value,
                InventoryTexture = _gameItems.Items[randomItem].InventoryTexture
            };
        }
    }
    
    private void SpawnItem(Character character)
    {
        switch (character.CharacterType)
        {
            case CharacterType.Player:
                break;
            case CharacterType.Enemy:
                int randomItem = Random.Range(0, _gameItems.Items.Length);
                Vector3 characterPosition = new Vector3(character.transform.position.x, 1.4f, character.transform.position.z);
                WorldItem worldItem = Instantiate(_gameItems.Items[randomItem].WorldItem, characterPosition, Quaternion.identity, transform).GetComponentInChildren<WorldItem>();
                worldItem.ItemData = new ItemData()
                {
                    Name = _gameItems.Items[randomItem].Name,
                    Description = _gameItems.Items[randomItem].Description,
                    ItemType = _gameItems.Items[randomItem].ItemType,
                    Value = _gameItems.Items[randomItem].Value,
                    InventoryTexture = _gameItems.Items[randomItem].InventoryTexture
                };

                break;
            case CharacterType.NPC:
                break;
            default:
                break;
        }
    }
}
