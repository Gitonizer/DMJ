using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIController : MonoBehaviour, ICharacterUIController
{
    [SerializeField] private Slider _slider;
    [SerializeField] private CanvasGroup _deathScreen;
    [SerializeField] private Text _spellText;
    [SerializeField] private Image _spellImage;
    [SerializeField] private DamageUIController UIDamagePrefab;
    [SerializeField] private Transform UIDamageParent;

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

    public void OnDamage(DamageInfo damageInfo)
    {
        _slider.value -= damageInfo.Value;
        _slider.value = Mathf.Clamp(_slider.value, _slider.minValue, _slider.maxValue);

        DamageUIController damageUIController = Instantiate(UIDamagePrefab, UIDamageParent);
        damageUIController.Initialize(damageInfo.Value, damageInfo.Element, damageInfo.EffectiveNess);
        damageUIController.transform.rotation = Quaternion.LookRotation(damageUIController.transform.position - Camera.main.transform.position);
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

    public void SelectSpell(SpellScriptable spell)
    {
        _spellText.text = spell.Name;

        switch (spell.Element)
        {
            case Element.Fire:
                _spellImage.color = Color.red;
                break;
            case Element.Ice:
                _spellImage.color = Color.blue;
                break;
            case Element.Wind:
                _spellImage.color = Color.green;
                break;
            default:
                break;
        }
    }
    private void OnDamage(float value, Element element, TypeEffectiveness effectiveness)
    {
        DamageUIController damageUIController = Instantiate(UIDamagePrefab, UIDamageParent);
        damageUIController.Initialize(value, element, effectiveness);
        damageUIController.transform.rotation = Quaternion.LookRotation(damageUIController.transform.position - Camera.main.transform.position);
    }
}
