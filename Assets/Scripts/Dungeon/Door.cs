using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    [SerializeField] private bool _isInteractable;

    public void Initialize(bool isInteractable)
    {
        _isInteractable = isInteractable;
    }
    public void Interact()
    {
        if (!_isInteractable)
            return;

        EventManager.OnOpenDoor?.Invoke(this);
    }

    public void Open()
    {
        StartCoroutine(AnimateOpen());
    }

    private IEnumerator AnimateOpen()
    {
        float duration = 2;
        float currentTime = 0;
        Vector3 initialPosition = transform.position;
        Vector3 targetPosition = new Vector3(transform.position.x, transform.position.y - 20, transform.position.z);

        while(currentTime < duration)
        {
            transform.position = Vector3.Lerp(initialPosition, targetPosition, currentTime / duration);
            currentTime += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }
}
