using UnityEngine;
using UnityEngine.UI;

public class PlayerPreView : MonoBehaviour
{
    [SerializeField] private Camera camera;
    [SerializeField] private RawImage viewImage;

    private Vector3 offset;

    private void Start()
    {
        offset = new Vector3(0, .5f, 0);
    }

    public void SetPreview(GameObject model)
    {
        model.transform.position = gameObject.transform.position;
        RenderTexture renderTexture = new RenderTexture((int)viewImage.rectTransform.rect.width,
            (int)viewImage.rectTransform.rect.height, 24);
        renderTexture.antiAliasing = 8;
        renderTexture.useMipMap = false;
        renderTexture.format = RenderTextureFormat.DefaultHDR;
        camera.targetTexture = renderTexture;
        viewImage.texture = renderTexture;
        
        camera.transform.position += (Vector3.back * 1.5f) + offset;
        camera.transform.LookAt(model.transform);
        camera.transform.rotation = Quaternion.identity;
    }
}
