using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLook : MonoBehaviour
{
    private GameObject _camera;
    private Transform _fakeCamera;
    private PlayerInputManager _inputManager;
    [SerializeField] private GameObject _virtualHead;

    private bool _dialogOpen;

    private void OnEnable()
    {
        EventManager.OnOpenDialog += OnOpenDialog;
        EventManager.OnCloseDialog += OnCloseDialog;
    }
    private void OnDisable()
    {
        EventManager.OnOpenDialog -= OnOpenDialog;
        EventManager.OnCloseDialog -= OnCloseDialog;
    }

    void Awake()
    {
        _camera = Camera.main.gameObject;
        _fakeCamera = _virtualHead.transform.GetChild(0);
        _inputManager = GetComponent<PlayerInputManager>();
    }

    void Update()
    {
        if (_dialogOpen)
            return;

        transform.localRotation = Quaternion.Euler(0, _inputManager.MouseLook.x, 0);
        _virtualHead.transform.localRotation = Quaternion.Euler(-_inputManager.MouseLook.y, 0, 0);
        _camera.transform.SetPositionAndRotation(_fakeCamera.position, _fakeCamera.rotation);
    }

    private void OnOpenDialog(StoryActor actor)
    {
        StopAllCoroutines();
        StartCoroutine(MoveCamera(_camera.transform, actor.FakeCamera.transform, () => { }));
        _dialogOpen = true;
    }

    private void OnCloseDialog()
    {
        StopAllCoroutines();
        StartCoroutine(MoveCamera(_camera.transform, _fakeCamera, () => _dialogOpen = false));
    }

    private IEnumerator MoveCamera(Transform initialTransform, Transform finalTransform, Action doAction)
    {
        float duration = 1f;
        float currentTime = 0f;
        Vector3 currentPosition;
        Quaternion currentRotation;

        while (currentTime < duration)
        {
            currentPosition = Vector3.Lerp(initialTransform.position, finalTransform.position, currentTime / duration);
            currentRotation = Quaternion.Lerp(initialTransform.rotation, finalTransform.rotation, currentTime / duration);

            _camera.transform.SetPositionAndRotation(currentPosition, currentRotation);

            currentTime += Time.deltaTime;

            yield return null;
        }

        doAction();
    }
}
