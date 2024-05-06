using Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CinemachineFreeLook))]
public class CameraLook : MonoBehaviour
{
    private CinemachineFreeLook _CinemachineFreeLook;
    private CameraControl _CameraInput;

    private bool isDragging = false;
    private float xlookSpeed = 5;
    private float ylookSpeed = 0.5f;

    private Vector2 startPos;
    private float distance = 0.3f;
    private bool isDoneCheck = false;
    private bool isPointerOverUI = false;

    private void Awake()
    {
        _CameraInput = new CameraControl();
        _CinemachineFreeLook = GetComponent<CinemachineFreeLook>();
    }

    private void OnEnable()
    {
        _CameraInput.Enable();
        _CameraInput.Camera.Drag.started += OnDragStarted;
        _CameraInput.Camera.Drag.canceled += OnDragEnded;
    }

    private void OnDisable()
    {
        _CameraInput.Disable();
        _CameraInput.Camera.Drag.started -= OnDragStarted;
        _CameraInput.Camera.Drag.canceled -= OnDragEnded;
    }
    private void OnDragStarted(InputAction.CallbackContext context)
    {
        startPos = _CameraInput.Camera.Look.ReadValue<Vector2>();
        isDragging = true;
        isDoneCheck = false;
    }

    private void OnDragEnded(InputAction.CallbackContext context)
    {
        startPos = Vector2.zero;
        isDragging = false;
        isPointerOverUI = false;
    }

    private void CheckCameraDrag()
    {
        Vector2 dragPos = _CameraInput.Camera.Look.ReadValue<Vector2>();

        float dragDistance = Vector2.Distance(startPos, dragPos);

        if (distance < dragDistance)
        {
            isDragging = true;
            isPointerOverUI = EventSystem.current.currentSelectedGameObject != null ||
                                   EventSystem.current.IsPointerOverGameObject();
            isDoneCheck = true;
        }
    }

    private void LateUpdate()
    {
        if (!isDragging)
            return;
        
        if (!isDoneCheck)
        {
            CheckCameraDrag();
        }
        else
        {
            if (!isPointerOverUI)
            {
                Vector2 delta = _CameraInput.Camera.Look.ReadValue<Vector2>();
                _CinemachineFreeLook.m_XAxis.Value += delta.x * xlookSpeed * Time.deltaTime;
                _CinemachineFreeLook.m_YAxis.Value += delta.y * ylookSpeed * Time.deltaTime;
            }
        }
    }
}
