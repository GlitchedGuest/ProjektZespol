using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
public class MainMenuController : MonoBehaviour
{
    #region Varibles
    private VisualElement mainMenu;
    private VisualElement mainContainer;
    private VisualElement settingsContainer;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private SceneController sceneController;
    private Button startButton;
    private Button settingsButton;
    private Button quitButton;
    private Button backButton;
    private Slider musicSlider;
    private Slider sfxSlider;

    #endregion
    #region Unity Methods
    private void Awake()
    {
        mainMenu = GetComponent<UIDocument>().rootVisualElement;
    }
    private void OnEnable()
    {
        mainContainer = mainMenu.Q<VisualElement>("Main");
        settingsContainer = mainMenu.Q<VisualElement>("Settings");

        startButton = mainContainer.Q<Button>("StartButton");
        settingsButton = mainContainer.Q<Button>("SettingsButton");
        quitButton = mainContainer.Q<Button>("QuitButton");
        backButton = settingsContainer.Q<Button>("BackButton");
        musicSlider = settingsContainer.Q<Slider>("MusicSlider");
        sfxSlider = settingsContainer.Q<Slider>("SFXSlider");

        startButton.clicked += OnStartButtonClicked;
        startButton.clicked += OnClick;
        settingsButton.clicked += OnSettingsButtonClicked;
        settingsButton.clicked += OnClick;
        quitButton.clicked += OnExitButtonClicked;
        quitButton.clicked += OnClick;
        backButton.clicked += OnBackButtonClicked;
        backButton.clicked += OnClick;

        musicSlider.RegisterValueChangedCallback(OnMusicVolumeChanged);
        sfxSlider.RegisterValueChangedCallback(OnSfxVolumeChanged);
    }
    #endregion
    #region Button And Slider Methods
    private void OnClick()
    {
        sfxSource.Play();
        sfxSource.pitch = Random.Range(0.8f, 1.2f);
    }
    private void OnStartButtonClicked()
    {
        sceneController.LoadScene("SampleScene");
        Debug.Log("Start");
    }
    private void OnSettingsButtonClicked()
    {
        mainContainer.AddToClassList("hidden");
        settingsContainer.RemoveFromClassList("hidden");
        Debug.Log("Ustawienia");
    }
    private void OnBackButtonClicked()
    {
        settingsContainer.AddToClassList("hidden");
        mainContainer.RemoveFromClassList("hidden");
        Debug.Log("Ustawienia");
    }

    private void OnExitButtonClicked()
    {
        Debug.Log("Wyjście z gry");
        Application.Quit();
    }

    private void OnMusicVolumeChanged(ChangeEvent<float> evt)
    {
        musicSource.volume = evt.newValue;
        Debug.Log($"Głośność muzyki: {evt.newValue}");
    }

    private void OnSfxVolumeChanged(ChangeEvent<float> evt)
    {
        sfxSource.volume = evt.newValue;
        Debug.Log($"Głośność sfx: {evt.newValue}");
    }
    #endregion
}
