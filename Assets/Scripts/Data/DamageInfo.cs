using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageInfo
{
    private float _value;
    private Element _element;
    private TypeEffectiveness _effectiveNess;

    public float Value { get { return _value; } }
    public Element Element { get { return _element; } }
    public TypeEffectiveness EffectiveNess { get { return _effectiveNess; } }
    public DamageInfo(float value, Element element, TypeEffectiveness effectiveNess)
    {
        _value = value;
        _element = element;
        _effectiveNess = effectiveNess;
    }
}
