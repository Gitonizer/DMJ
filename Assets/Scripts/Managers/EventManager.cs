using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager
{
    public static Action<Character> OnCharacterDeathAnimationFinished;
    public static Action<Character> OnCharacterDeath;
    public static Action<Door> OnOpenDoor;
    public static Action<int> OnEnemiesPlaced;
    public static Action<List<WorldItem>, bool> OnTrade;
    public static Action OnClose;
    public static Action<float> OnHeal;
    public static Action<StoryActor> OnOpenDialog;
    public static Action OnContinueDialog;
    public static Action OnCloseDialog;
    public static Action OnWinScreen;
    public static Action OnExitLevel;
}
