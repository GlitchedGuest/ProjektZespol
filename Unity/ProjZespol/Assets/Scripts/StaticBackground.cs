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
        string nextFactory = raptorCore.ResourceManager.GetCurrentFactory()?.name;
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
            case "Astral Rock of Solitude":
                rawImage.texture = backgrounds[0];
                break;
            case "Fields of War":
                rawImage.texture = backgrounds[1];
                break;
            case "The Last Forest":
                rawImage.texture = backgrounds[2];
                break;
            case "F4":
                rawImage.texture = backgrounds[3];
                break;
            case "F5":
                rawImage.texture = backgrounds[4];
                break;
            case "F6":
                rawImage.texture = backgrounds[5];
                break;
        }
    }
}
