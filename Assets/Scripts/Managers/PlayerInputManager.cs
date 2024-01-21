using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputManager : MonoBehaviour, IInputManager
{
    private float _forwardMovement;
    private float _sideMovement;
    private bool _jumped;
    private bool _attacked;
    private bool _enableInteractions;
    private bool _interacted;
    private bool _pressedInventoryButton;
    private bool _isSelectingSpell;
    private Vector3 _turn;
    private int _spellIndex;
    private bool _dialogOpen;
    private bool _dashing;

    private const float CAMERA_SENSITIVITY = 10f;

    private const float CAMERA_TURN_MINY = -70f;
    private const float CAMERA_TURN_MAXY = 50f;

    public float ForwardMovement { get  { return _forwardMovement; } }
    public float SideMovement { get { return _sideMovement; } }
    public bool Jumped { get { return _jumped; } }
    public bool Attacked { get { return _attacked; } }

    public bool Interacted { get { return _interacted; } set { _interacted = value; } }

    public bool PressedInventoryButton { get { return _pressedInventoryButton; } set { _pressedInventoryButton = value; } }
    public bool Dashing { get { return _dashing; } set { _dashing = value; } }

    public bool EnableInteractions { get { return _enableInteractions; } set { _enableInteractions = value; } }

    public Vector2 HorizontalMovement { get { return new Vector2(_sideMovement, _forwardMovement); } }

    public int SpellIndex { get { return _spellIndex; } }

    public Vector3 MouseLook { get { return _turn; } }

    public bool IsSelectingSpell { get { return _isSelectingSpell; } }

    private void OnEnable()
    {
        EventManager.OnOpenDialog += OnOpenDialog;
        EventManager.OnCloseDialog += OnCloseDialog;
    }

    private void OnDisable()
    {
        EventManager.OnOpenDialog -= OnOpenDialog;
        EventManager.OnCloseDialog -= OnCloseDialog;
    }

    private void Awake()
    {
        _turn = new Vector3();
        _enableInteractions = true;
        _spellIndex = 1;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void Update()
    {
        _isSelectingSpell = false;

        if (_dialogOpen)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                EventManager.OnContinueDialog?.Invoke();
            }
        }
        else
        {
            for (int number = 1; number <= 9; number++) // read all 9 numbers and assign pressed ones to spellIndex
            {
                if (Input.GetKeyDown(number.ToString()))
                {
                    _spellIndex = number;
                    _isSelectingSpell = true;
                }
            }

            _pressedInventoryButton = Input.GetKeyDown(KeyCode.I);
            _dashing = Input.GetKeyDown(KeyCode.LeftShift);

            if (Input.GetKeyDown(KeyCode.E))
            {
                _interacted = true;
            }

            _forwardMovement = Input.GetAxisRaw("Vertical");
            _sideMovement = Input.GetAxisRaw("Horizontal");

            _jumped = Input.GetButtonDown("Jump");

            if (_enableInteractions)
            {
                _attacked = Input.GetButtonDown("Fire1");
                _turn.x += Input.GetAxis("Mouse X") * CAMERA_SENSITIVITY;
                _turn.y += Input.GetAxis("Mouse Y") * CAMERA_SENSITIVITY;

                _turn.y = Mathf.Clamp(_turn.y, CAMERA_TURN_MINY, CAMERA_TURN_MAXY);
            }
        }
    }

    private void EnableMouse(bool enable)
    {
        Cursor.lockState = enable ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = enable;
    }

    private void OnOpenDialog(StoryActor actor)
    {
        // reset inputs
        _forwardMovement = 0f;
        _sideMovement = 0f;
        _attacked = false;
        _jumped = false;

        //enable mouse
        EnableMouse(true);

        //open dialog
        _dialogOpen = true;
    }
    private void OnCloseDialog()
    {
        EnableMouse(false);
        _dialogOpen = false;
    }
}
