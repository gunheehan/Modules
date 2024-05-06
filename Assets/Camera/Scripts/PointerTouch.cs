using UnityEngine;
using UnityEngine.AI;

public class PointerTouch : MonoBehaviour
{
    public Camera mainCamera;
    public NavMeshAgent playerAgent;

    private int mask;
    
    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        mask = LayerMask.GetMask("Enviroment");
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitPoint;

            if (Physics.Raycast(ray, out hitPoint))
            {
                Vector3 newTargetPoint = hitPoint.point;
                playerAgent.SetDestination(newTargetPoint);
            }
        }
    }
}
