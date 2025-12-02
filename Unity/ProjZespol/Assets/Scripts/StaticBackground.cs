using UnityEngine;
using UnityEngine.UI;

public class StaticBackground : MonoBehaviour
{
    [SerializeField] private Texture2D[] backgrounds;
    [SerializeField] private RaptorCore raptorCore;
    private string currentFactory;
    private RawImage rawImage;
    private RectTransform rectTransform;

    void Start()
    {
        rawImage = GetComponent<RawImage>();
        rectTransform = GetComponent<RectTransform>();
        rectTransform.localScale = Vector3.one;
        UpdateBackground();
    }

    void Update()
    {
        string nextFactory = raptorCore.GetCurrentFactory()?.name;
        if(nextFactory != currentFactory)
        {
            currentFactory = nextFactory;
            UpdateBackground();
        }
    }

    private void UpdateBackground()
    {
        if (string.IsNullOrEmpty(currentFactory)) return;
        switch (currentFactory)
        {
            case "F1":
                rawImage.texture = backgrounds[0];
                break;
            case "F2":
                rawImage.texture = backgrounds[1];
                break;
            case "F3":
                rawImage.texture = backgrounds[2];
                break;
        }
    }
}
