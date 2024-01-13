using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyUIController : MonoBehaviour, ICharacterUIController
{
    [SerializeField] private Slider _slider;
    [SerializeField] private DamageUIController UIDamagePrefab;

    private Camera _camera;

    private void Awake()
    {
        _camera = Camera.main;
    }

    private void Update()
    {
        _slider.transform.rotation = Quaternion.LookRotation(transform.position - _camera.transform.position);
    }

    public void Initialize(float maxHealth)
    {
        _slider.minValue = 0;
        _slider.maxValue = maxHealth;
        _slider.value = maxHealth;
    }
    public void Initialize(float maxHealth, float currentHealth)
    {
        _slider.minValue = 0;
        _slider.maxValue = maxHealth;
        _slider.value = currentHealth;
    }

    public void OnDamage(DamageInfo damageInfo)
    {
        _slider.value -= damageInfo.Value;
        _slider.value = Mathf.Clamp(_slider.value, _slider.minValue, _slider.maxValue);

        DamageUIController damageUIController = Instantiate(UIDamagePrefab, transform);
        damageUIController.Initialize(damageInfo.Value, damageInfo.Element, damageInfo.EffectiveNess);
        damageUIController.transform.rotation = Quaternion.LookRotation(damageUIController.transform.position - Camera.main.transform.position);
    }

    public void OnDeath(){}

    public void SelectSpell(SpellScriptable spell) {}
}
