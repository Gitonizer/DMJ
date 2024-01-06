using System.Collections.Generic;
using UnityEngine;

public class Quest
{
    public TextAsset StoryFile;
    public int RequiredNPCs;
    public List<ObjectiveGoal> ObjectiveGoals;

    public Quest(QuestScriptable quest)
    {
        StoryFile = quest.StoryFile;
        RequiredNPCs = quest.RequiredNPCs;
        ObjectiveGoals = new List<ObjectiveGoal>(quest.ObjectiveGoals);
    }
}
