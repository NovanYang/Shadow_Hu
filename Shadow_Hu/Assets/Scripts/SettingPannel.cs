using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class SettingPanel : MonoBehaviour
{
    [SerializeField] private KeyCode toggleKey = KeyCode.Escape;
    
    private UIDocument _uiDocument;
    private VisualElement _rootContainer;
    private Button _closeButton;
    private Button _replayButton;
    private Button _exitButton;
    private Slider _volumeSlider;
    
    private bool _isVisible = false;

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

        _rootContainer = root.Q<VisualElement>("RootContainer");
        _closeButton = root.Q<Button>("CloseButton");
        _replayButton = root.Q<Button>("ReplayButton");
        _exitButton = root.Q<Button>("ExitButton");
        _volumeSlider = root.Q<Slider>("VolumeSlider");

        if (_closeButton != null)
            _closeButton.RegisterCallback<ClickEvent>(OnCloseClicked);
        else
            Debug.LogWarning("CloseButton not found in UI");

        if (_replayButton != null)
            _replayButton.RegisterCallback<ClickEvent>(OnReplayClicked);
        else
            Debug.LogWarning("ReplayButton not found in UI");

        if (_exitButton != null)
            _exitButton.RegisterCallback<ClickEvent>(OnExitClicked);
        else
            Debug.LogWarning("ExitButton not found in UI");

        if (_volumeSlider != null)
        {
            _volumeSlider.RegisterValueChangedCallback(OnVolumeChanged);
            _volumeSlider.value = AudioListener.volume * 100f;
        }
        else
            Debug.LogWarning("VolumeSlider not found in UI");

        // Hide panel initially
        Hide();
    }

    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            Toggle();
        }
    }

    public void Toggle()
    {
        if (_isVisible)
            Hide();
        else
            Show();
    }

    public void Show()
    {
        if (_rootContainer == null) return;
        _rootContainer.style.display = DisplayStyle.Flex;
        _isVisible = true;
        Time.timeScale = 0f; // Pause game
    }

    public void Hide()
    {
        if (_rootContainer == null) return;
        _rootContainer.style.display = DisplayStyle.None;
        _isVisible = false;
        Time.timeScale = 1f; // Resume game
    }

    private void OnCloseClicked(ClickEvent evt)
    {
        Hide();
    }

    private void OnReplayClicked(ClickEvent evt)
    {
        Time.timeScale = 1f; // Resume time before reloading
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void OnExitClicked(ClickEvent evt)
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    private void OnVolumeChanged(ChangeEvent<float> evt)
    {
        AudioListener.volume = evt.newValue / 100f;
    }

    private void OnDestroy()
    {
        // Cleanup event subscriptions
        if (_closeButton != null) _closeButton.UnregisterCallback<ClickEvent>(OnCloseClicked);
        if (_replayButton != null) _replayButton.UnregisterCallback<ClickEvent>(OnReplayClicked);
        if (_exitButton != null) _exitButton.UnregisterCallback<ClickEvent>(OnExitClicked);
    }
}
