using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField] private CharacterStats _characterStats;
    [SerializeField] private PlayerState _playerState;
    [SerializeField] private CharacterType _characterType;
    [SerializeField] private InventoryManager _inventoryManager;

    private IInputManager _inputManager;
    private CharacterController _characterController;
    private Animator _animator;
    private AnimationController _animationController;
    private SpellController _spellController;
    private DamageController _damageController;
    private ICharacterUIController _characterUIController;

    [SerializeField]
    private Vector3 _gravity;
    
    [SerializeField]
    private float _playerSpeed = 10f;
    [SerializeField]
    private float _jumpSpeed = 10f;

    private float _attackTime = 0;
    private float _damageTime = 0;
    private float _deathTime = 0;

    private const float FALL_FACTOR = 8f;

    private bool _isInitialized;
    private Vector3 _initialPosition;

    public CharacterType CharacterType { get { return _characterType; } }

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        _animationController = GetComponent<AnimationController>();
        _spellController = GetComponent<SpellController>();
        _gravity = transform.up * 9.8f;
        _inputManager = GetComponent<IInputManager>();
        _damageController = GetComponent<DamageController>();
        _characterUIController = GetComponentInChildren<ICharacterUIController>();
        _isInitialized = false;
    }

    private void Start()
    {
        _damageController.Initialize(_characterStats);
        _characterUIController.Initialize(_characterStats.Health);
    }

    private void Update()
    {
        if (!_isInitialized)
            return;

        if (transform.position.x == 0 && transform.position.z == 0) //hack :/
        {
            transform.position = _initialPosition;
            return;
        }

        switch (_playerState)
        {
            case PlayerState.Grounded:
                _characterController.Move(Movement());
                _animationController.AnimateGroundMovement(_inputManager.HorizontalMovement);

                if (_inputManager.Attacked)
                {
                    _playerState = PlayerState.Attacking;
                    _animationController.CastSpell();
                    _spellController.CastSpell();
                }

                if (_inputManager.PressedInventoryButton)
                {
                    _inventoryManager.OnInventory();
                }

                if (_inputManager.Jumped)
                {
                    _playerState = PlayerState.Airborne;
                    _animationController.Jump(true);
                    _gravity = -(Vector3.up * _jumpSpeed);
                }

                if (!_characterController.isGrounded)
                    _playerState = PlayerState.Airborne;
                break;
            case PlayerState.Airborne:
                _characterController.Move(Movement());

                _gravity += Vector3.up * Time.deltaTime * FALL_FACTOR;
                if (_characterController.isGrounded)
                {
                    _gravity = transform.up * 9.8f;
                    _playerState = PlayerState.Grounded;

                    _animationController.Jump(false);
                }
                break;
            case PlayerState.Attacking:
                float attackAnimLength = _animator.GetCurrentAnimatorStateInfo(1).length;
                _characterController.Move(Movement());
                _animationController.AnimateGroundMovement(_inputManager.HorizontalMovement);

                if (_attackTime <= attackAnimLength)
                {
                    _attackTime += Time.deltaTime;
                }
                else
                {
                    _attackTime = 0;
                    _playerState = PlayerState.Grounded;
                }
                break;
            case PlayerState.Damaged:
                float damageAnimLengh = _animator.GetCurrentAnimatorStateInfo(0).length;
                if (_damageTime <= damageAnimLengh)
                {
                    _damageTime += Time.deltaTime;
                }
                else
                {
                    _damageTime = 0;
                    _playerState = PlayerState.Grounded;
                }
                break;
            case PlayerState.Death:
                float deathAnimLengh = _animator.GetCurrentAnimatorStateInfo(0).length;
                if (_deathTime <= deathAnimLengh)
                {
                    _deathTime += Time.deltaTime;
                }
                else
                {
                    EventManager.OnCharacterDeathAnimationFinished?.Invoke(this);
                }
                break;
            default:
                break;
        }

        if (_inputManager != null && _inventoryManager != null)
        {
            _inputManager.EnableInteractions = !_inventoryManager.IsUp;
        }

        _gravity = new Vector3(0f, _gravity.y, 0f);

        if (transform.position.y < -150f)
        {
            EventManager.OnCharacterDeathAnimationFinished?.Invoke(this);
        }
        else if (transform.position.y < -20f)
        {
            if (_characterType == CharacterType.Player) _characterUIController.OnDeath();
        }

        if (_spellController)
        {
            _spellController.SelectSpell(_inputManager.SpellIndex - 1);
            _characterUIController.SelectSpell(_spellController.Spells[_inputManager.SpellIndex - 1]);
        }

    }

    private void FixedUpdate()
    {
        if (_inventoryManager == null)
            return;

        RaycastHit hit;
        if (Physics.Raycast(transform.position + new Vector3(0, 1, 0), transform.TransformDirection(Vector3.forward), out hit, 2f))
        {
            if (hit.transform.gameObject.GetComponent<IInteractable>() == null)
                return; 

            IInteractable interactable = hit.transform.gameObject.GetComponent<IInteractable>();

            if (_inputManager.Interacted)
            {
                interactable.Interact();
                _inputManager.Interacted = false;
            }
        }

        _inputManager.Interacted = false;
    }

    public void Initialize(Vector3 position)
    {
        _initialPosition = position;
        Invoke(nameof(InitializeMovement), 1f);
    }

    private void InitializeMovement()
    {
        _isInitialized = true;
    }

    public void OnDamage(float damage, Element element)
    {
        if (_characterType == CharacterType.NPC)
            return;

        DamageInfo damageInfo = _damageController.OnDamage(damage, element);
        _characterUIController.OnDamage(damageInfo);

        if (_damageController.CurrentHealth <= 0)
        {
            GetComponent<Collider>().enabled = false;
            _playerState = PlayerState.Death;
            _animationController.Die();
            EventManager.OnCharacterDeath?.Invoke(this);
            _characterUIController.OnDeath();
        }
        else
        {
            _playerState = PlayerState.Damaged;
            _animationController.GetDamaged();
        }
    }

    private Vector3 Movement()
    {
        Vector3 forward = _inputManager.ForwardMovement * transform.forward;
        Vector3 strafe = _inputManager.SideMovement * transform.right;
        return _playerSpeed * Time.deltaTime * ((forward + strafe).normalized - _gravity);
    }
}
