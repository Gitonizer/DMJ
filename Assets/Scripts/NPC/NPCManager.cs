using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCManager : MonoBehaviour
{
    public GameObject Player;

    private List<Character> _npcs;

    [SerializeField] private GameObject _npcPrefab;

    private void Awake()
    {
        _npcs = new List<Character>();
    }

    public void SpawnNPC(Vector3 position)
    {
        Character character = GameObject.Instantiate(_npcPrefab, position, Quaternion.identity, transform).GetComponent<Character>();
        character.GetComponent<NPCInputManager>().Initialize(Player.transform);
        character.Initialize(position);
        _npcs.Add(character);
    }
}
