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
    private Vector3 _turn;

    private const float ATTACK_DISTANCE = 10f;

    [SerializeField] private bool _disableAttacks = false;

    public float ForwardMovement { get { return _forwardMovement; } }
    public float SideMovement { get { return _sideMovement; } }
    public bool Jumped { get { return _jumped; } }
    public bool Attacked { get { return _attacked; } }

    public Vector2 HorizontalMovement { get { return new Vector2(_sideMovement, _forwardMovement); } }

    public Vector3 MouseLook { get { return _turn; } }

    public void Initialize(Transform playerTransform)
    {
        PlayerTransform = playerTransform;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            _disableAttacks = !_disableAttacks;

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
