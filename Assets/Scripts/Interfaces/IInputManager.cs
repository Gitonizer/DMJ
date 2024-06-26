using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInputManager
{
    public float ForwardMovement { get; }
    public float SideMovement { get; }
    public bool Jumped { get; }
    public bool Attacked { get; }

    public bool Interacted { get; set; }
    public bool PressedInventoryButton { get; set; }
    public bool Dashing { get; set; }
    public bool EnableInteractions { get; set; }

    public Vector2 HorizontalMovement { get; }

    public int SpellIndex { get; }

    public Vector3 MouseLook { get; }

    public bool IsSelectingSpell { get; }
}
