using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyUIController : MonoBehaviour, ICharacterUIController
{
    [SerializeField] private Slider _slider;


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

    public void OnDamage(float currentHealth)
    {
        _slider.value = currentHealth;
    }

    public void OnDeath(){}
}
