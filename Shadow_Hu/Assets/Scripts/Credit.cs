using UnityEngine;
using UnityEngine.UIElements;

public class Credit : MonoBehaviour
{
    private UIDocument _uiDocument;
    private Button _exitButton;

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
        _exitButton = root.Q<Button>("ExitButton");

        // Register button callbacks
        if (_exitButton != null)
            _exitButton.RegisterCallback<ClickEvent>(OnExitClicked);
        else
            Debug.LogWarning("ExitButton not found in UI");
    }

    private void OnExitClicked(ClickEvent evt)
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    private void OnDestroy()
    {
        // Cleanup event subscriptions
        if (_exitButton != null) 
            _exitButton.UnregisterCallback<ClickEvent>(OnExitClicked);
    }
}
