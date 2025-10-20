using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DynamicBackground : MonoBehaviour
{
    #region Variables
    [Header("Ustawienia")]
    [SerializeField] private Texture2D[] backgrounds;
    [SerializeField] private float changeInterval = 5f;
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private bool enableZoom = true;
    [SerializeField] private float zoomValue = 0.03f;
    [SerializeField] private float maxZoomValue = 3f;
    private RawImage rawImage;
    private RectTransform rectTransform;
    private int currentIndex;
    #endregion

    #region Methods
    private void Start()
    {
        rawImage = GetComponent<RawImage>();
        rectTransform = GetComponent<RectTransform>();

        if (backgrounds == null || backgrounds.Length == 0)
        {
            Debug.LogError("Brak backgroundów do wyświetlenia.");
            return;
        }

        currentIndex = Random.Range(0, backgrounds.Length);
        rawImage.texture = backgrounds[currentIndex];
        rawImage.color = Color.white;

        StartCoroutine(ChangeBackgroundRoutine());
        if (enableZoom)
        {
            StartCoroutine(ZoomEffect());
        }
    }

    private IEnumerator ChangeBackgroundRoutine()
    {
        while(true)
        {
            yield return new WaitForSeconds(changeInterval);
            int nextIndex;
            do
            {
                nextIndex = Random.Range(0, backgrounds.Length);
            } while (nextIndex == currentIndex);

            Texture2D nextTexture = backgrounds[nextIndex];
            yield return StartCoroutine(ChangeBackground(nextTexture));
            currentIndex = nextIndex;
        }
    }

    private IEnumerator ChangeBackground(Texture2D backgroundImage)
    {
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            float alpha = 1 - (t / fadeDuration);
            rawImage.color = new Color(1, 1, 1, alpha);
            yield return null;
        }

        rawImage.texture = backgroundImage;
        rectTransform.localScale = Vector3.one;

        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            float alpha = t / fadeDuration;
            rawImage.color = new Color(1, 1, 1, alpha);
            yield return null;
        }

        rawImage.color = Color.white;
    }

    private IEnumerator ZoomEffect()
    {
        while (true)
        {
            rectTransform.localScale += Vector3.one * (zoomValue * Time.deltaTime);
            if (rectTransform.localScale.x >= maxZoomValue)
            {
                rectTransform.localScale = Vector3.one;
            }
            yield return null;
        }
    }
    #endregion
}
