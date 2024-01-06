using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectivesUI : MonoBehaviour
{
    [SerializeField] private Transform _objectivesGrid;
    [SerializeField] private GameObject _objectivePrefab;

    private QuestScriptable _quest;

    private List<ObjectiveUI> _objectiveUIs;

    private void OnEnable()
    {
        EventManager.OnCharacterDeath += OnCharacterDeath;
    }

    private void OnDisable()
    {
        EventManager.OnCharacterDeath -= OnCharacterDeath;
    }

    public void Initialize(QuestScriptable quest)
    {
        _quest = quest;
        _objectiveUIs = new List<ObjectiveUI>();

        foreach (var goal in _quest.ObjectiveGoals)
        {
            ObjectiveUI objUI = Instantiate(_objectivePrefab, _objectivesGrid).GetComponent<ObjectiveUI>();
            objUI.Initialize(goal);
            _objectiveUIs.Add(objUI);
        }
    }

    private void OnCharacterDeath(Character character)
    {
        foreach (var objUI in _objectiveUIs)
        {
            if (objUI.Type == ObjectiveType.Defeat)
            {
                objUI.UpdateObjective();
            }
        }
    }

    public void UpdateObjective(ObjectiveGoal goal)
    {
        foreach (var objUI in _objectiveUIs)
        {
            if (goal == objUI.Goal)
            {
                objUI.UpdateObjective();
                return;
            }
        }
    }
    public void UpdateObjective(ObjectiveType objectiveType)
    {
        foreach (var objUI in _objectiveUIs)
        {
            if (objUI.Type == objectiveType && !objUI.Goal.IsDone)
            {
                objUI.UpdateObjective();
                return;
            }
        }
    }
}
