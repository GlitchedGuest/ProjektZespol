using UnityEngine;
using UnityEngine.UIElements.Experimental;
using UnityEngine.UIElements;

public class CritVisualGenerator : MonoBehaviour
{
    private VisualElement ui;
    private VisualElement critContainer;

    void Start()
    {
        ui = GetComponent<UIDocument>().rootVisualElement;
        critContainer = ui.Q<VisualElement>("crit-container");

    }


    public void SpawnCrit(Vector2 mouseScreenPos)
    {
        var crit = new Label("CRITICAL!");


        mouseScreenPos.y = Screen.height - mouseScreenPos.y-30;


        Vector2 panelPos = RuntimePanelUtils.ScreenToPanel(critContainer.panel, mouseScreenPos);

        crit.style.position = Position.Absolute;
        crit.style.left = panelPos.x;
        crit.style.top = panelPos.y;


        crit.style.unityFontStyleAndWeight = FontStyle.Bold;
        crit.style.fontSize = 40 + Random.Range(-4, 6);
        crit.style.rotate = new Rotate(new Angle(Random.Range(-30, 30), AngleUnit.Degree));
        crit.style.color = Color.red;
        crit.style.opacity = 1f;

        critContainer.Add(crit);

   
        float duration = 0.5f;
        float start = Time.time;

        crit.schedule.Execute(() =>
        {
            float t = (Time.time - start) / duration;
            crit.style.opacity = 1f - t;

            if (t >= 1f)
                crit.RemoveFromHierarchy();

        }).Every(16);
    }


}
