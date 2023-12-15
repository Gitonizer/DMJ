using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager
{
    public static Action<Character> OnCharacterDeathAnimationFinished;
    public static Action<Character> OnCharacterDeath;
    public static Action<List<WorldItem>, bool> OnTrade;
    public static Action OnClose;
    public static Action<float> OnHeal;
}
