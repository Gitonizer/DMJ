using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldItem : MonoBehaviour, IInteractable
{
    public ItemData ItemData;
    public Collider ParentCollider;

    public void Interact()
    {
        ParentCollider.enabled = false;
        EventManager.OnTrade?.Invoke(new List<WorldItem>() { this }, false); // not a box
        ParentCollider.enabled = true;
    }
}
