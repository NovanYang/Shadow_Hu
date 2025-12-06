using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class FadeSceneTransition : MonoBehaviour
{
    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration = 1f;
    
    private static FadeSceneTransition instance;
    
    void Awake()
    {
        // Singleton pattern - persist across scenes
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        // Start with transparent
        if (fadeImage != null)
        {
            Color c = fadeImage.color;
            c.a = 0f;
            fadeImage.color = c;
        }
    }
    
    public static void LoadScene(string sceneName)
    {
        instance.StartCoroutine(instance.TransitionToScene(sceneName));
    }
    
    public static void LoadScene(int sceneIndex)
    {
        instance.StartCoroutine(instance.TransitionToScene(sceneIndex));
    }
    
    private IEnumerator TransitionToScene(string sceneName)
    {
        // Fade to black
        yield return StartCoroutine(Fade(1f));
        
        // Load scene
        SceneManager.LoadScene(sceneName);
        
        // Fade from black
        yield return StartCoroutine(Fade(0f));
    }
    
    private IEnumerator TransitionToScene(int sceneIndex)
    {
        // Fade to black
        yield return StartCoroutine(Fade(1f));
        
        // Load scene
        SceneManager.LoadScene(sceneIndex);
        
        // Fade from black
        yield return StartCoroutine(Fade(0f));
    }
    
    private IEnumerator Fade(float targetAlpha)
    {
        float startAlpha = fadeImage.color.a;
        float elapsed = 0f;
        
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / fadeDuration);
            
            Color c = fadeImage.color;
            c.a = alpha;
            fadeImage.color = c;
            
            yield return null;
        }
        
        // Ensure final value
        Color finalColor = fadeImage.color;
        finalColor.a = targetAlpha;
        fadeImage.color = finalColor;
    }
}