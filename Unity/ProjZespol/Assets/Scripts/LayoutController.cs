using UnityEngine;
using UnityEngine.UIElements;

public class LayoutController : MonoBehaviour
{
    public VisualElement ui;
    public Button btn1;
    public Button btn2;
    public Button btn3;
    public Label text;
    [SerializeField] private AudioSource audioSource;
    void Awake()
    {
        ui = GetComponent<UIDocument>().rootVisualElement;
        
    }

    private void OnEnable()
    {

        text = ui.Q<Label>("Napis");

        btn1 = ui.Q<Button>("btn");
        btn1.clicked += ClickBtn1;
        btn2 = ui.Q<Button>("btn2");
        btn2.clicked += ClickBtn2;
        btn3 = ui.Q<Button>("btn3");
        btn3.clicked += ClickBtn3;
    }

    private void ClickBtn1()
    {
        audioSource.Play();
        text.text = "elo";
    }
    private void ClickBtn2()
    {
        audioSource.Play();
        text.text = "¿elo";
    }
    private void ClickBtn3()
    {
        audioSource.Play();
        text.text = ":)";
    }

}
