using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellController : MonoBehaviour
{
    public Transform SpellParent;
    public Transform StartingTransform;
    public List<SpellScriptable> Spells;

    private SpellScriptable _currentSpell;

    private int _index;

    private void Awake()
    {
        _index = 0;
        _currentSpell = Spells[0];
    }

    public int Initialize(Transform projectilesParent, Element element)
    {
        SpellParent = projectilesParent;

        for (int i = 0; i < Spells.Count; i++)
        {
            if (Spells[i].Element == element)
            {
                _currentSpell = Spells[i];
                return i;
            }
        }

        return 0;
    }

    public void CastSpell(Character projectileOwner)
    {
        //logic for current spell later
        Projectile projectile = Instantiate(_currentSpell.Projectile, SpellParent);
        projectile.transform.SetPositionAndRotation(StartingTransform.position, StartingTransform.rotation);

        projectile.Initialize(_currentSpell, projectileOwner);
    }

    public void SelectSpell(int index)
    {
        if (index >= Spells.Count || index < 0)
        {
            Debug.LogError("Spell not assigned to slot number " + index);
            return;
        }

        _index = index;
        _currentSpell = Spells[index];
    }
}
