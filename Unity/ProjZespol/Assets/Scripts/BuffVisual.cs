using UnityEngine;
using UnityEngine.UIElements;

public class BuffVisual
{
    public string skillId;
    public VisualElement root;
    private Label valueLabel;

    public BuffVisual(string skillId, Texture2D icon)
    {
        this.skillId = skillId;

        root = new VisualElement();
        root.style.flexDirection = FlexDirection.Row;
        root.style.alignItems = Align.Center;
        root.style.backgroundColor = new Color(0, 0, 0, 0.65f);
        root.style.paddingLeft = 6;
        root.style.paddingRight = 6;
        root.style.paddingTop = 4;
        root.style.paddingBottom = 4;
        root.style.marginRight = 8;
        root.style.borderTopLeftRadius = 6;
        root.style.borderTopRightRadius = 6;
        root.style.borderBottomLeftRadius = 6;
        root.style.borderBottomRightRadius = 6;

        var image = new Image();
        image.image = icon;
        image.style.width = 64;
        image.style.height = 64;
        image.style.marginRight = 6;

        valueLabel = new Label("0");
        valueLabel.style.color = Color.white;
        valueLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
        valueLabel.style.width = 96;
        valueLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
        valueLabel.style.flexShrink = 0;

        root.Add(image);
        root.Add(valueLabel);

        root.style.display = DisplayStyle.None;
    }

    public void Show(double value)
    {
        valueLabel.text = value.ToString("0.##");
        root.style.display = DisplayStyle.Flex;
    }

    public void Hide()
    {
        root.style.display = DisplayStyle.None;
    }
}
