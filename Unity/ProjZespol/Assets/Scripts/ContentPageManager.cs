using UnityEngine;
using UnityEngine.UIElements;

public class ContentPageManager
{
    private VisualElement ui;
    private AudioSource audioSource;

    private Button btn1;
    private Button btn2;
    private Button btn3;
    private Button btn4;
    private Button btn5;
    private Button btn6;
    private Button btn7;

    private VisualElement contentPage1;
    private VisualElement contentPage2;
    private VisualElement contentPage3;
    private VisualElement contentPage4;
    private VisualElement contentPage5;
    private VisualElement contentPage6;
    private VisualElement contentPage7;

    public ContentPageManager(VisualElement root, AudioSource audio)
    {
        ui = root;
        audioSource = audio;
    }

    public void Initialize()
    {
        btn1 = ui.Q<Button>("btn");
        btn2 = ui.Q<Button>("btn2");
        btn3 = ui.Q<Button>("btn3");
        btn4 = ui.Q<Button>("btn4");
        btn5 = ui.Q<Button>("btn5");
        btn6 = ui.Q<Button>("btn6");
        btn7 = ui.Q<Button>("btn7");

        contentPage1 = ui.Q<VisualElement>("Content");
        contentPage2 = ui.Q<VisualElement>("Content2");
        contentPage3 = ui.Q<VisualElement>("Content3");
        contentPage4 = ui.Q<VisualElement>("Content4");
        contentPage5 = ui.Q<VisualElement>("Content5");
        contentPage6 = ui.Q<VisualElement>("Content6");
        contentPage7 = ui.Q<VisualElement>("Content7");

        btn1.clicked += () => ShowPage(1);
        btn2.clicked += () => ShowPage(2);
        btn3.clicked += () => ShowPage(3);
        btn4.clicked += () => ShowPage(4);
        btn5.clicked += () => ShowPage(5);
        btn6.clicked += () => ShowPage(6);
        btn7.clicked += () => ShowPage(7);
    }

    private void ShowPage(int pageNumber)
    {
        audioSource.Play();

        contentPage1.style.display = pageNumber == 1 ? DisplayStyle.Flex : DisplayStyle.None;
        contentPage2.style.display = pageNumber == 2 ? DisplayStyle.Flex : DisplayStyle.None;
        contentPage3.style.display = pageNumber == 3 ? DisplayStyle.Flex : DisplayStyle.None;
        contentPage4.style.display = pageNumber == 4 ? DisplayStyle.Flex : DisplayStyle.None;
        contentPage5.style.display = pageNumber == 5 ? DisplayStyle.Flex : DisplayStyle.None;
        contentPage6.style.display = pageNumber == 6 ? DisplayStyle.Flex : DisplayStyle.None;
        contentPage7.style.display = pageNumber == 7 ? DisplayStyle.Flex : DisplayStyle.None;
    }
}