using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PanZoomPC : MonoBehaviour
{
    [SerializeField] private float _zoomSmoothTime = 0.2f; // How quickly zoom converges to the target
    [SerializeField] private float _moveSpeed = 1f;
    [SerializeField] private float _moveLerpRate = 10f;
    [SerializeField] private float _minZoom = 5f;
    [SerializeField] private float _maxZoom = 7f;
    [SerializeField] private Vector2 _minPosition;
    [SerializeField] private Vector2 _maxPosition;
    [SerializeField] private GameObject[] _uiElements; // Array of UI elements

    private Camera _camera;
    private Controls _controls;
    private Vector2 _startPoint;
    private Vector3 _startCameraPosition;
    private float _zoomDelta;
    private float _targetZoom;
    private float _zoomVelocity = 0f;

    private void Awake()
    {
        _controls = new Controls();
        _camera = Camera.main;
    }

    private void Start()
    {
        _targetZoom = _camera.orthographicSize;
        _zoomDelta = (_maxZoom - _minZoom) / 2f;

        _controls.Enable();
        _controls.Main.MouseScroll.performed += OnZoom;
        _controls.Main.LeftClick.started += OnScrollButtonClick;
        _controls.Main.LeftClick.canceled += OnScrollButtonRelease;
    }

    private bool IsUIActive()
    {
        foreach (var uiElement in _uiElements)
        {
            if (uiElement.activeInHierarchy)
                return true;
        }
        return false;
    }

    private void OnZoom(InputAction.CallbackContext context)
    {
        if (IsUIActive() || _startPoint != Vector2.zero)
            return;

        float scrollDelta = context.ReadValue<float>();
        if (Mathf.Abs(scrollDelta) > 0.01f)
        {
            _targetZoom = Mathf.Clamp(_targetZoom - scrollDelta * _zoomDelta, _minZoom, _maxZoom);
        }
    }

    private void OnScrollButtonClick(InputAction.CallbackContext context)
    {
        if (IsUIActive())
           return;

        Vector2 point = _camera.ScreenToViewportPoint(Input.mousePosition);
        _startPoint = point;
        _startCameraPosition = _camera.transform.position;
    }

    private void OnScrollButtonRelease(InputAction.CallbackContext context)
    {
        _startPoint = Vector2.zero;
        _startCameraPosition = Vector3.zero;
    }

    private void Update()
    {
        // Smoothly update the camera's zoom level
        if (!IsUIActive())
        {
            _camera.orthographicSize = Mathf.SmoothDamp(_camera.orthographicSize, _targetZoom, ref _zoomVelocity, _zoomSmoothTime);
        }

        // Handle panning
        if (IsUIActive() || _startPoint == Vector2.zero)
            return;

        Vector2 point = _camera.ScreenToViewportPoint(Input.mousePosition);
        Vector2 offset = point - _startPoint;

        Vector3 newPosition = _startCameraPosition - (Vector3)(offset * _moveSpeed * (_camera.orthographicSize / 10f));
        newPosition.x = Mathf.Clamp(newPosition.x, _minPosition.x, _maxPosition.x);
        newPosition.y = Mathf.Clamp(newPosition.y, _minPosition.y, _maxPosition.y);

        transform.position = Vector3.Lerp(transform.position, newPosition, _moveLerpRate * Time.deltaTime);
        transform.position = new Vector3(transform.position.x, transform.position.y, -10f);
    }
}




