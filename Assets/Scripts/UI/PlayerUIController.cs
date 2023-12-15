using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIController : MonoBehaviour, ICharacterUIController
{
    [SerializeField] private Slider _slider;
    [SerializeField] private CanvasGroup _deathScreen;

    private void OnEnable()
    {
        EventManager.OnHeal += OnHeal;
    }

    private void OnDisable()
    {
        EventManager.OnHeal -= OnHeal;
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

    public void OnDeath()
    {
        StartCoroutine(CO_FadeInDeathScren());
    }

    public void OnHeal(float value)
    {
        _slider.value += value;
        _slider.value = Mathf.Clamp(_slider.value, _slider.minValue, _slider.maxValue);
    }

    private IEnumerator CO_FadeInDeathScren()
    {
        float currentTime = 0f;
        float duration = 1f;

        while (currentTime < duration)
        {
            _deathScreen.alpha = Mathf.Lerp(0, 1, currentTime / duration);
            currentTime += Time.deltaTime;
            yield return null;
        }
    }
}
