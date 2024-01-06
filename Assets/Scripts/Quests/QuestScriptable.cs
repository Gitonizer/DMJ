using Ink.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Quest", menuName = "ScriptableObjects/Quests")]
public class QuestScriptable : ScriptableObject
{
    public TextAsset StoryFile;
    public int RequiredNPCs;
    public List<ObjectiveGoal> ObjectiveGoals;

    public void Initialize()
    {
        foreach (var goal in ObjectiveGoals)
        {
            goal.IsDone = false;
        }
    }
}
