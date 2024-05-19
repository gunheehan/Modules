using Cinemachine;
using UnityEngine;

public class Cameras : MonoBehaviour
{
    [SerializeField] private Camera uiCamera;
    public Camera UICamera => uiCamera;

    [SerializeField] private CinemachineFreeLook freeLookCamera;
    public CinemachineFreeLook FreeLookCamera => freeLookCamera;
}
