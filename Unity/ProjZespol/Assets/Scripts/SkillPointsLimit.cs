using UnityEngine;

using UnityEngine.UIElements;

public class SkillPointsLimit : MonoBehaviour
{
    private VisualElement critContainer;
    private Button Skillbtn;
    public VisualElement ui;
    [SerializeField] private CharacterClass characterClass;
    private Color availableColor;
    private Color unavailableColor;

    private void Awake()
    {
        ui = GetComponent<UIDocument>().rootVisualElement;
        Skillbtn = ui.Q<Button>("btnSkill");
        ColorUtility.TryParseHtmlString("#C8B054", out availableColor);
        ColorUtility.TryParseHtmlString("#222222", out unavailableColor);
        unavailableColor.a = 0f;
        critContainer = ui.Q<VisualElement>("crit-container");
    }

    public void UpdateButton()
    {
        Skillbtn.text = "Skill Points:\r\n" + characterClass.skillpoints;
        if (characterClass.skillpoints > 0)
        {
            Skillbtn.style.backgroundColor = availableColor;
        }
        else
        {
            Skillbtn.style.backgroundColor = unavailableColor;
        }
    }
    public void DecSkillPoints()
    {
        characterClass.skillpoints--;
        UpdateButton();
    }
    public void IncSkillPoints()
    {
        characterClass.skillpoints++;
        UpdateButton();
    }
}
