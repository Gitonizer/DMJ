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
    [SerializeField] private LoadingUI _loadingUI;
    [SerializeField] private FloorUI _foorUI;
    private void Awake()
    {
        SaveManager.LoadData((data) =>
        {
            if (data != null)
            {
                _currentLevel = data.Level;
            }
            else print("data was null");
        });

        _foorUI.SetLevel(_currentLevel);
    }

    private void Start()
    {
        CurrentLevel = _currentLevel;
        _loadingUI.Initialize(_currentLevel);
    }

    private void OnEnable()
    {
        EventManager.OnCharacterDeath += OnCharacterDeath;
        EventManager.OnCharacterDeathAnimationFinished += OnCharacterDeathAnimationFinished;
        EventManager.OnLevelLoaded += OnRemoveLoadingUI;
        EventManager.OnExitLevel += OnExitLevel;
    }

    private void OnDisable()
    {
        EventManager.OnCharacterDeath -= OnCharacterDeath;
        EventManager.OnCharacterDeathAnimationFinished -= OnCharacterDeathAnimationFinished;
        EventManager.OnLevelLoaded -= OnRemoveLoadingUI;
        EventManager.OnExitLevel += OnExitLevel;
    }
    private void OnCharacterDeath(Character character)
    {
        switch (character.CharacterType)
        {
            case CharacterType.Player:
                SaveManager.SaveData(0, 0); //reset level
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
    private void OnExitLevel(float currentHealth)
    {
        _currentLevel++;
        CurrentLevel = _currentLevel;

        SaveManager.SaveData(_currentLevel, (int)currentHealth);

        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    private void OnRemoveLoadingUI()
    {
        StartCoroutine(_loadingUI.Hide());
    }
}
