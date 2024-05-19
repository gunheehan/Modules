using UnityEngine;
using UnityEngine.AddressableAssets;

public class BaseScene : MonoBehaviour
{
    [SerializeField] private Cameras cameras;
    [SerializeField] private Transform spawnPos;
    void Start()
    {
        InitPlayer();
    }

    private void InitPlayer()
    {
        if (spawnPos != null)
            Addressables.InstantiateAsync("Model",spawnPos).Completed += OnPlayerInstantiated;
        else
            Addressables.InstantiateAsync("Model").Completed += OnPlayerInstantiated;
    }

    private void OnPlayerInstantiated(UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<GameObject> handle)
    {
        if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
        {
            SetCamera(handle.Result);
            SetPlayerUI(handle.Result);
        }
        else
        {
            Debug.LogError("Failed to load the player model.");
        }
    }

    private void SetCamera(GameObject player)
    {
        if (cameras == null)
        {
            Debug.Log("Camera Component Null");
            return;
        }
        
        cameras.FreeLookCamera.Follow = player.transform;
        cameras.FreeLookCamera.LookAt = player.transform;
    }

    private void SetPlayerUI(GameObject player)
    {
        
    }
}
