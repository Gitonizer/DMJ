using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInputManager : MonoBehaviour, IInputManager
{
    private Transform PlayerTransform;

    private float _forwardMovement;
    private float _sideMovement;
    private bool _jumped;
    private bool _attacked;
    private bool _interacted;
    private bool _inventoryPressed;
    private Vector3 _turn;
    private int _spellIndex;
    private int _spellCount;

    private const float ATTACK_DISTANCE = 10f;
    private const float PURSUE_DISTANCE = 20f;

    [SerializeField] private bool _disableAttacks = false;

    public float ForwardMovement { get { return _forwardMovement; } }
    public float SideMovement { get { return _sideMovement; } }
    public bool Jumped { get { return _jumped; } }
    public bool Attacked { get { return _attacked; } }
    public bool Interacted { get { return false; } set { _interacted = value; } }
    public bool PressedInventoryButton { get; set; }
    public bool Dashing { get; set; }
    public bool EnableInteractions { get { return false; } set { _inventoryPressed = value; } }
    public Vector2 HorizontalMovement { get { return new Vector2(_sideMovement, _forwardMovement); } }

    public int SpellIndex { get { return _spellIndex; } }

    public Vector3 MouseLook { get { return _turn; } }
    public bool IsSelectingSpell { get; set; }

    public void Initialize(Transform playerTransform, int spellCount, int spellIndex)
    {
        _spellIndex = spellIndex + 1; // I don't know what I was smoking
        _spellCount = spellCount;
        PlayerTransform = playerTransform;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            _disableAttacks = !_disableAttacks;

        if (Vector3.Distance(transform.position, PlayerTransform.position) > PURSUE_DISTANCE)
            return;

        transform.LookAt(PlayerTransform);

        if (Vector3.Distance(transform.position, PlayerTransform.position) > ATTACK_DISTANCE)
        {
            _forwardMovement = 1f;
        }
        else
        {
            _forwardMovement = 0f;
            _attacked = !_disableAttacks;
        }
    }
}
