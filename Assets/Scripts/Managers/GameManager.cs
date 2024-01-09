using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private static int _currentLevel = 0;
    public int CurrentLevel;
    [SerializeField] private EnemyManager _enemyManager; //eventually remove this

    private void Start()
    {
        CurrentLevel = _currentLevel;
    }

    private void OnEnable()
    {
        EventManager.OnCharacterDeath += OnCharacterDeath;
        EventManager.OnCharacterDeathAnimationFinished += OnCharacterDeathAnimationFinished;
        EventManager.OnExitLevel += OnExitLevel;
    }

    private void OnDisable()
    {
        EventManager.OnCharacterDeath -= OnCharacterDeath;
        EventManager.OnCharacterDeathAnimationFinished -= OnCharacterDeathAnimationFinished;
        EventManager.OnExitLevel += OnExitLevel;
    }
    private void OnCharacterDeath(Character character)
    {
        switch (character.CharacterType)
        {
            case CharacterType.Player:
                break;
            case CharacterType.Enemy:
                _enemyManager.HandleDeath(character);
                break;
            case CharacterType.NPC:
                break;
            default:
                break;
        }
    }
    private void OnCharacterDeathAnimationFinished(Character character)
    {
        switch (character.CharacterType)
        {
            case CharacterType.Player:
                Scene scene = SceneManager.GetActiveScene();
                SceneManager.LoadScene(scene.name);

                break;
            case CharacterType.Enemy:
                Destroy(character.gameObject);
                break;
            case CharacterType.NPC:
                break;
            default:
                break;
        }
    }
    private void OnExitLevel()
    {
        _currentLevel++;
        CurrentLevel = _currentLevel;

        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }
}
