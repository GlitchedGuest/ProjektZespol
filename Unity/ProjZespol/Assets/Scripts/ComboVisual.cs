using UnityEngine;
using UnityEngine.UIElements;

public class ComboVisual : MonoBehaviour
{
    private VisualElement ui;
    private VisualElement comboContainer;
    private Label comboLabel;

    private bool isVisible = false;

    void Start()
    {
        ui = GetComponent<UIDocument>().rootVisualElement;
        comboContainer = ui.Q<VisualElement>("combo-container");
        comboLabel = comboContainer.Q<Label>("combolabel");

        // Pocz¹tkowe wartoœci (zgodne z USS)
        comboContainer.style.opacity = 0f;
        comboContainer.style.scale = new Scale(new Vector2(0.9f, 0.9f));

        // Ustawiamy display dopiero gdy trzeba
        comboContainer.style.display = DisplayStyle.None;
    }

    public void UpdateCombo(int acKombo)
    {
        if (acKombo > 0)
        {
            comboLabel.text = "Combo x" + acKombo.ToString();

            if (!isVisible)
            {
                isVisible = true;

                comboContainer.style.display = DisplayStyle.Flex;

                comboContainer.style.opacity = 1f;
                comboContainer.style.scale = new Scale(new Vector2(1f, 1f));
            }
        }
        else
        {
            if (isVisible)
            {
                isVisible = false;

                comboContainer.style.opacity = 0f;
                comboContainer.style.scale = new Scale(new Vector2(0.9f, 0.9f));

                // Po zakoñczeniu animacji ukrywamy element
                comboContainer.RegisterCallback<TransitionEndEvent>((evt) =>
                {
                    if (!isVisible)
                        comboContainer.style.display = DisplayStyle.None;
                });
            }
        }
    }
}
