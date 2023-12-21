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

    public DamageInfo OnDamage(float value, Element element)
    {
        DamageInfo damageInfo = CalculateDamage(value, element);
        _currentHealth -= damageInfo.Value;
        return damageInfo;
    }
    public void OnHeal(float value)
    {
        _currentHealth += value;
        _currentHealth = Mathf.Clamp(_currentHealth, 0, _maxHealth);
    }

    private DamageInfo CalculateDamage(float value, Element element)
    {
        if (_resistance == element)
        {
            value *= RESISTANCE_FACTOR;
            return new DamageInfo(value, element, TypeEffectiveness.Resistance);
        }
        if (_weakness == element)
        {
            value *= WEAKNESS_FACTOR;
            return new DamageInfo(value, element, TypeEffectiveness.Weakness);
        }

        return new DamageInfo(value, element, TypeEffectiveness.Neutral);
    }
}
