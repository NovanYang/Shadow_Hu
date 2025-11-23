using UnityEngine;
using UnityEngine.UIElements;

namespace UI
{
    public class TutorialUI : MonoBehaviour
    {

        private UIDocument _uiDocument;
        

        private void Start()
        {
            _uiDocument = GetComponent<UIDocument>();
            
            _uiDocument.rootVisualElement.style.position = Position.Absolute;
            _uiDocument.rootVisualElement.style.visibility = Visibility.Hidden;
            _uiDocument.rootVisualElement.style.left = Screen.width / 12;
            _uiDocument.rootVisualElement.style.top = 40;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                _uiDocument.rootVisualElement.style.visibility = Visibility.Visible;
            }
        }
        
        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                _uiDocument.rootVisualElement.style.visibility = Visibility.Hidden;
            }
        }
    }
}
