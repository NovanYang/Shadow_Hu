using UnityEngine;
using UnityEngine.UIElements;

public class StartMenu : MonoBehaviour
{
    [Header("Scene Settings")]
    [SerializeField] private string gameSceneName = "Level1";
    
    private UIDocument _uiDocument;
    private VisualElement _settingsOverlay;
    private Button _playButton;
    private Button _settingButton;
    private Button _quitButton;
    private Button _closeSettingsButton;
    private Button _backButton;
    private Slider _volumeSlider;
    private Label _volumeLabel;

    void OnEnable()
    {
        _uiDocument = GetComponent<UIDocument>();
        if (_uiDocument == null)
        {
            Debug.LogError("UIDocument component not found!");
            return;
        }

        var root = _uiDocument.rootVisualElement;
        if (root == null)
        {
            Debug.LogError("Root visual element is null!");
            return;
        }

        // Get UI Elements
        _settingsOverlay = root.Q<VisualElement>("SettingsPanelOverlay");
        _playButton = root.Q<Button>("PlayButton");
        _settingButton = root.Q<Button>("SettingButton");
        _quitButton = root.Q<Button>("QuitButton");
        _closeSettingsButton = root.Q<Button>("CloseSettingsButton");
        _backButton = root.Q<Button>("BackButton");
        _volumeSlider = root.Q<Slider>("VolumeSlider");
        _volumeLabel = root.Q<Label>("VolumeLabel");

        // Register button callbacks
        if (_playButton != null)
            _playButton.RegisterCallback<ClickEvent>(OnPlayClicked);
        else
            Debug.LogWarning("PlayButton not found in UI");

        if (_settingButton != null)
            _settingButton.RegisterCallback<ClickEvent>(OnSettingClicked);
        else
            Debug.LogWarning("SettingButton not found in UI");

        if (_quitButton != null)
            _quitButton.RegisterCallback<ClickEvent>(OnQuitClicked);
        else
            Debug.LogWarning("QuitButton not found in UI");

        if (_closeSettingsButton != null)
            _closeSettingsButton.RegisterCallback<ClickEvent>(OnCloseSettingsClicked);
        else
            Debug.LogWarning("CloseSettingsButton not found in UI");

        if (_backButton != null)
            _backButton.RegisterCallback<ClickEvent>(OnCloseSettingsClicked);
        else
            Debug.LogWarning("BackButton not found in UI");

        if (_volumeSlider != null)
        {
            _volumeSlider.RegisterValueChangedCallback(OnVolumeChanged);
            _volumeSlider.value = AudioListener.volume * 100f;
            UpdateVolumeLabel(_volumeSlider.value);
        }
        else
            Debug.LogWarning("VolumeSlider not found in UI");

        // Hide settings panel initially
        HideSettingsPanel();
    }

    private void OnPlayClicked(ClickEvent evt)
    {
        Util.SHSceneManager.Instance.SwitchNextScene();
    }

    private void OnSettingClicked(ClickEvent evt)
    {
        ShowSettingsPanel();
    }

    private void OnQuitClicked(ClickEvent evt)
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    private void OnCloseSettingsClicked(ClickEvent evt)
    {
        HideSettingsPanel();
    }

    private void OnVolumeChanged(ChangeEvent<float> evt)
    {
        AudioListener.volume = evt.newValue / 100f;
        UpdateVolumeLabel(evt.newValue);
    }

    private void UpdateVolumeLabel(float volume)
    {
        if (_volumeLabel != null)
        {
            int percentage = Mathf.RoundToInt(volume);
            _volumeLabel.text = percentage + "%";
        }
    }

    private void ShowSettingsPanel()
    {
        if (_settingsOverlay == null) return;
        _settingsOverlay.style.display = DisplayStyle.Flex;
    }

    private void HideSettingsPanel()
    {
        if (_settingsOverlay == null) return;
        _settingsOverlay.style.display = DisplayStyle.None;
    }

    private void OnDestroy()
    {
        // Cleanup event subscriptions
        if (_playButton != null) _playButton.UnregisterCallback<ClickEvent>(OnPlayClicked);
        if (_settingButton != null) _settingButton.UnregisterCallback<ClickEvent>(OnSettingClicked);
        if (_quitButton != null) _quitButton.UnregisterCallback<ClickEvent>(OnQuitClicked);
        if (_closeSettingsButton != null) _closeSettingsButton.UnregisterCallback<ClickEvent>(OnCloseSettingsClicked);
        if (_backButton != null) _backButton.UnregisterCallback<ClickEvent>(OnCloseSettingsClicked);
    }

    // Public method to set game scene name from inspector or other scripts
    public void SetGameSceneName(string sceneName)
    {
        gameSceneName = sceneName;
    }
}
