using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageController : MonoBehaviour
{
    [SerializeField] private float _currentHealth;
    private Element _resistance;
    private Element _weakness;

    private const float RESISTANCE_FACTOR = 0.5f;
    private const float WEAKNESS_FACTOR = 1.5f;

    public float CurrentHealth { get { return _currentHealth; } }

    private float _maxHealth;

    private void OnEnable()
    {
        EventManager.OnHeal += OnHeal;
    }

    private void OnDisable()
    {
        EventManager.OnHeal -= OnHeal;
    }

    public void Initialize(CharacterStats characterStats)
    {
        _maxHealth = _currentHealth = characterStats.Health;
        _resistance = characterStats.Resistance;
        _weakness = characterStats.Weakness;
    }

    public void OnDamage(float value, Element element)
    {
        switch (element)
        {
            case Element.Fire:
                CalculateDamage(value, element);
                break;
            case Element.Ice:
                CalculateDamage(value, element);
                break;
            case Element.Wind:
                CalculateDamage(value, element);
                break;
            default:
                break;
        }
        _currentHealth -= value;
    }
    public void OnHeal(float value)
    {
        _currentHealth += value;
        _currentHealth = Mathf.Clamp(_currentHealth, 0, _maxHealth);
    }

    private float CalculateDamage(float value, Element element)
    {
        if (_resistance == element)
        {
            value *= RESISTANCE_FACTOR;
            return value;
        }
        if (_weakness == element)
        {
            value *= WEAKNESS_FACTOR;
            return value;
        }

        return value;
    }
}
