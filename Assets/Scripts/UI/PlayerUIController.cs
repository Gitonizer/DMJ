using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIController : MonoBehaviour, ICharacterUIController
{
    [SerializeField] private Slider _healthSlider;
    [SerializeField] private CanvasGroup _deathScreen;
    [SerializeField] private CanvasGroup _winScreen;
    [SerializeField] private Text _spellText;
    [SerializeField] private Image _spellImage;
    [SerializeField] private DamageUIController UIDamagePrefab;
    [SerializeField] private Transform UIDamageParent;
    [SerializeField] private GameObject _dashSliderObject;

    private Slider _dashSlider;
    private Text _dashText;

    private void Awake()
    {
        _dashSlider = _dashSliderObject.GetComponentInChildren<Slider>();
        _dashText = _dashSliderObject.GetComponentInChildren<Text>();
    }

    private void OnEnable()
    {
        EventManager.OnWinScreen += OnWin;
        EventManager.OnHeal += OnHeal;
    }

    private void OnDisable()
    {
        EventManager.OnWinScreen -= OnWin;
        EventManager.OnHeal -= OnHeal;
    }

    public void Initialize(float maxHealth)
    {
        _healthSlider.minValue = 0;
        _healthSlider.maxValue = maxHealth;
        _healthSlider.value = maxHealth;
    }
    public void Initialize(float maxHealth, float currentHealth)
    {
        _healthSlider.minValue = 0;
        _healthSlider.maxValue = maxHealth;
        _healthSlider.value = currentHealth;
    }

    public void OnDamage(DamageInfo damageInfo)
    {
        _healthSlider.value -= damageInfo.Value;
        _healthSlider.value = Mathf.Clamp(_healthSlider.value, _healthSlider.minValue, _healthSlider.maxValue);

        DamageUIController damageUIController = Instantiate(UIDamagePrefab, UIDamageParent);
        damageUIController.Initialize(damageInfo.Value, damageInfo.Element, damageInfo.EffectiveNess);
        damageUIController.transform.rotation = Quaternion.LookRotation(damageUIController.transform.position - Camera.main.transform.position);
    }

    public void OnDeath()
    {
        StartCoroutine(CO_FadeInScreen(_deathScreen, () => { }));
    }

    public void OnWin()
    {
        StartCoroutine(CO_FadeInScreen(_winScreen, () => { EventManager.OnExitLevel?.Invoke(_healthSlider.value); }));
    }

    public void OnHeal(float value)
    {
        _healthSlider.value += value;
        _healthSlider.value = Mathf.Clamp(_healthSlider.value, _healthSlider.minValue, _healthSlider.maxValue);
    }

    private IEnumerator CO_FadeInScreen(CanvasGroup canvasGroup, Action callback)
    {
        float currentTime = 0f;
        float duration = 1f;

        while (currentTime < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(0, 1, currentTime / duration);
            currentTime += Time.deltaTime;
            yield return null;
        }

        callback();
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
    public void PlayDash(float dashcombotime, int dashcombo)
    {
        _dashText.text = "Combinação corrida: " + dashcombo;
        //start a coroutine that uses dashcombotime to animate a slider or some shit
        StartCoroutine(AnimateDashSlider(dashcombotime));
    }
    private IEnumerator AnimateDashSlider(float dashcombotime)
    {
        _dashSliderObject.SetActive(true);

        _dashSlider.maxValue = dashcombotime;
        float currentTime = 0f;

        while (currentTime < dashcombotime)
        {
            _dashSliderObject.SetActive(true);
            _dashSlider.value = Mathf.Lerp(0, _dashSlider.maxValue, currentTime / dashcombotime);
            currentTime += Time.deltaTime;
            yield return null;
        }

        _dashSliderObject.SetActive(false);
    }
    private void OnDamage(float value, Element element, TypeEffectiveness effectiveness)
    {
        DamageUIController damageUIController = Instantiate(UIDamagePrefab, UIDamageParent);
        damageUIController.Initialize(value, element, effectiveness);
        damageUIController.transform.rotation = Quaternion.LookRotation(damageUIController.transform.position - Camera.main.transform.position);
    }
}
