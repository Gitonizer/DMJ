using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingUI : MonoBehaviour
{
    [SerializeField] private Text[] _levelsText;
    [SerializeField] private GameObject _playerPic;

    private Canvas _canvas;
    private CanvasGroup _canvasGroup;

    private float _speed;
    private Vector3 _initialPosition;

    private void Awake()
    {
        _speed = 130f;
        _initialPosition = _playerPic.transform.position;
        _canvas = GetComponent<Canvas>();
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Update()
    {
        if (_playerPic.transform.position.y > _initialPosition.y + 100)
            _playerPic.transform.position = _initialPosition;

        _playerPic.transform.position += _speed * Time.deltaTime * Vector3.up;
    }

    public void Initialize(int currentLevel)
    {
        currentLevel--;

        foreach (var levelText in _levelsText)
        {
            levelText.text = currentLevel.ToString();
            currentLevel++;
        }
    }
    public IEnumerator Hide()
    {
        yield return new WaitForSeconds(1f);

        float currentTime = 0f;
        float duration = 1f;

        while (currentTime < duration)
        {
            _canvasGroup.alpha = Mathf.Lerp(1, 0, currentTime / duration);
            currentTime += Time.deltaTime;
            yield return null;
        }

        _canvas.enabled = false;
    }
}
