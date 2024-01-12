using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    private Animator _animator;

    private int _strafeLeft;
    private int _strafeRight;
    private int _runForward;
    private int _runBackward;
    private int _jump;
    private int _castSpell;
    private int _getDamaged;
    private int _die;
    private int _dash;

    private void Awake()
    {
        _animator = GetComponent<Animator>();

        //cache animator hashes
        _strafeLeft = Animator.StringToHash("StrafeLeft");
        _strafeRight = Animator.StringToHash("StrafeRight");
        _runForward = Animator.StringToHash("RunForward");
        _runBackward = Animator.StringToHash("RunBackward");
        _jump = Animator.StringToHash("Jump");
        _castSpell = Animator.StringToHash("CastSpell");
        _getDamaged = Animator.StringToHash("GetDamaged");
        _die = Animator.StringToHash("Die");
        _dash = Animator.StringToHash("Dash");
    }

    public void AnimateGroundMovement(Vector2 movement)
    {
        _animator.SetBool(_strafeLeft, movement.x < 0f);
        _animator.SetBool(_strafeRight, movement.x > 0f);
        _animator.SetBool(_runForward, movement.y > 0f);
        _animator.SetBool(_runBackward, movement.y < 0f);
    }

    public void Jump(bool value)
    {
        _animator.SetBool(_jump, value);
    }

    public void CastSpell()
    {
        _animator.SetTrigger(_castSpell);
    }

    public void GetDamaged()
    {
        _animator.SetTrigger(_getDamaged);
    }

    public void Die()
    {
        _animator.SetTrigger(_die);
    }

    public void Dash(bool value)
    {
        _animator.SetBool(_dash, value);
    }
}
