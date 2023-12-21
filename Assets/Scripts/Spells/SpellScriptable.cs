using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Spell", menuName = "ScriptableObjects/Spells")]
public class SpellScriptable : ScriptableObject
{
    public string Name;
    public Projectile Projectile;
    public Element Element;
    public float Damage;
    public float Cost;
    public float CastTime;
    public float LifeTime;
    public float Speed;
}
