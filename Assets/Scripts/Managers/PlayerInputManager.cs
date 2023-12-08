using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputManager : MonoBehaviour, IInputManager
{
    private float _forwardMovement;
    private float _sideMovement;
    private bool _jumped;
    private bool _attacked;
    private Vector3 _turn;

    private const float CAMERA_SENSITIVITY = 10f;

    private const float CAMERA_TURN_MINY = -70f;
    private const float CAMERA_TURN_MAXY = 50f;

    public float ForwardMovement { get  { return _forwardMovement; } }
    public float SideMovement { get { return _sideMovement; } }
    public bool Jumped { get { return _jumped; } }
    public bool Attacked { get { return _attacked; } }

    public Vector2 HorizontalMovement { get { return new Vector2(_sideMovement, _forwardMovement); } }

    public Vector3 MouseLook { get { return _turn; } }

    private void Awake()
    {
        _turn = new Vector3();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void Update()
    {
        _forwardMovement = Input.GetAxisRaw("Vertical");
        _sideMovement = Input.GetAxisRaw("Horizontal");

        _jumped = Input.GetButtonDown("Jump");
        _attacked = Input.GetButtonDown("Fire1");

        _turn.x += Input.GetAxis("Mouse X") * CAMERA_SENSITIVITY;
        _turn.y += Input.GetAxis("Mouse Y") * CAMERA_SENSITIVITY;

        _turn.y = Mathf.Clamp(_turn.y, CAMERA_TURN_MINY, CAMERA_TURN_MAXY);

    }
}
