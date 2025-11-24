using System;
using UnityEngine;
using UnityEngine.UIElements;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;
using static UnityEditor.PlayerSettings;

public static class Tooltip
{
    private static TemplateContainer root;
    private static VisualElement Frame;
    private static Label Tittle;
    private static Label Content;
    private static Label Lore;

    public static void Init(VisualElement uiRoot, VisualTreeAsset tooltipAsset)
    {

        root = tooltipAsset.Instantiate();
        
        Frame = root.Q<VisualElement>("Frame");
        Tittle = root.Q<Label>("Title");
        Content = root.Q<Label>("Content");
        Lore = root.Q<Label>("Lore");

        uiRoot.Add(root);

        root.contentContainer.style.position = new StyleEnum<Position>(Position.Absolute);
        root.contentContainer.style.width = new StyleLength(Screen.width);
        root.contentContainer.style.height = new StyleLength(Screen.height);
        root.contentContainer.style.left = new StyleLength(0.0f);
        root.pickingMode = PickingMode.Ignore;
        Hide();
    }

    public static void SizeRefresh(GeometryChangedEvent evt)
    {
        int newWidth = (int)evt.newRect.width;
        int newHeight = (int)evt.newRect.height;

        root.contentContainer.style.width = new StyleLength(newWidth);
        root.contentContainer.style.height = new StyleLength(newHeight);
    }

    public static void Show(string title,string content, Vector2 position, string lore = "Lorem ipsum dolor sit amet consectetur adipiscing elit. Consectetur adipiscing elit quisque faucibus ex sapien vitae. Ex sapien vitae pellentesque sem placerat in id. Placerat in id cursus mi pretium tellus duis. Pretium tellus duis convallis tempus leo eu aenean.")
    {
        Tittle.text = title;
        Content.text = content;
        Lore.text = lore;
        Frame.visible = true;
        Frame.style.width = StyleKeyword.Auto;
        Frame.style.height = StyleKeyword.Auto;
        
        Move(position);
    }


    public static void Move(Vector2 position)
    {
        float X_initial = position.x - (Frame.resolvedStyle.width * 0.5f);
        float X_clamped = Mathf.Min(X_initial, (Screen.width - Frame.resolvedStyle.width)*2);
        Frame.style.left = new StyleLength(X_clamped);
        Frame.style.top = new StyleLength(position.y + 10);
    }

    public static void Hide()
    {
        Frame.style.width = new StyleLength(0.0f);
        Frame.style.height = new StyleLength(0.0f);
        Frame.visible = false;
    }

    public static void Register(VisualElement element, string Title, string Content)
    {
        
        element.RegisterCallback<PointerEnterEvent>(evt =>
        {
            Show(Title,Content, evt.position);
        });

        element.RegisterCallback<PointerMoveEvent>(evt =>
        {
            Move(evt.position);
        });

        element.RegisterCallback<PointerLeaveEvent>(evt =>
        {
            Hide();
        });
    }
}
