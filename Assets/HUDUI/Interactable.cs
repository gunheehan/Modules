using System;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(SphereCollider))]
public class Interactable : MonoBehaviour, IPointerDownHandler, IPointerClickHandler
{
    public Action OnTriggerAction = null;
    [SerializeField] private float radius = .5f;
    private ScreenInteractable screenInteractable;
    private SphereCollider collider;
    
    void Start()
    {
        screenInteractable = new ScreenInteractable();
        collider = GetComponent<SphereCollider>();
        collider.isTrigger = true;
        collider.radius = radius;
    }

    private void OnDrawGizmos()
    {
        Color color = Color.blue;
        color.a = .5f;
        Gizmos.color = color;
        Gizmos.DrawSphere(transform.position, radius);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("OnClick HUDDisPlay");
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("OnEnter Trigger HUDDisPlay");
        OnTriggerAction?.Invoke();
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("OnExit Trigger HUDDisPlay");
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("OnClickPointer HUDDisPlay");
    }
}
