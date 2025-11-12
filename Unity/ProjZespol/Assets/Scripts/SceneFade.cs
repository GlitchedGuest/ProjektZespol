using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public class SceneFade : MonoBehaviour
{
    private Image sceneImage;
    private RectTransform rectTransform;
    [SerializeField] private RectTransform backgroundTransform;
    private void Awake()
    {
        sceneImage = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
    }

    private void LateUpdate()
    {
        if (backgroundTransform != null && gameObject.activeSelf)
        {
            rectTransform.localScale = backgroundTransform.localScale;
            rectTransform.sizeDelta = backgroundTransform.sizeDelta;
        }
    }

    public IEnumerator FadeInCoroutine(float duration)
    {
        Color startColor = new Color(sceneImage.color.r, sceneImage.color.g, sceneImage.color.b, 1);
        Color targetColor = new Color(sceneImage.color.r, sceneImage.color.g, sceneImage.color.b, 0);

        yield return FadeCoroutine(startColor, targetColor, duration);
        gameObject.SetActive(false);
    }
    public IEnumerator FadeOutCoroutine(float duration)
    {
        Color startColor = new Color(sceneImage.color.r, sceneImage.color.g, sceneImage.color.b, 0);
        Color targetColor = new Color(sceneImage.color.r, sceneImage.color.g, sceneImage.color.b, 1);
        gameObject.SetActive(true);
        yield return FadeCoroutine(startColor, targetColor, duration);
    }
    private IEnumerator FadeCoroutine(Color startColor, Color targetColor, float duration)
    {
        float time = 0;
        float percentage = 0;

        while (percentage < 1)
        {
            percentage = time / duration;
            sceneImage.color = Color.Lerp(startColor, targetColor, percentage);
            yield return null;
            time += Time.deltaTime; 
        }
        
    }
}
