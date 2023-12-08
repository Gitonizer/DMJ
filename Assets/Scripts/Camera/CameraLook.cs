using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLook : MonoBehaviour
{
    private GameObject _camera;
    private Transform _fakeCamera;
    private PlayerInputManager _inputManager;
    [SerializeField] private GameObject _virtualHead;

    void Awake()
    {
        _camera = Camera.main.gameObject;
        _fakeCamera = _virtualHead.transform.GetChild(0);
        _inputManager = GetComponent<PlayerInputManager>();
    }

    void Update()
    {
        transform.localRotation = Quaternion.Euler(0, _inputManager.MouseLook.x, 0);
        _virtualHead.transform.localRotation = Quaternion.Euler(-_inputManager.MouseLook.y, 0, 0);
        _camera.transform.SetPositionAndRotation(_fakeCamera.position, _fakeCamera.rotation);
    }
}
