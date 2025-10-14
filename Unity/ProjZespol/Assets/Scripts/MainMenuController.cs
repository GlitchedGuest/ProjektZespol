using UnityEngine;
using UnityEngine.UIElements;
public class MainMenuController : MonoBehaviour
{
    private VisualElement mainMenu;
    [SerializeField]private AudioSource audioSource;
    private Button startButton;
    private Button settingsButton;
    private Button quitButton;

    private void Awake()
    {
        mainMenu = GetComponent<UIDocument>().rootVisualElement;
    }
    private void OnEnable()
    {
        startButton = mainMenu.Q<Button>("StartButton");
        settingsButton = mainMenu.Q<Button>("SettingsButton");
        quitButton = mainMenu.Q<Button>("QuitButton");

        startButton.clicked += OnStartButtonClicked;
        startButton.clicked += OnClick;
        settingsButton.clicked += OnSettingsButtonClicked;
        settingsButton.clicked += OnClick;
        quitButton.clicked += OnExitButtonClicked;
        quitButton.clicked += OnClick;
    }
    private void OnClick()
    {
        audioSource.Play();
        audioSource.pitch = Random.Range(0.8f, 1.2f);
    }
    private void OnStartButtonClicked()
    {
        Debug.Log("Start");
    }
    private void OnSettingsButtonClicked()
    {
        Debug.Log("Ustawienia");
    }
    private void OnExitButtonClicked()
    {
        Debug.Log("Wyjście z gry");
        Application.Quit();
    }
}
