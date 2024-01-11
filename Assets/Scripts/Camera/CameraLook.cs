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

    private bool _cameraTransitioning;

    private void OnEnable()
    {
        EventManager.OnOpenDialog += OnOpenDialog;
        EventManager.OnCloseDialog += OnCloseDialog;
        EventManager.OnOpenDoor += OnOpenDoor;
        EventManager.OnDoorOpened += OnDoorOpened;
    }
    private void OnDisable()
    {
        EventManager.OnOpenDialog -= OnOpenDialog;
        EventManager.OnCloseDialog -= OnCloseDialog;
        EventManager.OnOpenDoor -= OnOpenDoor;
        EventManager.OnDoorOpened -= OnDoorOpened;
    }

    void Awake()
    {
        _camera = Camera.main.gameObject;
        _fakeCamera = _virtualHead.transform.GetChild(0);
        _inputManager = GetComponent<PlayerInputManager>();
    }

    void Update()
    {
        if (_cameraTransitioning)
            return;

        transform.localRotation = Quaternion.Euler(0, _inputManager.MouseLook.x, 0);
        _virtualHead.transform.localRotation = Quaternion.Euler(-_inputManager.MouseLook.y, 0, 0);
        _camera.transform.SetPositionAndRotation(_fakeCamera.position, _fakeCamera.rotation);
    }

    private void OnOpenDialog(StoryActor actor)
    {
        StopAllCoroutines();
        StartCoroutine(MoveCamera(_camera.transform, actor.FakeCamera.transform, () => { }));
        _cameraTransitioning = true;
    }

    private void OnCloseDialog()
    {
        StopAllCoroutines();
        StartCoroutine(MoveCamera(_camera.transform, _fakeCamera, () => _cameraTransitioning = false));
    }
    private void OnOpenDoor(Door door)
    {
        StopAllCoroutines();
        StartCoroutine(MoveCamera(_camera.transform, door.FakeCamera.transform, () => { }));
        _cameraTransitioning = true;
    }

    private void OnDoorOpened()
    {
        StopAllCoroutines();
        StartCoroutine(MoveCamera(_camera.transform, _fakeCamera, () => _cameraTransitioning = false));
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
