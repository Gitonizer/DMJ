using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public GameObject Player;

    [SerializeField] private GameObject _enemyPrefab;

    private List<Character> _enemies;
    [SerializeField] private int _deathCount;

    [SerializeField] private Transform _projectilesParent;

    public int DeathCount { get { return _deathCount; } }

    private void Awake()
    {
        _enemies = new List<Character>();

        SpawnEnemies();

        _deathCount = 0;
    }

    public void HandleDeath(Character deadEnemy)
    {
        _enemies.Remove(deadEnemy);

        SpawnEnemies();

        _deathCount++;
    }

    private void SpawnEnemies()
    {
        //do something here later for multiple spawns
        Vector3 randomPosition = new Vector3(Random.Range(-40, 40), 1f, Random.Range(-40, 40));

        foreach (var enemy in _enemies)
        {
            if (Vector3.Distance(randomPosition, enemy.transform.position) < 5)
            {
                SpawnEnemies();
            }
        }

        SpawnEnemy(randomPosition);
    }

    private void SpawnEnemy(Vector3 randomPosition)
    {
        Character character = GameObject.Instantiate(_enemyPrefab, transform).GetComponent<Character>();
        character.GetComponent<EnemyInputManager>().Initialize(Player.transform);
        character.GetComponent<SpellController>().Initialize(_projectilesParent);
        character.transform.position = randomPosition;
        _enemies.Add(character);
    }
}
