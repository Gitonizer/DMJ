using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    public void Initialize(CharacterStats characterStats);
    public void Damage(float valuem, Element element);
    public float CurrentHealth { get; }
}
