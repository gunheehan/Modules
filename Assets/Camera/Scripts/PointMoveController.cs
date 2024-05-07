using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class PointMoveController : MonoBehaviour
{
    public event Action<Vector3> OnMovePointUpdate = null;
    [SerializeField] private GameObject pointerPrefab;
    private Camera mainCamera;
    
    public NavMeshAgent playerAgent;
    
    private int layerMask;
    
    void Start()
    {
        mainCamera = Camera.main;
        layerMask = LayerMask.GetMask("Enviroment");
    }
    

    public void CheckPointMove(Vector2 input)
    {
        Debug.Log(input);
        if (CheckOnTouchUI())
            return;
        
        Ray ray = mainCamera.ScreenPointToRay(input);
        RaycastHit hitPoint;

        if (Physics.Raycast(ray, out hitPoint))
        {
            Vector3 newTargetPoint = hitPoint.point;
            playerAgent.SetDestination(newTargetPoint);
        }
    }

    private bool CheckOnTouchUI()
    {
        bool isTouchUI = EventSystem.current.currentSelectedGameObject != null ||
                          EventSystem.current.IsPointerOverGameObject();

        return isTouchUI;
    }
}
