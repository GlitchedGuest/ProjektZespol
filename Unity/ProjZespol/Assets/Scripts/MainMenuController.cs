using System.Collections.Generic;
using System.Linq;
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
    private Button DeleteSaveButton;
    private Toggle fullScreen;
    private Slider musicSlider;
    private Slider sfxSlider;
    private Toggle AutoSavetoggle;
    private DropdownField resolutionList;

    private Resolution[] resolutions;

    #endregion
    #region Unity Methods
    private void Awake()
    {
        resolutions = Screen.resolutions;
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

        fullScreen = settingsContainer.Q<Toggle>("FullScreen");
        resolutionList = settingsContainer.Q<DropdownField>("Res");

        startButton.clicked += OnStartButtonClicked;
        startButton.clicked += OnClick;
        settingsButton.clicked += OnSettingsButtonClicked;
        settingsButton.clicked += OnClick;
        quitButton.clicked += OnExitButtonClicked;
        quitButton.clicked += OnClick;
        backButton.clicked += OnBackButtonClicked;
        backButton.clicked += OnClick;

        //Delete button and autosave function overload
        DeleteSaveButton = settingsContainer.Q<Button>("DeleteSaveButton");
        AutoSavetoggle = settingsContainer.Q<Toggle>("AutoSavetoggle");

        DeleteSaveButton.clicked += AutoSaveSystem.DeleteSave;
        DeleteSaveButton.clicked += OnClick;
        AutoSavetoggle.value = AutoSaveSystem.AutoSave;
        AutoSavetoggle.RegisterValueChangedCallback(evt =>
        {
            AutoSaveSystem.AutoSave = evt.newValue;
            OnClick();
        });



        musicSlider.RegisterValueChangedCallback(OnMusicVolumeChanged);
        sfxSlider.RegisterValueChangedCallback(OnSfxVolumeChanged);

        fullScreen.RegisterValueChangedCallback(evt =>
        {
            SetFullScreen(evt.newValue);
        });

        resolutionList.choices.Clear();
        int currentRes = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = $"{resolutions[i].width} x {resolutions[i].height}";

            if (Screen.width == resolutions[i].width &&
                Screen.height == resolutions[i].height)
            {
                currentRes = i;
            }

            resolutionList.choices.Add(option);
        }

        if (resolutionList.choices.Count > 0)
        {
            resolutionList.value = resolutionList.choices[currentRes];
        }

        resolutionList.RegisterValueChangedCallback(evt =>
        {
            int index = resolutionList.choices.IndexOf(evt.newValue);
            SetResolution(index);
        });

        fullScreen.value = Screen.fullScreen;


        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
        sfxSlider.value = PlayerPrefs.GetFloat("SfxVolume", 1f);

        
        musicSource.volume = musicSlider.value;
        Debug.Log("wartosc w manu: "+ sfxSlider.value);
        sfxSource.volume = sfxSlider.value;
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
        PlayerPrefs.SetFloat("MusicVolume", evt.newValue);
        PlayerPrefs.Save(); 
        Debug.Log($"Głośność muzyki: {evt.newValue}");
    }

    private void OnSfxVolumeChanged(ChangeEvent<float> evt)
    {
        sfxSource.volume = evt.newValue;
        PlayerPrefs.SetFloat("SfxVolume", evt.newValue); 
        PlayerPrefs.Save();
        Debug.Log($"Głośność sfx: {evt.newValue}");
    }

    private void SetResolution(int resIndex)
    {
        Resolution resolution = resolutions[resIndex];
        Screen.SetResolution(resolution.width,resolution.height,Screen.fullScreen);

    }
    private void SetFullScreen(bool flag)
    {
        Screen.fullScreen = flag;
    }
    #endregion
}
