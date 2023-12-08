using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterStats", menuName = "ScriptableObjects/Characters")]
public class CharacterStats : ScriptableObject
{
    public float Health;
    public float Mana;
    public float ManaRegenRate;

    public Element Resistance;
    public Element Weakness;
}
