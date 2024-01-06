using Ink.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    public Text Name;
    public Text Body;
    public GameObject NextButton;

    private Canvas _canvas;

    [SerializeField] Transform _buttonGridParent;
    [SerializeField] GameObject _buttonPrefab;

    private void Awake()
    {
        _canvas = GetComponent<Canvas>();
    }

    public void OpenDialog(StoryActor actor, string bodyText)
    {
        Name.text = actor.Name;
        Body.text = bodyText;
        _canvas.enabled = true;
    }
    public void ContinueDialog(string bodyText)
    {
        Body.text = bodyText;
    }
    public void CloseDialog()
    {
        _canvas.enabled = false;
    }

    public void GenerateButtons(Story story, Action continueStory)
    {
        foreach (var choice in story.currentChoices)
        {
            //assign event
            EventTrigger buttonTrigger = Instantiate(_buttonPrefab, _buttonGridParent).GetComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener((eventData) =>
            {
                DestroyButtons();
                story.ChooseChoiceIndex(choice.index);
                continueStory();
            });
            buttonTrigger.triggers.Add(entry);

            //assign text
            buttonTrigger.GetComponentInChildren<Text>().text = choice.text;
        }
    }
    public void DestroyButtons()
    {
        foreach (Transform button in _buttonGridParent)
        {
            Destroy(button.gameObject);
        }
    }

    public void EnableNextButton(bool value)
    {
        NextButton.SetActive(value);
    }

    public bool HasButtons()
    {
        return _buttonGridParent.childCount > 0;
    }
}
