using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICharacterUIController
{
    public void Initialize(float maxHealth);
    public void Initialize(float maxHealth, float currentHealth);
    public void OnDamage(DamageInfo damageInfo);
    public void OnDeath();
    void SelectSpell(SpellScriptable spell);
}
