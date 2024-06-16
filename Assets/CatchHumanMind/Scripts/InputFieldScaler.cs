using System.Collections;
using UnityEngine;

public class InputFieldScaler : MonoBehaviour
{
    private Transform transform;
    private bool isOpenState = false;
    
    void Start()
    {
        transform = gameObject.transform;
        transform.localScale = new Vector3(0, 1, 1);
    }

    public void SetInputFieldOpen(bool isOpen)
    {
        if (isOpenState == isOpen)
            return;
        isOpenState = isOpen;
        StartCoroutine(ScaleAnimationRoutine(isOpen));
    }

    private IEnumerator ScaleAnimationRoutine(bool isOpen)
    {
        float startScaleX = isOpen ? 0 : 1;
        float targetScaleX = isOpen ? 1 : 0;
        float duration = 1f;
        float elapsed = 0f;

        Vector3 currentScale = transform.localScale;
        currentScale.x = startScaleX;
        transform.localScale = currentScale;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float newX = Mathf.Lerp(startScaleX, targetScaleX, elapsed / duration);
            currentScale = transform.localScale;
            currentScale.x = newX;
            transform.localScale = currentScale;

            yield return null;
        }

        currentScale = transform.localScale;
        currentScale.x = targetScaleX;
        transform.localScale = currentScale;
    }
}
