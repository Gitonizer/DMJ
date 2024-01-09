using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public GameObject Player;

    [SerializeField] private GameObject _enemyPrefab;

    private List<Character> _enemies;
    private int _totalEnemies;
    [SerializeField] private int _deathCount;

    [SerializeField] private Transform _projectilesParent;

    public int DeathCount { get { return _deathCount; } }

    public int TotalEnemies { get { return _enemies.Count; } }

    private void OnEnable()
    {
        EventManager.OnCharacterDeath += HandleDeath;
    }

    private void OnDisable()
    {
        EventManager.OnCharacterDeath -= HandleDeath;
    }

    private void Awake()
    {
        _enemies = new List<Character>();
        _deathCount = 0;
    }

    public void HandleDeath(Character deadEnemy)
    {
        _enemies.Remove(deadEnemy);

        //don't respawn enemy for now
        //Invoke(nameof(SpawnEnemies), 4);

        _deathCount++;
    }

    public void SpawnEnemy(Vector3 position)
    {
        Character character = GameObject.Instantiate(_enemyPrefab, position, Quaternion.identity, transform).GetComponent<Character>();
        character.GetComponent<EnemyInputManager>().Initialize(Player.transform, character.GetComponent<SpellController>().Spells.Count);
        character.GetComponent<SpellController>().Initialize(_projectilesParent);
        character.Initialize(position);
        _enemies.Add(character);
    }

    public IEnumerator QueryEnemyCount(Action<int> callback) // hacky
    {
        yield return new WaitForSeconds(2f);
        callback(transform.childCount);
    }
}
