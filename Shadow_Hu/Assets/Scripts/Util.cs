using UnityEngine.SceneManagement;

public class Util
{
    public class SHSceneManager
    {
        private static SHSceneManager _instance;
        private static readonly object _lock = new object();

        private int _currentSceneIndex = 0;
        
        public const string SceneTutorial = "TutorialLevel_Test";
        public const string SceneLevel1 = "Level1";
        public const string SceneLevel2 = "Level2";
        public const string SceneLevel3 = "Level3";
        public const string SceneLevel4 = "Level4";
        
        private static readonly string[] _sceneOrders = {SceneTutorial, SceneLevel1, SceneLevel2, SceneLevel3, SceneLevel4};
        
        public static SHSceneManager Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new SHSceneManager();
                    }
                    return _instance;
                }
            }
        }
        
        public void SwitchNextScene()
        {
            if (_currentSceneIndex < _sceneOrders.Length - 1)
            {
                _currentSceneIndex++;
                FadeSceneTransition.LoadScene(_sceneOrders[_currentSceneIndex]);    
            }
        } 
        
        public void ReloadCurrentScene()
        {
            //TODO: add a ReloadScene method in FadeSceneTransition
            FadeSceneTransition.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}