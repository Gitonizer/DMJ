using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICharacterUIController
{
    public void Initialize(float maxHealth);
    public void OnDamage(float currentHealth);
    public void OnDeath();
}
