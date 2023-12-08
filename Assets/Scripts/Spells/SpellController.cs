using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellController : MonoBehaviour
{
    public Transform SpellParent;
    public Transform StartingTransform;
    public List<SpellScriptable> Spells;

    private SpellScriptable _currentSpell;

    private int index;

    private void Awake()
    {
        index = 0;
        _currentSpell = Spells[0];
    }

    public void Initialize(Transform projectilesParent)
    {
        SpellParent = projectilesParent;
    }

    public void CastSpell()
    {
        //logic for current spell later
        Projectile projectile = Instantiate(_currentSpell.Projectile, SpellParent);
        projectile.transform.SetPositionAndRotation(StartingTransform.position, StartingTransform.rotation);

        projectile.Initialize(_currentSpell);

        index++;
        if (index >= Spells.Count)
            index = 0;

        _currentSpell = Spells[index];
    }
}
