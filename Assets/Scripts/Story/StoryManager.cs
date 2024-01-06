using Ink.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryManager : MonoBehaviour
{
    public List<QuestScriptable> Quests;
    public Story Story;

    [SerializeField] private DialogManager _dialogueUI;
    [SerializeField] private ObjectivesUI _objectivesUI;

    [Header("Dungeon Generator")]
    [SerializeField] private DungeonGenerator _dungeonGenerator;

    [Header("Managers")]
    [SerializeField] private InventoryManager _inventoryManager;
    [SerializeField] private EnemyManager _enemyManager;
    [SerializeField] private ItemsManager _itemsManager;

    private string _currentDialogText;
    private QuestScriptable _currentQuest;
    private StoryActor _currentActor;

    public QuestScriptable CurrentQuest { get { return _currentQuest; } }

    private void OnEnable()
    {
        EventManager.OnOpenDialog += OnOpenDialog;
        EventManager.OnCloseDialog += OnCloseDialog;
        EventManager.OnContinueDialog += OnContinueDialog;
        EventManager.OnOpenDoor += OnOpenDoor;
    }
    private void OnDisable()
    {
        EventManager.OnOpenDialog -= OnOpenDialog;
        EventManager.OnCloseDialog -= OnCloseDialog;
        EventManager.OnContinueDialog -= OnContinueDialog;
        EventManager.OnOpenDoor -= OnOpenDoor;
    }

    private void Awake()
    {
        int randomQuest = Random.Range(0, Quests.Count);
        _currentQuest = Quests[randomQuest];
        _currentQuest.Initialize();
        _objectivesUI.Initialize(_currentQuest);
        Story = new Story(_currentQuest.StoryFile.text);
    }

    private void Start()
    {
        _dungeonGenerator.Initialize(_currentQuest); //for now, quest only decides how many NPCs
    }

    private void OnOpenDialog(StoryActor actor)
    {
        _currentActor = actor;

        if (string.IsNullOrEmpty(_currentDialogText))
            ManageDialog();

        _dialogueUI.OpenDialog(_currentActor, _currentDialogText);
    }
    public void OnContinueDialog()
    {
        ManageDialog();
        _dialogueUI.ContinueDialog(_currentDialogText);
    }
    private void OnCloseDialog()
    {
        _dialogueUI.CloseDialog();
    }
    private void OnOpenDoor(Door door)
    {
        foreach (var goal in _currentQuest.ObjectiveGoals)
        {
            if (goal.Type == ObjectiveType.Door && !goal.IsDone)
            {
                if (_inventoryManager.DeliverItem(Items.Key))
                {
                    door.Open();
                    _objectivesUI.UpdateObjective(goal);
                    goal.IsDone = true;
                    return;
                }
            }
        }

        print("you have no key yet");
    }

    private void ManageDialog()
    {
        if (!HasRequestItemByDialogue(Story.currentTags, _currentActor))
        {
            _dialogueUI.CloseDialog();
            EventManager.OnCloseDialog?.Invoke();
            return;
        }

        if (Story.canContinue)
        {
            _currentDialogText = Story.Continue();
            _dialogueUI.EnableNextButton(true);

            if (Story.currentChoices.Count != 0)
            {
                _dialogueUI.GenerateButtons(Story, OnContinueDialog);
                _dialogueUI.EnableNextButton(false);
            }

        }
        else if (Story.currentChoices.Count != 0 && !_dialogueUI.HasButtons())
        {
            // do choices
            _dialogueUI.EnableNextButton(false);
            _dialogueUI.GenerateButtons(Story, OnContinueDialog);
        }
        else
        {
            // end of story?
            _dialogueUI.CloseDialog();
            EventManager.OnCloseDialog?.Invoke();
        }
    }
    public bool HasRequestItemByDialogue(List<string> currentTags, StoryActor actor)
    {
        if (currentTags.Count == 0)
            return true;

        foreach (string tag in currentTags)
        {
            string[] splitTags = tag.Split(':');

            string key = splitTags[0];
            string value = splitTags[1];

            switch (key)
            {
                case "needsCoin":
                    //check if player has coin
                    foreach (var goal in _currentQuest.ObjectiveGoals)
                    {
                        if (goal.Item == Items.Coin && !goal.IsDone)
                        {
                            if (_inventoryManager.DeliverItem(Items.Coin))
                            {
                                _objectivesUI.UpdateObjective(goal);
                                StartCoroutine(_itemsManager.SpawnItem(actor.transform.position + actor.transform.right * 1.2f, goal.ItemReward));
                                goal.IsDone = true;
                                return true;
                            }
                        }
                    }
                    return false;
                default:
                    return true;
            }
        }

        return true;
    }
}
