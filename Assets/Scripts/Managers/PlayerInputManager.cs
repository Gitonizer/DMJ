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
    private Vector3 _turn;
    private int _spellIndex;

    private const float CAMERA_SENSITIVITY = 10f;

    private const float CAMERA_TURN_MINY = -70f;
    private const float CAMERA_TURN_MAXY = 50f;

    public float ForwardMovement { get  { return _forwardMovement; } }
    public float SideMovement { get { return _sideMovement; } }
    public bool Jumped { get { return _jumped; } }
    public bool Attacked { get { return _attacked; } }

    public bool Interacted { get { return _interacted; } set { _interacted = value; } }

    public bool PressedInventoryButton { get { return _pressedInventoryButton; } set { _pressedInventoryButton = value; } }

    public bool EnableInteractions { get { return _enableInteractions; } set { _enableInteractions = value; } }

    public Vector2 HorizontalMovement { get { return new Vector2(_sideMovement, _forwardMovement); } }

    public int SpellIndex { get { return _spellIndex; } }

    public Vector3 MouseLook { get { return _turn; } }

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
        for (int number = 1; number <= 9; number++) // read all 9 numbers and assign pressed ones to spellIndex
        {
            if (Input.GetKeyDown(number.ToString()))
            {
                _spellIndex = number;
            }
        }

        _pressedInventoryButton = Input.GetKeyDown(KeyCode.I);

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
