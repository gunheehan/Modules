using System;
using System.Collections.Generic;
using UnityEngine;

public class ScreenInteractable : MonoBehaviour
{
    public Action OnClickInteractable;

    public void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("OnClick Thumbting");

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                Collider collider = hit.collider;
                if (collider != null)
                {
                    Debug.Log("OnClick Player");
                    OnClickInteractable?.Invoke();
                }
            }
        }
    }
}
