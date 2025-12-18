using UnityEngine;
using UnityEngine.UIElements;

public class OptionsManager
{
    private VisualElement ui;
    private AudioSource audioSource;

    private VisualElement menu;
    private VisualElement options;
    private VisualElement mainMenu;
    private Button optionBtn;
    private Button resume;
    private Button settings;
    private Button exit;
    private Button backSettings;

    private Slider musicSlider;
    private Slider sfxSlider;
    private Toggle fullScreen;
    private DropdownField resolutionList;
    private Resolution[] resolutions;

    private Button deleteSaveButton;
    private Toggle autoSaveToggle;

    private AudioSource critSource;

    public OptionsManager(VisualElement root, AudioSource audio, AudioSource crit)
    {
        ui = root;
        audioSource = audio;
        critSource = crit;
        resolutions = Screen.resolutions;
    }

    public void Initialize()
    {
        menu = ui.Q<VisualElement>("Menu");
        mainMenu = ui.Q<VisualElement>("Main");
        options = ui.Q<VisualElement>("Options");
        //optionBtn = ui.Q<Button>("Optionbtn");
        resume = ui.Q<Button>("Resume");
        settings = ui.Q<Button>("Settings");
        exit = ui.Q<Button>("Exit");
        backSettings = ui.Q<Button>("Back");

        //optionBtn.clicked += EnableOptions;
        resume.clicked += ResumeGame;
        settings.clicked += LoadSettings;
        exit.clicked += ExitGame;
        backSettings.clicked += ReturnSettings;

        InitializeAudioSettings();
        InitializeDisplaySettings();
        InitializeSaveSettings();
    }


    private void InitializeAudioSettings()
    {
        musicSlider = ui.Q<Slider>("Music");
        sfxSlider = ui.Q<Slider>("Sfx");

        sfxSlider.RegisterValueChangedCallback(OnSfxVolumeChanged);
        musicSlider.RegisterValueChangedCallback(OnMusicVolumeChanged);

        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
        sfxSlider.value = PlayerPrefs.GetFloat("SfxVolume", 1f);

        audioSource.volume = sfxSlider.value;
        critSource.volume = sfxSlider.value;
    }

    private void InitializeDisplaySettings()
    {
        fullScreen = ui.Q<Toggle>("Fullscreen");
        resolutionList = ui.Q<DropdownField>("Res");

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
    }

    private void InitializeSaveSettings()
    {
        deleteSaveButton = ui.Q<Button>("DeleteSaveButton");
        autoSaveToggle = ui.Q<Toggle>("AutoSavetoggle");

        deleteSaveButton.clicked += AutoSaveSystem.DeleteSave;
        deleteSaveButton.clicked += () => { audioSource.Play(); };
        autoSaveToggle.value = AutoSaveSystem.AutoSave;
        autoSaveToggle.RegisterValueChangedCallback(evt =>
        {
            AutoSaveSystem.AutoSave = evt.newValue;
            audioSource.Play();
        });
    }

    public void EnableOptions()
    {
        audioSource.Play();
        menu.style.display = DisplayStyle.Flex;
    }

    private void ResumeGame()
    {
        audioSource.Play();
        menu.style.display = DisplayStyle.None;
    }

    private void ExitGame()
    {
        audioSource.Play();
        Application.Quit();
    }

    private void LoadSettings()
    {
        mainMenu.style.display = DisplayStyle.None;
        options.style.display = DisplayStyle.Flex;
        audioSource.Play();
    }

    private void ReturnSettings()
    {
        mainMenu.style.display = DisplayStyle.Flex;
        options.style.display = DisplayStyle.None;
        audioSource.Play();
    }

    private void OnSfxVolumeChanged(ChangeEvent<float> evt)
    {
        Debug.Log($"Głośność sfx: {evt.newValue}");
        audioSource.volume = evt.newValue;
        critSource.volume = evt.newValue;
        PlayerPrefs.SetFloat("SfxVolume", evt.newValue);
        PlayerPrefs.Save();
    }

    private void OnMusicVolumeChanged(ChangeEvent<float> evt)
    {
        PlayerPrefs.SetFloat("MusicVolume", evt.newValue);
        PlayerPrefs.Save();
        Debug.Log($"Głośność muzyki: {evt.newValue}");
    }

    private void SetResolution(int resIndex)
    {
        Resolution resolution = resolutions[resIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    private void SetFullScreen(bool flag)
    {
        Screen.fullScreen = flag;
    }
}