using System;
using System.Collections.Generic;
using UnityEngine;

public class ScreenInteractable : MonoBehaviour
{
    public Action OnClickInteractable;
    private List<int> layerMask = new List<int>();
    private int combineLayerMask = 0;

    public void SetLayerMask(string layerIndex)
    {
        int layer = LayerMask.NameToLayer(layerIndex);
        layerMask.Add(layer);

        combineLayerMask = layer;

        if (layerMask.Count < 2)
            return;

        combineLayerMask = 0;

        foreach (int layerMask in layerMask)
        {
            combineLayerMask |= layerMask;
        }
    }
    
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
