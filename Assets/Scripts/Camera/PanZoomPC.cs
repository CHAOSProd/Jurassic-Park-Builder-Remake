using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PanZoomPC : MonoBehaviour
{
    [SerializeField] private float _timeToZoom = 0.5f;
    [SerializeField] private float _moveSpeed = 1f;
    [SerializeField] private float _moveLerpRate = 10f;
    [SerializeField] private float _minZoom = 5f;
    [SerializeField] private float _maxZoom = 7f;
    [SerializeField] private Vector2 _minPosition;
    [SerializeField] private Vector2 _maxPosition;

    private Camera _camera;
    private Controls _controls;
    private Vector2 _startPoint;
    private Vector3 _startCameraPosition; // Changed to Vector3 for clarity
    private float _zoomDelta;
    private bool _isZooming = false;

    private void Awake()
    {
        _controls = new Controls();
        _camera = Camera.main;
    }

    private void Start()
    {
        // Calculate the zoom step size
        _zoomDelta = (_maxZoom - _minZoom) / 2f;

        _controls.Enable();

        _controls.Main.MouseScroll.performed += OnZoom;
        _controls.Main.LeftClick.started += OnScrollButtonClick;
        _controls.Main.LeftClick.canceled += OnScrollButtonRelease;
    }

    private void OnZoom(InputAction.CallbackContext context)
    {
        if (_startPoint == Vector2.zero && !_isZooming)
        {
            float scrollDelta = context.ReadValue<float>();

            // Ignore negligible scroll deltas
            if (Mathf.Abs(scrollDelta) > 0.01f)
            {
                StartCoroutine(Zoom(_timeToZoom, scrollDelta));
            }
        }
    }

    private void OnScrollButtonClick(InputAction.CallbackContext context)
    {
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
        if (_startPoint == Vector2.zero) return;

        Vector2 point = _camera.ScreenToViewportPoint(Input.mousePosition);
        Vector2 offset = point - _startPoint;

        Vector3 newPosition = _startCameraPosition - (Vector3)(offset * _moveSpeed * (_camera.orthographicSize / 10f));
        newPosition.x = Mathf.Clamp(newPosition.x, _minPosition.x, _maxPosition.x);
        newPosition.y = Mathf.Clamp(newPosition.y, _minPosition.y, _maxPosition.y);

        transform.position = Vector3.Lerp(transform.position, newPosition, _moveLerpRate * Time.deltaTime);
        transform.position = new Vector3(transform.position.x, transform.position.y, -10f);
    }

    private IEnumerator Zoom(float time, float scrollDelta)
    {
        _isZooming = true;

        float elapsedTime = 0f;
        float startZoom = _camera.orthographicSize;
        float endZoom = Mathf.Clamp(startZoom - scrollDelta * _zoomDelta, _minZoom, _maxZoom);

        while (elapsedTime < time)
        {
            elapsedTime += Time.deltaTime;
            _camera.orthographicSize = Mathf.Lerp(startZoom, endZoom, elapsedTime / time);
            yield return null;
        }

        _camera.orthographicSize = endZoom;
        _isZooming = false;
    }
}


