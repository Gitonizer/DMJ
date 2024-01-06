using UnityEngine;

public class StoryActor : MonoBehaviour, IInteractable
{
    public GameObject FakeCamera;

    private string _name;
    private int _id;
    private int _storyID;
    private int[] _storyBeats;

    public string Name { get { return _name; } }
    public int ID { get { return _id; } }
    public int StoryID { get { return _storyID; } }
    public int[] StoryBeats { get { return _storyBeats; } }

    private void Awake()
    {
        _name = "Peidalhaço"; // remove this later
    }

    public void Initialize(string name, int id, int storyID, int[] storyBeats)
    {
        _name = name;
        _id = id;
        _storyID = storyID;
        _storyBeats = storyBeats;
    }

    public void Interact()
    {
        EventManager.OnOpenDialog?.Invoke(this);
    }
}
