using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Util
{
    public class SHSceneManager
    {
        private static SHSceneManager _instance;
        private static readonly object _lock = new object();
        
        private static readonly List<string> _sceneOrders = new List<string>()
        {
            "Start",
            "TutoriaLevel_Test",
            "TLevel1",
            "TLevel2",
            "TLevel3",
            "TLevel4",
            "Level1",
            "Level2",
            "Level3",
            "Level4",
            "Won"
        };
        
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
            var curSceneName = SceneManager.GetActiveScene().name;
            var index = _sceneOrders.FindIndex((val) => val == curSceneName);
            Debug.Log($"find current scene index {curSceneName}, index {index}");
            if (index != -1 && index < _sceneOrders.Count - 1)
            {
                FadeSceneTransition.LoadScene(_sceneOrders[index + 1]);    
            }
        } 
        
        public void ReloadCurrentScene()
        {
            //TODO: add a ReloadScene method in FadeSceneTransition
            FadeSceneTransition.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void LoadScene(string sceneName)
        {
            FadeSceneTransition.LoadScene(sceneName);
        }

        public void LoadScene(int sceneIndex)
        {
            FadeSceneTransition.LoadScene(sceneIndex);
        }
    }
}