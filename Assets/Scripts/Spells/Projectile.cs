using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Element Element;
    public ElementScriptable ElementScriptable;

    private ParticleSystem[] _particleSystems;

    private float _speed;
    private float _damage;

    private Character _projectileOwner;

    private void Awake()
    {
        _particleSystems = GetComponentsInChildren<ParticleSystem>();
    }
    void Update()
    {
        transform.Translate(_speed * Time.deltaTime * Vector3.forward);
    }

    public void Initialize(SpellScriptable spellScriptable, Character projectileOwner)
    {
        Destroy(gameObject, spellScriptable.LifeTime);
        _speed = spellScriptable.Speed;
        _damage = spellScriptable.Damage;
        Element = spellScriptable.Element;

        _projectileOwner = projectileOwner;

        switch (Element)
        {
            case Element.Fire:
                SetColorGradient(new Color(0.25f, 0, 0), Color.red);
                break;
            case Element.Ice:
                SetColorGradient(new Color(0, 0, 0.25f), Color.blue);
                break;
            case Element.Wind:
                SetColorGradient(new Color(0, 0.25f, 0), Color.green);
                break;
            default:
                break;
        }
    }

    private void SetColorGradient(Color color1, Color color2)
    {
        for (int i = 0; i < _particleSystems.Length; i++)
        {
            ParticleSystem.MainModule psMainModule = _particleSystems[i].main;
            psMainModule.startColor = new ParticleSystem.MinMaxGradient(color1, color2);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<Character>() != null)
        {
            Character character = collision.gameObject.GetComponent<Character>();

            if (character != _projectileOwner) // prevent hurting yourself
                character.OnDamage(_damage, Element, _projectileOwner);
        }

        //maybe spawn particles here later

        Destroy(gameObject);
    }
}
