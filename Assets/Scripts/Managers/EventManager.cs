using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager
{
    public static Action<Character> OnCharacterDeathAnimationFinished;
    public static Action<Character> OnCharacterDeath;
}
